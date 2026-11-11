using System;

namespace Core.Handlers.Domain;

/// <summary>
/// /// Represents a query
/// </summary>
public abstract class Query
{
	public Guid Id { get; protected set; } = Guid.NewGuid();
	public DateTime CreatedOn { get; protected set; } = DateTime.UtcNow;
}
