using AutoMapper.EquivalencyExpression;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using ContactsStore.Maintenance;
using ContactsStore.Middleware;
using ContactsStore.Startup;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace ContactsStore;

public class Program
{
	public static async Task Main(string[] args)
	{
		try
		{
			var builder = WebApplication.CreateBuilder(args);

			ConfigureConfiguration(builder.Configuration);
			ConfigureHost(builder.Host);

			var dependencies = EnrichWithDependencies(new AppDependenciesCollection(), builder.Environment);
			var parts = EnrichWithApplicationParts(new AppPartsCollection());

			ConfigureServices(builder.Services, parts, dependencies);

			var app = Configure(builder.Build(), dependencies);
			await RunAsync(app);
		}
		catch (Exception e)
		{
			Log.Error(e, "Application exited with error");
			await Log.CloseAndFlushAsync();
			throw;
		}
	}

	private static async Task RunAsync(WebApplication app)
	{
		var lifetime = app.Lifetime;

		var appRunner = app.RunAsync();
		lifetime.ApplicationStarted.WaitHandle.WaitOne();

		await using (var scope = app.Services.CreateAsyncScope())
		{
			var sp = scope.ServiceProvider;
			var maintenance = sp.GetRequiredService<IApplicationMaintenance>();
			maintenance.Enable("startup");
			await sp.GetRequiredService<IStartupActionCoordinator>()
				.PerformStartupActionsAsync(lifetime.ApplicationStopping);
			maintenance.Disable();
		}

		await appRunner;
	}

	private static IConfigurationBuilder ConfigureConfiguration(IConfigurationBuilder configuration)
		=> configuration
			.AddEnvironmentVariables()
			.AddUserSecrets<Program>(true, true);

	private static IHostBuilder ConfigureHost(IHostBuilder host)
		=> host.UseSerilog((context, sp, loggerConfiguration) =>
		{
			// When something wrong with logging - uncomment the line below
			// Serilog.Debugging.SelfLog.Enable(Console.Error);

			const string logOutputTemplate = "[{Timestamp:HH:mm:ss.fff}] "
			                                 + "[{RequestId}] "
			                                 + "[{SourceContext:l}] "
			                                 + "[{Level:u3}] "
			                                 + "{Message:lj}{NewLine}"
			                                 + "{Properties:j}{NewLine}"
			                                 + "{Exception}";

			loggerConfiguration
				.ReadFrom.Configuration(context.Configuration)
				.Enrich.FromLogContext()
				.Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
				.Enrich.WithThreadId();

			if (context.HostingEnvironment.IsDevelopment())
			{
				loggerConfiguration.WriteTo.Console(
						outputTemplate: logOutputTemplate,
						theme: AnsiConsoleTheme.Literate,
						restrictedToMinimumLevel: LogEventLevel.Debug)
					.WriteTo.Seq("http://localhost:5341");
			}
			else
			{
				loggerConfiguration.WriteTo.ApplicationInsights(TelemetryConverter.Events);
			}
		});

	private static IAppDependenciesCollection EnrichWithDependencies(IAppDependenciesCollection collection,
	                                                                 IWebHostEnvironment env)
		=> collection.AddDatabase(env);

	public static IAppPartsCollection EnrichWithApplicationParts(IAppPartsCollection collection)
		=> collection
			.AddUserPart()
			.AddContactsPart();

	private static IServiceCollection ConfigureServices(IServiceCollection services, IAppPartsCollection parts,
	                                                    IAppDependenciesCollection dependencies)
	{
		services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(s =>
		{
			s.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "ContactsStore",
				Version = "v1"
			});
		});

		services.AddFluentValidationRulesToSwagger(opt => opt.SetFluentValidationCompatibility());

		services.AddCore();

		services.AddHealthChecks();

		services.AddCoreMediatRBehaviors();

		services.AddAutoMapper(x => x.AddCollectionMappers(), typeof(Program).Assembly);
		services.AddAppParts(parts);
		services.AddDependencyServices(dependencies, parts);

		services.AddStartupAction<AutoMapperValidationAction>();
		return services;
	}

	private static WebApplication Configure(WebApplication app, IAppDependenciesCollection dependencies)
	{
		// Configure the HTTP request pipeline.
		app.UseSwagger();
		app.UseSwaggerUI();

		app.UseHealthChecks(new PathString("/api/health"));
		app.UseMiddleware<MaintenanceMiddleware>();

		app.UseHttpsRedirection();
		app.MapControllers();
		app.UseAuthentication();
		app.UseRouting();
		app.UseAuthorization();
		app.UseUserPartMiddlewares();

		app.UseAuthorization();
		app.UseDependencies(dependencies);

		return app;
	}
}