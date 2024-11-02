using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


[Serializable]
public class VoiceLine
{
    public string name;
    public int face;
    public int frame;
    public int[] cutin;
    public string[] duelist;
    public string text;
}

[Serializable]
public class VoiceLines
{
    public List<VoiceLine> infos;
}

public class VoiceLineHandler
{
    static VoiceLines linesA;
    static string jsonNameA;
    static VoiceLines linesB;
    static string jsonNameB;

    static void ReadJson(uint controller, string jsonName)
    {
        if (controller == 0)
        {
            if (jsonNameA != jsonName)
            {
                jsonNameA = jsonName;
                string path = "json/" + jsonName + ".json";
                if (File.Exists(path))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    string jsonString = Encoding.UTF8.GetString(bytes);
                    linesA = JsonUtility.FromJson<VoiceLines>(jsonString);
                }
                else
                    linesA = null;
            }
        }
        else
        {
            if (jsonNameB != jsonName)
            {
                jsonNameB = jsonName;
                string path = "json/" + jsonName + ".json";
                if (File.Exists(path))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    string jsonString = Encoding.UTF8.GetString(bytes);
                    linesB = JsonUtility.FromJson<VoiceLines>(jsonString);
                }
                else
                    linesB = null;
            }
        }
    }

    public static VoiceLine GetVoiceLine(uint controller, string name)
    {
        if (controller == 0)
        {
            ReadJson(controller, name.Substring(0, 5).ToUpper().Replace("V", "SN"));
            if (linesA == null)
                return null;
            foreach (VoiceLine line in linesA.infos)
                if (line.name == name)
                    return line;
            return null;
        }
        else
        {
            ReadJson(controller, name.Substring(0, 5).ToUpper().Replace("V", "SN"));
            if (linesB == null)
                return null;
            foreach (VoiceLine line in linesB.infos)
                if (line.name == name)
                    return line;
            return null;
        }
    }
}
