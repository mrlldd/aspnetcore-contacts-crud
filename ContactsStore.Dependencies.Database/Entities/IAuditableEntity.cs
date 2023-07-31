
using FluentValidation;
using JetBrains.Annotations;

namespace ContactsStore.Entities;

public interface IAuditableEntity : IEntity
{
	public DateTime CreatedAt { get; set; }

	public DateTime ModifiedAt { get; set; }

	[UsedImplicitly]
	public class Validator : AbstractValidator<IAuditableEntity>
	{
		public Validator()
		{
			RuleFor(x => x.CreatedAt).GreaterThanOrEqualTo(EntityValidatorConstants.DefaultDateTime);
			RuleFor(x => x.ModifiedAt).GreaterThanOrEqualTo(EntityValidatorConstants.DefaultDateTime);
		}
	}
}