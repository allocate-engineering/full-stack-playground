using Serilog;

using Allocate.Web.DemoApi;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var startup = new Startup(builder.Configuration, builder.Environment);
    startup.ConfigureServices(builder.Services);
    var app = builder.Build();
    startup.Configure(app, builder.Environment);

    await app.RunAsync();
}
catch (Exception ex)
{
    // Log.Logger will likely be internal type "Serilog.Core.Pipeline.SilentLogger".
    if (Log.Logger == null || Log.Logger.GetType().Name == "SilentLogger")
    {
        // Loading configuration or Serilog failed.
        // This will create simple, default console logger.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }

    Log.Fatal(ex, "---=== Host terminated unexpectedly!! ===---");
    return -1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;