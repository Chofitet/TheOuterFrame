using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataBaseMediator : IDataMediator<DataBaseType>
{

    string csvPath = Path.Combine(Application.dataPath, "Excels/DataBase.csv");
   

    public void SetDataOnSO(int id, DataBaseType data)
    {
        SaveToSO(id, data);
    }

    public void SetDataOnCVS(int id, DataBaseType data)
    {
        SaveToCSV(id, data);
    }

    void SaveToCSV(int id, DataBaseType data)
    {
        var lines = File.Exists(csvPath) ? File.ReadAllLines(csvPath).ToList() : new System.Collections.Generic.List<string>();

        if (lines.Count == 0) Debug.LogError($"Try to access a empy excel in {csvPath}");

        // Buscar fila existente
        bool found = false;
        for (int i = 1; i < lines.Count; i++)
        {
            var parts = lines[i].Split(';');
            if (Guid.Parse(parts[0]) == data.ID)
            {
                lines[i] = $"{data.ID.ToString()};{data.name};{data.GetText()}";
                found = true;
                break;
            }
        }

        // Si no existe -> agregar
        if (!found) lines.Add($"{data.ID.ToString()};{data.name};{data.GetText()}");

        File.WriteAllLines(csvPath, lines);
    }

    void SaveToSO(int id, DataBaseType data)
    {
        if (!File.Exists(csvPath)) return;

        var lines = File.ReadAllLines(csvPath);

        for (int i = 1; i < lines.Length; i++) // saltamos header
        {
            var parts = lines[i].Split(',');
            if (Guid.TryParse(parts[0], out Guid guidFromCsv) && guidFromCsv == data.ID)
            {
                data.name = parts[1];
                data.SetText(parts[2]);
                return;
            }
        }
    }
}
