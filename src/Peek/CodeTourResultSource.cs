using System.Threading;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace CodeTourVS
{
    public class CodeTourResultSource : IPeekResultSource
    {
        public void FindResults(string relationshipName, IPeekResultCollection resultCollection, CancellationToken cancellationToken, IFindPeekResultsCallback callback)
        {
            if (relationshipName == CodeTourPeekRelationship.RelationshipName)
            {
                resultCollection.Add(new CodeTourPeekResult());
                callback.ReportProgress(1);
            }
        }
    }
}
