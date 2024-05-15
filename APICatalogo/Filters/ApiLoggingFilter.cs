using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalogo.Filters;

public class ApiLoggingFilter : IActionFilter
{
    private readonly ILogger _logger;

    public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // executa antes o action
        _logger.LogInformation($"### Executando antes da Action -> OnExecuting");
        _logger.LogInformation($"#########################################");
        _logger.LogInformation($"{DateTime.Now.ToShortTimeString()}");
        _logger.LogInformation($"ModelState {context.ModelState.IsValid}");
        _logger.LogInformation($"#########################################");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // executa depois o action
        _logger.LogInformation($"### Executando depois da Action -> OnExecuted");
        _logger.LogInformation($"#########################################");
        _logger.LogInformation($"{DateTime.Now.ToShortTimeString()}");
        _logger.LogInformation($"ModelState {context.HttpContext.Response.StatusCode}");
        _logger.LogInformation($"#########################################");
    }
}
