using ContactsStore.Extensions;
using ContactsStore.Identity;
using ContactsStore.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace ContactsStore.Operations.Groups;

public record RemoveContactFromGroup(int ContactId) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<RemoveContactFromGroup>
	{
		public Validator() => RuleFor(x => x.ContactId).GreaterThan(0);
	}

	[UsedImplicitly]
	internal class RemoveContactFromGroupHandler : IRequestHandler<RemoveContactFromGroup>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IUserContextAccessor _accessor;

		public RemoveContactFromGroupHandler(IContactsStoreDatabase database, IUserContextAccessor accessor)
		{
			_database = database;
			_accessor = accessor;
		}

		public async Task Handle(RemoveContactFromGroup request, CancellationToken cancellationToken)
		{
			var contact = await _database.ReadAsync((db, ct) => db.Context
				.UserContacts(_accessor)
				.FirstOrExceptionAsync(x => x.PersonId == request.ContactId, ct), cancellationToken);
			contact.GroupId = null;
			await _database.PersistAsync(cancellationToken);
		}
	}
}