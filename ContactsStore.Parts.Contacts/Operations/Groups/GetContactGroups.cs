using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContactsStore.Entities;
using ContactsStore.Identity;
using ContactsStore.Models.Groups;
using ContactsStore.Persistence;
using ContactsStore.Utilities.Paging;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace ContactsStore.Operations.Groups;

public record GetContactGroups(int Page, int Size) : IRequest<PagedResult<ShortContactGroupDto>>, IPagedRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<GetContactGroups>
	{
		public Validator(IValidator<IPagedRequest> prValidator) => Include(prValidator);
	}

	[UsedImplicitly]
	internal class GetContactGroupsHandler : IRequestHandler<GetContactGroups, PagedResult<ShortContactGroupDto>>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IMapper _mapper;
		private readonly IUserContextAccessor _accessor;

		public GetContactGroupsHandler(IContactsStoreDatabase database, IMapper mapper, IUserContextAccessor accessor)
		{
			_database = database;
			_mapper = mapper;
			_accessor = accessor;
		}

		public Task<PagedResult<ShortContactGroupDto>> Handle(GetContactGroups request,
		                                                      CancellationToken cancellationToken)
			=> _database.ReadAsync((db, ct) => db.Context
				.UserContactGroups(_accessor)
				.ProjectTo<ShortContactGroupDto>(_mapper.ConfigurationProvider)
				.ToPagedResultAsync(request, ct), cancellationToken);
	}
}