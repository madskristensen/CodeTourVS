using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace CodeTourVS
{
    [Export(typeof(IPeekableItemSourceProvider))]
    [ContentType("text")]
    [Name(CodeTourPeekRelationship.RelationshipName)]
    [SupportsStandaloneFiles(true)]
    [Order(Before = "definition")]
    public class CodeTourPeekableItemSourceProvider : IPeekableItemSourceProvider
    {
        [Import]
        private IPeekResultFactory PeekResultFactory { get; set; }

        [Import]
        private IViewTagAggregatorFactoryService ViewTagAggregatorService { get; set; }

        public IPeekableItemSource TryCreatePeekableItemSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new CodeTourPeekableItemSource(PeekResultFactory, textBuffer, ViewTagAggregatorService));
        }
    }
}
