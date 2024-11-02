using DefaultNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AudioClipMergeTest : MonoBehaviour
{
    public int Hz = 44100;

    AudioClip s1;
    AudioClip s2;
    AudioClip s3;

    AudioSource source;
    AudioClip clip;

    string character1 = "V0001";
    string character2 = "V0501";

    AssetBundle ab1;
    AssetBundle ab2;
    void Start()
    {
        ab1 = AssetBundle.LoadFromFile(Boot.root + "assetbundle/voice/" + character1.ToLower());
        ab2 = AssetBundle.LoadFromFile(Boot.root + "assetbundle/voice/" + character2.ToLower());

        s1 = GetClipFromAB(0, "V0001_13_00_16_0");
        s2 = GetClipFromAB(0, "V0001_13_00_16_1");
        s3 = GetClipFromAB(0, "V0001_13_00_16_2");

        s1.name = "s1";
        s2.name = "s2";
        s3.name = "s3";

        float[] data1 = new float[s1.samples * s1.channels];
        float[] data2 = new float[s2.samples * s2.channels];
        float[] data3 = new float[s3.samples * s3.channels];

        s1.GetData(data1, 0);
        s2.GetData(data2, 0);
        s3.GetData(data3, 0);

        List<float> ar = new List<float>();
        ar.AddRange(data1);
        ar.AddRange(data2);
        ar.AddRange(data3);

        float[] datas = ar.ToArray();

        clip = AudioClip.Create("temp", datas.Length, 1, Hz, false);
        clip.SetData(datas, 0);

        source = this.GetComponent<AudioSource>();
        source.clip = clip;
        source.Play();
    }


    AudioClip GetClipFromAB(uint controller, string p)
    {
        if (controller == 0)
            return ab1.LoadAsset<AudioClip>(p);
        else
            return ab2.LoadAsset<AudioClip>(p);
    }
}
