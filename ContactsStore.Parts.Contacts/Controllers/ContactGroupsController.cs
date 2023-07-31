using ContactsStore.Models.Groups;
using ContactsStore.Operations;
using ContactsStore.Operations.Groups;
using ContactsStore.Utilities.Paging;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactsStore.Controllers;

[Authorize]
[Route("api/contacts/groups")]
[ApiController]
public class ContactGroupsController : Controller
{
	private readonly IMediator _mediator;

	public ContactGroupsController(IMediator mediator) => _mediator = mediator;

	[HttpGet(Name = "GetContactGroups")]
	public Task<PagedResult<ShortContactGroupDto>> GetContactGroups([FromQuery(Name = nameof(page))] int page,
	                                                                [FromQuery(Name = nameof(size))] int size,
	                                                                CancellationToken cancellationToken)
		=> _mediator.Send(new GetContactGroups(page, size), cancellationToken);

	[HttpGet("{id:int}", Name = "GetContactGroupById")]
	public Task<ContactGroupDto> GetContactGroupById([FromRoute(Name = nameof(id))] int id,
	                                                 CancellationToken cancellationToken)
		=> _mediator.Send(new GetContactGroup(id), cancellationToken);

	[HttpPost(Name = "CreateContactGroup")]
	public Task CreateContactGroup([FromBody] EditContactGroupDto group, CancellationToken cancellationToken)
		=> _mediator.Send(new CreateContactGroup(group), cancellationToken);

	[HttpPatch(Name = "EditContactGroup")]
	public Task EditContactGroup([FromBody] EditContactGroupDto group,
	                             CancellationToken cancellationToken)
		=> _mediator.Send(new EditContactGroup(group), cancellationToken);

	[HttpDelete("{id:int}", Name = "DeleteContactGroup")]
	public Task DeleteContactGroup([FromRoute(Name = nameof(id))] int id, CancellationToken cancellationToken)
		=> _mediator.Send(new DeleteContactGroup(id), cancellationToken);

	[HttpPatch("{id:int}/add", Name = "AddContactToGroup")]
	public Task AddContactToGroup([FromRoute(Name = nameof(id))] int id,
	                              [FromQuery(Name = nameof(contactId))] int contactId,
	                              CancellationToken cancellationToken)
		=> _mediator.Send(new AddContactToGroup(id, contactId), cancellationToken);

	[HttpPatch("remove", Name = "RemoveContactFromGroup")]
	public Task RemoveContactFromGroup([FromQuery(Name = nameof(contactId))] int contactId,
	                                   CancellationToken cancellationToken)
		=> _mediator.Send(new RemoveContactFromGroup(contactId), cancellationToken);
}