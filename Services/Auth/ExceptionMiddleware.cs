namespace Services.Auth
{
    /// <summary>
    /// Catches exceptions thrown by the application and returns a
    /// specific JSON response.
    /// </summary>
    public class ExceptionMiddleware
    {
        readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                // If we throw UnauthorizedAccessException, return 401 to
                // the client.
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = new { message = ex.Message ?? "Unauthorized" };
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                // For all other exceptions, return 500 to the client.
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
#if DEBUG
                await context.Response.WriteAsJsonAsync(ex.ToString());
#endif
            }
        }
    }

}
