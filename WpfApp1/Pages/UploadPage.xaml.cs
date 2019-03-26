using System;
using System.Collections.Generic;
using System.IO;
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
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Drive.v3;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class UploadPage : Page
    {
        static string[] Scopes = {
                       DriveService.Scope.Drive,
                       DriveService.Scope.DriveAppdata,
                       DriveService.Scope.DriveFile,
                       DriveService.Scope.DriveMetadataReadonly,
                       DriveService.Scope.DriveReadonly,
                       DriveService.Scope.DriveScripts};


        public UploadPage()
        {
            InitializeComponent();
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text Files (*.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                Textbox1.Text = System.IO.File.ReadAllText(filename);
                UploadPath.Text = filename;
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if(UploadPath.Text != "")
                UploadToDrive();
        }

        private void UploadToDrive()
        {
            UserCredential credential;

            using (var stream =
                new FileStream(Environment.CurrentDirectory + @"client_secret_79775938575 -0kt0h5kt772i3i0mth1j720tmtl4dkgp.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = System.IO.Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });


            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

            //file upload
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                MimeType = "text/plain",//uploading a google docs from a text file
                Name = UploadPath.Text //name of the file in the drive
            };
            FilesResource.CreateMediaUpload request;

            //string to file we're uploading
            string path = UploadPath.Text;
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            try
            {
                request = service.Files.Create(fileMetadata, fileStream, "text/plain");
                request.Fields = "id";
                request.Upload();
            }
            //always close the stream
            finally
            {
                fileStream.Close();
            }
            var file = request.ResponseBody;

        }


    }
}
