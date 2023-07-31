using System.Runtime.CompilerServices;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using AutoMapper.Extensions.ExpressionMapping;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ContactsStore.Config;
using ContactsStore.Entities;
using ContactsStore.Entities.Configuration;
using ContactsStore.Persistence;
using ContactsStore.Persistence.Interceptors;
using ContactsStore.Persistence.Internal;
using ContactsStore.Startup;
using ContactsStore.Utilities;
using ContactsStore.Utilities.Paging;
using ContactsStore.Validation;

[assembly: InternalsVisibleTo("ContactsStore.Dependencies.Database.Tests.Unit")]
namespace ContactsStore;

internal class DatabaseDependency : IAppDependency
{
	private readonly IHostEnvironment _environment;
	private const string ConfigurationSection = "Database";

	public DatabaseDependency(IHostEnvironment environment) => _environment = environment;

	public void ConfigureServices(IServiceCollection services, IAppPartsCollection parts)
	{
		services.AddAutoMapper((sp, c) => c.UseEntityFrameworkCoreModel<ContactsStoreDbContext>(sp), Array.Empty<Type>());
		var dbAssemblyToScan = new[] {typeof(DatabaseDependency).Assembly};
		services.CollectCoreServicesFromAssemblies(dbAssemblyToScan);
		services.CollectDatabaseEntities(dbAssemblyToScan);
		
		if (_environment.IsDevelopment())
		{
			services.AddOptionsWithValidator<DatabaseConfig, DatabaseConfig.DevEnvValidator>(ConfigurationSection);
		}
		else
		{
			services.AddOptionsWithValidator<DatabaseConfig, DatabaseConfig.ProdEnvValidator>(ConfigurationSection);
		}

		services.AddOptionsWithValidator<DatabaseEntitiesConfig, DatabaseEntitiesConfig.Validator>($"{ConfigurationSection}:Entities");

		var migrationsAssemblyName = $"{typeof(DatabaseDependency).Assembly.GetName().Name}.Migrations";
		services.AddDbContext<ContactsStoreDbContext>((sp, dbContextOptionsBuilder) =>
		{
			dbContextOptionsBuilder.AddInterceptors(sp.GetServices<IOrderedInterceptor>().OrderBy(x => x.Order));
			var host = sp.GetRequiredService<IWebHostEnvironment>();
			var dbConfig = sp.GetRequiredService<IOptionsMonitor<DatabaseConfig>>()
				.CurrentValue;

			if (host.IsDevelopment())
			{
				dbContextOptionsBuilder.EnableSensitiveDataLogging();
			}
			dbContextOptionsBuilder
				.UseSqlServer(host.IsDevelopment()
						? dbConfig.ConnectionString
						: $"Server=tcp:{dbConfig.Host},{dbConfig.Port};Initial Catalog={dbConfig.DbInstanceIdentifier};Persist Security Info=False;User ID={dbConfig.Username};Password={dbConfig.Password};MultipleActiveResultSets=True;",
					builder => builder
						.EnableRetryOnFailure(dbConfig.Retries)
						.CommandTimeout(dbConfig.Timeout)
						.MigrationsAssembly(migrationsAssemblyName));
		}, ServiceLifetime.Transient);
		services.AddStartupAction<DatabaseMigrationAction>();
		services.AddSaveChangesInterceptor<ValidationSaveChangesInterceptor>();
		services.AddSaveChangesInterceptor<AuditSaveChangesInterceptor>();
		services.TryAddScoped<IContactsStoreDatabase, ContactsStoreDatabase>();
		services.TryAddScoped<IDatabasePolicySet, DatabasePolicySet>();
		services.TryAddScoped<IDatabaseConfigurator, DatabaseConfigurator>();
		services.TryAddScoped<IValidator<IPagedRequest>, IPagedRequest.Validator>();
		services.TryAddScoped<IValidator<IAuditableEntity>, IAuditableEntity.Validator>();
		services.TryAddScoped<IValidator<ISoftDeletableEntity>, ISoftDeletableEntity.Validator>();
		AddInternalServices(services);
		

		services.CollectDatabaseEntities(parts.Select(x => x.GetType().Assembly));
	}

	private static void AddInternalServices(IServiceCollection services)
		=> services.TryAddSingleton<IEntityValidatorsProvider, EntityValidatorsProvider>();


	public void ConfigureApplication(IApplicationBuilder builder)
	{
	}
}