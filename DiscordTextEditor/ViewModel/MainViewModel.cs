using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;

namespace DiscordTextEditor.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _text = string.Empty;
        public string Text
        {
            get { return _text; } 
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private CoreWebView2? _coreWebView;

        public CoreWebView2? CoreWebView
        {
            get { return _coreWebView; }
            set 
            { 
                _coreWebView = value;
                OnPropertyChanged(nameof(CoreWebView));
                
            }
        }


        public RelayCommand ChangeTextCommand { get; set; }

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

        public void InitializeWebView(CoreWebView2 webView)
        {
            CoreWebView = webView;
            CoreWebView.WebMessageReceived += CoreWebView_WebMessageReceived;
        }

        private void CoreWebView_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string textFromWeb = e.WebMessageAsJson.Trim('"');
            Text = textFromWeb;

        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
