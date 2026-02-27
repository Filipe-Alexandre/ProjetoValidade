using System.Net;
using System.Text.Json;
using ValiKop.Api.Exceptions;

namespace ValiKop.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status;
        string message;

        switch (exception)
        {
            case NotFoundException:
                status = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case BusinessException:
                status = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case UnauthorizedException:
                status = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;

            default:
                status = HttpStatusCode.InternalServerError;
                message = "Erro interno inesperado.";
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var response = new
        {
            error = message,
            status = context.Response.StatusCode
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
