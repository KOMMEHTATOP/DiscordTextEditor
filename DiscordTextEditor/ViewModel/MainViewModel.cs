using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using System.Windows;
using System.Windows.Input;
using AnsiMarkdownLib.Formatters;

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

        private async void ExecuteChangeText(object? parameter)
        {
            await ApplyFormatting("bold");
        }

        private async Task ApplyFormatting(string command)
        {
            if (CoreWebView == null) return;

            await CoreWebView.ExecuteScriptAsync($"document.execCommand('{command}');");

            // Обновляем свойство Text
            Text = await GetEditorHtml();
        }

        private async Task<string> GetEditorHtml()
        {
            if (CoreWebView == null) return string.Empty;

            string html = await CoreWebView.ExecuteScriptAsync("document.getElementById('editor').innerHTML;");
            return html.Trim('"'); // Убираем лишние кавычки
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
                //Debug.WriteLine($"Получено сообщение из JS в метод CoreWebView_WebMessageReceived: {e.WebMessageAsJson}");
                string textFromWeb = e.WebMessageAsJson.Trim('"');
                Text = textFromWeb;
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
