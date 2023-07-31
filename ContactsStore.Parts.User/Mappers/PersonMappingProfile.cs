using AutoMapper;
using ContactsStore.Entities.Information;
using ContactsStore.Models;
using JetBrains.Annotations;

namespace ContactsStore.Mappers;

[UsedImplicitly]
internal class PersonMappingProfile : Profile
{
	public PersonMappingProfile()
	{
		CreateMap<Person, PersonDto>(MemberList.Destination)
			.ReverseMap();

		CreateMap<EmailAddress, EmailAddressDto>(MemberList.Destination)
			.ReverseMap();

		CreateMap<PhoneNumber, PhoneNumberDto>(MemberList.Destination)
			.ReverseMap();
	}
}