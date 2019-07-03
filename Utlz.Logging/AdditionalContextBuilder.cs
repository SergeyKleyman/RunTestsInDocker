using System;

namespace Utlz.Logging
{
	public class AdditionalContextBuilder
	{
		internal static ValueTuple<string, object>[] AdditionalContext(
			params ValueTuple<string, object>[] nameValuePairs) => nameValuePairs;
	}
}
