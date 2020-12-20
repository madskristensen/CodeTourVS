using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace CodeTourVS
{
    internal class CodeTourGlyphFactory : IGlyphFactory
    {
        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            var image = new Image();

            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                image.Source = await KnownMonikers.Favorite.ToImageSourceAsync(10);
            });

            return image;
        }
    }
}