using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using System.Windows;
using System.Windows.Input;
using Microsoft.Web.WebView2.Wpf;

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
            if (CoreWebView == null) return;

            string selectedTextJson = await CoreWebView.ExecuteScriptAsync("window.getSelectedText();");
            Debug.WriteLine($"Получен выделенный текст (JSON): {selectedTextJson}");

            string selectedText = selectedTextJson.Trim('"');

            if (string.IsNullOrWhiteSpace(selectedText)) return;

            // Проверяем, содержится ли выделенный текст в полном тексте
            if (!Text.Contains(selectedText)) return;

            // Применяем форматирование
            string boldText = StringBuilderForDiscord.ApplyMarkDownBold(selectedText);

            // Заменяем выделенный текст в общем тексте
            Text = Text.Replace(selectedText, boldText);

            string textToWeb = StringBuilderForDiscord.ConvertToHtml(Text);
            Debug.WriteLine($"В методе ExecuteChangeText, преобразовалось свойство Text на {textToWeb}");

            // Обновляем содержимое редактора
            await CoreWebView.ExecuteScriptAsync($"document.getElementById('editor').innerHTML = `{textToWeb}`;");
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

        private async Task<string> GetSelectedTextAsync()
        {
            if (CoreWebView == null)
                return string.Empty;

            string result = await CoreWebView.ExecuteScriptAsync("window.getSelectedText();");
            return result.Trim('"'); // Убираем кавычки, т.к. JS возвращает строку в JSON-формате
        }


        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
