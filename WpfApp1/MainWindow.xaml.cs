using System;
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
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public String outputFolder, outputFilePath = "";
        public WaveInEvent waveIn = new WaveInEvent();
        public WaveFileWriter writer = null;
        public string x = "Base Value of String";
        private SettingsPage settingsPage = new SettingsPage();
        private RecordPage recordingPage = new RecordPage();
        private NLPPage nlpPage = new NLPPage();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = settingsPage;
        }

        private void Record_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = recordingPage;
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new UploadPage();
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Details();
        }

        private void NLP_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = nlpPage;
        }

        public string getString()
        {
            return this.x;
        }
    }

}
