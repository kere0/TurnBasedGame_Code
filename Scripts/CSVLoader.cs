using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;

public class CSVLoader
{
    public Dictionary<string, object> dict = new Dictionary<string, object>();
    public void LoadCSV<T>(string fileName) where T : InterfaceID, new()
    {
        List<T> list = new List<T>();
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        if (textAsset == null)
        {
            Debug.Log($"{fileName} 못찾음");
        }

        StringReader reader = new StringReader(textAsset.text);
        string[] headrs = reader.ReadLine().Split(',');

        Debug.Log(headrs.Length);
        while (reader.Peek() >= 0)
        {
            string line = reader.ReadLine();
            string[] values = line.Split(',');
            
            T data = new T();
            for (int i = 0; i < headrs.Length; i++)
            {
                string title = headrs[i];
                string value = values[i];
                
                FieldInfo field = typeof(T).GetField(title);
                if (field != null)
                {
                    Object converted;
                    if (field.FieldType.IsEnum)
                    {
                        converted = Enum.Parse(field.FieldType, value);
                    }
                    else 
                    {
                        converted = Convert.ChangeType(value, field.FieldType);
                    }
                    field.SetValue(data, converted);
                }
            }
            dict.Add($"{data.ID}{data.Rarity}{data.Star}", data);
        }
    }
    public T Get<T>(string id, Rarity rarity, int star) where T : InterfaceID
    {
        var key = $"{id}{rarity}{star}";
        if (dict.TryGetValue(key, out object value))
        {
            return (T)value;
        }
        Debug.Log($"ID {key} 없음");
        return default;
    }
}