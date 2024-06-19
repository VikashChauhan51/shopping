using Polly;
using Polly.Extensions.Http;

namespace Shopping.Web.Resilience;


public class PollyPolicies
{
    private readonly ILogger<PollyPolicies> _logger;

    public PollyPolicies(ILogger<PollyPolicies> logger)
    {
        _logger = logger;
    }

    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (outcome, timespan, attempt, context) =>
                {
                    _logger.LogWarning($"Retry attempt {attempt} after {timespan.TotalSeconds} seconds due to {outcome.Exception?.Message ?? outcome.Result.ReasonPhrase}");
                });
    }

    public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, breakDelay) =>
                {
                    _logger.LogError($"Circuit broken due to {outcome.Exception?.Message ?? outcome.Result.ReasonPhrase}. Break duration: {breakDelay.TotalSeconds} seconds.");
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit reset.");
                });
    }
}

