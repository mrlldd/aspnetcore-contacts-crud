using AutoMapper;
using ContactsStore.Entities;
using ContactsStore.Identity;
using ContactsStore.Models.Groups;
using ContactsStore.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace ContactsStore.Operations.Groups;

public record CreateContactGroup(EditContactGroupDto Group) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<CreateContactGroup>
	{
		public Validator(IValidator<EditContactGroupDto> validator)
		{
			RuleFor(x => x.Group.ContactGroupId).Equal(0);
			RuleFor(x => x.Group).SetValidator(validator);
		}
	}

	[UsedImplicitly]
	internal class CreateContactGroupHandler : IRequestHandler<CreateContactGroup>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IMapper _mapper;
		private readonly IUserContextAccessor _accessor;

		public CreateContactGroupHandler(IContactsStoreDatabase database, IMapper mapper, IUserContextAccessor accessor)
		{
			_database = database;
			_mapper = mapper;
			_accessor = accessor;
		}

		public async Task Handle(CreateContactGroup request, CancellationToken cancellationToken)
		{
			var group = _mapper.Map<ContactGroup>(request.Group);
			group.OwnerId = _accessor.UserId;
			await _database.Context.AddAsync(group, cancellationToken);
			await _database.PersistAsync(cancellationToken);
		}
	}
}