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
using System.Runtime.InteropServices;
using Google.Cloud.Speech.V1;
using System.IO;

using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security;

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

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.StopRecording();
                waveSource.Dispose();
            }
        }
        private void Transcribe_Click(object sender, RoutedEventArgs e)
        {
            string wav=" ";
            string fout=" ";
            System.Windows.Forms.MessageBox.Show("Choose audio file");
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if(openFileDialog1.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                wav = openFileDialog1.FileName;
                System.Windows.Forms.MessageBox.Show(wav);
            }

            System.Windows.Forms.MessageBox.Show("Choose output file");
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fout = openFileDialog2.FileName;
                System.Windows.Forms.MessageBox.Show(fout);
            }

            //remember to change working directory
            ProcessStartInfo pythonInfo = new ProcessStartInfo();
            Process python;

            pythonInfo.FileName = @"C:\Users\Emmanuel\AppData\Local\Programs\Python\Python35\python.exe";
            pythonInfo.Arguments = @"C:\Users\Emmanuel\Documents\Engineering_4\Degree\SpeechtoText\speech-to-text-wavenet-master\recognize.py" + " " +wav + " "+ 
                fout;
            pythonInfo.CreateNoWindow = false;
            pythonInfo.UseShellExecute = false;
            pythonInfo.RedirectStandardOutput = true;

            ConsoleManager.Show();
            Console.WriteLine("Python Starting");

            python = Process.Start(pythonInfo);
            StreamReader myStreamReader = python.StandardOutput;
            string myString = myStreamReader.ReadLine();
            python.WaitForExit();
            python.Close();

            Console.WriteLine("Value received from script: " + myString);
            //Console.WriteLine(fout);

            try
            {

                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(fout);

                //Write a line of text
                sw.WriteLine(myString);

                //Write a second line of text
                sw.WriteLine("From the StreamWriter class");

                //Close the file
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            
            /* finally
             {
                 Console.WriteLine("Executing finally block.");
             }*/
            //ConsoleManager.Hide();
            
        }
       // [SuppressUnmanagedCodeSecurity]
        public static class ConsoleManager
        {
            private const string Kernel32_DllName = "kernel32.dll";

            [DllImport(Kernel32_DllName)]
            private static extern bool AllocConsole();

            [DllImport(Kernel32_DllName)]
            private static extern bool FreeConsole();

            [DllImport(Kernel32_DllName)]
            private static extern IntPtr GetConsoleWindow();

            [DllImport(Kernel32_DllName)]
            private static extern int GetConsoleOutputCP();

            public static bool HasConsole
            {
                get { return GetConsoleWindow() != IntPtr.Zero; }
            }

            /// <summary>
            /// Creates a new console instance if the process is not attached to a console already.
            /// </summary>
            public static void Show()
            {
                //#if DEBUG
                if (!HasConsole)
                {
                    AllocConsole();
                    InvalidateOutAndError();
                }
                //#endif
            }

            /// <summary>
            /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
            /// </summary>
            public static void Hide()
            {
                //#if DEBUG
                if (HasConsole)
                {
                    SetOutAndErrorNull();
                    FreeConsole();
                }
                //#endif
            }

            public static void Toggle()
            {
                if (HasConsole)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }

            static void InvalidateOutAndError()
            {
                Type type = typeof(System.Console);

                System.Reflection.FieldInfo _out = type.GetField("_out",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

                System.Reflection.FieldInfo _error = type.GetField("_error",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

                System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError",
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

                Debug.Assert(_out != null);
                Debug.Assert(_error != null);

                Debug.Assert(_InitializeStdOutError != null);

                _out.SetValue(null, null);
                _error.SetValue(null, null);

                _InitializeStdOutError.Invoke(null, new object[] { true });
            }

            static void SetOutAndErrorNull()
            {
                Console.SetOut(TextWriter.Null);
                Console.SetError(TextWriter.Null);
            }
            
        }
    }
}