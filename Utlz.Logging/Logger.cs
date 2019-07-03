using System;
using System.Runtime.CompilerServices;
using Utlz.Logging.Impl;

namespace Utlz.Logging
{
	internal struct Logger
	{
		private readonly DoLogger _doLogger;

		internal Logger(DoLogger doLogger) => _doLogger = doLogger;

		internal DoLoggerWrapper? Trace => ForLevel(Level.Trace);

		internal DoLoggerWrapper? Debug => ForLevel(Level.Debug);

		internal DoLoggerWrapper? Info => ForLevel(Level.Info);

		internal DoLoggerWrapper? Warn => ForLevel(Level.Warn);

		internal DoLoggerWrapper? Error => ForLevel(Level.Error);

		internal DoLoggerWrapper? Fatal => ForLevel(Level.Fatal);

		internal DoLoggerWrapper? ForLevel(Level level) =>
			_doLogger.IsEnabled(level) ? new DoLoggerWrapper(_doLogger, level) : (DoLoggerWrapper?) null;

		internal struct DoLoggerWrapper
		{
			private readonly DoLogger _doLogger;
			private readonly Level _level;

			internal DoLoggerWrapper(DoLogger doLogger, Level level)
			{
				_doLogger = doLogger;
				_level = level;
			}

			internal void Log(
				FormattableString messageWithArgs,
				ValueTuple<string, object>[] additionalContext = null,
				[CallerMemberName] string memberName = "",
				[CallerFilePath] string sourceFilePath = "",
				[CallerLineNumber] int sourceLineNumber = 0) =>
				_doLogger.DoLog(new SourceContext(_level, memberName, sourceFilePath, sourceLineNumber),
					new StructuredMessage(messageWithArgs.Format, messageWithArgs.GetArguments(), additionalContext));

			internal void Log(
				Exception exception,
				FormattableString messageWithArgs,
				ValueTuple<string, object>[] additionalContext = null,
				[CallerMemberName] string memberName = "",
				[CallerFilePath] string sourceFilePath = "",
				[CallerLineNumber] int sourceLineNumber = 0) =>
				_doLogger.DoLog(new SourceContext(_level, memberName, sourceFilePath, sourceLineNumber),
					new StructuredMessage(messageWithArgs.Format, messageWithArgs.GetArguments(), additionalContext,
						exception));

			internal void Log(
				NonFormattableString textOnlyMessage,
				ValueTuple<string, object>[] additionalContext = null,
				[CallerMemberName] string memberName = "",
				[CallerFilePath] string sourceFilePath = "",
				[CallerLineNumber] int sourceLineNumber = 0) =>
				_doLogger.DoLog(new SourceContext(_level, memberName, sourceFilePath, sourceLineNumber),
					new StructuredMessage(textOnlyMessage.StringArg, additionalContext));

			internal void Log(
				Exception exception,
				NonFormattableString textOnlyMessage,
				ValueTuple<string, object>[] additionalContext = null,
				[CallerMemberName] string memberName = "",
				[CallerFilePath] string sourceFilePath = "",
				[CallerLineNumber] int sourceLineNumber = 0) =>
				_doLogger.DoLog(new SourceContext(_level, memberName, sourceFilePath, sourceLineNumber),
					new StructuredMessage(textOnlyMessage.StringArg, additionalContext, exception));

			/// <summary>
			///     Taken from https://stackoverflow.com/questions/35770713/overloaded-string-methods-with-string-interpolation
			///     https://www.bartwolff.com/Blog/2017/09/14/intercepting-interpolated-strings-in-c,
			///     and http://www.damirscorner.com/blog/posts/20180921-FormattableStringAsMethodParameter.html
			///     It's exactly the approach that EF Core 2.0 uses.
			/// </summary>
			internal sealed class NonFormattableString
			{
				public readonly string StringArg;

				public NonFormattableString(string stringArg) => StringArg = stringArg;

				public static implicit operator NonFormattableString(string stringArg) =>
					new NonFormattableString(stringArg);

				public static implicit operator NonFormattableString(FormattableString fs) =>
					throw new InvalidOperationException("This method should never be called");
			}
		}
	}
}
