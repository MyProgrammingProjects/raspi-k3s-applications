using System.Collections.Generic;
using System.Linq;

namespace Core.Helpers;

public static class Enumerable
{
	public static bool IsNullOrEmpty<T>(this IEnumerable<T>? e)
	{
		return e == null || e.Count() == 0;
	}
}
