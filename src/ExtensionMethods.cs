using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using EnvDTE80;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodeTourVS
{
    public static class ExtensionMethods
    {
        public static string GetToursFolder(this Solution2 solution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var slnDir = Path.GetDirectoryName(solution.FullName);
            return Path.Combine(slnDir, Constants.ToursDirName);
        }

        public static async Task<BitmapSource> ToImageSourceAsync(this ImageMoniker moniker, int size)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            IVsUIObject result = await ToUiObjectAsync(moniker, size);

            result.get_Data(out var data);

            if (data == null)
                return null;

            return data as BitmapSource;
        }

        public static async Task<IVsUIObject> ToUiObjectAsync(this ImageMoniker moniker, int size)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsImageService2 imageService = await AsyncServiceProvider.GlobalProvider.GetServiceAsync<SVsImageService, IVsImageService2>();
            var backColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);

            var imageAttributes = new ImageAttributes
            {
                Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags | unchecked((uint)_ImageAttributesFlags.IAF_Background),
                ImageType = (uint)_UIImageType.IT_Bitmap,
                Format = (uint)_UIDataFormat.DF_WPF,
                LogicalHeight = size,
                LogicalWidth = size,
                Background = (uint)backColor.ToArgb(),
                StructSize = Marshal.SizeOf(typeof(ImageAttributes))
            };

            return imageService.GetImage(moniker, imageAttributes);
        }
    }
}
