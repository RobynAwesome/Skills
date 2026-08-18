using Kopano.Sovereign.EstateAdapter;

if (args.Contains("--self-test", StringComparer.OrdinalIgnoreCase))
{
    Environment.ExitCode = SelfTest.Run();
    return;
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DomainBindingStore>();
builder.Services.AddSingleton<EstateParser>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "Kopano.Sovereign.EstateAdapter",
    renterAssertion = DomainBindingStore.RenterAssertion,
    state = "stateless"
}));

app.MapGet("/v1/bindings", (DomainBindingStore store) => Results.Ok(new
{
    renterAssertion = DomainBindingStore.RenterAssertion,
    bindings = store.All
}));

app.MapPost("/v1/route", (RouteRequest request, DomainBindingStore store) =>
{
    try
    {
        return Results.Ok(store.Resolve(request.Url));
    }
    catch (ArgumentException exception)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
});

app.MapPost("/v1/parse", (ParseRequest request, EstateParser parser, DomainBindingStore store) =>
    Results.Ok(parser.Parse(request, store)));

app.MapPost("/v1/converge", (ParseRequest request, EstateParser parser, DomainBindingStore store) =>
    Results.Ok(parser.Parse(request, store)));

app.Run();
