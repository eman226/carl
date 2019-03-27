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

using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Diagnostics;
using System.Security;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for NLPPage.xaml
    /// </summary>
    public partial class NLPPage : Page
    {
        public NLPPage()
        {
            InitializeComponent();
        }

        private void Refine_Click(object sender, RoutedEventArgs e)
        {
            string fin = " ";
            string fout = " ";
            System.Windows.Forms.MessageBox.Show("Choose raw file");
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fin = openFileDialog1.FileName;
                System.Windows.Forms.MessageBox.Show(fin);
            }

            System.Windows.Forms.MessageBox.Show("Choose output file");
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fout = openFileDialog2.FileName;
                System.Windows.Forms.MessageBox.Show(fout);
            }

            string corp = File.ReadAllText(fin, Encoding.UTF8);//read raw text file
            string[] words = corp.Split(new string[] { " " }, StringSplitOptions.None);//tokenize raw text file
            List<string> line = new List<string>();//initialize line, temporarily holds words for each line in new refined text
            string path = fout;//path of refined text
            string temp_path = @"C:\Users\Emmanuel\Documents\Engineering_4\Degree\SpeechtoText\speech-to-text-wavenet-master\asset\transcripts\temp_path.txt";
           
            StreamWriter sw = File.AppendText(path);
            

            int i = 0;
            string temp;

            while (i < words.Length)
            {
                StreamWriter temp_sw = File.AppendText(temp_path);
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 17; k++)
                    {
                        if ((i + k) < words.Length - 1)
                            line.Add(words[i + k]);
                    }

                    temp = string.Join(" ", line);

                    temp_sw.WriteLine(temp);
                    
                    line.Clear();
                    i += 17;
                }
                temp_sw.Close();
                ProcessStartInfo pythonInfo = new ProcessStartInfo();
                Process python;
                pythonInfo.FileName = @"C:\Users\Emmanuel\AppData\Local\Programs\Python\Python35\python.exe";
                pythonInfo.Arguments = @"C:\Users\Emmanuel\Documents\Engineering_4\Degree\SpeechtoText\speech-to-text-wavenet-master\subject_extract.py" + " " + temp_path;
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

                sw.WriteLine(myString);
                sw.WriteLine(File.ReadAllText(temp_path, Encoding.UTF8));
                sw.WriteLine(" ");
                sw.WriteLine(" ");
                sw.WriteLine(" ");

                File.WriteAllText(temp_path, String.Empty);//clear temporary file
            }
            
            sw.Close();
        }


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

        private void SpellCorrect_Click(object sender, RoutedEventArgs e)
        {
            ConsoleManager.Show();
            const int initialCapacity = 82765*2;
            const int maxEditDistance = 5;
            const int prefixLength = 7;
            SymSpell symSpell = new SymSpell(initialCapacity, maxEditDistance, prefixLength);

            long memSize = GC.GetTotalMemory(true);
            // Load a frequency dictionary
            //wordfrequency_en.txt  ensures high correction quality by combining two data sources: 
            //Google Books Ngram data  provides representative word frequencies (but contains many entries with spelling errors)  
            //SCOWL — Spell Checker Oriented Word Lists which ensures genuine English vocabulary (but contained no word frequencies)
            string path = @"C:\Users\Emmanuel\source\repos\Project-Carl\Project-CARL\WpfApp1\frequency_dictionary_en_82_765.txt";
            string dict2 = @"C:\Users\Emmanuel\source\repos\Project-Carl\Project-CARL\WpfApp1\unigram_freq.txt";
            
            
            long memDelta = GC.GetTotalMemory(true) - memSize;
            if (!symSpell.LoadDictionary(path, 0, 1))
            {
                Console.Error.WriteLine("\rFile not found: " + System.IO.Path.GetFullPath(path));
                Console.ReadKey();
                //return;
            }
            if (!symSpell.LoadDictionary(dict2, 0, 1))
            {
                Console.Error.WriteLine("\rFile not found: " + System.IO.Path.GetFullPath(path));
                Console.ReadKey();
                //return;
            }
    

            //Open textfile
            String correctionFile = "";
            System.Windows.Forms.MessageBox.Show("Choose file to Correct");
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                correctionFile = openFileDialog1.FileName;
                System.Windows.Forms.MessageBox.Show(correctionFile);
            }


            //read words into array/list
            string corp = File.ReadAllText(correctionFile, Encoding.UTF8);//read raw text file
            string[] words = corp.Split(new string[] { " " }, StringSplitOptions.None);//tokenize raw text file
            List<SymSpell.SuggestItem> suggestedWord = null; //list of all corrected words
            List<string> correctedWords = new List<string>(); //Output of the corrected words
            
            //submit word to symSpell
            for(int i = 0; i < words.Length; i++)
            {
                suggestedWord = (symSpell.Lookup(words[i], SymSpell.Verbosity.Closest));
                correctedWords.Add(suggestedWord.First().term);
            }

            //save words to file
            string fileName = System.IO.Path.GetRandomFileName() + ".txt"; //random file name for our corrected text

            //save the directory of the correction file we selected previously
            string pathString = System.IO.Path.GetDirectoryName(correctionFile);
            // Use Combine again to add the file name to the path.
            pathString = System.IO.Path.Combine(pathString, fileName);

            string tmpstring = "";
            foreach(string word in correctedWords)
            {
                tmpstring += (word + " ");
            }

            File.WriteAllText(pathString, tmpstring);
        }

        private void Summarize_Click(object sender, RoutedEventArgs e)
        {
            string fin = " ";
            string fout = " ";

            System.Windows.Forms.MessageBox.Show("Choose text file");
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fin = openFileDialog1.FileName;
                System.Windows.Forms.MessageBox.Show(fin);
            }

            System.Windows.Forms.MessageBox.Show("Choose output file");
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fout = openFileDialog2.FileName;
                System.Windows.Forms.MessageBox.Show(fout);
            }

            string path = fout;//path to refined text
            File.WriteAllText(path, String.Empty);//clear temp file
            StreamWriter sw = File.AppendText(path);//pointer to write to temp file

            ProcessStartInfo pythonInfo = new ProcessStartInfo();
            Process python;
            pythonInfo.FileName = @"C:\Users\Emmanuel\AppData\Local\Programs\Python\Python35\python.exe";
            pythonInfo.Arguments = @"C:\Users\Emmanuel\Documents\Engineering_4\Degree\SpeechtoText\speech-to-text-wavenet-master\summarize.py" + " " + fin;
            pythonInfo.CreateNoWindow = false;
            pythonInfo.UseShellExecute = false;
            pythonInfo.RedirectStandardOutput = true;
            ConsoleManager.Show();
            Console.WriteLine("Python Starting");
            python = Process.Start(pythonInfo);
            StreamReader myStreamReader = python.StandardOutput;
            string myString = myStreamReader.ReadToEnd();
            python.WaitForExit();
            python.Close();

            sw.WriteLine("SUMMARY:");
            sw.WriteLine(" ");
            sw.WriteLine(myString);
            sw.Close();
            ConsoleManager.Hide();
        }
    }
}
