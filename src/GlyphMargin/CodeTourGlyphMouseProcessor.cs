using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace CodeTourVS
{
    public class CodeTourGlyphMouseProcessor : MouseProcessorBase
    {
        private readonly IWpfTextViewHost _host;
        private readonly IToolTipService _toolTipService;
        private readonly IPeekBroker _peekBroker;
        private ITagAggregator<CodeTourTag> _tagAggregator;
        private CodeTourTag _currentTag;
        private IToolTipPresenter _toolTipPresenter;

        public CodeTourGlyphMouseProcessor(IWpfTextViewHost host, IViewTagAggregatorFactoryService viewTagAggregatorService, IToolTipService toolTipService, IPeekBroker peekBroker)
        {
            _tagAggregator = viewTagAggregatorService.CreateTagAggregator<CodeTourTag>(host.TextView);
            _host = host;
            _toolTipService = toolTipService;
            _peekBroker = peekBroker;
        }

        public override void PostprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            ITextView view = _host.TextView;

            Point mousePosition = Mouse.GetPosition(_host.TextView.VisualElement);
            mousePosition.Offset(_host.TextView.ViewportLeft, _host.TextView.ViewportTop);

            var line = view.TextViewLines.GetTextViewLineContainingYCoordinate(mousePosition.Y);
            view.Caret.MoveTo(line);
            _peekBroker.TriggerPeekSession(_host.TextView, CodeTourPeekRelationship.RelationshipName);
        }

        public override void PostprocessMouseMove(MouseEventArgs e)
        {
            ITextView view = _host.TextView;

            Point mousePosition = Mouse.GetPosition(_host.TextView.VisualElement);
            mousePosition.Offset(_host.TextView.ViewportLeft, _host.TextView.ViewportTop);

            var line = view.TextViewLines.GetTextViewLineContainingYCoordinate(mousePosition.Y);
            if (line != null)
            {
                IMappingTagSpan<CodeTourTag> eSpan = _tagAggregator.GetTags(line.ExtentAsMappingSpan).FirstOrDefault();

                if (eSpan != null)
                    SetTooltip(eSpan.Tag, view.TextSnapshot.CreateTrackingSpan(line.Start, line.Length, SpanTrackingMode.EdgeExclusive));
                else
                    SetTooltip(null, null);
            }
        }

        private void SetTooltip(CodeTourTag newTag, ITrackingSpan trackingSpan)
        {
            if (newTag != null)
            {
                if ((_currentTag == null) || (newTag.Step != _currentTag.Step))
                {

                    _toolTipPresenter = _toolTipService.CreatePresenter(_host.TextView);
                    _toolTipPresenter.StartOrUpdate(trackingSpan, new[] { newTag.Step.Title });
                    _currentTag = newTag;
                }
            }
            else if (_currentTag != null)
            {
                _toolTipPresenter.Dismiss();
                _currentTag = null;
            }
        }
    }
}
