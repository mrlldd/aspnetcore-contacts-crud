using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ContactsStore.Entities.Configuration;

public interface IDatabaseConfigurator
{
	void OnModelCreating(ModelBuilder modelBuilder);
	void ConfigureConventions(ModelConfigurationBuilder modelConfigurationBuilder, DatabaseFacade databaseFacade);
}