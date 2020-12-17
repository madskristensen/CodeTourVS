using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace CodeTourVS
{
    [Export(typeof(IGlyphMouseProcessorProvider))]
    [Name(nameof(CodeTourGlyphMouseProcessorProvider))]
    [Order]
    [ContentType("text")]
    class CodeTourGlyphMouseProcessorProvider : IGlyphMouseProcessorProvider
    {
        [Import]
        private IViewTagAggregatorFactoryService ViewTagAggregatorService { get; set; }

        [Import]
        private IPeekBroker PeekBroker { get; set; }

        [Import]
        internal IToolTipService ToolTipProvider { get; set; }

        public IMouseProcessor GetAssociatedMouseProcessor(IWpfTextViewHost host, IWpfTextViewMargin margin)
        {
            return new CodeTourGlyphMouseProcessor(host, ViewTagAggregatorService, ToolTipProvider, PeekBroker);
        }
    }
}
