namespace Utlz.RunTestsInDocker
{
	internal static class Consts
	{
		internal const string ShadowCopyDirectoryName = "shadow_copy";

		internal const string PrepareLogFileName = "prepare.log";
		internal const string PrepareErrOnlyLogFileName = "prepare_ERROR.log";

		internal static class TestResults
		{
			internal const string DirectoryName = "test_results";
			internal const string CurrentIterationDirectoryName = "current_iteration";

			internal const string LogFileName = "all_output.log";
			internal const string ErrorOnlyLogFileName = "ERROR.log";

			internal const string FailedIterationsArchiveDirectoryName = "FAILED_iterations_archive";
			internal const string SuccessfulIterationsArchiveDirectoryName = "successful_iterations_archive";
		}
	}
}
