using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace CodeTourVS
{
    internal sealed class GoToStepCommand
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();

            var cmdId = new CommandID(PackageGuids.CodeTourCmdSet, PackageIds.GoToStepCommand);
            var cmd = new OleMenuCommand(Execute, cmdId);
            cmd.ParametersDescription = "*";
            commandService.AddCommand(cmd);
        }

        private static void Execute(object sender, EventArgs e)
        {
            if (e is OleMenuCmdEventArgs cmdArgs && cmdArgs.InValue is string arg)
            {
                var parameters = arg.Split('|');

                if (parameters.Length == 2 && int.TryParse(parameters[1], out var stepNumber))
                {
                    var tourFile = parameters[0];
                    NavigateAsync(tourFile, stepNumber).FileAndForget(nameof(GoToStepCommand));
                }
            }
        }

        private static async Task NavigateAsync(string tourFile, int stepNumber)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var tourFolder = Path.GetDirectoryName(tourFile);
            var manager = await CodeTourManager.FromFolderAsync(tourFolder);
            var tour = manager.GetTour(tourFile);

            if (tour != null && tour.Steps.Count() >= stepNumber)
            {
                var step = tour.Steps.ElementAt(stepNumber);

                var dte = await AsyncServiceProvider.GlobalProvider.GetServiceAsync<DTE, DTE>();
                dte?.ExecuteCommand("Edit.GoTo", step.Line.ToString());
            }

        }
    }
}
