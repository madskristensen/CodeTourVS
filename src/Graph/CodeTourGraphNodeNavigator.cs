using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.GraphModel;
using Microsoft.VisualStudio.GraphModel.CodeSchema;
using Microsoft.VisualStudio.GraphModel.Schemas;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodeTourVS
{
    public class CodeTourGraphNodeNavigator : IGraphNavigateToItem
    {
        public IServiceProvider serviceProvider;

        public int GetRank(GraphObject graphObject) => 0; // not sure what this is for

        public void NavigateTo(GraphObject obj)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var node = (GraphNode)obj;
            SourceLocation loc = node.GetValue<SourceLocation>(CodeNodeProperties.SourceLocation);

            if (loc.FileName != null)
            {
                using (new NewDocumentStateScope(__VSNEWDOCUMENTSTATE2.NDS_TryProvisional, VSConstants.NewDocumentStateReason.SolutionExplorer))
                {
                    VsShellUtilities.OpenDocument(serviceProvider, loc.FileName.OriginalString);
                }
            }

            if (loc.StartPosition.Line > 0)
            {
                var dte = serviceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                dte?.ExecuteCommand("Edit.GoTo", loc.StartPosition.Line.ToString());
            }
        }
    }
}
