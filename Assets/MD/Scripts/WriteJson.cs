using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using UnityEngine;

public class WriteJson : MonoBehaviour
{
    void Start()
    {
        string path = "sound/voice";
        var dirs = Directory.GetDirectories(path);
        for(int i = 0; i < dirs.Length; i++)
        {
            VoiceLines voiceLines = new VoiceLines();
            voiceLines.infos = new List<VoiceLine>();
            var oggs = Directory.GetFiles(dirs[i], "*.ogg");
            foreach (var ogg in oggs)
            {
                VoiceLine voiceLine = new VoiceLine();
                voiceLine.name = ogg.Substring(18, 16);
                voiceLines.infos.Add(voiceLine);
            }
            Write(voiceLines, dirs[i].Substring(12, 5).Replace("V", "SN"));
        }
    }
    void Write(VoiceLines lines, string name)
    {
        string jsonDate = JsonUtility.ToJson(lines);
        jsonDate = jsonDate.Replace("[{\"name\"", "[\n\t{\"name\"").Replace(",{\"name\"", ",\n\t{\"name\"")
            .Replace("{\"name\"", "{\n\t    \"name\"")
            .Replace(",\"face\"", ",\n\t    \"face\"")
            .Replace(",\"frame\"", ",\n\t    \"frame\"")
            .Replace(",\"cutin\"", ",\n\t    \"cutin\"")
            .Replace(",\"duelist\"", ",\n\t    \"duelist\"")
            .Replace(",\"text\"", ",\n\t    \"text\"")
            .Replace("},", "\n\t},")
            .Replace("}]}", "\n\t}\n    ]\n}")
            .Replace("\"infos\":[", "\"infos\":\n    [")
            ;
        File.WriteAllText("json/blank/" + name + ".json", jsonDate);
    }
}
