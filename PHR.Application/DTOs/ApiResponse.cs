namespace PHR.Application.DTOs
{
	public class ApiResponse<T>
	{
		public string Code { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public T? Data { get; set; }
		public bool Success { get; set; }
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
		public ApiResponse()
		{
		}
		public ApiResponse(string code, string message, T? data = default, bool success = true)
		{
			Code = code;
			Message = message;
			Data = data;
			Success = success;
		}
		public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
		{
			return new ApiResponse<T>(ResponseCodes.Success, message, data, true);
		}
		public static ApiResponse<T> CreatedResponse(T data, string message = "Resource created successfully")
		{
			return new ApiResponse<T>(ResponseCodes.Created, message, data, true);
		}
		public static ApiResponse<T> NotFoundResponse(string message = "Resource not found")
		{
			return new ApiResponse<T>(ResponseCodes.NotFound, message, default, false);
		}
		public static ApiResponse<T> BadRequestResponse(string message = "Bad request")
		{
			return new ApiResponse<T>(ResponseCodes.BadRequest, message, default, false);
		}
		public static ApiResponse<T> UnauthorizedResponse(string message = "Unauthorized access")
		{
			return new ApiResponse<T>(ResponseCodes.Unauthorized, message, default, false);
		}
		public static ApiResponse<T> ForbiddenResponse(string message = "Access forbidden")
		{
			return new ApiResponse<T>(ResponseCodes.Forbidden, message, default, false);
		}
		public static ApiResponse<T> ValidationErrorResponse(string message = "Validation failed")
		{
			return new ApiResponse<T>(ResponseCodes.ValidationError, message, default, false);
		}
		public static ApiResponse<T> InternalServerErrorResponse(string message = "Internal server error")
		{
			return new ApiResponse<T>(ResponseCodes.InternalServerError, message, default, false);
		}
	}
	public class ApiResponse : ApiResponse<object>
	{
		public ApiResponse() : base() { }
		public ApiResponse(string code, string message, object? data = null, bool success = true)
			: base(code, message, data, success) { }
		public static ApiResponse SuccessResponse(string message = "Operation successful")
		{
			return new ApiResponse(ResponseCodes.Success, message, null, true);
		}
		public static ApiResponse CreatedResponse(string message = "Resource created successfully")
		{
			return new ApiResponse(ResponseCodes.Created, message, null, true);
		}
		public static ApiResponse NotFoundResponse(string message = "Resource not found")
		{
			return new ApiResponse(ResponseCodes.NotFound, message, null, false);
		}
		public static ApiResponse BadRequestResponse(string message = "Bad request")
		{
			return new ApiResponse(ResponseCodes.BadRequest, message, null, false);
		}
		public static ApiResponse UnauthorizedResponse(string message = "Unauthorized access")
		{
			return new ApiResponse(ResponseCodes.Unauthorized, message, null, false);
		}
		public static ApiResponse ValidationErrorResponse(string message = "Validation failed")
		{
			return new ApiResponse(ResponseCodes.ValidationError, message, null, false);
		}
		public static ApiResponse InternalServerErrorResponse(string message = "Internal server error")
		{
			return new ApiResponse(ResponseCodes.InternalServerError, message, null, false);
		}
	}
	public static class ResponseCodes
	{
		public const string Success = "00";
		public const string Created = "01";
		public const string BadRequest = "40";
		public const string Unauthorized = "41";
		public const string Forbidden = "43";
		public const string NotFound = "44";
		public const string ValidationError = "42";
		public const string InternalServerError = "55";
		public const string ServiceUnavailable = "53";
	}
}