using AutoMapper;
using ContactsStore.Entities;
using ContactsStore.Identity;
using ContactsStore.Models;
using ContactsStore.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace ContactsStore.Operations;

public record CreateContact(EditContactDto Contact) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<CreateContact>
	{
		public Validator(IValidator<EditContactDto> validator)
		{
			RuleFor(x => x.Contact).SetValidator(validator);
		}
	}

	[UsedImplicitly]
	internal class CreateContactHandler : IRequestHandler<CreateContact>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IMapper _mapper;
		private readonly IUserContextAccessor _accessor;

		public CreateContactHandler(IContactsStoreDatabase database, IMapper mapper, IUserContextAccessor accessor)
		{
			_database = database;
			_mapper = mapper;
			_accessor = accessor;
		}

		public async Task Handle(CreateContact request, CancellationToken cancellationToken)
		{
			var contact = _mapper.Map<Contact>(request.Contact);
			contact.OwnerId = _accessor.UserId;
			await _database.Context.AddAsync(contact, cancellationToken);
			await _database.PersistAsync(cancellationToken);
		}
	}
}