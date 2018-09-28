using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    /// 
   
    public partial class MainWindow : Window
    {
        WebClient client;
        string path;
        string URL;
        FolderBrowserDialog ofd;
        Task downloadTask;

        public MainWindow()
        {
            InitializeComponent();
            URL = "http://ap.imagensbrasil.org/images/imagens-lobos.jpg";
            InputURL.Text = URL;
            client = new WebClient();
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
            DownloadLabel.Content = "";
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            System.Windows.MessageBox.Show("Download complete");

            Dispatcher.Invoke(() =>
            {
                progressDownload.Value = 0;
                //DownloadLabel.Content = "Download Progress:";
                DownloadLabel.Content = "";
            });
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(()=> 
            {
                progressDownload.Minimum = 0;
                double receive = double.Parse(e.BytesReceived.ToString());
                double total = double.Parse(e.TotalBytesToReceive.ToString());
                double received = receive / total * 100;
                progressDownload.Value = received;
                DownloadLabel.Content = "Download Progress:" + received + "%";
            });
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            path = InputSavePath.Text;
            URL = InputURL.Text;

            Uri uri = new Uri("https://google.com");

            bool result = Uri.TryCreate(URL, UriKind.Absolute, out uri);

            if (!string.IsNullOrEmpty(URL) && result)
            {
                uri = new Uri(URL);
            }
            else
            {
                InvalidURL();
                return;
            }

            string fileName = System.IO.Path.GetFileName(uri.AbsolutePath);

            if (CanDownload())
            {
                if (Directory.Exists(path))
                {
                    downloadTask = new Task(() =>
                    {
                        client.DownloadFileAsync(uri, path + "/" + fileName);
                    });

                    downloadTask.Start();
                }
                else
                {
                    PathNull();
                }
            }
        }

        public void PrintFullPath(string arg)
        {
           System.Windows.MessageBox.Show(arg);
        }


        public bool CanDownload()
        {
            if(!string.IsNullOrWhiteSpace(InputURL.Text) && !string.IsNullOrWhiteSpace(InputSavePath.Text))
            {
                return true;
            }

            return false;
        }

        private void PathNull()
        {
            System.Windows.MessageBox.Show("The path is null.");
        }

        private void DownloadStart()
        {
            System.Windows.MessageBox.Show("Download started.");
        }

        private void InvalidURL()
        {
            System.Windows.MessageBox.Show("The URL is invalid.");
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            ofd = new FolderBrowserDialog();
            
            using (var ofd = new FolderBrowserDialog())
            {
                var result = ofd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(ofd.SelectedPath))
                {
                    InputSavePath.Text = ofd.SelectedPath;
                    path = InputSavePath.Text;
                }
            }
        }
    }
}
