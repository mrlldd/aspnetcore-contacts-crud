namespace ContactsStore.Models.Groups;

public class ContactGroupItemDto
{
	public int PersonId { get; set; }
	
	public string? Description { get; set; }

	public PersonDto Person { get; set; } = null!;
}