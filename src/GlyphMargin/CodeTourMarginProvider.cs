using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace CodeTourVS
{
    public class CodeTourMarginProvider
    {
        //[Export(typeof(IWpfTextViewMarginProvider))]
        [Name("MarginRightFactory")]
        [Order(After = PredefinedMarginNames.LineNumber)]
        [MarginContainer(PredefinedMarginNames.Left)]
        [ContentType("text")]
        [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
        public class BrowserMarginRightProvider : IWpfTextViewMarginProvider
        {
            [Import]
            public ITextDocumentFactoryService DocumentService { get; set; }

            [Import(typeof(SVsServiceProvider))]
            public IServiceProvider ServiceProvider { get; set; }

            public IWpfTextViewMargin CreateMargin(IWpfTextViewHost host, IWpfTextViewMargin marginContainer)
            {
                if (!DocumentService.TryGetTextDocument(host.TextView.TextDataModel.DocumentBuffer, out ITextDocument doc))
                {
                    return null;
                }
                
                return host.TextView.Properties.GetOrCreateSingletonProperty(() => 
                    new CodeTourrMargin(host.TextView, doc, ServiceProvider)
                );
            }
        }

    }
}
