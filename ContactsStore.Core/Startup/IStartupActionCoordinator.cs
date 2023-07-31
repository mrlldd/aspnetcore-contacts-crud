namespace ContactsStore.Startup;

public interface IStartupActionCoordinator
{
	Task PerformStartupActionsAsync(CancellationToken cancellationToken);
}