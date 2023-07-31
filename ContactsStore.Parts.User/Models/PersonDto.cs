using FluentValidation;
using JetBrains.Annotations;

namespace ContactsStore.Models;

public class PersonDto
{
	public int PersonId { get; set; }
	public string Name { get; set; } = null!;

	public string Surname { get; set; } = null!;
	
	public List<PhoneNumberDto> PhoneNumbers { get; set; } = new();
	public List<EmailAddressDto> EmailAddresses { get; set; } = new();

	[UsedImplicitly]
	internal class Validator : AbstractValidator<PersonDto>
	{
		public Validator(IValidator<PhoneNumberDto> phoneNumberValidator, IValidator<EmailAddressDto> emailAddressValidator)
		{
			RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
			RuleFor(x => x.Surname).NotEmpty().MaximumLength(50);
			RuleForEach(x => x.PhoneNumbers).SetValidator(phoneNumberValidator);
			RuleForEach(x => x.EmailAddresses).SetValidator(emailAddressValidator);
		}
	}
}