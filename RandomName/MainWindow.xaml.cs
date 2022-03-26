using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;


namespace RandomName
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        RandomNameSelector selector;
        public MainWindow()
        {
            InitializeComponent();
            List<string> list = new List<string>() { "赵12","钱45","孙13", "李23"};
            selector = new RandomNameSelector();
            selector.Selecting += Selector_Selecting;
            selector.Selected += Selector_Selected;
            this.ICCandidateNames.ItemsSource = selector._Namelist;
            this.ProgressBarForLeft.Maximum = selector._Namelist.Count;
            this.ProgressBarForLeft.Value = selector._Namelist.Count;
        }

        private void Selector_Selected(object sender, NameEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.ICSelectedNames.Items.Add($"{ICSelectedNames.Items.Count + 1}. {e.Name}");
                selector._Namelist.Remove(this.TextBlockSelectedName.Text);
                this.ProgressBarForLeft.Value = selector._Namelist.Count;
            }));
        }

        private void Selector_Selecting(object sender, NameEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.TextBlockSelectedName.Text = e.Name;
            }));
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            ICSelectedNames.Items.Clear();
            if (selector._Namelist.Count != 0)
            {
                if (this.ToggleButtonIsMulti.IsChecked == true)
                {
                    if (selector._Namelist.Count < int.Parse(TextBoxPeopleNum.Text))
                    {
                        TextBoxPeopleNum.Text = $"{selector._Namelist.Count}";
                    }
                    this.ButtonStart.IsEnabled = false;
                    await selector.StartSelectingWithTime(int.Parse(this.TextBoxPeopleNum.Text));
                    this.ButtonStart.IsEnabled = true;
                } else
                {
                    if (this.ButtonStart.Content.Equals("开始"))
                    {
                        this.ButtonStart.Content = "停止";
                        await selector.StartSelecting();
                    } else
                    {
                        this.ButtonStart.Content = "开始";
                        selector.IsSelecting = false;
                    }
                }
            }
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            selector.ResetNameList();
            this.ProgressBarForLeft.Value = this.ProgressBarForLeft.Maximum;
        }

        private void RButtonNumAdd_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(TextBoxPeopleNum.Text) < selector._Namelist.Count)
            {
                TextBoxPeopleNum.Text = $"{int.Parse(TextBoxPeopleNum.Text) + 1}";
            }
        }

        private void RButtonNumSub_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(TextBoxPeopleNum.Text) > 1)
            {
                TextBoxPeopleNum.Text = $"{int.Parse(TextBoxPeopleNum.Text) - 1}";
            }
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonCopy.Content = "已复制";
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 3000;
            timer.AutoReset = false;
            timer.Elapsed += (s, e2) =>this.Dispatcher.Invoke(() => this.ButtonCopy.Content = "复制");
            timer.Start();
            string copyText = "";
            foreach (var item in ICSelectedNames.Items)
            {
                copyText += $"{item.ToString()}\n";
            }

            Clipboard.SetText(copyText);
        }
    }
}
