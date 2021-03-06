﻿using System.Collections.Generic;
using System.Linq;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CodeTourVS
{
    public class CodeTourPeekableItemSource : IPeekableItemSource
    {
        private readonly IPeekResultFactory _peekResultFactory;
        private readonly ITextBuffer _textBuffer;
        private readonly IViewTagAggregatorFactoryService _viewTagAggregatorService;

        public CodeTourPeekableItemSource(IPeekResultFactory peekResultFactory, ITextBuffer textBuffer, IViewTagAggregatorFactoryService viewTagAggregatorService)
        {
            _peekResultFactory = peekResultFactory;
            _textBuffer = textBuffer;
            _viewTagAggregatorService = viewTagAggregatorService;
        }

        public void AugmentPeekSession(IPeekSession session, IList<IPeekableItem> peekableItems)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (session.RelationshipName == CodeTourPeekRelationship.RelationshipName)
            {
                IWpfTextView view = GetTextView();
                ITagAggregator<CodeTourTag> aggregator = _viewTagAggregatorService.CreateTagAggregator<CodeTourTag>(view);

                SnapshotPoint point = view.Caret.Position.BufferPosition.TranslateTo(_textBuffer.CurrentSnapshot, PointTrackingMode.Positive);
                SnapshotSpan line = point.GetContainingLine().Extent;
                IMappingTagSpan<CodeTourTag> tag = aggregator.GetTags(line).FirstOrDefault();

                if (tag != null)
                {
                    var item = new CodeTourPeekableItem(_peekResultFactory, tag.Tag.Step, line.Span);
                    peekableItems.Add(item);
                }
            }
        }

        public IWpfTextView GetTextView()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var compService = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            Assumes.Present(compService);

            IVsEditorAdaptersFactoryService editorAdapter = compService.GetService<IVsEditorAdaptersFactoryService>();

            var textManager = ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager)) as IVsTextManager;
            Assumes.Present(textManager);

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out IVsTextView activeView));

            return editorAdapter.GetWpfTextView(activeView);
        }

        public void Dispose()
        {

        }
    }
}
