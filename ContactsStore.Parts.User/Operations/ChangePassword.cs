using ContactsStore.Entities.Identity;
using ContactsStore.Identity;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Operations;

public record ChangePassword(string OldPassword, string NewPassword) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<ChangePassword>
	{
		public Validator()
		{
			RuleFor(x => x.OldPassword).NotEmpty().MinimumLength(8);
			RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
		}
	}

	[UsedImplicitly]
	internal class ResetPasswordHandler : IRequestHandler<ChangePassword>
	{
		private readonly UserManager<CSUser> _userManager;
		private readonly IUserContextAccessor _accessor;

		public ResetPasswordHandler(UserManager<CSUser> userManager, IUserContextAccessor accessor)
		{
			_userManager = userManager;
			_accessor = accessor;
		}

		public async Task Handle(ChangePassword request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(_accessor.UserId.ToString("D"));
			if (user is null)
			{
				throw new InvalidOperationException("Could not find user");
			}
			var verificationResult =
				_userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.OldPassword);
			if (verificationResult == PasswordVerificationResult.Failed)
			{
				throw new InvalidOperationException("Old password is invalid");
			}

			var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
			await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
		}
	}
}