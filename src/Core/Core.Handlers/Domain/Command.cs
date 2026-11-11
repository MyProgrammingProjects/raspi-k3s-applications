using System;

namespace Core.Handlers.Domain;

/// <summary>
/// /// Represents a command
/// </summary>
public abstract class Command
{
	public Guid Id { get; protected set; } = Guid.NewGuid();
	public DateTime CreatedOn { get; protected set; } = DateTime.UtcNow;
}
