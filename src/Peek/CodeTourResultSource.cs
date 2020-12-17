using System.Threading;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace CodeTourVS
{
    public class CodeTourResultSource : IPeekResultSource
    {
        private readonly IPeekResultFactory _peekResultFactory;
        private readonly Step _step;
        private readonly Span _span;

        public CodeTourResultSource(IPeekResultFactory peekResultFactory, Step step, Span span)
        {
            _peekResultFactory = peekResultFactory;
            _step = step;
            _span = span;
        }

        public void FindResults(string relationshipName, IPeekResultCollection resultCollection, CancellationToken cancellationToken, IFindPeekResultsCallback callback)
        {
            if (relationshipName == CodeTourPeekRelationship.RelationshipName)
            {
                resultCollection.Add(new CodeTourPeekResult(_step));
                callback.ReportProgress(1);
            }
        }
    }
}
