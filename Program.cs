using ApiToolkit.Net;

var builder = WebApplication.CreateBuilder(args);

// Initialize the APItoolkit client

var app = builder.Build();

var config = new Config
{
  Debug = true,
};
var client = APIToolkit.NewClient(config);

app.Use(async (context, next) =>
{
  var apiToolkit = new APIToolkit(next, client);
  await apiToolkit.InvokeAsync(context);
});

app.MapGet("/", async (context) =>
{
  var observingHandlerOptions = new ATOptions
  {
    PathWildCard = "/posts/{id}/{name}",
    RedactHeaders = new List<string> { "User-Agent" },
    RedactRequestBody = new List<string> { "$.user.password" },
    RedactResponseBody = new List<string> { "$.user.data.email" }
  };
  using var httpClient = new HttpClient(client.APIToolkitObservingHandler(context, new ATOptions { PathWildCard = "/todos/{id}" }));
  var request = new HttpRequestMessage(HttpMethod.Get, "https://jsonplaceholder.typicode.com/todos/1?vood=dooo");
  Client.ReportError(context, new Exception("ssssssssssss"));
  // Add headers to the request
  request.Headers.Add("User-Agent", "MyApp"); // Example header


  var response = await httpClient.SendAsync(request);
  var body = await response.Content.ReadAsStringAsync();
  // Console.WriteLine(body);
  await context.Response.WriteAsync(body);

});

app.MapGet("/error-tracking", async context =>
{
  // Attempt to open a non-existing file (just an example)
  using (var fileStream = System.IO.File.OpenRead("nonexistingfile.txt"))
  {
    // File opened successfully, do something if needed
  }
  await context.Response.WriteAsync($"Hello, {context.Request.RouteValues["name"]}!");
});

app.MapPost("/post-request", async context =>
{
  // Enable buffering to read the request body multiple times
  context.Request.EnableBuffering();

  // Read the request body
  using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
  {
    string requestBody = await reader.ReadToEndAsync();
    context.Request.Body.Position = 0; // Reset the stream position for further processing

    // Return the request body as the response
    await context.Response.WriteAsync($"Request Body: {requestBody}");
  }
});

app.Run();


