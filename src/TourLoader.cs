using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace CodeTourVS
{
    public class TourLoader : IVsInfoBarUIEvents
    {
        private readonly Solution2 _solution;
        private bool _isVisible = false;
        private IVsInfoBarUIElement _uiElement;
        private readonly static InfoBarHyperlink _typingsFolderHyperlink;

        private readonly static InfoBarModel _infoBarModel =
           new InfoBarModel(
               new[] {
                    new InfoBarTextSpan("Code Tours for solution found. "),
                    _typingsFolderHyperlink = new InfoBarHyperlink("Show them")
               },
               KnownMonikers.PlayStepGroup,
               true);

        public TourLoader(Solution2 solution)
        {
            _solution = solution;
        }

        public async Task HandleOpenSolutionAsync()
        {
            if (ToursExist() && !SolutionFolderExist())
            {
                await ShowInfoBarAsync();
            }
        }

        public void AddCodeToursToSolution()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Project solFolder = _solution.AddSolutionFolder(Constants.VirtualFolderName);
            var toursDir = GetToursFolder();

            foreach (var file in Directory.EnumerateFiles(toursDir, $"*{Constants.TourFileExtension}"))
            {
                solFolder.ProjectItems.AddFromFile(file);
            }
        }

        public void CloseInfoBar()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_isVisible && _uiElement != null)
            {
                _uiElement.Close();
            }
        }

        private  bool ToursExist()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var toursDir = GetToursFolder();

            return Directory.Exists(toursDir);
        }

        private string GetToursFolder()
        {
            var slnDir = Path.GetDirectoryName(_solution.FullName);
            return Path.Combine(slnDir, Constants.ToursDirName);
        }

        private  bool SolutionFolderExist()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solutionFolder = _solution.Projects
                .Cast<Project>()
                .FirstOrDefault(p => p.Kind == ProjectKinds.vsProjectKindSolutionFolder && p.Name == Constants.VirtualFolderName);

            return solutionFolder != null;
        }

        public async Task ShowInfoBarAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (_isVisible)
            {
                return;
            }

            if (!await TryCreateInfoBarUIAsync(_infoBarModel))
            {
                return;
            }

            _uiElement.Advise(this, out _);
            ToolWindowPane solutionExplorer = GetSolutionExplorerPane();

            if (solutionExplorer != null)
            {
                solutionExplorer.AddInfoBar(_uiElement);
                _isVisible = true;
            }
        }

        private async Task<bool> TryCreateInfoBarUIAsync(IVsInfoBar infoBar)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsInfoBarUIFactory infoBarUIFactory = await AsyncServiceProvider.GlobalProvider.GetServiceAsync<SVsInfoBarUIFactory, IVsInfoBarUIFactory>();
           
            _uiElement = infoBarUIFactory.CreateInfoBar(infoBar);
            return _uiElement != null;
        }

        private static ToolWindowPane GetSolutionExplorerPane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
            Assumes.Present(uiShell);

            var slnExplorerGuid = new Guid(ToolWindowGuids80.SolutionExplorer);

            if (ErrorHandler.Succeeded(uiShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref slnExplorerGuid, out IVsWindowFrame frame)))
            {
                if (ErrorHandler.Succeeded(frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out var pane)))
                {
                    return pane as ToolWindowPane;
                }
            }

            return null;
        }

        public void OnClosed(IVsInfoBarUIElement infoBarUIElement)
        {
            _isVisible = false;
        }

        public void OnActionItemClicked(IVsInfoBarUIElement infoBarUIElement, IVsInfoBarActionItem actionItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (actionItem.Equals(_typingsFolderHyperlink))
            {
                AddCodeToursToSolution();
                infoBarUIElement.Close();
            }
        }
    }
}
