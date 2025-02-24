using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using System.Windows;
using System.Windows.Input;
using AnsiMarkdownLib.Formatters;
using DiscordTextEditor.Helpers;
using System.Text.Json;

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
                    CopyTextCommand.RaiseCanExecuteChanged();
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
            CopyTextCommand = new RelayCommand(ExecuteCopyText, CanExecuteCopyText);
        }

        #region Commands
        public RelayCommand ChangeTextCommand { get; set; }
        public RelayCommand CopyTextCommand { get; set; }

        private void ExecuteCopyText(object? parameter)
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                string markdownText = ConvertHtmlToMarkdown.Convert(Text);
                Clipboard.SetText(markdownText);
            }
        }

        private bool CanExecuteCopyText(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Text);
        }


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

            string jsonHtml = await CoreWebView.ExecuteScriptAsync("document.getElementById('editor').innerHTML;");

            // Декодируем JSON-строку, убирая лишние символы экранирования
            return JsonSerializer.Deserialize<string>(jsonHtml) ?? string.Empty;
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
                Text = ConvertHtmlToMarkdown.Convert(textFromWeb);
                Debug.WriteLine($"Текст в поле Text: {Text}");
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
