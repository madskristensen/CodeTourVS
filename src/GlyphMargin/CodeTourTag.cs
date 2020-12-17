using Microsoft.VisualStudio.Text.Editor;

namespace CodeTourVS
{
    internal class CodeTourTag : IGlyphTag
    {
        public CodeTourTag(Step step)
        {
            Step = step;
        }

        public Step Step { get; set; }
    }
}