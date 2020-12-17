using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SolutionEvents = Microsoft.VisualStudio.Shell.Events.SolutionEvents;
using Task = System.Threading.Tasks.Task;

namespace CodeTourVS
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("a139934a-4daf-44ff-af02-aa018c2ae51f")]
    [ProvideBindingPath()]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class CodeTourVSPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // Since this package might not be initialized until after a solution has finished loading,
            // we need to check if a solution has already been loaded and then handle it.
            var isSolutionLoaded = await IsSolutionLoadedAsync();

            await JoinableTaskFactory.SwitchToMainThreadAsync();
            var dte = await GetServiceAsync(typeof(DTE)) as DTE;
            Assumes.Present(dte);

            var loader = new CodeTourInfoBar(dte.Solution as Solution2);

            if (isSolutionLoaded)
            {
                loader.HandleOpenSolutionAsync()
                    .FileAndForget(nameof(CodeTourVSPackage));
            }

            // Listen for subsequent solution events
            SolutionEvents.OnAfterBackgroundSolutionLoadComplete += delegate
            {
                loader.HandleOpenSolutionAsync()
                    .FileAndForget(nameof(CodeTourVSPackage));
            };

            SolutionEvents.OnAfterCloseSolution += delegate
            {
                loader.CloseInfoBar();
            };
            await GoToStepCommand.InitializeAsync(this);
        }

        private async Task<bool> IsSolutionLoadedAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            var solService = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            Assumes.Present(solService);

            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out var value));

            return value is bool isSolOpen && isSolOpen;
        }
    }
}
