using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace CodeTourVS
{
    internal class CodeTourrMargin : DockPanel, IWpfTextViewMargin
    {
        private IWpfTextView _textView;
        private ITextDocument _document;
        private readonly IServiceProvider _provider;

        public CodeTourrMargin(IWpfTextView textView, ITextDocument document, IServiceProvider provider)
        {
            _textView = textView;
            _document = document;
            _provider = provider;
            
            Initialized += CodeTourrMargin_Initialized;
            VerticalAlignment = VerticalAlignment.Top;
        }

        private void CodeTourrMargin_Initialized(object sender, EventArgs e)
        {
            var line = _textView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(4);
            var height = _textView.GetTextViewLineContainingBufferPosition(line.Start).Top;
            
            ThreadHelper.JoinableTaskFactory.RunAsync(async() => {
                
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var Image = new Image
                {
                    Margin = new Thickness(0, height, 0, 0),
                    Width = 12,
                    Height = 12,
                    Source = await KnownMonikers.PlayStep.ToImageSourceAsync(12)
                };

                Children.Add(Image);
            });
        }

        public FrameworkElement VisualElement => this;

        public double MarginSize => 12;

        public bool Enabled => true;

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return this;
        }

        public void Dispose()
        {
            // TODO: dispose things
        }
    }
}