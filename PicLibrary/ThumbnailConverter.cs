using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace PicLibrary
{
    class ThumbnailConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value!=null)
            {
                var thumbnailStream = (IRandomAccessStream)value;
                var image = new BitmapImage();
                image.SetSource(thumbnailStream);
                return image;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
