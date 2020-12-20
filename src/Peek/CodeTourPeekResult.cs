using System;
using Microsoft.VisualStudio.Language.Intellisense;

namespace CodeTourVS
{
    internal class CodeTourPeekResult : IPeekResult
    {
        public IPeekResultDisplayInfo DisplayInfo =>
             new PeekResultDisplayInfo("Code Tour", null, "Code Tour: " + CodeTourManager.CurrentStep.Title, "Code Tour");

        public bool CanNavigateTo => true;

        public Action<IPeekResult, object, object> PostNavigationCallback => null;

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