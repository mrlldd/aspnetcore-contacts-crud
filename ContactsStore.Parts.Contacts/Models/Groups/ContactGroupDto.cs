namespace ContactsStore.Models.Groups;

public class ContactGroupDto
{
	public string Name { get; set; } = null!;
	
	public string? Description { get; set; }

	public List<ContactGroupItemDto> Contacts { get; set; } = new();

}