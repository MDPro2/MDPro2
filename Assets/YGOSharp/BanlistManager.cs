using System.Collections.Generic;
using System.IO;

namespace YGOSharp
{
    public static class BanlistManager
    {
        public static List<Banlist> Banlists = new List<Banlist>();
        public static List<string> BanlistsNames = new List<string>();

        public static void initialize(string fileName)
        {
            AddBanlist(fileName);
            AddBanlistNA(fileName);
        }

        public static void AddBanlistNA(string fileName)
        {
            if (fileName != "config/lflist.conf") return;
            Banlist current = new Banlist();
            current.Name = "N/A";
            Banlists.Add(current);
            BanlistsNames.Add(current.Name);
        }

        public static void AddBanlist(string fileName)
        {
            Banlist current = null;
            StreamReader reader = new StreamReader(fileName);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                try
                {
                    if (line == null)
                        continue;
                    if (line.StartsWith("#"))
                        continue;
                    if (line.StartsWith("!"))
                    {
                        current = new Banlist();
                        current.Name = line.Substring(1, line.Length - 1);
                        if (BanlistsNames.Contains(current.Name)) continue;
                        Banlists.Add(current);
                        BanlistsNames.Add(current.Name);
                        continue;
                    }
                    if (!line.Contains(" "))
                        continue;
                    if (current == null)
                        continue;
                    string[] data = line.Split(new char[] {  ' '  }, System.StringSplitOptions.RemoveEmptyEntries);
                    int id = int.Parse(data[0]);
                    int count = int.Parse(data[1]);
                    current.Add(id, count);
                }
                catch (System.Exception e)  
                {
                    UnityEngine.Debug.Log(line);
                    UnityEngine.Debug.Log(e);
                }
            }
        }

        public static int GetIndex(uint hash)
        {
            for (int i = 0; i < Banlists.Count; i++)
                if (Banlists[i].Hash == hash)
                    return i;
            return 0;
        }

        public static string GetName(uint hash)    
        {
            for (int i = 0; i < Banlists.Count; i++)
                if (Banlists[i].Hash == hash)
                    return Banlists[i].Name;
            return InterString.Get("未知卡表");
        }

        public static List<string> getAllName()
        {
            List<string> returnValue = new List<string>();
            foreach (var name in BanlistsNames)
            {
                returnValue.Add(name);
            }
            return returnValue;
        }

        public static Banlist GetByName(string name)
        {
            Banlist returnValue = Banlists[Banlists.Count - 1];
            foreach (var item in Banlists)
            {
                if (item.Name == name)
                {
                    returnValue = item;
                }
            }
            return returnValue;
        }

        public static Banlist GetByHash(uint hash)
        {
            Banlist returnValue = Banlists[Banlists.Count - 1];
            foreach (var item in Banlists)
            {
                if (item.Hash == hash)
                {
                    returnValue = item;
                }
            }
            return returnValue;
        }

    }
}