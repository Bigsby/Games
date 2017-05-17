using System;
using Microsoft.OneDrive.Sdk;
using Windows.Storage;
using System.Threading.Tasks;

namespace UniversalFlowProcessor.ViewModels
{
    static class Engine
    {
        public static OneDriveClient Client;
        public const int ImageSize = 720;
        public const double DPIs = 240;
        public static Item Source;
        public static Item OthersFolder;
        public static Item HandledFolder;
        private const string _dataFolder = "data";
        public const string _imagesFolder = "images";
        public static StorageFolder BaseFolder;
        public static async Task<StorageFolder> DataFolder() => await BaseFolder.GetFolderAsync(_dataFolder);
        public static async Task<StorageFolder> ImagesFolder() => await BaseFolder.GetFolderAsync(_imagesFolder);
    }
}