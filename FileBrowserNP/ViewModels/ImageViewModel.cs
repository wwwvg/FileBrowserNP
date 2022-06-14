using FileBrowserNP.Models.MyEventArgs;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileBrowserNP.ViewModels
{
    public class ImageViewModel : BindableBase
    {
        public ImageViewModel(string fullPath)
        {
            _fullPath = fullPath;
            SetImage();
        }

        private string _fullPath;

        #region СВОЙСТВА

        private ImageSource _picture;
        public ImageSource Picture
        {
            get { return _picture; }
            set { SetProperty(ref _picture, value); }
        }
        #endregion

        #region ВЫВОД КАРТИНКИ НА ЭКРАН

        void SetImage()
        {
            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bi.UriSource = new Uri(_fullPath, UriKind.Absolute);
                bi.EndInit();

                Picture = bi;

                // Image = new BitmapImage(new Uri(_fileInfoModel.FullPath, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        #endregion

        private void OnDeleted()
        {
            Picture = null;
        }
    }
}
