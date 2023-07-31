using FluentValidation.Results;

namespace ContactsStore.Persistence.Internal;

internal delegate Task<ValidationResult> AsyncValidationExecutor(object validator,
																 object entity,
																 CancellationToken cancellationToken);