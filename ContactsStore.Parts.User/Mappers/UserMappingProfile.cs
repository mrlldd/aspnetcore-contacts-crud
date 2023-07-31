using AutoMapper;
using ContactsStore.Entities.Identity;
using ContactsStore.Models;
using JetBrains.Annotations;

namespace ContactsStore.Mappers;

[UsedImplicitly]
public class UserMappingProfile : Profile
{
	public UserMappingProfile() => CreateMap<CSUser, UserDto>(MemberList.Destination)
		.ReverseMap()
		.ForMember(x => x.UserName, x => x.MapFrom(u => u.Email));
}