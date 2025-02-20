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
        public event PropertyChangedEventHandler? PropertyChanged;

        #region Свойства класса
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

        #endregion
        public MainViewModel()
        {
            ChangeTextCommand = new RelayCommand(ExecuteChangeText, CanExecuteChangeText);
            DragCommand = new RelayCommand(Drag);
            CloseCommand = new RelayCommand(CloseApp);
        }

        #region Commands
        public RelayCommand ChangeTextCommand { get; set; }
        public RelayCommand DragCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }


        private void CloseApp(object obj)
        {
            Application.Current.Shutdown();
        }


        private void Drag(object obj)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.DragMove();
            }
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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
