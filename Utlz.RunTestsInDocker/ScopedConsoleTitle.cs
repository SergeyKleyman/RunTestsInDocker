using System;

namespace Utlz.RunTestsInDocker
{
	public class ScopedConsoleTitle: IDisposable
	{
		private readonly string _savedTitle;

		public ScopedConsoleTitle(string newTitle)
		{
			_savedTitle = Console.Title;
			Console.Title = newTitle;
		}

		public void Dispose()
		{
			Console.Title = _savedTitle;
		}
	}
}
