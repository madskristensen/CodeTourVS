using System;
using Microsoft.VisualStudio.Language.Intellisense;

namespace CodeTourVS
{
    internal class CodeTourPeekResult : IPeekResult
    {
        public CodeTourPeekResult(Step step)
        {
            Step = step;
        }

        public IPeekResultDisplayInfo DisplayInfo =>
             new PeekResultDisplayInfo("Code Tour", null, "Code Tour", "Code Tour");

        public bool CanNavigateTo => true;

        public Action<IPeekResult, object, object> PostNavigationCallback => null;

        public Step Step { get; }

        public event EventHandler Disposed;

        public void Dispose()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public void NavigateTo(object data)
        {
        }
    }
}