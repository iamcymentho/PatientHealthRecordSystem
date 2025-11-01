using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PHR.Application.Health.Queries;
namespace PHR.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class HealthController : ControllerBase
	{
		private readonly IMediator _mediator;
		public HealthController(IMediator mediator)
		{
			_mediator = mediator;
		}
		[HttpGet("ping")]
		public async Task<IActionResult> Ping([FromQuery] string? message)
		{
			var result = await _mediator.Send(new PingQuery(message));
			return Ok(new { message = result });
		}
	}
}