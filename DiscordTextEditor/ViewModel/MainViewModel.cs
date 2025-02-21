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
        private string _text = "asdf";
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                    ChangeTextCommand.RaiseCanExecuteChanged();
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
            string baseText = Text;
            if (baseText.StartsWith("<b>") && baseText.EndsWith("<b>"))
            {
                Text = baseText.Substring(3, baseText.Length - 7);
            }
            else
            {
                Text = $"<b>{baseText}<b>";
            }

            Debug.WriteLine($"Свойство в поле Text изменено: {Text}");
            //отправляем текст обратно в HTML
            CoreWebView?.ExecuteScriptAsync($"document.getElementById('editor').innerHTML = `{Text}`;");

        }

        private bool CanExecuteChangeText(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Text);
        }

        #endregion

        #region Web
        public void InitializeWebView(CoreWebView2 webView)
        {
            if (webView == null)
            {
                Debug.WriteLine("Ошибка: WebView2 не инициализирован");
                return;
            }
            CoreWebView = webView;
            CoreWebView.WebMessageReceived += CoreWebView_WebMessageReceived;
            Debug.WriteLine("WebView2 успешно инициализирован");
        }

        private void CoreWebView_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string textFromWeb = e.WebMessageAsJson.Trim('"');
                Text = textFromWeb;
                Debug.WriteLine($"Получено сообщение из браузера: {Text}");
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Ошибка обработки сообщения из WebView2: {ex.Message}");
            }
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
