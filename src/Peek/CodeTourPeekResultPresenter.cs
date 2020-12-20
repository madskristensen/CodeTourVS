using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Utilities;

namespace CodeTourVS
{
    [Export(typeof(IPeekResultPresenter))]
    [Name("Code Tour Peek Presenter")]
    internal class CodeTourPeekResultPresenter : IPeekResultPresenter
    {
        public IPeekResultPresentation TryCreatePeekResultPresentation(IPeekResult result)
        {
            return result is CodeTourPeekResult ? new CodeTourPeekResultPresentation() : null;
        }
    }
}