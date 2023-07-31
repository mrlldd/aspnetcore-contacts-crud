using ContactsStore.Models.Groups;

namespace ContactsStore.Models;

public class ContactDto
{
	public int ContactId { get; set; }
	
	public string? Description { get; set; }
	
	public ShortContactGroupDto? Group { get; set; }

	public PersonDto Person { get; set; } = null!;
}