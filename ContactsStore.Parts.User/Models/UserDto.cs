using FluentValidation;

namespace ContactsStore.Models;

public class UserDto
{
	public string Email { get; set; } = null!;

	public PersonDto Person { get; set; } = null!;

	public class Validator : AbstractValidator<UserDto>
	{
		public Validator(IValidator<PersonDto> personValidator)
		{
			RuleFor(x => x.Person).SetValidator(personValidator);
			RuleFor(x => x.Email).EmailAddress();
		}
	}
}