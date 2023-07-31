using ContactsStore.Extensions;
using ContactsStore.Identity;
using ContactsStore.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace ContactsStore.Operations.Groups;

public record DeleteContactGroup(int GroupId) : IRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<DeleteContactGroup>
	{
		public Validator() => RuleFor(x => x.GroupId).GreaterThan(0);
	}

	[UsedImplicitly]
	internal class DeleteContactGroupHandler : IRequestHandler<DeleteContactGroup>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IUserContextAccessor _accessor;

		public DeleteContactGroupHandler(IContactsStoreDatabase database, IUserContextAccessor accessor)
		{
			_database = database;
			_accessor = accessor;
		}

		public async Task Handle(DeleteContactGroup request, CancellationToken cancellationToken)
		{
			var group = await _database.ReadAsync((db, ct) => db.Context
				.UserContactGroups(_accessor)
				.FirstOrExceptionAsync(x => x.ContactGroupId == request.GroupId, ct), cancellationToken);
			_database.Context.Remove(group);
			await _database.PersistAsync(cancellationToken);
		}
	}
}