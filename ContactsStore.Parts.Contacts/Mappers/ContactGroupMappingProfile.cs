using AutoMapper;
using ContactsStore.Entities;
using ContactsStore.Models.Groups;
using ContactsStore.Operations.Groups;
using JetBrains.Annotations;

namespace ContactsStore.Mappers;

[UsedImplicitly]
public class ContactGroupMappingProfile : Profile
{
	public ContactGroupMappingProfile()
	{
		CreateMap<ContactGroup, ShortContactGroupDto>(MemberList.Destination);
		CreateMap<ContactGroup, ContactGroupDto>(MemberList.Destination);
		CreateMap<EditContactGroupDto, ContactGroup>(MemberList.Source);
	}
}