namespace Core.Handlers.Output;

public record Error(ErrorTypeEnum Type, string Code, string Description);

public static class ErrorExtensions
{
	public static string ToString(this Error error)
	{
		return $"{error.Code} {error.Description}";
	}
}
