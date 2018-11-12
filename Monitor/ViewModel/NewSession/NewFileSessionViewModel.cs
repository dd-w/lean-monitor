using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Monitor.Model.Sessions;

namespace Monitor.ViewModel.NewSession
{
    public class NewFileSessionViewModel : ViewModelBase, INewSessionViewModel, IDataErrorInfo
    {
        private readonly ISessionService _sessionService;
        private readonly FileSessionParameters _fileSessionParameters = new FileSessionParameters
        {
            FileName = "",
            Watch = true
        };

        public NewFileSessionViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
            OpenCommand = new RelayCommand(Open, CanOpen);
            BrowseCommand = new RelayCommand(Browse);

        }

        private void Open()
        {
            _sessionService.OpenFile(_fileSessionParameters);
        }

        private bool CanOpen()
        {
            var fieldsToValidate = new[]
{
                nameof(FileName),
            };

            return fieldsToValidate.All(field => string.IsNullOrEmpty(this[field]));
        }

        public string FileName
        {
            get { return _fileSessionParameters.FileName; }
            set
            {
                _fileSessionParameters.FileName = value;
                RaisePropertyChanged();
                OpenCommand.RaiseCanExecuteChanged();
            }
        }

        public bool FileWatch
        {
            get { return _fileSessionParameters.Watch; }
            set
            {
                _fileSessionParameters.Watch = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand OpenCommand { get; }
        public RelayCommand BrowseCommand { get; }

        public string Header { get; } = "From file";

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(FileName):
                        if (string.IsNullOrWhiteSpace(FileName)) return "Filename is required";
                        if (!File.Exists(FileName)) return "File does not exist";
                        return string.Empty;

                    default:
                        return string.Empty;
                }
            }
        }

        public string Error { get; } = null;

        public void Browse()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "JSON Files (*.json)|*.json" // Set filter for file extension and default file extension 
            };
            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                FileName = dlg.FileName;
            }
        }
    }
}