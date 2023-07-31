using FluentValidation;
using JetBrains.Annotations;

namespace ContactsStore.Models;

public class EmailAddressDto
{
	public int EmailAddressId { get; set; }
	public string Address { get; set; } = null!;

	[UsedImplicitly]
	internal class Validator : AbstractValidator<EmailAddressDto>
	{
		public Validator() => RuleFor(x => x.Address).EmailAddress();
	}
}