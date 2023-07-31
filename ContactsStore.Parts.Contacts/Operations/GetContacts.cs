using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContactsStore.Entities;
using ContactsStore.Identity;
using ContactsStore.Models;
using ContactsStore.Persistence;
using ContactsStore.Utilities.Paging;
using FluentValidation;
using JetBrains.Annotations;
using MediatR;

namespace ContactsStore.Operations;

public record GetContacts(int Page, int Size) : IRequest<PagedResult<ContactDto>>, IPagedRequest
{
	[UsedImplicitly]
	public class Validator : AbstractValidator<GetContacts>
	{
		public Validator(IValidator<IPagedRequest> prValidator)
		{
			Include(prValidator);
		}
	}

	[UsedImplicitly]
	internal class GetContactsHandler : IRequestHandler<GetContacts, PagedResult<ContactDto>>
	{
		private readonly IContactsStoreDatabase _database;
		private readonly IMapper _mapper;
		private readonly IUserContextAccessor _accessor;

		public GetContactsHandler(IContactsStoreDatabase database, IMapper mapper, IUserContextAccessor accessor)
		{
			_database = database;
			_mapper = mapper;
			_accessor = accessor;
		}

		public Task<PagedResult<ContactDto>> Handle(GetContacts request, CancellationToken cancellationToken)
			=> _database.ReadAsync((db, ct) => db.Context
				.UserContacts(_accessor)
				.ProjectTo<ContactDto>(_mapper.ConfigurationProvider)
				.ToPagedResultAsync(request, ct), cancellationToken);
	}
}