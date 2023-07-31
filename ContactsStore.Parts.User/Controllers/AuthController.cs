using ContactsStore.Operations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContactsStore.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : Controller
{
	private readonly IMediator _mediator;

	public AuthController(IMediator mediator) => _mediator = mediator;

	[HttpPost("signIn", Name = "SignIn")]
	public Task SignIn([FromBody] PasswordSignIn passwordSignIn, CancellationToken cancellationToken)
		=> _mediator.Send(passwordSignIn, cancellationToken);

	[HttpPost("register", Name = "SignUp")]
	public Task Register([FromBody] RegisterUser register, CancellationToken cancellationToken)
		=> _mediator.Send(register, cancellationToken);

	[HttpDelete("signOut", Name = "SignOut")]
	public Task SignOut(CancellationToken cancellationToken)
		=> _mediator.Send(new SignOut(), cancellationToken);

	[HttpPatch(Name = "ChangePassword")]
	public Task ChangePassword([FromQuery(Name = nameof(oldPassword))] string oldPassword,
	                           [FromQuery(Name = nameof(newPassword))] string newPassword,
	                           CancellationToken cancellationToken)
		=> _mediator.Send(new ChangePassword(oldPassword, newPassword), cancellationToken);
}