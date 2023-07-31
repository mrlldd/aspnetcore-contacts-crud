using AutoMapper;
using ContactsStore.Entities;
using ContactsStore.Models;
using ContactsStore.Models.Groups;
using JetBrains.Annotations;

namespace ContactsStore.Mappers;

[UsedImplicitly]
internal class ContactMappingProfile : Profile
{
	public ContactMappingProfile()
	{
		CreateMap<Contact, ContactDto>(MemberList.Destination)
			.ForMember(x => x.ContactId, x => x.MapFrom(c => c.PersonId));

		CreateMap<EditContactDto, Contact>(MemberList.Source)
			.ForMember(x => x.PersonId, x => x.MapFrom(c => c.ContactId));

		CreateMap<Contact, ContactGroupItemDto>(MemberList.Destination);
	}
}