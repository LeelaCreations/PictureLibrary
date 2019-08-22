using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PicLibrary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        
        ObservableCollection<BitmapImage> fullImages;
        UploadImages uploadImages;
        public List<string> albumNames { get; set; }
        public MyPics selectedPic { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            uploadImages = new UploadImages();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var picLibrary = ApplicationData.Current.LocalFolder;
            await uploadImages.GetAllImagesAsync(picLibrary);
            albumNames = await uploadImages.GetAllAlbums(picLibrary);
            albumsListView.DataContext = albumNames;

        }

        private async void UploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            await mycontentDialoge.ShowAsync();
        }

        private async void Albums_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            await albumContentDialoge.ShowAsync();
        }

        private async void GvPhotoLibrary_ItemClick(object sender, ItemClickEventArgs e)
        {
            fullImages = new ObservableCollection<BitmapImage>();
            selectedPic = (MyPics)e.ClickedItem;
            fullImages = await uploadImages.GetFlipViewOfImages(selectedPic);
            imageFlipView.DataContext = fullImages;
            var button = sender as Button;
            await imageFlipView.ShowAsync();
        }

        private async void MycontentDialoge_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            uploadImages = new UploadImages();
            await uploadImages.uploadingImagesAsync();
            this.Frame.Navigate(typeof(MainPage));
        }

        private void AlbumContentDialoge_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            uploadImages = new UploadImages();
            uploadImages.addAlbum(albumName.Text);
        }

        private async void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            uploadImages = new UploadImages();
            await uploadImages.uploadCameraPic();
            this.Frame.Navigate(typeof(MainPage));
            
        }

        private void ImageFlipView_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
    public class MyPics
    {
        public string FileName { get; set; }
        public StorageItemThumbnail Thumbnail { get; set; }
    }
}
