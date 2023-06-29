using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DeviceControlPanel.Classes
{

    public class BackgroundSwitcher
    {
        private readonly List<ImageSource> _backgroundImages;
        private int _currentIndex;

        public BackgroundSwitcher(string folderPath)
        {
            _backgroundImages = LoadBackgroundImages(folderPath);
            _currentIndex = 0;
        }

        public void SwitchBackground(Border border)
        {
            if (_backgroundImages.Count == 0)
                return;

            _currentIndex = (_currentIndex + 1) % _backgroundImages.Count;

            ImageBrush brush = new ImageBrush(_backgroundImages[_currentIndex]);
            brush.Stretch = Stretch.UniformToFill;
            border.Background = brush;
        }

        private List<ImageSource> LoadBackgroundImages(string folderPath)
        {
            List<ImageSource> images = new List<ImageSource>();

            try
            {
                foreach (string file in Directory.EnumerateFiles(folderPath))
                {
                    string extension = Path.GetExtension(file).ToLower();

                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp")
                    {
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(file, UriKind.RelativeOrAbsolute);
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        images.Add(image);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading background images: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return images;
        }
    }

}
