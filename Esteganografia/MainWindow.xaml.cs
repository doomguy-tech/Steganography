using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Steganography.Controller;
using System.ComponentModel;
using System.Collections;

namespace Steganography
{
    /// <summary>
    /// Logic MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

        private readonly BackgroundWorker worker_Reveal = new BackgroundWorker();

        public MainWindow()
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            //worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            //worker.WorkerReportsProgress = true;

            //worker_Reveal.WorkerReportsProgress = true;
            worker_Reveal.DoWork += workerReveal_DoWork;
            worker_Reveal.RunWorkerCompleted += workerReveal_RunWorkerCompleted;
            //worker_Reveal.ProgressChanged += new ProgressChangedEventHandler(workerReveal_ProgressChanged);

            InitializeComponent();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Hashtable parameters =(Hashtable) e.Argument;
            SteganographyLogic Logic = new SteganographyLogic();
            Logic.HideText(parameters["textBoxSrc"].ToString(), parameters["textBoxDest"].ToString(), parameters["textBoxMessage"].ToString());

        }

        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            labelStatus.Content = "";
            button1.IsEnabled = true;
            button2.IsEnabled = true;
        }

        private void workerReveal_DoWork(object sender, DoWorkEventArgs e)
        {
            Hashtable parameters = (Hashtable)e.Argument;
            SteganographyLogic Logic = new SteganographyLogic();
            e.Result= Logic.RevealText(parameters["textBoxDest"].ToString(), (bool)parameters["checkBoxGarbageAvoid"]);
        }

        private void workerReveal_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            labelStatus.Content = "";
            textBoxShowMessage.Text = e.Result.ToString();
            button1.IsEnabled = true;
            button2.IsEnabled = true;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Hashtable parameters = new Hashtable();
            try
            {

                parameters.Add("textBoxSrc", textBoxSrc.Text);
                parameters.Add("textBoxDest", textBoxDest.Text);
                parameters.Add("textBoxMessage", textBoxMessage.Text);
                worker.RunWorkerAsync(parameters);
                labelStatus.Content = "Hiding the message, please wait...";
                button1.IsEnabled = false;
                button2.IsEnabled = false;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
           Hashtable parameters = new Hashtable();
 
           try
           {

               parameters.Add("textBoxDest", textBoxDest.Text);
               parameters.Add("checkBoxGarbageAvoid", (bool)checkBoxGarbageAvoid.IsChecked);
               worker_Reveal.RunWorkerAsync(parameters);
               labelStatus.Content = "Revealing the message, please wait...";
               button1.IsEnabled = false;
               button2.IsEnabled = false;
               BitmapImage myBitmapImage = new BitmapImage();
               myBitmapImage.BeginInit();
               myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
               myBitmapImage.UriSource = new Uri(textBoxDest.Text);
               myBitmapImage.EndInit();
               image2.Source = myBitmapImage;
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message);
           }
        }

        private void buttonBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            SteganographyLogic Logic = new SteganographyLogic();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".bmp";
            dlg.Filter = "Bitmap Images (.bmp)|*.bmp|JPG (.JPG)|*.jpg" ;
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                textBoxSrc.Text = filename;
                textBoxDest.Text = filename.Contains(".bmp")? filename.Replace(".bmp", "_output.bmp"): filename.Replace(".jpg", "_output.jpg");
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                myBitmapImage.UriSource = new Uri(filename);
                myBitmapImage.EndInit();
                image1.Source = myBitmapImage;
                labelLimitWords.Content = Logic.Capacity(File.ReadAllBytes(filename));
            }
        }

        private void buttonBrowseDest_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".bmp";
            dlg.Filter = "Bitmap Images (.bmp)|*.bmp|JPG (.JPG)|*.jpg";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                textBoxDest.Text = filename;
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                myBitmapImage.UriSource = new Uri(filename);
                myBitmapImage.EndInit();
                image2.Source = myBitmapImage;
                
            }
        }

        private void textBoxMessage_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            labelCurrentWords.Content = textBoxMessage.Text.Length;
        }

        private void textBoxShowMessage_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            labelWordsShowed.Content = textBoxShowMessage.Text.Length;
        }

        private void textBoxShowMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            labelWordsShowed.Content = textBoxShowMessage.Text.Length;
        }
    }
}
