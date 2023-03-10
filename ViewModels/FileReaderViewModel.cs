using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiApp2.ViewModels
{
    public class FileReaderViewModel : ObservableObject
    {

        private readonly ContentPage _page;


        #region properties


        private string _selectedDirectoryPath;

        public string SelectedDirectoryPath
        {
            get => _selectedDirectoryPath;
            set => SetProperty(ref _selectedDirectoryPath, value);
        }



        private string _selectedFile;

        public string SelectedFile
        {
            get => _selectedFile;
            set
            {
                SetProperty(ref _selectedFile, value);
                if (!string.IsNullOrEmpty(_selectedFile))
                {
                    var fileInfo = new FileInfo(_selectedFile);
                    FileName = fileInfo.Name;
                    FileSize = fileInfo.Length;
                    FileExtension = fileInfo.Extension;
                    CreationTime = fileInfo.CreationTime;
                }
            }
        }


        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set { SetProperty(ref _fileName, value); }
        }

        private long _fileSize;

        public long FileSize
        {
            get => _fileSize;
            set { SetProperty(ref _fileSize, value); }
        }

        private string _fileExtension;

        public string FileExtension
        {
            get => _fileExtension;
            set { SetProperty(ref _fileExtension, value); }
        }



        private DateTime _creationTime;

        public DateTime CreationTime
        {
            get => _creationTime;
            set { SetProperty(ref _creationTime, value); }
        }




        public ObservableCollection<string> Files { get; set; } = new ObservableCollection<string>();

        #endregion



        public ICommand SelectFolderCommand { get; set; }
        public ICommand ReadFilesInFolderCommand { get; }



        public FileReaderViewModel(ContentPage page)
        {
            _page = page;
            SelectFolderCommand = new AsyncRelayCommand(SelectFolder);
            ReadFilesInFolderCommand = new AsyncRelayCommand(ReadFilesInFolder);
        }



        private async Task ReadFilesInFolder()
        {
            if (!string.IsNullOrEmpty(SelectedDirectoryPath))
            {
                Files.Clear();
                try
                {

                    foreach (var item in Directory.EnumerateFiles(_selectedDirectoryPath))
                    {
                        Files.Add(item);
                    }

                }
                catch (Exception error)
                {

                    await _page.DisplayAlert("Alert", $"cannot open folder picker, {error.Message}", "OK");

                }
            }

        }


        private async Task SelectFolder()
        {
            CheckPermissions();

            var pickFolder = await FolderPicker.Default.PickAsync(CancellationToken.None);
            if (pickFolder.IsSuccessful)
            {
                SelectedDirectoryPath = pickFolder.Folder?.Path ?? "not choosen";
            }
            else
            {
                ;
                await _page.DisplayAlert("Alert", $"cannot open folder picker, {pickFolder.Exception.Message}", "OK");
            }
        }


        private async void CheckPermissions()
        {
            var writePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            var readPermission = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            if (writePermission != PermissionStatus.Granted || readPermission != PermissionStatus.Granted)
            {
                var results = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (results != PermissionStatus.Granted)
                {
                    // Permission denied, handle appropriately
                }


                results = await Permissions.RequestAsync<Permissions.StorageRead>();
                if (results != PermissionStatus.Granted)
                {
                    // Permission denied, handle appropriately
                }
            }
        }

    }
}
