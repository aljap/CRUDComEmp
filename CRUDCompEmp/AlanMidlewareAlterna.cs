public class AlanMidlewareAlterna
{
    private readonly RequestDelegate _next;

    public AlanMidlewareAlterna(RequestDelegate next)
    {
        _next = next;

    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine("------------>ENTRE AL MIDDLEWARE<--------------------");

        await _next(context);
    }

}