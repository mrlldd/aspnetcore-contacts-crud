using FluentValidation;
using FluentValidation.Results;
using NCrontab;

namespace ContactsStore.Validation;

public static class RuleBuilderExtensions
{
	public static IRuleBuilder<T, string> CronExpression<T>(this IRuleBuilder<T, string> builder) => builder
		.Custom((value, context) =>
		{
			try
			{
				var _ = CrontabSchedule.Parse(value);
			}
			catch
			{
				context.AddFailure(
					new ValidationFailure(
						context.PropertyName,
						$"'{context.PropertyName}' does not contain a valid CRON expression.",
						value));
			}
		});

	public static IRuleBuilder<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> builder)
	{
		const string allowedCharacters = "0123456789+-() ";
		return builder.Must(x => x.All(c => allowedCharacters.Contains(c, StringComparison.OrdinalIgnoreCase)))
			.WithMessage("Phone number should contain only allowed characters");
	}
}