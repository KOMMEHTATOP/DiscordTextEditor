using DiscordTextEditor.ViewModel.Commands;
using Microsoft.Web.WebView2.WinForms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using Microsoft.Web.WebView2.Wpf;
using System.Diagnostics;

namespace DiscordTextEditor.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string? _text;
        public string Text
        {
            get => _text!;
            set
            {
                _text = value;
                OnPropertyChanged();
                ChangeTextCommand.RaiseCanExecuteChanged(); 
            }
        }

        public RelayCommand ChangeTextCommand { get; }

        public MainViewModel()
        {
            ChangeTextCommand = new RelayCommand(ExecuteChangeText, CanExecuteChangeText);
        }


        private void ExecuteChangeText(object? parameter)
        {
            Text = "TextChanged!";
            Debug.WriteLine($"Свойство в поле Text изменено: {Text}");
        }

        private bool CanExecuteChangeText(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Text);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
