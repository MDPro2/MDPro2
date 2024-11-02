using UnityEngine;

public class TestLoad : MonoBehaviour
{
    GameObject a1;
    GameObject a2;
    public bool showMe;
    public bool showOp;
    void OnEnable()
    {
        //ABLoader.LoadABFromFile("timeline/summon/summonxyz/summonxyz03_01");
        Boot.root = "StreamingAssets/";
        ABLoader.LoadABFromFile("timeline/summon/summonxyz/summonxyz03_01");

    }

    private void Update()
    {

    }
}
