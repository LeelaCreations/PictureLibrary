using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.Storage.FileProperties;
using System.Collections.ObjectModel;
using Windows.Media.Capture;
using Windows.UI.Xaml.Media.Imaging;

namespace PicLibrary
{
    class UploadImages
    {
        public ObservableCollection<MyPics> myPics { get; private set; }
        public ObservableCollection<BitmapImage> fullImages { get; private set; }
        public BitmapImage fullImage { get; set; }
        public string AlbumName { get; set; }
        public List<string> albumNameList { get; set; }
        public static List<StorageFile> allPicFiles { get; set; }
        public static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        public static IReadOnlyList<StorageFile> images;
        public UploadImages()
        {
            myPics = new ObservableCollection<MyPics>();
            fullImages = new ObservableCollection<BitmapImage>();
        }
        static UploadImages()
        {

            allPicFiles =  new List<StorageFile>();
        }
        public async Task uploadingImagesAsync()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            images = await picker.PickMultipleFilesAsync();
            foreach (var image in images)
            {
                var stream = await image.CopyAsync(storageFolder, image.Name, NameCollisionOption.GenerateUniqueName);
            }
            return;
        }
        public async void addAlbum(string name)
        {
            string albumName = name;
            StorageFolder newFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(albumName);
            var messageDialogue = new MessageDialog("Album has been added");
            await messageDialogue.ShowAsync();
        }
        public async Task uploadCameraPic()
        {
            CameraCaptureUI cameraCapture = new CameraCaptureUI();
            cameraCapture.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            StorageFile pic = await cameraCapture.CaptureFileAsync(CameraCaptureUIMode.Photo);
            await pic.CopyAsync(ApplicationData.Current.LocalFolder, pic.Name, NameCollisionOption.GenerateUniqueName);
            
        }
        public async Task<List<string>> GetAllAlbums(StorageFolder localFolder)
        {
            albumNameList = new List<string>();

            var items = await localFolder.GetItemsAsync();
            foreach (var item in items)
            {
                if (item.IsOfType(StorageItemTypes.Folder))
                {
                    albumNameList.Add(item.Name);
                }
                else
                {
                    continue;
                }
            }
            return albumNameList;
        }
        public async Task GetAllImagesAsync(StorageFolder localFolder)
        {
           
            var items = await localFolder.GetItemsAsync();
            foreach (var item in items)
            {
                if (item.IsOfType(StorageItemTypes.Folder))
                {
                    await GetAllImagesAsync(item as StorageFolder);
                }
                else
                {
                    var file = item as StorageFile;
                    allPicFiles.Add(file);
                    var myPic = new MyPics { FileName = file.Name };
                    myPic.Thumbnail = await file.GetThumbnailAsync(ThumbnailMode.PicturesView);
                    myPics.Add(myPic);
                }
            }
        }
        public async Task<ObservableCollection<BitmapImage>> GetFlipViewOfImages(MyPics selectedPic)
        {
            
            foreach (var file in UploadImages.allPicFiles)
            {
                if (file.Name == selectedPic.FileName)
                {
                    var selectedStream = await file.OpenReadAsync();
                    fullImage = new BitmapImage();
                    fullImage.SetSource(selectedStream);
                    fullImages.Add(fullImage);
                }

            }
            foreach (var file in UploadImages.allPicFiles)
            {
                if (file.Name != selectedPic.FileName)
                {
                    var stream = await file.OpenReadAsync();
                    fullImage = new BitmapImage();
                    await fullImage.SetSourceAsync(stream);
                    fullImages.Add(fullImage);
                }
            }
            UploadImages.allPicFiles.Clear();
            return fullImages;
        }
    }
}

