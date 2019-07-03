using System;

namespace Utlz.Logging
{
	public struct StructuredMessage
	{
		private static readonly LeaveOnlyArgNamesFormatProvider LeaveOnlyArgNamesFormatProviderSingleton =
			new LeaveOnlyArgNamesFormatProvider();

		public readonly object[] Arguments;

		public readonly string Template;

		public readonly ValueTuple<string, object>[] AdditionalContext;

		public readonly Exception Exception;

		internal StructuredMessage(
			string textOnlyMessage,
			ValueTuple<string, object>[] additionalContext,
			Exception exception = null) : this(textOnlyMessage, null, additionalContext, exception)
		{
		}

		internal StructuredMessage(
			string template,
			object[] arguments,
			ValueTuple<string, object>[] additionalContext,
			Exception exception = null)
		{
			Template = template;
			Arguments = arguments ?? Array.Empty<object>();
			AdditionalContext = additionalContext ?? Array.Empty<ValueTuple<string, object>>();
			Exception = exception;
		}

		public string GetTemplateWithArgNamesOnly() =>
			string.Format(LeaveOnlyArgNamesFormatProviderSingleton, Template, Arguments);

		private class LeaveOnlyArgNamesFormatProvider : IFormatProvider
		{
			public object GetFormat(Type formatType) =>
				typeof(ICustomFormatter).IsAssignableFrom(formatType) ? new LeaveOnlyArgNamesCustomFormatter() : null;
		}

		private class LeaveOnlyArgNamesCustomFormatter : ICustomFormatter
		{
			public string Format(string format, object arg, IFormatProvider formatProvider) => $"{{{format}}}";
		}
	}
}
