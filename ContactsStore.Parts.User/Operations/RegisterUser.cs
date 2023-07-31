using AutoMapper;
using ContactsStore.Entities.Identity;
using ContactsStore.Models;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ContactsStore.Operations;

public record RegisterUser(UserDto User, string Password) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<RegisterUser>
	{
		public Validator(IValidator<UserDto> validator) => RuleFor(x => x.User).SetValidator(validator);
	}

	[UsedImplicitly]
	internal class RegisterUserHandler : IRequestHandler<RegisterUser>
	{
		private readonly UserManager<CSUser> _userManager;
		private readonly IMapper _mapper;

		public RegisterUserHandler(UserManager<CSUser> userManager, IMapper mapper)
		{
			_userManager = userManager;
			_mapper = mapper;
		}

		public async Task Handle(RegisterUser request, CancellationToken cancellationToken)
		{
			var user = _mapper.Map<CSUser>(request.User);
			var result = await _userManager.CreateAsync(user, request.Password);
			if (!result.Succeeded)
			{
				throw new InvalidOperationException("Could not create a user. Errors: "
				                                    + string.Join(", ", result.Errors.Select(x => x.Description)));
			}
		}
	}
}