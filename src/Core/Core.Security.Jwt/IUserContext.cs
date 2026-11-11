namespace Core.Security.Jwt;

public interface IUserContextAccessor
{
    UserContext UserContext { get; }
}
