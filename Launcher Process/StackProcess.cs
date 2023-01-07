using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace Launcher_Process;

public class StackProcess
{
    private const string PATH = "SaveProcess.JSON";
    public string? Name { get; set; }
    private bool _isUpdate = false;
    
    Dictionary<string?, TimeSpan> _procesAdd = new ();

        //Сохраняем Данные
    private TimeSpan ActivProcess()
    {
        return new TimeSpan(1);
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
           _procesAdd[item.Key] = (item.Value);
       }

        using (StreamWriter writer = File.CreateText(PATH))
            {
                string output = JsonConvert.SerializeObject(_procesAdd);
                writer.Write(output);
            }
    }
    private Dictionary<string?, TimeSpan> LoadProcess()
    {
        Defult();
        using (var reader = File.OpenText(PATH))
        {
            var filetext = reader.ReadToEnd();
            Dictionary<string, TimeSpan>? procesLoad = JsonConvert.DeserializeObject<Dictionary<string,TimeSpan>>(filetext);
            return procesLoad;
        }
    }
    //Обновляем данные
    public  virtual void  Update(string? Name,TimeSpan time)
    {
       
       Dictionary<string?, TimeSpan> CurrentUpdateTime = LoadProcess();
       CurrentUpdateTime[Name] += time;
       
        using (StreamWriter writer = File.CreateText(PATH))
        {
            string output = JsonConvert.SerializeObject(CurrentUpdateTime);
            writer.Write(output);
        }
        
    }
    
    public Dictionary<string?, TimeSpan> OpenLoad()
    {
        Console.WriteLine(" Сохраненые процессы: ");
        foreach (var item in LoadProcess())
        {
            Console.WriteLine($"    Название процесса - {item.Key}, Общее время  запуска - {item.Value}");
        }
        return LoadProcess();
    }
}