using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Launcher_Process
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _addNameProcess = null!;
        private string _buffer = null!;
        private string? _currentProcessItem;
        private System.Diagnostics.Process _process = null!;
        private Dictionary<string, TimeSpan> _viewWindows = new ();
        
        public MainWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Minimized;
            
            StackProcess stackProcess = new StackProcess();
            
            foreach (var item in stackProcess.OpenLoad())
            {
                CollectionProcess.Items.Add(item.Key);
                TotalTextBox.Text += $" {item.Key}: \n {(int)item.Value.TotalHours} hour, {Math.Round(item.Value.TotalMinutes)} minute;\n";
            }
        }
        private void AddProcess_OnClick(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "EXE files (*.exe)|*.exe";
            if (openFileDialog.ShowDialog() == true)
            {

                string buf = openFileDialog.FileName;
                int v = buf.LastIndexOf(@"\");
                buf = buf.Substring(v + 1);


                string flippedText = new string(buf.Reverse().ToArray());
                v = flippedText.LastIndexOf(@".");
                buf = flippedText.Substring(v + 1);
                _buffer = new string(buf.Reverse().ToArray());
                StackProcess stackProcess = new StackProcess
                {
                    Name = _buffer
                };
                stackProcess.CurrentProcess();
            }
            
            CollectionProcess.Items.Clear();
            StackProcess stackProcess1 = new StackProcess();
            // stackProcess.Update();
            _viewWindows = stackProcess1.OpenLoad();
            foreach (var item in _viewWindows)
            {
                CollectionProcess.Items.Add(item.Key);
            }

        }

        private void CollectionProcess_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();

            object selectedItem = CollectionProcess.SelectedItem;


            if (selectedItem != null)
            {
                _currentProcessItem = selectedItem.ToString();
                if (processes.Length > 0)
                {
                    System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                    myProcess.StartInfo.FileName = selectedItem.ToString();
                    _process = myProcess;
                    myProcess.EnableRaisingEvents = true;
                    myProcess.Exited += (ProcessOnExited);
                    myProcess.Start();
                    CurrentTextBox.Text = "Activate";
                }
            }
        }
        private void ProcessOnExited(object? sender, EventArgs e)
        {
            if (_process != null)
            {
                StackProcess stackProcess = new StackProcess();
                TimeSpan elapsedTime = _process.ExitTime - _process.StartTime;
                CurrentTextBox.Dispatcher.BeginInvoke(() =>
                {
                    CurrentTextBox.Text = $" completed: {_currentProcessItem}\n {(int)elapsedTime.TotalHours} hour, {Math.Round(elapsedTime.TotalMinutes,2)} minute";
                    stackProcess.Update( _currentProcessItem,elapsedTime);
                    TotalTextBox.Clear();
                    foreach (var item in stackProcess.OpenLoad())
                    {
                        TotalTextBox.Text += $" {item.Key}: \n {(int)item.Value.TotalHours} hour, {Math.Round(item.Value.TotalMinutes)} minute;\n";
                    }
                    return CurrentTextBox.Text ;
                });
                
            }
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}