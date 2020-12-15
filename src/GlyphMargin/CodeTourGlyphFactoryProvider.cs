using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Tagging;

namespace CodeTourVS
{
    [Export(typeof(IGlyphFactoryProvider))]
    [Name(nameof(CodeTourGlyphFactoryProvider))]
    [Order(Before = "VsTextMarker")]
    [ContentType("code")]
    [TagType(typeof(CodeTourTag))]
    internal sealed class CodeTourGlyphFactoryProvider : IGlyphFactoryProvider
    {
        // TODO: Implement IGlyphMouseProcessorProvider

        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            return new CodeTourGlyphFactory();
        }

    }
}