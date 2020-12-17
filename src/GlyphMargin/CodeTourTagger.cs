using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace CodeTourVS
{
    internal class CodeTourTagger : ITagger<CodeTourTag>
    {
        private ITextView _textView;
        private readonly ITextDocument _document;
        private readonly IEnumerable<Step> _steps;

        public CodeTourTagger(ITextView textView, ITextDocument document, IEnumerable<Step> steps)
        {
            _textView = textView;
            _document = document;
            _steps = steps;
        }

        IEnumerable<ITagSpan<CodeTourTag>> ITagger<CodeTourTag>.GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (SnapshotSpan curSpan in spans)
            {
                var parentSpan = curSpan.TranslateTo(_textView.TextBuffer.CurrentSnapshot, SpanTrackingMode.EdgeExclusive);
                var line = parentSpan.Snapshot.GetLineFromPosition(parentSpan.Start.Position);

                var step = _steps.FirstOrDefault(s => s.Line == line.LineNumber);

                if (step != null)
                {
                    var ctSpan = new SnapshotSpan(parentSpan.Snapshot, line.Extent);
                    yield return new TagSpan<CodeTourTag>(ctSpan, new CodeTourTag(step));
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }
    }
}