using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using Microsoft.Win32;
using Ressy;
using Serilization;
using static System.Diagnostics.Process;
using Timer = System.Timers.Timer;

namespace Process
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private string _buffer;
       
        private string? _cuurentProcessItem;
        private System.Diagnostics.Process _process;
        Dictionary<string?, TimeSpan> Viewm = new ();

        public MainWindow()
        {
            InitializeComponent();
        }

        

        private void Add_OnClick(object sender, RoutedEventArgs e)
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

        }

        private void Update_OnClick(object sender, RoutedEventArgs e)
        {
            ProcessCombobox.Items.Clear();
            StackProcess stackProcess = new StackProcess();
            // stackProcess.Update();

            Viewm = stackProcess.OpenLoad();
            foreach (var item in Viewm)
            {
                ProcessCombobox.Items.Add(item.Key);
            }
        }

        private void ProcessCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();

            object selectedItem = ProcessCombobox.SelectedItem;


            if (selectedItem != null)
            {
                _cuurentProcessItem = selectedItem.ToString();
                if (processes.Length > 0)
                {
                    System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                     myProcess.StartInfo.FileName = selectedItem.ToString();
                     _process = myProcess;
                    myProcess.EnableRaisingEvents = true;
                    myProcess.Exited += (ProcessOnExited);
                    myProcess.Start();
                }
               
            }
        }
        private void ProcessOnExited(object? sender, EventArgs e)
        {
            if (_process != null)
            {
                TimeSpan elapsedTime = _process.ExitTime - _process.StartTime;
                TotalTime.Dispatcher.BeginInvoke(() => TotalTime.Text = elapsedTime.ToString());
                StackProcess stackProcess = new StackProcess();
                stackProcess.Update( _cuurentProcessItem,elapsedTime);
            }
        }
    }
}