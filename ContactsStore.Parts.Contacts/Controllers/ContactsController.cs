using ContactsStore.Models;
using ContactsStore.Operations;
using ContactsStore.Utilities.Paging;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactsStore.Controllers;

[Authorize]
[Route("api/contacts")]
[ApiController]
public class ContactsController : Controller
{
	private readonly IMediator _mediator;

	public ContactsController(IMediator mediator) => _mediator = mediator;

	[HttpGet(Name = "GetContacts")]
	public Task<PagedResult<ContactDto>> GetContacts([FromQuery(Name = nameof(page))] int page,
	                                                 [FromQuery(Name = nameof(size))] int size,
	                                                 CancellationToken cancellationToken)
		=> _mediator.Send(new GetContacts(page, size), cancellationToken);

	[HttpPost(Name = "CreateContact")]
	public Task CreateContact([FromBody] EditContactDto contact, CancellationToken cancellationToken)
		=> _mediator.Send(new CreateContact(contact), cancellationToken);

	[HttpPatch(Name = "EditContact")]
	public Task EditContact([FromBody] EditContactDto contact,
	                        CancellationToken cancellationToken)
		=> _mediator.Send(new EditContact(contact), cancellationToken);

	[HttpDelete("{id:int}", Name = "DeleteContact")]
	public Task DeleteContact([FromRoute(Name = nameof(id))] int id, CancellationToken cancellationToken)
		=> _mediator.Send(new DeleteContact(id), cancellationToken);
}