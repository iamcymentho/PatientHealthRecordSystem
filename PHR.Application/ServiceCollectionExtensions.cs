using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PHR.Application.Services.Interfaces;
using PHR.Application.Services.Implementation;
namespace PHR.Application
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddApplication(this IServiceCollection services)
		{
			services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
			services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IAccessRequestService, AccessRequestService>();
			services.AddScoped<IPatientRecordService, PatientRecordService>();
			return services;
		}
	}
}