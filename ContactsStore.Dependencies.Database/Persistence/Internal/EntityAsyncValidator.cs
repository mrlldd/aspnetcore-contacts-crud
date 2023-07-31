using FluentValidation.Results;

namespace ContactsStore.Persistence.Internal;

internal delegate Task<ValidationResult> EntityAsyncValidator(object entity, CancellationToken cancellationToken);