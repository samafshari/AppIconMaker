﻿using System;
using System.Collections.Generic;
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Path = System.IO.Path;
using System.Diagnostics;
using System.Reflection;

namespace AppIconMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        bool OverWrite => chkOverwrite.IsChecked.GetValueOrDefault();

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            var infile = txtInput.Text;
            var outdir = txtOutput.Text;

            if (!File.Exists(infile))
            {
                MessageBox.Show("Input file not found!", "Error");
                return;
            }

            if (!Directory.Exists(outdir))
            {
                MessageBox.Show("Output directory not found!", "Error");
                return;
            }

            var fi = new FileInfo(infile);
            var suffix = rdoIos.IsChecked.GetValueOrDefault() ? "ios" :
                rdoWatch.IsChecked.GetValueOrDefault() ? "iwatch" : "droid";
            outdir = Path.Combine(outdir, $"{fi.Name}_{suffix}");
            Directory.CreateDirectory(outdir);
            if (rdoIos.IsChecked.GetValueOrDefault())
                DoiOS(infile, outdir);
            else if (rdoWatch.IsChecked.GetValueOrDefault())
                DoiWatch(infile, outdir);
            else
                DoDroid(infile, outdir);

            MessageBox.Show("Operation Done.");
        }

        public string ReadEmbeddedResource(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"AppIconMaker.{resource}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        public void CopyEmbeddedResource(string from, string to, bool overwrite)
        {
            if (!overwrite && File.Exists(to)) return;
            File.Delete(to);
            File.WriteAllText(to, ReadEmbeddedResource(from));
        }

        private void btnBrowseInput_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.DefaultExt = "png";
            dialog.ShowDialog();
            
            SetInFile(dialog.FileName);
        }

        void SetInFile(string filename)
        {
            txtInput.Text = filename;
            txtOutput.Text = new FileInfo(txtInput.Text).DirectoryName;
            img.Source = new ImageSourceConverter().ConvertFromString(txtInput.Text) as ImageSource;
        }

        private void btnBrowseOutput_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            txtOutput.Text = dialog.SelectedPath;
        }

        class Size
        {
            public string Name { get; set; }
            public string Dimensions { get; set; }
            public Size(string name, string dimensions)
            {
                Name = name;
                Dimensions = dimensions;
            }
        }

        void DoiWatch(string infile, string outdir)
        {
            CopyEmbeddedResource("ContentsWatch.json", Path.Combine(outdir, "Contents.json"), OverWrite);

            var sizes = new Size[]
            {
                new Size("Icon-24@2x.png", "48x48"),
                new Size("Icon-27.5@2x.png", "55x55"),
                new Size("Icon-29@2x.png", "58x58"),
                new Size("Icon-29@3x.png", "87x87"),
                new Size("Icon-40@2x.png", "80x80"),
                new Size("Icon-44@2x.png", "88x88"),
                new Size("icon-172.png", "172x172"),
                new Size("icon-196.png", "196x196"),
                new Size("icon-216.png", "216x216"),
                new Size("icon-1024.png", "1024x1024"),
                new Size("Icon-Small-50x50@2x.png", "100x100")
            };

            foreach (var item in sizes)
            {
                (int width, int height) = ExtractSizes(item.Dimensions);
                Convert(infile, Path.Combine(outdir, item.Name), width, height, false);
            }
        }

        void DoiOS(string infile, string outdir)
        {
            CopyEmbeddedResource("Contents.json", Path.Combine(outdir, "Contents.json"), OverWrite);

            var sizes = new Size[]
            {
                new Size("icon_rect_1024.png", "1024x1024"),
                new Size("Icon-App-20x20@1x.png", "20x20"),
                new Size("Icon-App-20x20@2x.png", "40x40"),
                new Size("Icon-App-20x20@3x.png", "60x60"),
                new Size("Icon-App-29x29@1x.png", "29x29"),
                new Size("Icon-App-29x29@2x.png", "58x58"),
                new Size("Icon-App-29x29@2x1.png", "58x58"),
                new Size("Icon-App-29x29@3x.png", "87x87"),
                new Size("Icon-App-40x40@1x.png", "40x40"),
                new Size("Icon-App-40x40@2x.png", "80x80"),
                new Size("Icon-App-40x40@2x1.png", "80x80"),
                new Size("Icon-App-40x40@3x.png", "120x120"),
                new Size("Icon-App-57x57@1x.png", "57x57"),
                new Size("Icon-App-57x57@2x.png", "114x114"),
                new Size("Icon-App-60x60@1x.png", "60x60"),
                new Size("Icon-App-60x60@2x.png", "120x120"),
                new Size("Icon-App-60x60@3x.png", "180x180"),
                new Size("Icon-App-72x72@1x.png", "72x72"),
                new Size("Icon-App-72x72@2x.png", "144x144"),
                new Size("Icon-App-76x76@1x.png", "76x76"),
                new Size("Icon-App-76x76@2x.png", "152x152"),
                new Size("Icon-App-76x76@3x.png", "228x228"),
                new Size("Icon-App-83.5x83.5@2x.png", "167x167"),
                new Size("Icon-Small-50x50@1x.png", "50x50"),
                new Size("Icon-Small-50x50@2x.png", "100x100"),
            };

            foreach (var item in sizes)
            {
                (int width, int height) = ExtractSizes(item.Dimensions);
                Convert(infile, Path.Combine(outdir, item.Name), width, height, false);
            }
        }

        void DoDroid(string infile, string outdir)
        {
            Directory.CreateDirectory(Path.Combine(outdir, "drawable-hdpi"));
            Directory.CreateDirectory(Path.Combine(outdir, "drawable-xhdpi"));
            Directory.CreateDirectory(Path.Combine(outdir, "drawable-xxhdpi"));

            Convert(infile, Path.Combine(outdir, "drawable-hdpi", "icon.png"), 72, 72, true);
            Convert(infile, Path.Combine(outdir, "drawable-xhdpi", "icon.png"), 96, 96, true);
            Convert(infile, Path.Combine(outdir, "drawable-xxhdpi", "icon.png"), 144, 144, true);
        }

        void Convert(string infile, string outfile, int width, int height, bool alpha)
        {
            if (File.Exists(outfile) && OverWrite)
                File.Delete(outfile);
            var sourceBmp = Bitmap.FromFile(infile);
            ResizeImage(sourceBmp, width, height, alpha).Save(outfile);
        }

        static Bitmap ResizeImage(System.Drawing.Image image, int width, int height, bool alpha)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var pixelFormat = alpha ? System.Drawing.Imaging.PixelFormat.Format32bppArgb : System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            var destImage = new Bitmap(width, height, pixelFormat);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static (int w, int h) ExtractSizes(string sizeString)
        {
            var fields = sizeString.Split(',', 'x', 'X', ' ');
            if (fields.Length != 2) throw new ArgumentException("Size string is incorrect.");
            return (int.Parse(fields[0]), int.Parse(fields[1]));
        }

        private void img_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                SetInFile(files[0]);
            }
            Debug.WriteLine(e.Source);
        }
    }
}
