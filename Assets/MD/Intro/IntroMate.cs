using System.IO;
using UnityEngine;

public class IntroMate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AssetBundle AB = AssetBundle.LoadFromFile("assetbundle/mate/m07701_model");
        var prefab = AB.LoadAsset<GameObject>("m07701_model");
        Instantiate(prefab);

        AssetBundle AB2 = AssetBundle.LoadFromFile("assetbundle/mate/m12950_model");
        var prefab2 = AB2.LoadAsset<GameObject>("m12950_model");
        Instantiate(prefab2);


        //Animator[] animators = go.GetComponentsInChildren<Animator>();
        //foreach (Animator animator in animators)
        //{
        //    animator.gameObject.AddComponent<AnimationTest>();
        //    animator.gameObject.AddComponent<EventSETest>();
        //}

        string path = "assetbundle/duelentry/";
        if (Directory.Exists(path))
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*");
            for (int i = 0; i < files.Length; i++)
            {
                //Debug.Log("�ļ���:" + files[i].Name);
                //Debug.Log("�ļ�����·��:" + files[i].FullName);
                //Debug.Log("�ļ�����Ŀ¼:" + files[i].DirectoryName);
                var ab = AssetBundle.LoadFromFile(files[i].FullName);
                var prefabs = ab.LoadAllAssets();
                for (int j = 0; j < prefabs.Length; j++)
                {
                    Instantiate(prefabs[j]);
                }
            }
        }
        else Debug.Log("path do not exist!");
    }
}
