namespace TextReplacer.Model
{
    public class TextReplaceScriptGlobals
    {
        public TextReplaceScriptGlobals(string sourceText)
        {
            SourceText = sourceText;
        }
        #region Public Properties

        public string SourceText { get; }

        #endregion
    }
}