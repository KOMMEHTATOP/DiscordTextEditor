﻿using DiscordTextEditor.ViewModel;
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
            Window_Loaded();

            WebViewControl.CoreWebView2InitializationCompleted += WebView2_CoreWebView2InitializationCompleted;
        }

        private void WebView2_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (WebViewControl.CoreWebView2 != null)
            {
                var viewModel = (MainViewModel)DataContext;
                viewModel.InitializeWebView(WebViewControl.CoreWebView2);
            }
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