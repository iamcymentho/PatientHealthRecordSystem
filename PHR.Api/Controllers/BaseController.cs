using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PHR.Application.DTOs;
using PHR.Application.Exceptions;
namespace PHR.Api.Controllers
{
	public abstract class BaseController : ControllerBase
	{
		protected Guid GetCurrentUserId()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
			{
				throw new UnauthorizedException("Invalid user ID in token.");
			}
			return userId;
		}
		protected IActionResult Success<T>(T data, string message = "Operation successful")
		{
			return Ok(ApiResponse<T>.SuccessResponse(data, message));
		}
		protected IActionResult Success(string message = "Operation successful")
		{
			return Ok(ApiResponse.SuccessResponse(message));
		}
		protected IActionResult Created<T>(T data, string message = "Resource created successfully")
		{
			return StatusCode(201, ApiResponse<T>.CreatedResponse(data, message));
		}
		protected IActionResult Created(string message = "Resource created successfully")
		{
			return StatusCode(201, ApiResponse.CreatedResponse(message));
		}
		protected IActionResult NotFound(string message = "Resource not found")
		{
			return StatusCode(404, ApiResponse.NotFoundResponse(message));
		}
		protected IActionResult BadRequest(string message = "Bad request")
		{
			return StatusCode(400, ApiResponse.BadRequestResponse(message));
		}
		protected IActionResult Unauthorized(string message = "Unauthorized access")
		{
			return StatusCode(401, ApiResponse.UnauthorizedResponse(message));
		}
		protected IActionResult Forbidden(string message = "Access forbidden")
		{
			return StatusCode(403, ApiResponse.ForbiddenResponse(message));
		}
		protected IActionResult ValidationError(string message = "Validation failed")
		{
			return StatusCode(400, ApiResponse.ValidationErrorResponse(message));
		}
		protected IActionResult HandleApiResponse<T>(ApiResponse<T> apiResponse)
		{
			return apiResponse.Code switch
			{
				ResponseCodes.Success => Ok(apiResponse),
				ResponseCodes.Created => StatusCode(201, apiResponse),
				ResponseCodes.NotFound => StatusCode(404, apiResponse),
				ResponseCodes.BadRequest => StatusCode(400, apiResponse),
				ResponseCodes.Unauthorized => StatusCode(401, apiResponse),
				ResponseCodes.Forbidden => StatusCode(403, apiResponse),
				ResponseCodes.InternalServerError => StatusCode(500, apiResponse),
				_ => StatusCode(500, apiResponse)
			};
		}
		protected IActionResult HandleApiResponse(ApiResponse apiResponse)
		{
			return apiResponse.Code switch
			{
				ResponseCodes.Success => Ok(apiResponse),
				ResponseCodes.Created => StatusCode(201, apiResponse),
				ResponseCodes.NotFound => StatusCode(404, apiResponse),
				ResponseCodes.BadRequest => StatusCode(400, apiResponse),
				ResponseCodes.Unauthorized => StatusCode(401, apiResponse),
				ResponseCodes.Forbidden => StatusCode(403, apiResponse),
				ResponseCodes.InternalServerError => StatusCode(500, apiResponse),
				_ => StatusCode(500, apiResponse)
			};
		}
	}
}