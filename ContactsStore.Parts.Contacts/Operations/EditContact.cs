using AutoMapper;
using ContactsStore.Extensions;
using ContactsStore.Identity;
using ContactsStore.Models;
using ContactsStore.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContactsStore.Operations;

public record EditContact(EditContactDto Contact) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<EditContact>
	{
		public Validator(IValidator<EditContactDto> validator)
		{
			RuleFor(x => x.Contact.ContactId).GreaterThan(0);
			RuleFor(x => x.Contact).SetValidator(validator);
		}
	}

	[UsedImplicitly]
	internal class EditContactHandler : IRequestHandler<EditContact>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IMapper _mapper;
		private readonly IUserContextAccessor _accessor;

		public EditContactHandler(IContactsStoreDatabase database, IMapper mapper, IUserContextAccessor accessor)
		{
			_database = database;
			_mapper = mapper;
			_accessor = accessor;
		}

		public async Task Handle(EditContact request, CancellationToken cancellationToken)
		{
			var contact = await _database.ReadAsync((db, ct) => db.Context
				.UserContacts(_accessor)
				.Include(x => x.Person!)
				.ThenInclude(x => x.EmailAddresses)
				.Include(x => x.Person!)
				.ThenInclude(x => x.PhoneNumbers)
				.AsSplitQuery()
				.FirstOrExceptionAsync(x => x.PersonId == request.Contact.ContactId, ct), cancellationToken);
			_mapper.Map(request.Contact, contact);
			await _database.PersistAsync(cancellationToken);
		}
	}
}