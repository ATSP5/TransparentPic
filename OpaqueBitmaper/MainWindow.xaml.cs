using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Encoder = System.Drawing.Imaging.Encoder;

namespace OpaqueBitmaper
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        BitmapImage image_in, mask_image_in, image_out;
        int width, height;
        WriteableBitmap writeableBitmap, writableBitmapMask, writableBitmapOut;
        int stride;
        int arraySize;
        byte[] pixels_in, pixels_out, A_Mask;

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image files (*.png;*.jpeg;*.JPEG;*.jpg;*.JPG*.bmp;*.BMP)|*.png;*.jpeg*.JPEG;*.jpg;*.JPG;.bmp;*.BMP|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {

                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image_out));

                using (var fileStream = new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

            }
        }

        bool loaded_in = false, loaded_m=false;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if(loaded_in==true&&loaded_m==true)
            {
                int color = 0;
                int j = 0;
                pixels_out = new byte[arraySize];
                for (int i = 0; i < pixels_in.Length / 4; ++i)
                {
                    color = (A_Mask[j] + A_Mask[j + 1] + A_Mask[j + 2]) / 3;
                    pixels_out[j] = pixels_in[j+0]; //R
                    pixels_out[j + 1] = pixels_in[j+1];//G
                    pixels_out[j + 2] = pixels_in[j+2];//B
                    pixels_out[j + 3] = (byte)color;//A
                    j += 4;
                }
                Int32Rect rect = new Int32Rect(0, 0, width, height);
                writableBitmapOut = new WriteableBitmap(width, height, image_in.DpiX, image_in.DpiY, PixelFormats.Pbgra32, image_in.Palette);
                writableBitmapOut.WritePixels(rect, pixels_out, stride, 0);
                image_out = new BitmapImage();
                image_out = ConvertWriteableBitmapToBitmapImage(writableBitmapOut);
                ResultImage.Source = image_out;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter= "Image files (*.png;*.jpeg;*.JPEG;*.jpg;*.JPG)|*.png;*.jpeg*.JPEG;*.jpg;*.JPG|All files (*.*)|*.*";
            
            if (openFileDialog.ShowDialog()==true)
            {
                image_in = new BitmapImage(new Uri(openFileDialog.FileName));
                ColoredImage.Source = image_in;
            }
            if(image_in!=null)
            {
                width = (int)image_in.Width;
                height = (int)image_in.Height;
                writeableBitmap = new WriteableBitmap(image_in);              
                stride = width * ((this.writeableBitmap.Format.BitsPerPixel + 7) / 8);
                arraySize = stride * height;
                pixels_in = new byte[arraySize];
                writeableBitmap.CopyPixels(pixels_in, stride, 0);
                loaded_in = true;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.JPEG;*.jpg;*.JPG)|*.png;*.jpeg*.JPEG;*.jpg;*.JPG|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                mask_image_in = new BitmapImage(new Uri(openFileDialog.FileName));
                MaskImage.Source = mask_image_in;
            }
            if(mask_image_in!=null)
            {
                width = (int)mask_image_in.Width;
                height = (int)mask_image_in.Height;
                writableBitmapMask = new WriteableBitmap(mask_image_in);
                stride = width * ((this.writableBitmapMask.Format.BitsPerPixel + 7) / 8);
                arraySize = stride * height;
                A_Mask = new byte[arraySize];
                writableBitmapMask.CopyPixels(A_Mask, stride, 0);
                loaded_m = true;
            }
        }
        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

    }
}
