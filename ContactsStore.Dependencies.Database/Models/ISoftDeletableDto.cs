namespace ContactsStore.Models;

public interface ISoftDeletableDto
{
	DateTime? DeletedAt { get; set; }
}