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
using Ressy;
using Serilization;
using static System.Diagnostics.Process;

namespace Process
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _buffer;
        Dictionary<string,double> Viewm = new Dictionary<string, double>();
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
                buf = flippedText.Substring(v+1);
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
            stackProcess.Update();
           
            Viewm = stackProcess.OpenLoad();
            foreach (var item in Viewm)
            {
                ProcessCombobox.Items.Add(item.Key);
            }
        }

        private void ProcessCombobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool IsActiv = false;
            string buf="";
            
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            
            object selectedItem = ProcessCombobox.SelectedItem;
            

            if (selectedItem!=null)
            {
                StackProcess stackProcess = new StackProcess();
                stackProcess.Update();
                
                Viewm = stackProcess.OpenLoad();
                foreach (var item in Viewm)
                {
                    if ((string) selectedItem ==item.Key )
                    {
                        if ((int)item.Value/60>=1)
                        {
                            TotalTime.Text =$"  {(int)item.Value/60} hour  {(int)item.Value} minute";
                        }
                        else
                        {
                            TotalTime.Text =$"   {(int)item.Value} minute ";
                        }
                   
                    }
                
                }
                foreach (var item in processes)
                {
                
                    if (item.ProcessName == selectedItem.ToString())
                    {
                        buf = "active";
                        IsActiv = true;

                    }
               
                }
                if (!IsActiv)
                {
                    buf = "no active";
                }
                Block.Text = buf;
            }
            else
            {
                Block.Text = "";
            }
            
            
            
        }
    }
}