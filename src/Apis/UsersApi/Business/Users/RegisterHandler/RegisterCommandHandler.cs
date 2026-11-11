using Core.Exceptions.BusinessExceptions;
using Core.Handlers.Handlers;
using Core.Handlers.Output;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using UsersApi.Domain.Contexts;
using UsersApi.Domain.Entities;

namespace UsersApi.Business.Users.RegisterHandler;

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegisterCommandResult>
{
	private readonly UserDBContext _userDBContext;
	public RegisterCommandHandler(UserDBContext userDBContext)
	{
		_userDBContext = userDBContext ?? throw new ArgumentNullException(nameof(userDBContext));
	}
	public async Task<Result<RegisterCommandResult>> HandleAsync(RegisterCommand cmd, CancellationToken ct)
	{
		var user = new User(cmd.Username, cmd.Password, cmd.Email);

		/*
        FIX: System.InvalidOperationException: The LINQ expression 'DbSet<User>()
            .Any(u => u.Username.Value == __cmd_Username_0)' could not be translated. Either rewrite the query in a 
            form that can be translated, or switch to client evaluation explicitly by inserting a call to 'AsEnumerable', 
            'AsAsyncEnumerable', 'ToList', or 'ToListAsync'. 

                    if(_userDBContext.Users.Any(_=> _.Username.Value == cmd.Username))
                        throw new UsernameAlreadyRegisteredException(cmd.Username);

                    if(_userDBContext.Users.Any(_=> _.Email.Value == cmd.Email))
                        throw new UsernameAlreadyRegisteredException(cmd.Username);

        System.InvalidOperationException: The LINQ expression 'DbSet<User>()
                  .Any(u => u == __user_0)' could not be translated. 

                     if(_userDBContext.Users.Any(_=> _ == user))
                        throw new UsernameAlreadyRegisteredException(cmd.Username);
        */


		//With AsEnumerable() it works because this is "Explicit client evaluation" (https://learn.microsoft.com/en-us/ef/core/querying/client-eval)
		//However this is not what I want
		// if (_userDBContext.Users.AsEnumerable().Any(u=>u.Username == new Username(cmd.Username)))
		//     throw new UsernameAlreadyRegisteredException(cmd.Username);


		/*
        * TODO see:
        * https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects
        * https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
        * https://stackoverflow.com/questions/65584171/how-to-convert-a-method-of-a-valueobject-to-sql-with-entity-framework-core-3-1
        */

		User? existingUser = await _userDBContext.Users.SingleOrDefaultAsync(u => u.Username == new Username(cmd.Username), cancellationToken: ct);
		if (existingUser is not null)
			throw new UsernameAlreadyRegisteredException(cmd.Username);

		_userDBContext.Users.Add(user);
		await _userDBContext.SaveChangesAsync(ct).ConfigureAwait(false);

		var output = Result<RegisterCommandResult>.Success(new RegisterCommandResult
		{
			Username = user.Username.Value
		});

		return output;
	}
}
