using ContactsStore.Entities.Identity;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Operations;

public record SignOut : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<SignOut>
	{
	}

	[UsedImplicitly]
	internal class SignOutHandler : IRequestHandler<SignOut>
	{
		private readonly SignInManager<CSUser> _signInManager;

		public SignOutHandler(SignInManager<CSUser> signInManager) => _signInManager = signInManager;

		public Task Handle(SignOut request, CancellationToken cancellationToken) 
			=> _signInManager.SignOutAsync();
	}
}