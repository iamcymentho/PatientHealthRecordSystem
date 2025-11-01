using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PHR.Application.DTOs;
using PHR.Application.Exceptions;
namespace PHR.Api.Middleware
{
	public class GlobalExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
		public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
				await HandleExceptionAsync(context, ex);
			}
		}
		private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			ApiResponse response;
			int statusCode;
			switch (exception)
			{
				case BaseException baseEx:
					statusCode = baseEx.HttpStatusCode;
					response = new ApiResponse(baseEx.Code, baseEx.Message, null, false);
					break;
				case UnauthorizedAccessException:
					statusCode = (int)HttpStatusCode.Unauthorized;
					response = ApiResponse.UnauthorizedResponse(exception.Message);
					break;
				case ArgumentNullException:
				case ArgumentException:
					statusCode = (int)HttpStatusCode.BadRequest;
					response = ApiResponse.BadRequestResponse(exception.Message);
					break;
				case NotImplementedException:
					statusCode = (int)HttpStatusCode.NotImplemented;
					response = new ApiResponse("51", "Feature not implemented", null, false);
					break;
				case TimeoutException:
					statusCode = (int)HttpStatusCode.RequestTimeout;
					response = new ApiResponse("52", "Request timeout", null, false);
					break;
				default:
					statusCode = (int)HttpStatusCode.InternalServerError;
					response = ApiResponse.InternalServerErrorResponse("An unexpected error occurred. Please try again later.");
					break;
			}
			context.Response.StatusCode = statusCode;
			var jsonOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true,
				ReferenceHandler = ReferenceHandler.IgnoreCycles
			};
			jsonOptions.Converters.Add(new JsonStringEnumConverter());
			var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
			await context.Response.WriteAsync(jsonResponse);
		}
	}
}