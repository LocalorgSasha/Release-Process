using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Serilization;

public class StackProcess
{ 
    private const string PATH = "SaveProcess.JSON";
    public string Name { get; set; }
    private bool _isUpdate = false;
    Dictionary<string, double> _procesAdd = new Dictionary<string, double>();

        //Сохраняем Данные
    private int ActivProcess()
    {
        bool isActiv = false;
        double hour=0;
        double minute=0;
        System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
        foreach (var item in processes)
        {
            if (item.ProcessName == Name)
            {
                isActiv = true;
                break;
            }
        }

        if (isActiv)
        {
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetProcessesByName(Name)[0];
            ProcessThreadCollection processThreads = proc.Threads;
            foreach (ProcessThread thread in processThreads)
            {
                hour=(DateTime.Now - thread.StartTime).TotalHours;
                minute=(DateTime.Now - thread.StartTime).TotalMinutes;
                break;
            }
            
            _isUpdate = true;
        }

        int m = (int) minute % 60;
        return m;
    }
    public void CurrentProcess()
    {
       
        Console.WriteLine(" Выбранный текущий процесс:");
        _procesAdd.Add(Name,ActivProcess());
        foreach (var item in _procesAdd)
        {
            Console.WriteLine($"    Название процесса - {item.Key}, Время текущего запуска - {item.Value}");
        }
        Defult();
        SaveProcess();
    }

    public void Defult()
    {
        var fileExists = File.Exists(PATH);
        if (!fileExists)
        {
            File.CreateText(PATH).Dispose();
            StackProcess stackProcess = new StackProcess()
            {
                Name = "Defult"
            };
           SaveDefult();
        }
    }
    private void SaveDefult()
    {
        
        using (StreamWriter writer = File.CreateText(PATH))
        {
            string output = JsonConvert.SerializeObject(_procesAdd);
            writer.Write(output);
        }
    }
    private void SaveProcess()
    {
       // var keyValuePairs = _procesAdd.Concat(LoadProcess()!);
       foreach (var item in LoadProcess()!)
       {
           _procesAdd[item.Key] = Convert.ToInt32(item.Value);
       }

        using (StreamWriter writer = File.CreateText(PATH))
            {
                string output = JsonConvert.SerializeObject(_procesAdd);
                writer.Write(output);
            }
    }
    private Dictionary<string, double> LoadProcess()
    {
        Defult();
        using (var reader = File.OpenText(PATH))
        {
           
            var filetext = reader.ReadToEnd();
            Dictionary<string, double>? procesLoad = JsonConvert.DeserializeObject<Dictionary<string,double>>(filetext);
            return procesLoad;
        }
    }
    //Обновляем данные
    public void Update()
    {
        double minute=0;
        double buffer=0;
        bool IsActiv = false;
       Dictionary<string,double> procupdate = LoadProcess();

        for (int i = 0; i < LoadProcess().Count; i++)
        {
            try
            {
                var pair = LoadProcess().ElementAt(i);
                
                IsActiv = System.Diagnostics.Process.GetProcessesByName(pair.Key)[0].HasExited;
               
                if (!IsActiv)
                {
                    Console.WriteLine(pair.Key);
                }
               
                System.Diagnostics.Process proc = System.Diagnostics.Process.GetProcessesByName(pair.Key)[0];
                ProcessThreadCollection processThreads = proc.Threads;
                foreach (ProcessThread thread in processThreads)
                {
                   
                    minute = (DateTime.Now - thread.StartTime).TotalMinutes;
                    buffer =  minute ;
                    break;
                }

                if (Convert.ToInt32(pair.Value) != buffer)
                {
                        
                        double col = Math.Abs(Math.Round(buffer) - Convert.ToInt32(pair.Value));
                        pair = new KeyValuePair<string, double>(pair.Key, pair.Value+col);
                        procupdate![pair.Key] = pair.Value; 
                        
                }
                else
                {
                    pair = new KeyValuePair<string, double>(pair.Key, (int)minute % 60);
                    procupdate![pair.Key] = pair.Value;
                }

            }
            catch (Exception e) { }
        }
        
        using (StreamWriter writer = File.CreateText(PATH))
        {
            string output = JsonConvert.SerializeObject(procupdate);
            writer.Write(output);
        }
        
    }
    
    public Dictionary<string,double> OpenLoad()
    {
        Console.WriteLine(" Сохраненые процессы: ");
        foreach (var item in LoadProcess())
        {
            Console.WriteLine($"    Название процесса - {item.Key}, Общее время  запуска - {item.Value}");
        }

        return LoadProcess();
    }
}