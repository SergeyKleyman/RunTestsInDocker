namespace Utlz.Logging
{
    public struct SourceContext
    {
        public SourceContext(Level level, string memberName, string sourceFilePath, int sourceLineNumber)
        {
            Level = level;
            MemberName = memberName;
            SourceFilePath = sourceFilePath;
            SourceLineNumber = sourceLineNumber;
        }

        public readonly Level Level;
        public readonly string MemberName;
        public readonly string SourceFilePath;
        public readonly int SourceLineNumber;
    }
}