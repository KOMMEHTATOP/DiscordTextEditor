using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using System.Windows;
using System.Windows.Input;

namespace DiscordTextEditor.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Property
        private string _text = string.Empty;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        private CoreWebView2? _coreWebView;
        public CoreWebView2? CoreWebView
        {
            get { return _coreWebView; }
            set
            {
                if (_coreWebView != value)
                {
                    _coreWebView = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
        public MainViewModel()
        {
            ChangeTextCommand = new RelayCommand(ExecuteChangeText, CanExecuteChangeText);
        }

        #region Commands
        public RelayCommand ChangeTextCommand { get; set; }

        private void ExecuteChangeText(object? parameter)
        {
            Text = "TextChanged!";
            Debug.WriteLine($"Свойство в поле Text изменено: {Text}");
        }

        private bool CanExecuteChangeText(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Text);
        }

        #endregion

        #region Web
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

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
