using ContactsStore.Entities;
using ContactsStore.Exceptions;
using ContactsStore.Extensions;
using ContactsStore.Identity;
using ContactsStore.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContactsStore.Operations.Groups;

public record AddContactToGroup(int GroupId, int ContactId) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<AddContactToGroup>
	{
		public Validator()
		{
			RuleFor(x => x.GroupId).GreaterThan(0);
			RuleFor(x => x.ContactId).GreaterThan(0);
		}
	}

	[UsedImplicitly]
	internal class AssignContactToGroupHandler : IRequestHandler<AddContactToGroup>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IUserContextAccessor _accessor;

		public AssignContactToGroupHandler(IContactsStoreDatabase database, IUserContextAccessor accessor)
		{
			_database = database;
			_accessor = accessor;
		}

		public async Task Handle(AddContactToGroup request, CancellationToken cancellationToken)
		{
			var contact = await _database.ReadAsync((db, ct) => db.Context
				.UserContacts(_accessor)
				.FirstOrExceptionAsync(x => x.PersonId == request.ContactId, ct), cancellationToken);

			var groupExists = await _database.ReadAsync((db, ct) => db.Context
				.UserContactGroups(_accessor)
				.AnyAsync(x => x.ContactGroupId == request.GroupId, ct), cancellationToken);

			if (!groupExists)
			{
				throw new CouldNotFindEntityException(typeof(ContactGroup), request.GroupId);
			}

			contact.GroupId = request.GroupId;
			await _database.PersistAsync(cancellationToken);
		}
	}
}