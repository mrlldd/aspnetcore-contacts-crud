using FluentValidation;
using JetBrains.Annotations;

namespace ContactsStore.Models;

public class EditContactDto
{
	public int ContactId { get; set; }
	public string? Description { get; set; }
	
	public int? GroupId { get; set; }

	public PersonDto Person { get; set; } = null!;

	[UsedImplicitly]
	internal class Validator : AbstractValidator<EditContactDto>
	{
		public Validator(IValidator<PersonDto> personValidator)
		{
			RuleFor(x => x.Description).NotEmpty().MaximumLength(500).When(x => x.Description is not null);
			RuleFor(x => x.GroupId).GreaterThan(0).When(x => x.GroupId is not null);
			RuleFor(x => x.Person).SetValidator(personValidator);
		}
	}
}