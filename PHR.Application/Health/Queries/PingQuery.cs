using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
namespace PHR.Application.Health.Queries
{
	public record PingQuery(string? Message) : IRequest<string>;
	public class PingQueryHandler : IRequestHandler<PingQuery, string>
	{
		public Task<string> Handle(PingQuery request, CancellationToken cancellationToken)
		{
			try
			{
				var msg = string.IsNullOrWhiteSpace(request.Message) ? "pong" : request.Message.Trim();
				return Task.FromResult(msg);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Failed to process PingQuery.", ex);
			}
		}
	}
}