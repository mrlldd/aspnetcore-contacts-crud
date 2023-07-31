using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContactsStore.Extensions;
using ContactsStore.Identity;
using ContactsStore.Models.Groups;
using ContactsStore.Persistence;
using ContactsStore.Utilities.Paging;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace ContactsStore.Operations.Groups;

public record GetContactGroup(int GroupId) : IRequest<ContactGroupDto>
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<GetContactGroup>
	{
		public Validator(IValidator<IPagedRequest> prValidator) 
			=> RuleFor(x => x.GroupId).GreaterThan(0);
	}

	[UsedImplicitly]
	internal class GetContactGroupHandler : IRequestHandler<GetContactGroup, ContactGroupDto>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IMapper _mapper;
		private readonly IUserContextAccessor _accessor;

		public GetContactGroupHandler(IContactsStoreDatabase database, IMapper mapper, IUserContextAccessor accessor)
		{
			_database = database;
			_mapper = mapper;
			_accessor = accessor;
		}

		public Task<ContactGroupDto> Handle(GetContactGroup request, CancellationToken cancellationToken)
			=> _database.ReadAsync((db, ct) => db.Context
				.UserContactGroups(_accessor)
				.Where(x => x.ContactGroupId == request.GroupId)
				.ProjectTo<ContactGroupDto>(_mapper.ConfigurationProvider)
				.FirstOrExceptionAsync(ct), cancellationToken);
	}
}