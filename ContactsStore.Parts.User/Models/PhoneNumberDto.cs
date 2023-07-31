using ContactsStore.Validation;
using FluentValidation;
using JetBrains.Annotations;

namespace ContactsStore.Models;

public class PhoneNumberDto
{
	public int PhoneNumberId { get; set; }
	public string Number { get; set; } = null!;

	[UsedImplicitly]
	internal class Validator : AbstractValidator<PhoneNumberDto>
	{
		public Validator() => RuleFor(x => x.Number).NotEmpty().PhoneNumber();
	}
}