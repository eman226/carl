using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ComboBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ComboBoxItem comboBoxItem = new ComboBoxItem();
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose the save location for your Audio Files";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SavePath.Text = dialog.SelectedPath;
                    Settings1.Default.outputFolder= dialog.SelectedPath;
                    //SavePath.Text = Settings1.Default.outputFolder;
                }
            }
        }



        // multiply by 100 because the Progress bar's default maximum value is 100 
        //public float CurrentInputLevel { get { return lastPeak * 100; } }

    }

}
