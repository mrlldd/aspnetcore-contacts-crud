using ContactsStore.Entities.Identity;
using ContactsStore.Identity;
using ContactsStore.Models;
using ContactsStore.Operations;
using ContactsStore.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ContactsStore.Parts.User.Tests.Unit;

public static class ServiceProviderExtensions
{
	public static async Task SetDefaultUserContextAsync(this IServiceProvider serviceProvider)
	{
		const string email = "user@mail.com";
		var mediator = serviceProvider
			.GetRequiredService<IMediator>();
		await mediator
			.Send(new RegisterUser(new UserDto
			{
				Email = email,
				Person = new PersonDto
				{
					Name = "user",
					Surname = "for tests",
					EmailAddresses = new List<EmailAddressDto>
					{
						new()
						{
							Address = email
						}
					},
					PhoneNumbers = new List<PhoneNumberDto>
					{
						new()
						{
							Number = "123123123"
						}
					}
				}
			}, "gentlePassword"));

		var ctx = serviceProvider.GetRequiredService<ContactsStoreDbContext>();
		var user = await ctx.Set<CSUser>()
			.SingleAsync(x => x.Email == email);
		var mock = serviceProvider.GetRequiredService<Mock<IUserContextAccessor>>();
		mock.Setup(x => x.HasContext).Returns(true);
		mock.Setup(x => x.HasUser).Returns(true);
		mock.Setup(x => x.UserId).Returns(user.Id);
	}
}