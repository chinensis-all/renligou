namespace Renligou.Core.Shared.Ddd
{
    public sealed record Error(
        string Code,
        string? Message
    ) {
        public static Error Validation(string message)
        => new("validation_error", message);

        public static Error NotFound(string message)
            => new("not_found", message);

        public static Error Conflict(string message)
            => new("conflict", message);
    }
}
