using System;
using PHR.Application.DTOs;
namespace PHR.Application.Exceptions
{
	public abstract class BaseException : Exception
	{
		public string Code { get; }
		public int HttpStatusCode { get; }
		protected BaseException(string code, string message, int httpStatusCode) : base(message)
		{
			Code = code;
			HttpStatusCode = httpStatusCode;
		}
		protected BaseException(string code, string message, int httpStatusCode, Exception innerException) 
			: base(message, innerException)
		{
			Code = code;
			HttpStatusCode = httpStatusCode;
		}
	}
	public class NotFoundException : BaseException
	{
		public NotFoundException(string message = "Resource not found") 
			: base(ResponseCodes.NotFound, message, 404)
		{
		}
		public NotFoundException(string resourceType, object id)
			: base(ResponseCodes.NotFound, $"{resourceType} with ID '{id}' was not found", 404)
		{
		}
	}
	public class BadRequestException : BaseException
	{
		public BadRequestException(string message = "Bad request") 
			: base(ResponseCodes.BadRequest, message, 400)
		{
		}
	}
	public class UnauthorizedException : BaseException
	{
		public UnauthorizedException(string message = "Unauthorized access") 
			: base(ResponseCodes.Unauthorized, message, 401)
		{
		}
	}
	public class ForbiddenException : BaseException
	{
		public ForbiddenException(string message = "Access forbidden") 
			: base(ResponseCodes.Forbidden, message, 403)
		{
		}
	}
	public class ValidationException : BaseException
	{
		public ValidationException(string message = "Validation failed") 
			: base(ResponseCodes.ValidationError, message, 400)
		{
		}
		public ValidationException(string field, string error)
			: base(ResponseCodes.ValidationError, $"Validation failed for field '{field}': {error}", 400)
		{
		}
	}
	public class BusinessRuleException : BaseException
	{
		public BusinessRuleException(string message) 
			: base(ResponseCodes.BadRequest, message, 400)
		{
		}
	}
	public class SystemException : BaseException
	{
		public SystemException(string message = "Internal server error") 
			: base(ResponseCodes.InternalServerError, message, 500)
		{
		}
		public SystemException(string message, Exception innerException) 
			: base(ResponseCodes.InternalServerError, message, 500, innerException)
		{
		}
	}
}