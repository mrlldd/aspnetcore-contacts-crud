using AutoMapper;
using ContactsStore.Extensions;
using ContactsStore.Identity;
using ContactsStore.Models.Groups;
using ContactsStore.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace ContactsStore.Operations.Groups;

public record EditContactGroup(EditContactGroupDto Group) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<EditContactGroup>
	{
		public Validator(IValidator<EditContactGroupDto> validator)
		{
			RuleFor(x => x.Group.ContactGroupId).GreaterThan(0);
			RuleFor(x => x.Group).SetValidator(validator);
		}
	}

	[UsedImplicitly]
	internal class EditContactGroupHandler : IRequestHandler<EditContactGroup>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IMapper _mapper;
		private readonly IUserContextAccessor _accessor;

		public EditContactGroupHandler(IContactsStoreDatabase database, IMapper mapper, IUserContextAccessor accessor)
		{
			_database = database;
			_mapper = mapper;
			_accessor = accessor;
		}

		public async Task Handle(EditContactGroup request, CancellationToken cancellationToken)
		{
			var group = await _database.ReadAsync((db, ct) => db.Context
				.UserContactGroups(_accessor)
				.FirstOrExceptionAsync(x => x.ContactGroupId == request.Group.ContactGroupId, ct), cancellationToken);
			_mapper.Map(request.Group, group);
			await _database.PersistAsync(cancellationToken);
		}
	}
}