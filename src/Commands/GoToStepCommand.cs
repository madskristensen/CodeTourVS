using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Task = System.Threading.Tasks.Task;

namespace CodeTourVS
{
    internal sealed class GoToStepCommand
    {
        private readonly AsyncPackage _package;
        private readonly DTE _dte;

        public GoToStepCommand(AsyncPackage package, DTE dte)
        {
            _package = package;
            _dte = dte;
        }

        [Import]
        private IPeekBroker PeekBroker { get; set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            DTE dte = await AsyncServiceProvider.GlobalProvider.GetServiceAsync<DTE, DTE>();
            IComponentModel compositionService = await package.GetServiceAsync<SComponentModel, IComponentModel>();

            var instance = new GoToStepCommand(package, dte);

            var cmdId = new CommandID(PackageGuids.CodeTourCmdSet, PackageIds.GoToStepCommand);
            var cmd = new OleMenuCommand(instance.Execute, cmdId);
            commandService.AddCommand(cmd);

            compositionService.DefaultCompositionService.SatisfyImportsOnce(instance);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            using (new NewDocumentStateScope(__VSNEWDOCUMENTSTATE2.NDS_TryProvisional, VSConstants.NewDocumentStateReason.SolutionExplorer))
            {
                VsShellUtilities.OpenDocument(_package, CodeTourManager.CurrentStep.AbsoluteFile);
            }

            if (CodeTourManager.CurrentStep.Line > 0)
            {
                _dte.ExecuteCommand("Edit.GoTo", CodeTourManager.CurrentStep.Line.ToString());
            }

            PeekBroker.TriggerPeekSession(GetTextView(), CodeTourPeekRelationship.RelationshipName);
        }

        //private async Task NavigateAsync(string tourFile, int stepNumber)
        //{
        //    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        //    var tourFolder = Path.GetDirectoryName(tourFile);
        //    var manager = await CodeTourManager.FromFolderAsync(tourFolder);
        //    var tour = manager.GetTour(tourFile);

        //    if (tour != null && tour.Steps.Count() >= stepNumber)
        //    {
        //        var step = tour.Steps.ElementAt(stepNumber);

        //        var dte = await AsyncServiceProvider.GlobalProvider.GetServiceAsync<DTE, DTE>();
        //        dte?.ExecuteCommand("Edit.GoTo", step.Line.ToString());
        //    }
        //}

        public IWpfTextView GetTextView()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var compService = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            Assumes.Present(compService);

            IVsEditorAdaptersFactoryService editorAdapter = compService.GetService<IVsEditorAdaptersFactoryService>();

            var textManager = ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager)) as IVsTextManager;
            Assumes.Present(textManager);

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out IVsTextView activeView));

            return editorAdapter.GetWpfTextView(activeView);
        }
    }
}
