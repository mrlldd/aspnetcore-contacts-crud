using ContactsStore.Entities.Identity;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Operations;

public record PasswordSignIn(string Email, string Password) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<PasswordSignIn>
	{
		public Validator()
		{
			RuleFor(x => x.Email).EmailAddress();
			RuleFor(x => x.Password).MinimumLength(8);
		}
	}

	[UsedImplicitly]
	internal class PasswordSignInHandler : IRequestHandler<PasswordSignIn>
	{
		private readonly SignInManager<CSUser> _userManager;

		public PasswordSignInHandler(SignInManager<CSUser> userManager) => _userManager = userManager;

		public async Task Handle(PasswordSignIn request, CancellationToken cancellationToken)
		{
			var result = await _userManager.PasswordSignInAsync(request.Email, request.Password, true, true);
			if (!result.Succeeded)
			{
				throw new InvalidOperationException("SignIn failed");
			}
		}
	}
}