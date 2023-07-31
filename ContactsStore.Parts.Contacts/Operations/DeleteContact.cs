using ContactsStore.Extensions;
using ContactsStore.Identity;
using ContactsStore.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContactsStore.Operations;

public record DeleteContact(int ContactId) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<DeleteContact>
	{
		public Validator() => RuleFor(x => x.ContactId).GreaterThan(0);
	}

	[UsedImplicitly]
	internal class DeleteContactHandler : IRequestHandler<DeleteContact>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IUserContextAccessor _accessor;

		public DeleteContactHandler(IContactsStoreDatabase database, IUserContextAccessor accessor)
		{
			_database = database;
			_accessor = accessor;
		}

		public async Task Handle(DeleteContact request, CancellationToken cancellationToken)
		{
			var contact = await _database.ReadAsync((db, ct) => db.Context
				.UserContacts(_accessor)
				.Include(x => x.Person!)
				.ThenInclude(x => x.EmailAddresses)
				.Include(x => x.Person!)
				.ThenInclude(x => x.PhoneNumbers)
				.FirstOrExceptionAsync(x => x.PersonId == request.ContactId, ct), cancellationToken);
			_database.Context.Remove(contact);
			await _database.PersistAsync(cancellationToken);
		}
	}
}