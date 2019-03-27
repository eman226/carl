using NAudio.Wave;
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

using Google.Cloud.Speech.V1;
using System.IO;

namespace WpfApp1
{
    public partial class RecordPage : Page
    {
        private WaveInEvent micInput = new WaveInEvent();

        private WaveIn waveSource = Settings1.Default.waveIn;
        private WaveFileWriter waveFile = null;

        public RecordPage()
        {
            InitializeComponent();
        }

        private void Record_Click(object sender, RoutedEventArgs e)
        {
            
            waveSource = new WaveIn
            {
                WaveFormat = new WaveFormat(44100, 1)
            };

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            // Console.WriteLine(Settings1.Default.outputFilePath);
            if (waveFile == null)
            {
                waveFile = new WaveFileWriter(String.Concat(Settings1.Default.outputFolder, "\\out_test.wav"), waveSource.WaveFormat);
                waveSource.StartRecording();
            }
            
        }

        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();
            }
        }

        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            
           if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
                waveFile.Dispose();
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.StopRecording();
                waveSource.Dispose();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var speech = SpeechClient.Create();
            var longOperation = speech.LongRunningRecognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 44100,
                LanguageCode = "en",
            }, RecognitionAudio.FromFile(String.Concat(Settings1.Default.outputFolder, "\\out_test.wav")));
            longOperation = longOperation.PollUntilCompleted();
            var response = longOperation.Result;

            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(@"C:\Users\Emmanuel\Documents\Engineering 4\Degree\Outputs\out1.txt", true))
            {
               


                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        file.WriteLine(alternative.Transcript);
                        //Console.WriteLine(alternative.Transcript);
                        //System.IO.File.WriteAllText(@"C:\Users\Emmanuel\Documents\Engineering 4\Degree\Outputs\out_test.txt", alternative.Transcript);

                    }
                }
            }
        }
    }
}
