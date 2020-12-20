using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.GraphModel;
using Microsoft.VisualStudio.GraphModel.CodeSchema;
using Microsoft.VisualStudio.GraphModel.Schemas;
using Microsoft.VisualStudio.Shell;

namespace CodeTourVS
{
    public class CodeTourGraphNodeNavigator : IGraphNavigateToItem
    {
        public IServiceProvider serviceProvider;

        public int GetRank(GraphObject graphObject) => 0;

        public void NavigateTo(GraphObject obj)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var node = (GraphNode)obj;
            SourceLocation loc = node.GetValue<SourceLocation>(CodeNodeProperties.SourceLocation);

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                CodeTourManager manager = await CodeTourManager.FromSourceFileAsync(loc.FileName.OriginalString);

                IEnumerable<Step> stepsInFile = manager.GetStepsContainingFile(loc.FileName.OriginalString);
                CodeTourManager.CurrentStep = stepsInFile.FirstOrDefault(s => s.Line == loc.StartPosition.Line);

                if (CodeTourManager.CurrentStep != null)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    EnvDTE.DTE dte = await AsyncServiceProvider.GlobalProvider.GetServiceAsync<EnvDTE.DTE, EnvDTE.DTE>();
                    dte?.ExecuteCommand("CodeTours.GoToStep");
                }
            }).FileAndForget(nameof(CodeTourGraphNodeNavigator));
        }
    }
}
