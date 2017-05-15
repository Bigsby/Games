using Microsoft.OneDrive.Sdk;
using Windows.Storage;

namespace UniversalFlowProcessor.ViewModels
{
    static class Engine
    {
        public static OneDriveClient Client;
        public static Item Source;
        public static StorageFolder Target;
    }
}
