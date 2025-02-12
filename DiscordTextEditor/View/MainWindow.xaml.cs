using DiscordTextEditor.ViewModel;
using Microsoft.Web.WebView2.Core;
using System.Windows;

using System.Windows.Input;


namespace DiscordTextEditor
{
    public partial class MainWindow : Window
    {
        public static MainWindow? Window;
        public MainWindow()
        {
            InitializeComponent();
            Window = this;
            DataContext = new MainViewModel();
            Window_Loaded();
            WebViewControl.CoreWebView2InitializationCompleted += WebView2_CoreWebView2InitializationCompleted;
        }

        private void WebView2_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            WebViewControl.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived; 
        }

        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString();

            string convertedText = AnsiMarkdownLib.Builders.StringBuilderForDiscord.ApplyMultipleAnsi(message);

            // Отправляем обратно в WebView2
            WebViewControl.CoreWebView2.ExecuteScriptAsync($"document.getElementById('editor').value = '{convertedText}'");
        }


        private async void Window_Loaded()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string htmlPath = System.IO.Path.Combine(appDirectory, "View", "Web", "webEditor.html");
            if (!System.IO.Path.Exists(htmlPath))
            {
                MessageBox.Show($"Файл не найден: {htmlPath}");
                return;
            }
            WebViewControl.Source = new Uri(htmlPath);
            await WebViewControl.EnsureCoreWebView2Async();

        }

        private void Drag(object sender, RoutedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && MainWindow.Window != null)
            {
                MainWindow.Window.DragMove();
            }
        }

        public void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}