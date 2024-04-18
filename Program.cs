using ApiToolkit.Net;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var config = new Config
{
    Debug = true,
    ApiKey = "z6EYf5FMa3gzzNUfgKZsHjtN9GLETNaev7/v0LkNozFQ89nH"
};
var client = await APIToolkit.NewClientAsync(config);

app.Use(async (context, next) =>
{
    var apiToolkit = new APIToolkit(next, client);
    await apiToolkit.InvokeAsync(context);
});

app.MapGet("/don-net", async (context) =>
{
    var observingHandlerOptions = new ATOptions
    {
        PathWildCard = "/posts/{id}/{name}",
        RedactHeaders = ["User-Agent"],
        RedactRequestBody = ["$.user.password"],
        RedactResponseBody = ["$.user.data.email"]
    };
    using var httpClient = new HttpClient(client.APIToolkitObservingHandler(context, new ATOptions { PathWildCard = "/todos/{id}" }));
    var request = new HttpRequestMessage(HttpMethod.Get, "https://jsonplaceholder.typicode.com/todos/1?vood=dooo");
    client.ReportError(context, new Exception("ssssssssssss"));
    // Add headers to the request
    request.Headers.Add("User-Agent", "MyApp"); // Example header


    var response = await httpClient.SendAsync(request);
    var body = await response.Content.ReadAsStringAsync();
    Console.WriteLine(body);
    await context.Response.WriteAsync("Hello World!");

});

app.MapGet("/error-tracking", async context =>
{
    try
    {
        // Attempt to open a non-existing file (just an example)
        using (var fileStream = System.IO.File.OpenRead("nonexistingfile.txt"))
        {
            // File opened successfully, do something if needed
        }
        await context.Response.WriteAsync($"Hello, {context.Request.RouteValues["name"]}!");
    }
    catch (Exception ex)
    {
        client.ReportError(context, ex);
        await context.Response.WriteAsync("Error reported!");
    }
});

app.MapPost("/post-request", async context =>
{
    // Handle POST request here
    await context.Response.WriteAsync("This is a POST request endpoint");
});

app.Run();
