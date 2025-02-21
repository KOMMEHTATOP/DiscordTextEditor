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
            DataContext = new MainViewModel();
            Loaded += MainWindow_Loaded;
            WebViewControl.CoreWebView2InitializationCompleted += WebView2_CoreWebView2InitializationCompleted;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string htmlPath = System.IO.Path.Combine(appDirectory, "View", "Web", "webEditor.html");
            if (!System.IO.File.Exists(htmlPath))
            {
                MessageBox.Show($"Файл не найден: {htmlPath}");
                return;
            }
            WebViewControl.Source = new Uri(htmlPath);
        }

        private async void WebView2_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (WebViewControl.CoreWebView2 != null)
            {
                await WebViewControl.EnsureCoreWebView2Async();
                var viewModel = (MainViewModel)DataContext;
                viewModel.InitializeWebView(WebViewControl.CoreWebView2);
            }
        }
    }
}