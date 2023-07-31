using FluentValidation;

namespace ContactsStore.Config;

public abstract class OptionsValidator<T> : AbstractValidator<T>, IOptionsValidator where T : class, new()
{

}