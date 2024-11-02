using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Boot : MonoBehaviour
{
    public Slider progressBar;
    public Text text;

    string title;
    string dots;
    float time;
    bool extracting;
    int totalNum;
    int nowNum;

    public static string root = "";

    IEnumerator CopyAssetbundlle(string androidPath, string toPath, string refPath)
    {
        string[] android_files = Directory.GetFiles(androidPath, ".", SearchOption.AllDirectories);

        string[] dirs = Directory.GetDirectories(refPath);
        foreach (string dir in dirs)
        {
            string newDir = dir.Replace(refPath + "\\", "");
            if (newDir.StartsWith("u"))
                continue;
            string targetToPath = toPath + "/" + newDir;
            Directory.CreateDirectory(targetToPath);

            foreach (var file in Directory.GetFiles(refPath + "/" + newDir))
            {
                string fileName = Path.GetFileName(file).Replace("-", "");
                Debug.Log(fileName);
                bool found = false;
                foreach (var android_file in android_files)
                {
                    //Debug.Log(android_file);
                    if (android_file.EndsWith(fileName))
                    {
                        File.Copy(android_file, targetToPath + "/" + fileName, true);
                        found = true;
                        break;
                    }
                }
                if(!found)
                {
                    File.Create(targetToPath + "/" + fileName + ".txt");
                }
                yield return null;

            }

            //string dirName = Path.GetDirectoryName(file).Split('\\')[1];

        }



    }



    void Start()
    {
        //StartCoroutine(CopyAssetbundlle("assetbundle/masterduel_android_assetbundle", "assetbundle/Android", "assetbundle/card"));

#if !UNITY_EDITOR && UNITY_ANDROID
        Environment.CurrentDirectory = Application.persistentDataPath;
        Directory.SetCurrentDirectory(Application.persistentDataPath);
        StartCoroutine(CheckFile());
#elif UNITY_EDITOR && UNITY_ANDROID
        root = "StreamingAssets/";
        StartCoroutine(LoadSceneAsync(1));
#else
        StartCoroutine(LoadSceneAsync(1));
#endif
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time > 0.33f)
        {
            time = 0;
            dots += ".";
            if (dots == "....")
                dots = "";
        }
        if (extracting && totalNum != 0)
        {
            float progress = (float)nowNum / totalNum;
            progressBar.value = progress;
        }
        if (totalNum == 0)
            text.text = title + dots;
        else
            text.text = title + "(" + nowNum + "/" + totalNum + ")";
    }

    List<string> zips = new List<string>
    {
        "assetbundle",
        "data",
        "picture",
        "sound",
        "texture"
    };

    bool installNeverStarted;

    IEnumerator CheckFile()
    {
        if(!Directory.Exists("config"))
            Directory.CreateDirectory("config");
        Config.initialize("config/config.conf");
        IEnumerator enumerator;
        foreach (string zip in zips)
        {
            if(Config.Get(zip + "_install", "0") == "0")
            {
                enumerator = Check(zip);
                StartCoroutine(enumerator);
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                Config.Set(zip + "_install", "1");
            }
        }
        yield return null;
        StartCoroutine(LoadSceneAsync(1));
    }

    IEnumerator Check(string type)
    {
        title = "正在读取" + type + ".zip";
        nowNum = 0;
        totalNum = 0;
        string filePath = Application.streamingAssetsPath + "/" + type + ".zip";
        var www = new WWW(filePath);
        while (!www.isDone)
        {
            float progress = Mathf.Clamp01(www.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }
        title = "正在解压" + type + ".zip";
        byte[] bytes = www.bytes;
        IEnumerator enumerator = ExtractZipFile(bytes, "");
        StartCoroutine(enumerator);
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    IEnumerator LoadSceneAsync(int scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        title = "正在进入游戏";
        nowNum = 0;
        totalNum = 0;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }
    }

    public static void FastExtractZipFile(string file, string dir, string password = "")
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        FastZip zip = new FastZip();
        zip.Password = password;
        zip.ExtractZip(file, dir, "");
    }

    IEnumerator ExtractZipFile(byte[] data, string outFolder)
    {
        ZipFile zf = null;
        using (MemoryStream mstrm = new MemoryStream(data))
        {
            zf = new ZipFile(mstrm);
            int count = 0;
            foreach (ZipEntry zipEntry in zf)
                count++;
            totalNum = count;
            nowNum = 0;
            extracting = true;
            foreach (ZipEntry zipEntry in zf)
            {
                nowNum++;
                if (!zipEntry.IsFile)
                {
                    continue;
                }
                String entryFileName = zipEntry.Name;
                byte[] buffer = new byte[4096];
                Stream zipStream = zf.GetInputStream(zipEntry);
                String fullZipToPath = Path.Combine(outFolder, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                    Directory.CreateDirectory(directoryName);
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
                yield return null;
            }
            if (zf != null)
            {
                zf.IsStreamOwner = true;
                zf.Close();
            }
        }
    }
}
