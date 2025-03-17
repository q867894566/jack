using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadMod();
        }

        private void LoadMod()
        {
            try
            {
                string modFilePath = "mod.txt";
                if (!File.Exists(modFilePath))
                {
                    MessageBox.Show("mod.txt 文件未找到，使用默認設置。");
                    return;
                }

                var lines = File.ReadAllLines(modFilePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split('=');
                    if (parts.Length != 2) continue;

                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "Icon":
                            AppIcon.Source = new BitmapImage(new Uri(value, UriKind.Absolute));
                            break;

                        case "Name":
                            AppName.Text = value;
                            this.Title = value;
                            break;

                        case "FunctionBarPosition":
                            FunctionBar.HorizontalAlignment = value.ToLower() == "left" ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                            break;

                        case "Buttons":
                            ProcessButtons(value);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"載入 Mod 時發生錯誤：{ex.Message}");
            }
        }

        private void ProcessButtons(string buttonsConfig)
        {
            ToolBar.Children.Clear(); // 清空現有按鈕
            var operations = buttonsConfig.Split(',');

            List<string> buttonList = new List<string>();

            foreach (var operation in operations)
            {
                var parts = operation.Split(':');
                if (parts.Length < 2) continue;

                string action = parts[0].Trim();
                string buttonName = parts.Length > 1 ? parts[1].Trim() : "";
                string newName = parts.Length > 2 ? parts[2].Trim() : "";

                switch (action.ToLower())
                {
                    case "add":
                        if (!buttonList.Contains(buttonName))
                            buttonList.Add(buttonName);
                        break;

                    case "edit":
                        if (buttonList.Contains(buttonName))
                        {
                            buttonList[buttonList.IndexOf(buttonName)] = newName;
                        }
                        break;

                    case "delete":
                        buttonList.Remove(buttonName);
                        break;

                    case "order":
                        buttonList = newName.Split('|').Select(s => s.Trim()).ToList();
                        break;
                }
            }

            // 重新生成按鈕
            foreach (var btnName in buttonList)
            {
                Button button = new Button
                {
                    Content = btnName,
                    Width = 100,
                    Height = 30,
                    Margin = new Thickness(5)
                };
                ToolBar.Children.Add(button);
            }
        }

        // 視窗拖動
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        // 縮小按鈕
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // 放大/還原按鈕
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                MaximizeButton.Content = "□";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                MaximizeButton.Content = "🞏";
            }
        }

        // 關閉按鈕
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}