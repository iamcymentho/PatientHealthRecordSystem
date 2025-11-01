using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PHR.Application.Abstractions.Caching;
using PHR.Application.Abstractions.Repositories;
using PHR.Application.Abstractions.Services;
using PHR.Application.Auth.Abstractions;
using PHR.Infrastructure.Auth;
using PHR.Infrastructure.Caching;
using PHR.Infrastructure.Configuration;
using PHR.Infrastructure.Repositories;
using PHR.Infrastructure.Repositories.Cached;
using PHR.Infrastructure.Security;
using PHR.Infrastructure.Services;
namespace PHR.Infrastructure
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		services.AddDbContext<PHRDbContext>(options =>
		{
			options.UseSqlite(connectionString);
		});
		services.AddScoped<PHR.Application.Abstractions.Data.IApplicationDbContext>(provider => provider.GetRequiredService<PHRDbContext>());
			services.Configure<Infrastructure.Auth.JwtOptions>(configuration.GetSection("Jwt"));
			services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));
			services.AddSingleton<PHR.Application.Auth.Abstractions.ITokenService, TokenService>();
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					var jwt = configuration.GetSection("Jwt");
					var key = jwt.GetValue<string>("SigningKey") ?? string.Empty;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateIssuerSigningKey = true,
						ValidateLifetime = true,
						ValidIssuer = jwt.GetValue<string>("Issuer"),
						ValidAudience = jwt.GetValue<string>("Audience"),
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
					};
				});
			services.AddAuthorization(PHR.Infrastructure.Authorization.AuthorizationPolicies.AddPolicies);
			services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, PHR.Infrastructure.Authorization.PermissionAuthorizationHandler>();
			services.AddScoped<PHR.Infrastructure.Authorization.IAccessControlService, PHR.Infrastructure.Authorization.AccessControlService>();
			// Register caching services first
			services.AddMemoryCache();
			services.AddScoped<ICacheService, CacheService>();
			// Register security services
			services.AddScoped<BcryptPasswordHasher>();
			services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
			services.AddScoped<IPasswordHasherAbstraction>(provider => provider.GetRequiredService<BcryptPasswordHasher>());
			services.AddScoped<IPasswordVerifier>(provider => provider.GetRequiredService<BcryptPasswordHasher>());
			services.AddScoped<TokenService>();
			services.AddScoped<ITokenIssuer>(provider => provider.GetRequiredService<TokenService>());
			// Register base repositories
			services.AddScoped<UserRepository>();
			services.AddScoped<PatientRecordRepository>();
			// Register cached repositories using decorator pattern
			services.AddScoped<IUserRepository>(provider =>
			{
				var baseRepo = provider.GetRequiredService<UserRepository>();
				var cache = provider.GetRequiredService<ICacheService>();
				return new CachedUserRepository(baseRepo, cache);
			});
			services.AddScoped<IPatientRecordRepository>(provider =>
			{
				var baseRepo = provider.GetRequiredService<PatientRecordRepository>();
				var cache = provider.GetRequiredService<ICacheService>();
				return new CachedPatientRecordRepository(baseRepo, cache);
			});
			// Register other repositories (no caching needed)
			services.AddScoped<IRoleRepository, RoleRepository>();
			services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
			services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
			services.AddScoped<IUserRoleRepository, UserRoleRepository>();
			// Register application services
			services.AddScoped<IAuditService, AuditService>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddCors();
			return services;
		}
	}
}