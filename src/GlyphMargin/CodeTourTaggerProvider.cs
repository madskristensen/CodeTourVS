using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace CodeTourVS
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(CodeTourTag))]
    class CodeTourTaggerProvider : IViewTaggerProvider
    {
        [Import]
        public ITextDocumentFactoryService DocumentService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {

            if (DocumentService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out ITextDocument doc))
            {
                CodeTourManager manager = null;

                ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    manager = await CodeTourManager.FromSourceFileAsync(doc.FilePath);
                });

                var steps = manager.GetStepsContainingFile(doc.FilePath);

                if (steps.Any())
                {
                    return new CodeTourTagger(textView, doc, steps) as ITagger<T>;
                }

            }

            return null;
        }
    }
}