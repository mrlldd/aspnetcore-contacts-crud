using FluentValidation;
using JetBrains.Annotations;

namespace ContactsStore.Models.Groups;

public class EditContactGroupDto
{
	public int ContactGroupId { get; set; }
	public string Name { get; set; } = null!;
	
	public string? Description { get; set; }

	[UsedImplicitly]
	internal class Validator : AbstractValidator<EditContactGroupDto>
	{
		public Validator()
		{
			RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
			RuleFor(x => x.Description).NotEmpty().MaximumLength(500).When(x => x.Description is not null);
		}
	}
}