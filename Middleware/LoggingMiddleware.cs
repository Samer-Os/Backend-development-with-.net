using System.Diagnostics;

namespace UserManagementAPI.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            try
            {
                _logger.LogInformation(
                    "Request {RequestId}: {Method} {Path} started at {Time}",
                    requestId,
                    context.Request.Method,
                    context.Request.Path,
                    DateTime.UtcNow
                );

                await _next(context);

                stopwatch.Stop();
                _logger.LogInformation(
                    "Request {RequestId}: Completed with status {StatusCode} in {ElapsedMs}ms",
                    requestId,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Request {RequestId}: Failed with error {Error}",
                    requestId,
                    ex.Message
                );
                throw;
            }
        }
    }
}