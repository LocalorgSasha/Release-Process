using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
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

        private string GetName(string FileOfPath)
        {
            string NameProc = "";
            string buf = FileOfPath;
            int v = buf.LastIndexOf(@"\");
            buf = buf.Substring(v + 1);


            string flippedText = new string(buf.Reverse().ToArray());
            v = flippedText.LastIndexOf(@".");
            buf = flippedText.Substring(v + 1);
            NameProc = new string(buf.Reverse().ToArray());
            return NameProc;
        }

      
        public MainWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Minimized;
           
            StackProcess stackProcess = new StackProcess();
            
            foreach (var item in stackProcess.OpenLoad())
            {
                CollectionProcess.Items.Add(GetName(item.Key));
               
                TotalTextBox.Text += $" {GetName(item.Key)}: \n {(int)item.Value.TotalHours} hour, {Math.Round(item.Value.TotalMinutes)} minute;\n";
            }
        }
        private void AddProcess_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "EXE files (*.exe)|*.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                
                StackProcess stackProcess = new StackProcess
                {
                    Name = openFileDialog.FileName
                };
                stackProcess.CurrentProcess();
            }
            CollectionProcess.Items.Clear();
            StackProcess stackProcess1 = new StackProcess();
            // stackProcess.Update();
            _viewWindows = stackProcess1.OpenLoad();
            foreach (var item in _viewWindows)
            {
                CollectionProcess.Items.Add(GetName(item.Key));
            }

        }

        
        private void CollectionProcess_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string FileOfPATH = "";
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();

            object selectedItem = CollectionProcess.SelectedItem;
           string PATH =  Assembly.GetExecutingAssembly().Location;

            if (selectedItem != null)
            {
                StackProcess process = new StackProcess();
                
                _currentProcessItem = selectedItem.ToString();
                foreach (var item in process.OpenLoad())
                {
                  
                    if (GetName(item.Key)== _currentProcessItem)
                    {
                        FileOfPATH = item.Key;
                        _currentProcessItem = item.Key;
                        break;
                    }
                }
                if (processes.Length > 0)
                {
                    System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                   
                  // myProcess.StartInfo.FileName = "C:\\CLion 2022.3.1\\bin\\"+ selectedItem;
                  myProcess.StartInfo.FileName = FileOfPATH;
                  //  myProcess.StartInfo.Verb =selectedItem.ToString();
                    _process = myProcess;
                    myProcess.EnableRaisingEvents = true;
                    myProcess.StartInfo.UseShellExecute = true;
                   // myProcess.StartInfo.WorkingDirectory = "C:\\CLion 2022.3.1\\bin\\";
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
                    CurrentTextBox.Text = $" completed: {GetName(_currentProcessItem)}\n {(int)elapsedTime.TotalHours} hour, {Math.Round(elapsedTime.TotalMinutes,2)} minute";
                    stackProcess.Update( _currentProcessItem,elapsedTime);
                    TotalTextBox.Clear();
                    foreach (var item in stackProcess.OpenLoad())
                    {
                        TotalTextBox.Text += $" {GetName(item.Key)}: \n {(int)item.Value.TotalHours} hour, {Math.Round(item.Value.TotalMinutes)} minute;\n";
                    }
                    CollectionProcess.Items.Clear();
                    StackProcess process = new StackProcess();
                    foreach (var item in process.OpenLoad())
                    {
                        CollectionProcess.Items.Add(GetName(item.Key));
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