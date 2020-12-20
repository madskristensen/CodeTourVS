using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;

namespace CodeTourVS
{
    internal class CodeTourPeekResultPresentation : IPeekResultPresentation
    {
        public double ZoomLevel
        {
            get { return 1.0; }
            set { }
        }

        public bool IsDirty => false;

        public bool IsReadOnly => false;

        public event EventHandler<RecreateContentEventArgs> RecreateContent = delegate { };
        public event EventHandler IsDirtyChanged
        {
            add { }
            remove { }
        }

        public event EventHandler IsReadOnlyChanged
        {
            add { }
            remove { }
        }

        public bool CanSave(out string defaultPath)
        {
            defaultPath = null;
            return false;
        }

        public IPeekResultScrollState CaptureScrollState()
        {
            return null;
        }

        public void Close()
        {
        }

        public UIElement Create(IPeekSession session, IPeekResultScrollState scrollState)
        {
            var image = new Image();

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                image.Source = await KnownMonikers.Reference.ToImageSourceAsync(256);
            });


            return new StepView(CodeTourManager.CurrentStep);
        }

        public void Dispose()
        {
        }

        public void ScrollIntoView(IPeekResultScrollState scrollState)
        {
        }

        public void SetKeyboardFocus()
        {
        }

        public bool TryOpen(IPeekResult otherResult) => false;

        public bool TryPrepareToClose() => true;

        public bool TrySave(bool saveAs) => true;
    }
}