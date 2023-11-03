using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;
using AeC.digitalJourney.Lib.Monitor;

namespace MinimalLambda;

public class Function : IHttpFunction
{
    public CustomLogger _logger;

    public Function()
    {
        var env = new Env { ENV = "production", LOG_MIN_LEVEL = "debug", OUTPUT = "console", APPLICATION = "minimal-lambda" };
        var logger = LoggerConfigurationHelper.ConfigureLogger(env);
        _logger = logger;
    }

    public async Task HandleAsync(HttpContext context)
    {
        HttpRequest request = context.Request;
        HttpResponse response = context.Response;

        string requestBody;
        using (StreamReader reader = new StreamReader(request.Body))
        {
            requestBody = await reader.ReadToEndAsync();
        }

        GcpLambdaHttpLoggingEnricher.Enrich(context, _logger);
        _logger.Information("request processado", "200", requestBody);
        _logger.ClearContext();

        response.StatusCode = (int) System.Net.HttpStatusCode.OK;

        return;
    }
}
