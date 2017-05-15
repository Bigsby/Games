using Microsoft.OneDrive.Sdk;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace UniversalFlowProcessor.ViewModels
{
    class MainViewModel : FrameObservableObject
    {
        const string _clientId = "000000004018EE88";

        public MainViewModel(Frame frame) : base(frame)
        {
        }

        public ICommand SignIn => new ActionCommand("", async p =>
        {
            await DoWithProgress(async () =>
            {
                var authenticator = new OnlineIdAuthenticationProvider(new[] { "onedrive.readwrite" });
                await authenticator.AuthenticateUserAsync().ConfigureAwait(false);
                if (!authenticator.IsAuthenticated)
                    return;

                Engine.Client = new OneDriveClient("https://api.onedrive.com/v1.0", authenticator);
                await Navigate(typeof(FolderSelector)).ConfigureAwait(false);
            });
        });
    }
}