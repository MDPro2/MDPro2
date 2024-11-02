using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;

public class LoadSFX : MonoBehaviour
{
    public static string spType = "SPECIAL";
    public static List<Card> materials = new List<Card>();
    public static bool me_pendulum;
    public static bool op_pendulum;
    //LinkTL
    public PlayableAsset summonLink01_01TL;
    public PlayableAsset summonLink02_01TL;
    public PlayableAsset summonLink03_01TL;
    public PlayableAsset summonLinkmain01TL;
    public PlayableAsset summonLinkmain02TL;
    public PlayableAsset summonLinkmain03TL;
    public PlayableAsset linkTrailIn01TL;
    public PlayableAsset linkTrailIn02TL;
    public PlayableAsset linkTrailIn03TL;
    public PlayableAsset summonLinkpostLinkTL;
    //SynchroTL
    public PlayableAsset summonSynchroPostSynchroTL;
    public PlayableAsset summonSynchro01TL;
    //XyzTL
    public PlayableAsset summonXYZ03_01TL;
    public PlayableAsset summonXYZexplosion01TL;
    public PlayableAsset summonXYZGalaxy01TL;
    public PlayableAsset summonXYZpostXYZTL;
    public PlayableAsset xyzTrailIn03TL;

    GameObject link1;
    GameObject link2;
    GameObject link3;
    GameObject synchro;
    GameObject xyz;

    public Texture2D LPendulumNum00;
    public Texture2D LPendulumNum01;
    public Texture2D LPendulumNum02;
    public Texture2D LPendulumNum03;
    public Texture2D LPendulumNum04;
    public Texture2D LPendulumNum05;
    public Texture2D LPendulumNum06;
    public Texture2D LPendulumNum07;
    public Texture2D LPendulumNum08;
    public Texture2D LPendulumNum09;
    public Texture2D RPendulumNum00;
    public Texture2D RPendulumNum01;
    public Texture2D RPendulumNum02;
    public Texture2D RPendulumNum03;
    public Texture2D RPendulumNum04;
    public Texture2D RPendulumNum05;
    public Texture2D RPendulumNum06;
    public Texture2D RPendulumNum07;
    public Texture2D RPendulumNum08;
    public Texture2D RPendulumNum09;

    //Card Hightlight
    public static GameObject hl_spsom;
    public static GameObject hl_spsom_sct;
    public static GameObject hl_set;
    public static GameObject hl_set_sct;


    Transform SFXContainer;
    private void OnEnable()
    {
        SFXContainer = new GameObject("SFXContainer").transform;

        hl_set = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_set_001", SFXContainer);
        hl_set_sct = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_set_sct_001", SFXContainer);
        hl_spsom = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_spsom_001", SFXContainer);
        hl_spsom_sct = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_spsom_sct_001", SFXContainer);
        hl_set.SetActive(false);
        hl_set_sct.SetActive(false);
        hl_spsom.SetActive(false);
        hl_spsom_sct.SetActive(false);
    }

    private void Start()
    {
        link1 = SummonLink1();
        link2 = SummonLink2();
        link3 = SummonLink3();
        link1.SetActive(false);
        link2.SetActive(false);
        link3.SetActive(false);
        synchro = SummonSynhro();
        synchro.SetActive(false);
        xyz = SummonXyz();
        xyz.SetActive(false);

        link1.transform.parent = SFXContainer;
        link2.transform.parent = SFXContainer;
        link3.transform.parent = SFXContainer;
        synchro.transform.parent = SFXContainer;
        xyz.transform.parent = SFXContainer;
    }
    public static void DelayDecoration(Vector3 pos, int position, string sfx = "无", string sound = "无", float t = 0, bool singleFile = false, int layer = 0)
    {
        LoadSFX loadSFX = GameObject.Find("Program").GetComponent<LoadSFX>();
        loadSFX.StartDelayDecoration(pos, position ,sfx ,sound, t, singleFile, layer);
    }

    void StartDelayDecoration(Vector3 pos, int position, string sfx = "无", string sound = "无", float t = 0, bool singleFile = false, int layer = 0)
    {
        StartCoroutine(Fx(pos, position, sfx, sound, t, singleFile, layer));
    }

    IEnumerator Fx(Vector3 pos, int position, string sfx = "无", string sound = "无", float t =0, bool singleFile = false, int layer = 0)
    {
        if(!Ocgcore.inSkiping) yield return new WaitForSeconds(t);
        GameObject decoration = null;
        if (sfx != "无" && !singleFile)
        {
            decoration = ABLoader.LoadABFromFolder(sfx, "Fx");
            decoration.transform.position = new Vector3(pos.x, pos.y - 0.1f, pos.z);
            if((position & (uint)CardPosition.Defence) > 0)
                decoration.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            decoration.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            Destroy(decoration, 10f);
        }
        else if (sfx != "无" && singleFile)
        {
            decoration = ABLoader.LoadABFromFile(sfx);
            decoration.transform.position = new Vector3(pos.x, pos.y - 0.1f, pos.z);
            decoration.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            Destroy(decoration, 10f);
        }
        if(decoration != null)
            ABLoader.ChangeLayer(decoration, LayerMask.LayerToName(layer));
        if (sound != "无") UIHelper.playSound(sound, 1f);
    }
    public static GameObject Decoration(string path, bool singleFile, Transform parent, string name = default)
    {
        GameObject decoration;
        if (singleFile)
            decoration = ABLoader.LoadABFromFile(path);
        else
            decoration = ABLoader.LoadABFromFolder(path, name);
        decoration.transform.parent = parent;
        return decoration;
    }

    public void SummonAnimation(float time, gameCard card)
    {
        StartCoroutine(DelaySummonAnimation(time, card));
    }

    IEnumerator DelaySummonAnimation(float time, gameCard card)
    {
        string type = spType;
        GameObject summon = null;
        if(type == "FUSION")
        {
            if (materials.Count > 5)
                summon = Decoration("timeline/summon/summonfusion/fusionnum", false, GameObject.Find("main_3d").transform, "SummonAnimation").transform.GetChild(0).gameObject;
            else
                summon = Decoration("timeline/summon/summonfusion/summonfusion0" + materials.Count + "_01", false, GameObject.Find("main_3d").transform, "SummonAnimation").transform.GetChild(0).gameObject;
            try
            {
                ABLoader.ChangeLayer(summon, "main_3d");
                Transform post = summon.transform;
                Transform front = summon.transform;
                Transform materialCards = summon.transform;
                foreach (Transform child in summon.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "PostFusion")
                        post = child;
                    if (child.name == "CardModel_front")
                        front = child;
                    if (child.name.StartsWith("SummonFusionInCard"))
                        materialCards = child;
                }
                Texture2D pic = GameTextureManager.GetCardPictureNow(card.get_data().Id);
                post.GetComponent<Renderer>().material.SetTexture("_CardFrameA", pic);
                front.GetComponent<Renderer>().material.SetTexture("_CardFrameA", pic);

                if (materialCards.name == "SummonFusionInCardNum")
                {
                    for (int i = 0; i < 6; i++)
                        materialCards.transform.GetChild(i).GetComponent<Renderer>().material.SetTexture("_CardFrameA", GameTextureManager.GetCardPictureNow(materials[i].Id));
                }
                else
                {
                    materialCards.GetChild(0).GetComponent<Renderer>().material.SetTexture("_CardFrameA", GameTextureManager.GetCardPictureNow(materials[0].Id));
                    if (materials.Count > 1)
                        materialCards.GetChild(0).GetComponent<Renderer>().material.SetTexture("_CardFrameB", GameTextureManager.GetCardPictureNow(materials[1].Id));
                    if (materials.Count > 2)
                        materialCards.GetChild(0).GetComponent<Renderer>().material.SetTexture("_CardFrameC", GameTextureManager.GetCardPictureNow(materials[2].Id));
                    if (materials.Count > 3)
                        materialCards.GetChild(0).GetComponent<Renderer>().material.SetTexture("_CardFrameD", GameTextureManager.GetCardPictureNow(materials[3].Id));
                    if (materials.Count > 4)
                        materialCards.GetChild(0).GetComponent<Renderer>().material.SetTexture("_CardFrameE", GameTextureManager.GetCardPictureNow(materials[4].Id));

                    if (materials.Count == 5)
                    {
                        materialCards.GetChild(1).GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetTexture("_CardFrameA", GameTextureManager.GetCardPictureNow(materials[0].Id));
                        materialCards.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetTexture("_CardFrameA", GameTextureManager.GetCardPictureNow(materials[1].Id));
                        materialCards.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetTexture("_CardFrameA", GameTextureManager.GetCardPictureNow(materials[2].Id));
                        materialCards.GetChild(1).GetChild(3).GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetTexture("_CardFrameA", GameTextureManager.GetCardPictureNow(materials[3].Id));
                        materialCards.GetChild(1).GetChild(4).GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetTexture("_CardFrameA", GameTextureManager.GetCardPictureNow(materials[4].Id));
                    }
                    else
                    {
                        foreach (Transform child in materialCards.GetComponentsInChildren<Transform>(true))
                        {
                            var renderer = child.GetComponent<Renderer>();
                            if (renderer != null && renderer.name != materialCards.GetChild(0).name)
                            {
                                renderer.material.CopyPropertiesFromMaterial(materialCards.GetChild(0).GetComponent<Renderer>().material);
                            }
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        else if (type == "RITUAL")
        {
            summon = Decoration("timeline/summon/summonritual/summonritual01", false, GameObject.Find("main_3d").transform, "SummonAnimation");
            foreach (Transform child in summon.transform)
            {
                if (child.GetComponent<MeshRenderer>() != null)
                {
                    Destroy(child.gameObject);                    
                }
                if (child.name == "SummonRitual01(Clone)")
                    summon = child.gameObject;
            }
            Vector3 black_ = summon.transform.Find("Black").localScale;
            summon.transform.Find("Black").localScale = new Vector3(black_.x * 10, black_.y, black_.z);
            ABLoader.ChangeLayer(summon, "main_3d");
            Transform post = summon.transform;
            Transform front = summon.transform;
            foreach (Transform child in summon.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "CardModel_frontAdd")
                    post = child;
                if (child.name == "CardModel_front")
                    front = child;
            }
            Texture2D pic = GameTextureManager.GetCardPictureNow(card.get_data().Id);
            post.GetComponent<Renderer>().material.mainTexture = pic;
            front.GetComponent<Renderer>().material.mainTexture = pic;

            if(materials.Count == 1)
            {
                foreach (Transform child in summon.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "RitualTrailIn02")
                        child.gameObject.SetActive(false);
                    if (child.name == "RitualTrailIn03")
                        child.gameObject.SetActive(false);
                }
            }
            else if (materials.Count == 2)
            {
                foreach (Transform child in summon.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "RitualTrailIn01")
                        child.gameObject.SetActive(false);
                    if (child.name == "RitualTrailIn03")
                        child.gameObject.SetActive(false);
                }
            }
            else if (materials.Count == 3)
            {
                foreach (Transform child in summon.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "RitualTrailIn01")
                        child.gameObject.SetActive(false);
                    if (child.name == "RitualTrailIn02")
                        child.gameObject.SetActive(false);
                }
            }
            else if (materials.Count == 4)
            {
                foreach (Transform child in summon.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "RitualTrailIn02")
                        child.gameObject.SetActive(false);
                }
            }
            else if (materials.Count == 5)
            {
                foreach (Transform child in summon.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "RitualTrailIn01")
                        child.gameObject.SetActive(false);
                }
            }
         }
        else if (type == "LINK")
        {
            GameObject link = null;
            summon = new GameObject();
            summon.name = "SummonAnimation";
            if (card.get_data().Level == 1)
            {
                link = Instantiate(link1);
                link.transform.parent = summon.transform;
            }                
            if (card.get_data().Level == 2)
            {
                link = Instantiate(link2);
                link.transform.parent = summon.transform;
            }                
            if(card.get_data().Level > 2)
            {
                link = Instantiate(link3);
                link.SetActive(true);
            }                
            link.transform.parent = summon.transform;
            summon.transform.parent = GameObject.Find("main_3d").transform;
            summon = summon.transform.GetChild(0).gameObject;
            ABLoader.ChangeLayer(summon, "main_3d");
            Transform post = summon.transform;
            Transform front = summon.transform;
            foreach (Transform child in summon.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "CardModel_frontAdd")
                    post = child;
                if (child.name == "CardModel_front")
                    front = child;
            }
            Texture2D pic = GameTextureManager.GetCardPictureNow(card.get_data().Id);
            post.GetComponent<Renderer>().material.mainTexture = pic;
            front.GetComponent<Renderer>().material.mainTexture = pic;

            Transform Markers = summon.transform.Find("Marker01");
            Transform Trails = summon.transform.Find("LinkTrailIn01");
            int one = 0;
            int one2 = 0;
            if (!card.get_data().HasLinkMarker(CardLinkMarker.TopLeft) || (one !=0 && card.get_data().Level < 6) || (one2 != 0 && card.get_data().Level == 6))
            {
                Destroy(Markers.transform.GetChild(0).gameObject);
                Destroy(Trails.transform.GetChild(0).gameObject);
            }
            else if (one == 0)
                one = 1;
            else if (one2 == 0 &&card.get_data().Level == 6)
                one2 = 1;
            if (!card.get_data().HasLinkMarker(CardLinkMarker.Top) || (one != 0 && card.get_data().Level < 6) || (one2 !=0 && card.get_data().Level == 6))
            {
                Destroy(Markers.transform.GetChild(1).gameObject);
                Destroy(Trails.transform.GetChild(1).gameObject);
            }
            else if (one == 0)
                one = 2;
            else if (one2 == 0 && card.get_data().Level == 6)
                one2 = 2;
            if (!card.get_data().HasLinkMarker(CardLinkMarker.TopRight) || (one != 0 && card.get_data().Level < 6) || (one2 != 0 && card.get_data().Level == 6))
            {
                Destroy(Markers.transform.GetChild(2).gameObject);
                Destroy(Trails.transform.GetChild(2).gameObject);
            }
            else if (one == 0)
                one = 3;
            else if (one2 == 0 && card.get_data().Level == 6)
                one2 = 3;
            if (!card.get_data().HasLinkMarker(CardLinkMarker.Left) || (one != 0 && card.get_data().Level < 6) || (one2 != 0 && card.get_data().Level == 6))
            {
                Destroy(Markers.transform.GetChild(3).gameObject);
                Destroy(Trails.transform.GetChild(3).gameObject);
            }
            else if (one == 0)
                one = 4;
            else if (one2 == 0 && card.get_data().Level == 6)
                one2 = 4;
            if (!card.get_data().HasLinkMarker(CardLinkMarker.Right) || (one != 0 && card.get_data().Level < 6) || (one2 != 0 && card.get_data().Level == 6))
            {
                Destroy(Markers.transform.GetChild(4).gameObject);
                Destroy(Trails.transform.GetChild(4).gameObject);
            }
            else if (one == 0)
                one = 5;
            else if (one2 == 0 && card.get_data().Level == 6)
                one2 = 5;
            if (!card.get_data().HasLinkMarker(CardLinkMarker.BottomLeft) || (one != 0 && card.get_data().Level < 6) || (one2 != 0 && card.get_data().Level == 6))
            {
                Destroy(Markers.transform.GetChild(5).gameObject);
                Destroy(Trails.transform.GetChild(5).gameObject);
            }
            else if (one == 0)
                one = 6;
            else if (one2 == 0 && card.get_data().Level == 6)
                one2 = 6;
            if (!card.get_data().HasLinkMarker(CardLinkMarker.Bottom) || (one != 0 && card.get_data().Level < 6) || (one2 != 0 && card.get_data().Level == 6))
            {
                Destroy(Markers.transform.GetChild(6).gameObject);
                Destroy(Trails.transform.GetChild(6).gameObject);
            }
            else if (one == 0)
                one = 7;
            else if (one2 == 0 && card.get_data().Level == 6)
                one2 = 7;
            if (!card.get_data().HasLinkMarker(CardLinkMarker.BottomRight) || (one != 0 && card.get_data().Level < 6) || (one2 != 0 && card.get_data().Level == 6))
            {
                Destroy(Markers.transform.GetChild(7).gameObject);
                Destroy(Trails.transform.GetChild(7).gameObject);
            }
            else if (one == 0)
                one = 8;
            else if (one2 == 0 && card.get_data().Level == 6)
                one2 = 8;

            int two = 0;
            int two2 = 0;
            if (card.get_data().Level > 1)
            {
                Markers = summon.transform.Find("Marker02");
                Trails = summon.transform.Find("LinkTrailIn02");
                if (!card.get_data().HasLinkMarker(CardLinkMarker.TopLeft) || one == 1 || one2 == 1 || (two != 0 && card.get_data().Level < 5) || (two2 != 0 && card.get_data().Level >4))
                {
                    Destroy(Markers.transform.GetChild(0).gameObject);
                    Destroy(Trails.transform.GetChild(0).gameObject);
                }
                else if (two == 0)
                    two = 1;
                else if (two2 == 0 && card.get_data().Level > 4)
                    two2 = 1;
                if (!card.get_data().HasLinkMarker(CardLinkMarker.Top) || one == 2 || one2 == 2 || (two != 0 && card.get_data().Level < 5) || (two2 != 0 && card.get_data().Level > 4))
                {
                    Destroy(Markers.transform.GetChild(1).gameObject);
                    Destroy(Trails.transform.GetChild(1).gameObject);
                }
                else if (two == 0)
                    two = 2;
                else if (two2 == 0 && card.get_data().Level > 4)
                    two2 = 2;
                if (!card.get_data().HasLinkMarker(CardLinkMarker.TopRight) || one == 3 || one2 == 3 || (two != 0 && card.get_data().Level < 5) || (two2 != 0 && card.get_data().Level > 4))
                {
                    Destroy(Markers.transform.GetChild(2).gameObject);
                    Destroy(Trails.transform.GetChild(2).gameObject);
                }
                else if (two == 0)
                    two = 3;
                else if (two2 == 0 && card.get_data().Level > 4)
                    two2 = 3;
                if (!card.get_data().HasLinkMarker(CardLinkMarker.Left) || one == 4 || one2 == 4 || (two != 0 && card.get_data().Level < 5) || (two2 != 0 && card.get_data().Level > 4))
                {
                    Destroy(Markers.transform.GetChild(3).gameObject);
                    Destroy(Trails.transform.GetChild(3).gameObject);
                }
                else if (two == 0)
                    two = 4;
                else if (two2 == 0 && card.get_data().Level > 4)
                    two2 = 4;
                if (!card.get_data().HasLinkMarker(CardLinkMarker.Right) || one == 5 || one2 == 5 || (two != 0 && card.get_data().Level < 5) || (two2 != 0 && card.get_data().Level > 4))
                {
                    Destroy(Markers.transform.GetChild(4).gameObject);
                    Destroy(Trails.transform.GetChild(4).gameObject);
                }
                else if (two == 0)
                    two = 5;
                else if (two2 == 0 && card.get_data().Level > 4)
                    two2 = 5;
                if (!card.get_data().HasLinkMarker(CardLinkMarker.BottomLeft) || one == 6 || one2 == 6 || (two != 0 && card.get_data().Level < 5) || (two2 != 0 && card.get_data().Level > 4))
                {
                    Destroy(Markers.transform.GetChild(5).gameObject);
                    Destroy(Trails.transform.GetChild(5).gameObject);
                }
                else if (two == 0)
                    two = 6;
                else if (two2 == 0 && card.get_data().Level > 4)
                    two2 = 6;
                if (!card.get_data().HasLinkMarker(CardLinkMarker.Bottom) || one == 7 || one2 == 7 || (two != 0 && card.get_data().Level < 5) || (two2 != 0 && card.get_data().Level > 4))
                {
                    Destroy(Markers.transform.GetChild(6).gameObject);
                    Destroy(Trails.transform.GetChild(6).gameObject);
                }
                else if (two == 0)
                    two = 7;
                else if (two2 == 0 && card.get_data().Level > 4)
                    two2 = 7;
                if (!card.get_data().HasLinkMarker(CardLinkMarker.BottomRight) || one == 8 || one2 == 8 || (two != 0 && card.get_data().Level < 5) || (two2 != 0 && card.get_data().Level > 4))
                {
                    Destroy(Markers.transform.GetChild(7).gameObject);
                    Destroy(Trails.transform.GetChild(7).gameObject);
                }
                else if (two == 0)
                    two = 8;
                else if (two2 == 0 && card.get_data().Level > 4)
                    two2 = 8;
            }
            if(card.get_data().Level > 2)
            {
                Markers = summon.transform.Find("Marker03");
                Trails = summon.transform.Find("LinkTrailIn03");
                if (!card.get_data().HasLinkMarker(CardLinkMarker.TopLeft) || one == 1 || one2 == 1 || two == 1 || two2 == 1)
                {
                    Destroy(Markers.transform.GetChild(0).gameObject);
                    Destroy(Trails.transform.GetChild(0).gameObject);
                }
                if (!card.get_data().HasLinkMarker(CardLinkMarker.Top) || one == 2 || one2 == 2 || two == 2 || two2 == 2)
                {
                    Destroy(Markers.transform.GetChild(1).gameObject);
                    Destroy(Trails.transform.GetChild(1).gameObject);
                }
                if (!card.get_data().HasLinkMarker(CardLinkMarker.TopRight) || one == 3 || one2 == 3 || two == 3 || two2 == 3)
                {
                    Destroy(Markers.transform.GetChild(2).gameObject);
                    Destroy(Trails.transform.GetChild(2).gameObject);
                }
                if (!card.get_data().HasLinkMarker(CardLinkMarker.Left) || one == 4 || one2 == 4 || two == 4 || two2 == 4)
                {
                    Destroy(Markers.transform.GetChild(3).gameObject);
                    Destroy(Trails.transform.GetChild(3).gameObject);
                }
                if (!card.get_data().HasLinkMarker(CardLinkMarker.Right) || one == 5 || one2 == 5 || two == 5 || two2 == 5)
                {
                    Destroy(Markers.transform.GetChild(4).gameObject);
                    Destroy(Trails.transform.GetChild(4).gameObject);
                }
                if (!card.get_data().HasLinkMarker(CardLinkMarker.BottomLeft) || one == 6 || one2 == 6 || two == 6 || two2 == 6)
                {
                    Destroy(Markers.transform.GetChild(5).gameObject);
                    Destroy(Trails.transform.GetChild(5).gameObject);
                }
                if (!card.get_data().HasLinkMarker(CardLinkMarker.Bottom) || one == 7 || one2 == 7 || two == 7 || two2 == 7)
                {
                    Destroy(Markers.transform.GetChild(6).gameObject);
                    Destroy(Trails.transform.GetChild(6).gameObject);
                }
                if (!card.get_data().HasLinkMarker(CardLinkMarker.BottomRight) || one == 8 || one2 == 8 || two == 8 || two2 == 8)
                {
                    Destroy(Markers.transform.GetChild(7).gameObject);
                    Destroy(Trails.transform.GetChild(7).gameObject);
                }
            }            
        }
        else if (type == "SYNCHRO")
        {
            GameObject _synchro = Instantiate(synchro);
            summon = new GameObject();
            summon.name = "SummonAnimation";
            _synchro.transform.parent = summon.transform;
            summon.transform.parent = GameObject.Find("main_3d").transform;
            summon = summon.transform.GetChild(0).gameObject;
            ABLoader.ChangeLayer(summon, "main_3d");
            Transform post = null;
            Transform front = null;
            Transform starSet = null;
            Transform circleSet = null;
            Transform marker = null;
            foreach (Transform child in summon.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "CardModel_frontAdd")
                    post = child;
                if (child.name == "CardModel_front")
                    front = child;
                if (child.name == "SynchroStarSet")
                    starSet = child;
                if (child.name == "SummonSynchroCircleSet")
                    circleSet = child;
                if (child.name == "Marker")
                    marker = child;
            }
            Texture2D pic = GameTextureManager.GetCardPictureNow(card.get_data().Id);
            post.GetComponent<Renderer>().material.mainTexture = pic;
            front.GetComponent<Renderer>().material.mainTexture = pic;

            if (card.get_data().Level > 4)
                Destroy(circleSet.GetChild(0).gameObject);
            else
                Destroy(circleSet.GetChild(1).gameObject);

            int tunerLV = 0;
            int nonTunerLV = 0;
            for(int i = 0; i< materials.Count; i++)
            {
                if ((materials[i].Type & (int)CardType.Tuner) > 0)
                    tunerLV += materials[i].Level;
            }
            if(tunerLV == 0)
                tunerLV = 1;
            nonTunerLV = card.get_data().Level - tunerLV;
            for (int i = 0; i< 11; i++)
            {
                if (i != tunerLV - 1)
                    Destroy(marker.GetChild(0).GetChild(0).GetChild(i).gameObject);
                if (i != nonTunerLV - 1)
                {
                    Destroy(starSet.GetChild(i).gameObject);
                    Destroy(marker.GetChild(1).GetChild(0).GetChild(i).gameObject);
                }                    
            }
        }
        else if(type == "XYZ")
        {
            GameObject _xyz = Instantiate(xyz);
            summon = new GameObject();
            summon.name = "SummonAnimation";
            _xyz.transform.parent = summon.transform;
            summon.transform.parent = GameObject.Find("main_3d").transform;
            summon = summon.transform.GetChild(0).gameObject;
            ABLoader.ChangeLayer(summon, "main_3d");
            Transform post = null;
            Transform trail = null;
            foreach(Transform t in summon.GetComponentsInChildren<Transform>(true))
            {
                if(t.name == "DummyCardHandForTL")
                    post = t;
                if (t.name == "XyzTrailIn03")
                    trail = t;
            }
            post.GetChild(1).GetComponent<Renderer>().material.mainTexture = GameTextureManager.GetCardPictureNow(card.get_data().Id);
            if(materials.Count <= 2)
            {
                Destroy(trail.GetChild(2).gameObject);
                Destroy(trail.GetChild(5).gameObject);
            }
            if (materials.Count == 1)
            {
                Destroy(trail.GetChild(1).gameObject);
                Destroy(trail.GetChild(4).gameObject);
            }
        }
        summon.SetActive(false);
        yield return new WaitForSeconds(time);
        summon.SetActive(true);
        Destroy(summon.transform.parent.gameObject, 6f);
    }
    public void MaterialShow(float time, bool hasCutin, float time_move, gameCard gameCard)
    {
        StartCoroutine(DelayMaterialShow(time, hasCutin, time_move, gameCard));
    }

    IEnumerator DelayMaterialShow(float time, bool hasCutin, float time_move, gameCard gameCard)
    {
        string type = spType;

        GameObject ms;
        switch (type)
        {
            case "FUSION":
                if(materials.Count > 8)
                {
                    ms = ABLoader.LoadABFromFolder("timeline/summon/summonfusion/summonfusionshowunitcard08");
                }
                else
                    ms = ABLoader.LoadABFromFolder("timeline/summon/summonfusion/summonfusionshowunitcard0" + materials.Count);
                break;
            case "LINK":
                if (materials.Count > 8)
                {
                    ms = ABLoader.LoadABFromFolder("timeline/summon/summonfusion/summonfusionshowunitcard08");
                }
                else
                    ms = ABLoader.LoadABFromFolder("timeline/summon/summonfusion/summonfusionshowunitcard0" + materials.Count);
                foreach(Transform t in ms.transform.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name.StartsWith("CardBack"))
                    {
                        var p = t.GetComponent<ParticleSystem>();
                        if (p != null)
                        {
                            var main = p.main;
                            main.startColor = Color.red;
                        }
                    }
                }
                break;
            case "SYNCHRO":
                if (materials.Count > 8)
                {
                    ms = ABLoader.LoadABFromFolder("timeline/summon/summonfusion/summonfusionshowunitcard08");
                }
                else
                    ms = ABLoader.LoadABFromFolder("timeline/summon/summonfusion/summonfusionshowunitcard0" + materials.Count);
                foreach (Transform t in ms.transform.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name.StartsWith("CardBack"))
                    {
                        var p = t.GetComponent<ParticleSystem>();
                        if (p != null)
                        {
                            var main = p.main;
                            main.startColor = Color.cyan;
                        }
                    }
                }
                break;
            case "XYZ":
                if (materials.Count > 8)
                {
                    ms = ABLoader.LoadABFromFolder("timeline/summon/summonfusion/summonfusionshowunitcard08");
                }
                else
                    ms = ABLoader.LoadABFromFolder("timeline/summon/summonfusion/summonfusionshowunitcard0" + materials.Count);
                foreach (Transform t in ms.transform.GetComponentsInChildren<Transform>(true))
                {
                    if (t.name.StartsWith("CardBack"))
                    {
                        var p = t.GetComponent<ParticleSystem>();
                        if (p != null)
                        {
                            var main = p.main;
                            main.startColor = Color.yellow;
                        }
                    }
                }
                break;
            case "RITUAL":
                if (materials.Count > 6)
                {
                    ms = ABLoader.LoadABFromFolder("timeline/summon/summonritual/summonritualshowunitcard06");
                }
                else
                    ms = ABLoader.LoadABFromFolder("timeline/summon/summonritual/summonritualshowunitcard0" + materials.Count);
                break;
            default:
                ms = ABLoader.LoadABFromFolder("timeline/summon/summonxyz/summonxyzshowunitcard0" + materials.Count);
                break;
        }
        
        ABLoader.ChangeLayer(ms, "main_3d");
        Transform cards = ms.transform.GetChild(0).GetChild(0);
        for(int i = 0; i < materials.Count; i++)
        {
            if (type == "RITUAL" && i == 6)
                break;
            if (i == 8)
                break;
            cards.GetChild(i).GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetComponent<Renderer>().material.mainTexture = GameTextureManager.GetCardPictureNow(materials[i].Id);
        }

        if (type == "SYNCHRO")
        {
            List<ParticleSystem> ps = new List<ParticleSystem>();
            foreach (Transform t in ms.transform.GetComponentsInChildren<Transform>(true))
            {
                if (t.name.StartsWith("CardBack"))
                {
                    var p = t.GetComponent<ParticleSystem>();
                    if (p != null)
                        ps.Add(p);
                }
            }
            for(int i =0; i < materials.Count; i++)
            {
                
                if ((materials[i].Type & (int)CardType.Tuner) > 0)
                {
                    var main = ps[2 * i].main;
                    main.startColor = Color.green;
                    main = ps[2 * i + 1].main;
                    main.startColor = Color.green;
                }
            }
        }
        ms.SetActive(false);
        int count = materials.Count;

        yield return new WaitForSeconds(time);

        if (type == "FUSION")
            BlackBehaviour.FadeIn(0.25f);
        if (materials.Count > 2)
            SEHandler.Play("SE_DUEL/SE_SMN_CMN_CARD_02");
        else
            SEHandler.Play("SE_DUEL/SE_SMN_CMN_CARD_01");

        ms.SetActive(true);
        ms.transform.GetChild(0).GetComponent<PlayableDirector>().Play();
        Destroy(ms, 2f);

        //启动SE
        var tsc = GameObject.Find("SE_Timeline").GetComponent<TimelineSEControl>();
        tsc.enabled = true;
        tsc.Play(ms, hasCutin, type, count, time_move, gameCard);
    }

    GameObject SummonLink1()
    {
        var go = ABLoader.LoadABFromFile("timeline/summon/summonlink/summonlink01_01");
        GameObject front1 = go;
        GameObject back1 = go;
        GameObject side1 = go;
        GameObject post = go;
        GameObject cardLocal = go;
        GameObject frontAdd = go;
        GameObject particle = go;
        GameObject particlePre = go;
        GameObject blackNormal = go;
        GameObject main01 = go;
        GameObject group = go;
        GameObject square02 = go;
        GameObject linkTrail01 = go;
        GameObject linkTrail0101 = go;
        GameObject linkTrail0103 = go;
        GameObject linkTrail0102 = go;
        GameObject linkTrail0104 = go;
        GameObject linkTrail0105 = go;
        GameObject linkTrail0106 = go;
        GameObject linkTrail0107 = go;
        GameObject linkTrail0108 = go;
        GameObject line = go;
        GameObject marker01 = go;
        foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.name == "CardModel_front")
                front1 = t.gameObject;
            if (t.gameObject.name == "CardModel_back")
                back1 = t.gameObject;
            if (t.gameObject.name == "CardModel_side")
                side1 = t.gameObject;
            if (t.gameObject.name == "SummonLinkpostLink")
                post = t.gameObject;
            if (t.gameObject.name == "CardLocal")
                cardLocal = t.gameObject;
            if (t.gameObject.name == "CardModel_frontAdd")
                frontAdd = t.gameObject;
            if (t.gameObject.name == "Particle")
                particle = t.gameObject;
            if (t.gameObject.name == "ParticlePre")
                particlePre = t.gameObject;

            if (t.gameObject.name == "BlackNormal")
                blackNormal = t.gameObject;
            if (t.gameObject.name == "SummonLinkmain01")
                main01 = t.gameObject;
            if (t.gameObject.name == "Group")
                group = t.gameObject;
            if (t.gameObject.name == "EtLinkGateSquare02")
                square02 = t.gameObject;
            if (t.gameObject.name == "LinkTrailIn01")
                linkTrail01 = t.gameObject;
            foreach (Transform tt in linkTrail01.transform)
            {

                if (tt.gameObject.name == "LinkTrailG01")
                    linkTrail0101 = tt.gameObject;

                if (tt.gameObject.name == "LinkTrailG02")
                {
                    linkTrail0102 = tt.gameObject;
                    if(!linkTrail0102.GetComponent<Animator>())
                        linkTrail0102.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG03")
                {
                    linkTrail0103 = tt.gameObject;
                    if (!linkTrail0103.GetComponent<Animator>())
                        linkTrail0103.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG04")
                {
                    linkTrail0104 = tt.gameObject;
                    if (!linkTrail0104.GetComponent<Animator>())
                        linkTrail0104.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG05")
                {
                    linkTrail0105 = tt.gameObject;
                    if (!linkTrail0105.GetComponent<Animator>())
                        linkTrail0105.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG06")
                {
                    linkTrail0106 = tt.gameObject;
                    if (!linkTrail0106.GetComponent<Animator>())
                        linkTrail0106.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG07")
                {
                    linkTrail0107 = tt.gameObject;
                    if (!linkTrail0107.GetComponent<Animator>())
                        linkTrail0107.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG08")
                {
                    linkTrail0108 = tt.gameObject;
                    if (!linkTrail0108.GetComponent<Animator>())
                        linkTrail0108.gameObject.AddComponent<Animator>();
                }
            }

            if (t.gameObject.name == "line")
                line = t.gameObject;
            if (t.gameObject.name.StartsWith("frare0"))
            {
                var main = t.GetComponent<ParticleSystem>().main;
                main.playOnAwake = true;
            }
            if (t.gameObject.name == "Marker01")
            {
                marker01 = t.gameObject;
                marker01.AddComponent<Animator>();
            }
        }
        blackNormal.transform.localEulerAngles = new Vector3(70f, 0f, 0f);

        var go2 = ABLoader.LoadABFromFolder("timeline/summon/summonritual/summonritual01");
        GameObject front2 = go2;
        GameObject back2 = go2;
        GameObject side2 = go2;
        foreach (Transform t in go2.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.name == "CardModel_front")
                front2 = t.gameObject;
            if (t.gameObject.name == "CardModel_back")
                back2 = t.gameObject;
            if (t.gameObject.name == "CardModel_side")
                side2 = t.gameObject;
        }
        front1.GetComponent<Renderer>().material = front2.GetComponent<Renderer>().material;
        back1.GetComponent<Renderer>().material = back2.GetComponent<Renderer>().material;
        side1.GetComponent<Renderer>().material = side2.GetComponent<Renderer>().material;

        Destroy(go2);

        PlayableDirector director = post.GetComponent<PlayableDirector>();
        director.playOnAwake = true;
        director.playableAsset = summonLinkpostLinkTL;
        Dictionary<string, PlayableBinding> bindingDict = new Dictionary<string, PlayableBinding>();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_CardLocal"].sourceObject, cardLocal);
        director.SetGenericBinding(bindingDict["Animation_FrontAdd"].sourceObject, frontAdd);
        director.SetGenericBinding(bindingDict["Activation_FrontAdd"].sourceObject, frontAdd);
        director.SetGenericBinding(bindingDict["Activation_Particle"].sourceObject, particle);
        director.SetGenericBinding(bindingDict["Activation_ParticlePre"].sourceObject, particlePre);


        director = main01.GetComponent<PlayableDirector>();
        director.playableAsset = summonLinkmain01TL;
        director.playOnAwake = true;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_Group"].sourceObject, group);

        director = linkTrail01.GetComponent<PlayableDirector>();
        director.playableAsset = linkTrailIn01TL;
        director.playOnAwake = true;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
//#if !UNITY_ANDROID
//        director.SetGenericBinding(bindingDict["Animation_Trail0101_android"].sourceObject, linkTrail0101);
//        director.SetGenericBinding(bindingDict["Animation_Trail0102_android"].sourceObject, linkTrail0102);
//        director.SetGenericBinding(bindingDict["Animation_Trail0103_android"].sourceObject, linkTrail0103);
//        director.SetGenericBinding(bindingDict["Animation_Trail0104_android"].sourceObject, linkTrail0104);
//        director.SetGenericBinding(bindingDict["Animation_Trail0105_android"].sourceObject, linkTrail0105);
//        director.SetGenericBinding(bindingDict["Animation_Trail0106_android"].sourceObject, linkTrail0106);
//        director.SetGenericBinding(bindingDict["Animation_Trail0107_android"].sourceObject, linkTrail0107);
//        director.SetGenericBinding(bindingDict["Animation_Trail0108_android"].sourceObject, linkTrail0108);
//#else
        director.SetGenericBinding(bindingDict["Animation_Trail0101"].sourceObject, linkTrail0101);
        director.SetGenericBinding(bindingDict["Animation_Trail0102"].sourceObject, linkTrail0102);
        director.SetGenericBinding(bindingDict["Animation_Trail0103"].sourceObject, linkTrail0103);
        director.SetGenericBinding(bindingDict["Animation_Trail0104"].sourceObject, linkTrail0104);
        director.SetGenericBinding(bindingDict["Animation_Trail0105"].sourceObject, linkTrail0105);
        director.SetGenericBinding(bindingDict["Animation_Trail0106"].sourceObject, linkTrail0106);
        director.SetGenericBinding(bindingDict["Animation_Trail0107"].sourceObject, linkTrail0107);
        director.SetGenericBinding(bindingDict["Animation_Trail0108"].sourceObject, linkTrail0108);
//#endif

        director = go.GetComponent<PlayableDirector>();
        director.playableAsset = summonLink01_01TL;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_BlackNormal"].sourceObject, blackNormal);
        director.SetGenericBinding(bindingDict["Activation_Main01"].sourceObject, main01);
        director.SetGenericBinding(bindingDict["Activation_Post"].sourceObject, post);
        director.SetGenericBinding(bindingDict["Activation_LinkTrail01"].sourceObject, linkTrail01);
        director.SetGenericBinding(bindingDict["Animation_Square02"].sourceObject, square02);
        director.SetGenericBinding(bindingDict["Activation_Marker01"].sourceObject, marker01);
        director.SetGenericBinding(bindingDict["Animation_Marker01"].sourceObject, marker01);
        director.SetGenericBinding(bindingDict["Activationl_Line"].sourceObject, line);

        TimelineAsset timelineAsset = summonLink01_01TL as TimelineAsset;
        TrackAsset track = timelineAsset.GetOutputTrack(2);//control_main01

        TimelineClip controlClip = null;
        foreach(var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "Main01Control";
            director.SetReferenceValue(new PropertyName("Main01Control"), main01);
        }

        track = timelineAsset.GetOutputTrack(4);//control_post

        controlClip = null;
        foreach (var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "PostLinkControl";
            director.SetReferenceValue(new PropertyName("PostLinkControl"), post);
        }
        Vector3 black = go.transform.Find("BlackNormal").localScale;
        go.transform.Find("BlackNormal").localScale = new Vector3(black.x * 10, black.y, black.z);

        return go;
    }

    GameObject SummonLink2()
    {
        var go = ABLoader.LoadABFromFile("timeline/summon/summonlink/summonlink02_01");
        GameObject front1 = go;
        GameObject back1 = go;
        GameObject side1 = go;
        GameObject post = go;
        GameObject cardLocal = go;
        GameObject frontAdd = go;
        GameObject particle = go;
        GameObject particlePre = go;
        GameObject blackNormal = go;
        GameObject main02 = go;
        GameObject group = go;
        GameObject square02 = go;
        GameObject linkTrail01 = go;
        GameObject linkTrail0101 = go;
        GameObject linkTrail0103 = go;
        GameObject linkTrail0102 = go;
        GameObject linkTrail0104 = go;
        GameObject linkTrail0105 = go;
        GameObject linkTrail0106 = go;
        GameObject linkTrail0107 = go;
        GameObject linkTrail0108 = go;
        GameObject linkTrail02 = go;
        GameObject linkTrail0201 = go;
        GameObject linkTrail0203 = go;
        GameObject linkTrail0202 = go;
        GameObject linkTrail0204 = go;
        GameObject linkTrail0205 = go;
        GameObject linkTrail0206 = go;
        GameObject linkTrail0207 = go;
        GameObject linkTrail0208 = go;
        GameObject line = go;
        GameObject marker01 = go;
        GameObject marker02 = go;
        foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.name == "CardModel_front")
                front1 = t.gameObject;
            if (t.gameObject.name == "CardModel_back")
                back1 = t.gameObject;
            if (t.gameObject.name == "CardModel_side")
                side1 = t.gameObject;
            if (t.gameObject.name == "SummonLinkpostLink")
                post = t.gameObject;
            if (t.gameObject.name == "CardLocal")
                cardLocal = t.gameObject;
            if (t.gameObject.name == "CardModel_frontAdd")
                frontAdd = t.gameObject;
            if (t.gameObject.name == "Particle")
                particle = t.gameObject;
            if (t.gameObject.name == "ParticlePre")
                particlePre = t.gameObject;

            if (t.gameObject.name == "BlackNormal")
                blackNormal = t.gameObject;
            if (t.gameObject.name == "SummonLinkmain02")
                main02 = t.gameObject;
            if (t.gameObject.name == "Group")
                group = t.gameObject;
            if (t.gameObject.name == "EtLinkGateSquare02")
                square02 = t.gameObject;
            if (t.gameObject.name == "LinkTrailIn01")
                linkTrail01 = t.gameObject;
            foreach (Transform tt in linkTrail01.transform)
            {

                if (tt.gameObject.name == "LinkTrailG01")
                    linkTrail0101 = tt.gameObject;

                if (tt.gameObject.name == "LinkTrailG02")
                {
                    linkTrail0102 = tt.gameObject;
                    if (!linkTrail0102.GetComponent<Animator>())
                        linkTrail0102.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG03")
                {
                    linkTrail0103 = tt.gameObject;
                    if (!linkTrail0103.GetComponent<Animator>())
                        linkTrail0103.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG04")
                {
                    linkTrail0104 = tt.gameObject;
                    if (!linkTrail0104.GetComponent<Animator>())
                        linkTrail0104.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG05")
                {
                    linkTrail0105 = tt.gameObject;
                    if (!linkTrail0105.GetComponent<Animator>())
                        linkTrail0105.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG06")
                {
                    linkTrail0106 = tt.gameObject;
                    if (!linkTrail0106.GetComponent<Animator>())
                        linkTrail0106.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG07")
                {
                    linkTrail0107 = tt.gameObject;
                    if (!linkTrail0107.GetComponent<Animator>())
                        linkTrail0107.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG08")
                {
                    linkTrail0108 = tt.gameObject;
                    if (!linkTrail0108.GetComponent<Animator>())
                        linkTrail0108.gameObject.AddComponent<Animator>();
                }



            }
            if (t.gameObject.name == "LinkTrailIn02")
                linkTrail02 = t.gameObject;
            foreach (Transform tt in linkTrail02.transform)
            {

                if (tt.gameObject.name == "LinkTrailG01")
                    linkTrail0201 = tt.gameObject;

                if (tt.gameObject.name == "LinkTrailG02")
                {
                    linkTrail0202 = tt.gameObject;
                    if (!linkTrail0202.GetComponent<Animator>())
                        linkTrail0202.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG03")
                {
                    linkTrail0203 = tt.gameObject;
                    if (!linkTrail0203.GetComponent<Animator>())
                        linkTrail0203.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG04")
                {
                    linkTrail0204 = tt.gameObject;
                    if (!linkTrail0204.GetComponent<Animator>())
                        linkTrail0204.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG05")
                {
                    linkTrail0205 = tt.gameObject;
                    if (!linkTrail0205.GetComponent<Animator>())
                        linkTrail0205.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG06")
                {
                    linkTrail0206 = tt.gameObject;
                    if (!linkTrail0206.GetComponent<Animator>())
                        linkTrail0206.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG07")
                {
                    linkTrail0207 = tt.gameObject;
                    if (!linkTrail0207.GetComponent<Animator>())
                        linkTrail0207.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG08")
                {
                    linkTrail0208 = tt.gameObject;
                    if (!linkTrail0208.GetComponent<Animator>())
                        linkTrail0208.gameObject.AddComponent<Animator>();
                }
            }
            if (t.gameObject.name == "line")
                line = t.gameObject;
            if (t.gameObject.name.StartsWith("frare0"))
            {
                var main = t.GetComponent<ParticleSystem>().main;
                main.playOnAwake = true;
            }
            if (t.gameObject.name == "Marker01")
            {
                marker01 = t.gameObject;
                marker01.AddComponent<Animator>();
            }
            if (t.gameObject.name == "Marker02")
            {
                marker02 = t.gameObject;
                marker02.AddComponent<Animator>();
            }
        }

        blackNormal.transform.localEulerAngles = new Vector3(70f, 0f, 0f);
        var go2 = ABLoader.LoadABFromFolder("timeline/summon/summonritual/summonritual01");
        GameObject front2 = go2;
        GameObject back2 = go2;
        GameObject side2 = go2;
        foreach (Transform t in go2.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.name == "CardModel_front")
                front2 = t.gameObject;
            if (t.gameObject.name == "CardModel_back")
                back2 = t.gameObject;
            if (t.gameObject.name == "CardModel_side")
                side2 = t.gameObject;
        }
        front1.GetComponent<Renderer>().material = front2.GetComponent<Renderer>().material;
        back1.GetComponent<Renderer>().material = back2.GetComponent<Renderer>().material;
        side1.GetComponent<Renderer>().material = side2.GetComponent<Renderer>().material;

        Destroy(go2);

        PlayableDirector director = post.GetComponent<PlayableDirector>();
        director.playOnAwake = true;
        director.playableAsset = summonLinkpostLinkTL;
        Dictionary<string, PlayableBinding> bindingDict = new Dictionary<string, PlayableBinding>();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_CardLocal"].sourceObject, cardLocal);
        director.SetGenericBinding(bindingDict["Animation_FrontAdd"].sourceObject, frontAdd);
        director.SetGenericBinding(bindingDict["Activation_FrontAdd"].sourceObject, frontAdd);
        director.SetGenericBinding(bindingDict["Activation_Particle"].sourceObject, particle);
        director.SetGenericBinding(bindingDict["Activation_ParticlePre"].sourceObject, particlePre);

        director = main02.GetComponent<PlayableDirector>();
        director.playableAsset = summonLinkmain02TL;
        director.playOnAwake = true;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_Group"].sourceObject, group);

        director = linkTrail01.GetComponent<PlayableDirector>();
        director.playableAsset = linkTrailIn01TL;
        director.playOnAwake = true;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_Trail0101"].sourceObject, linkTrail0101);
        director.SetGenericBinding(bindingDict["Animation_Trail0102"].sourceObject, linkTrail0102);
        director.SetGenericBinding(bindingDict["Animation_Trail0103"].sourceObject, linkTrail0103);
        director.SetGenericBinding(bindingDict["Animation_Trail0104"].sourceObject, linkTrail0104);
        director.SetGenericBinding(bindingDict["Animation_Trail0105"].sourceObject, linkTrail0105);
        director.SetGenericBinding(bindingDict["Animation_Trail0106"].sourceObject, linkTrail0106);
        director.SetGenericBinding(bindingDict["Animation_Trail0107"].sourceObject, linkTrail0107);
        director.SetGenericBinding(bindingDict["Animation_Trail0108"].sourceObject, linkTrail0108);

        director = linkTrail02.GetComponent<PlayableDirector>();
        director.playableAsset = linkTrailIn02TL;
        director.playOnAwake = true;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_Trail0201"].sourceObject, linkTrail0201);
        director.SetGenericBinding(bindingDict["Animation_Trail0202"].sourceObject, linkTrail0202);
        director.SetGenericBinding(bindingDict["Animation_Trail0203"].sourceObject, linkTrail0203);
        director.SetGenericBinding(bindingDict["Animation_Trail0204"].sourceObject, linkTrail0204);
        director.SetGenericBinding(bindingDict["Animation_Trail0205"].sourceObject, linkTrail0205);
        director.SetGenericBinding(bindingDict["Animation_Trail0206"].sourceObject, linkTrail0206);
        director.SetGenericBinding(bindingDict["Animation_Trail0207"].sourceObject, linkTrail0207);
        director.SetGenericBinding(bindingDict["Animation_Trail0208"].sourceObject, linkTrail0208);

        director = go.GetComponent<PlayableDirector>();
        director.playableAsset = summonLink02_01TL;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_BlackNormal"].sourceObject, blackNormal);
        director.SetGenericBinding(bindingDict["Activation_Main02"].sourceObject, main02);
        director.SetGenericBinding(bindingDict["Activation_Post"].sourceObject, post);
        director.SetGenericBinding(bindingDict["Activation_LinkTrail01"].sourceObject, linkTrail01);
        director.SetGenericBinding(bindingDict["Activation_LinkTrail02"].sourceObject, linkTrail02);
        director.SetGenericBinding(bindingDict["Animation_Square02"].sourceObject, square02);
        director.SetGenericBinding(bindingDict["Activation_Marker01"].sourceObject, marker01);
        director.SetGenericBinding(bindingDict["Animation_Marker0201"].sourceObject, marker01);
        director.SetGenericBinding(bindingDict["Activation_Marker02"].sourceObject, marker02);
        director.SetGenericBinding(bindingDict["Animation_Marker0202"].sourceObject, marker02);
        director.SetGenericBinding(bindingDict["Activationl_Line"].sourceObject, line);

        TimelineAsset timelineAsset = summonLink02_01TL as TimelineAsset;
        TrackAsset track = timelineAsset.GetOutputTrack(2);//control_main02

        TimelineClip controlClip = null;
        foreach (var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "Main02Control";
            director.SetReferenceValue(new PropertyName("Main02Control"), main02);
        }

        track = timelineAsset.GetOutputTrack(4);//control_post
        controlClip = null;
        foreach (var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "PostLinkControl";
            director.SetReferenceValue(new PropertyName("PostLinkControl"), post);
        }
        Vector3 black = go.transform.Find("BlackNormal").localScale;
        go.transform.Find("BlackNormal").localScale = new Vector3(black.x * 10, black.y, black.z);
        return go;
    }

    GameObject SummonLink3()
    {
        var go = ABLoader.LoadABFromFile("timeline/summon/summonlink/summonlink03_01");
        GameObject front1 = go;
        GameObject back1 = go;
        GameObject side1 = go;
        GameObject post = go;
        GameObject cardLocal = go;
        GameObject frontAdd = go;
        GameObject particle = go;
        GameObject particlePre = go;
        GameObject blackNormal = go;
        GameObject main03 = go;
        GameObject group = go;
        GameObject square02 = go;
        GameObject linkTrail01 = go;
        GameObject linkTrail0101 = go;
        GameObject linkTrail0103 = go;
        GameObject linkTrail0102 = go;
        GameObject linkTrail0104 = go;
        GameObject linkTrail0105 = go;
        GameObject linkTrail0106 = go;
        GameObject linkTrail0107 = go;
        GameObject linkTrail0108 = go;
        GameObject linkTrail02 = go;
        GameObject linkTrail0201 = go;
        GameObject linkTrail0203 = go;
        GameObject linkTrail0202 = go;
        GameObject linkTrail0204 = go;
        GameObject linkTrail0205 = go;
        GameObject linkTrail0206 = go;
        GameObject linkTrail0207 = go;
        GameObject linkTrail0208 = go;
        GameObject linkTrail03 = go;
        GameObject linkTrail0301 = go;
        GameObject linkTrail0303 = go;
        GameObject linkTrail0302 = go;
        GameObject linkTrail0304 = go;
        GameObject linkTrail0305 = go;
        GameObject linkTrail0306 = go;
        GameObject linkTrail0307 = go;
        GameObject linkTrail0308 = go;
        GameObject line = go;
        GameObject marker01 = go;
        GameObject marker02 = go;
        GameObject marker03 = go;
        foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.name == "CardModel_front")
                front1 = t.gameObject;
            if (t.gameObject.name == "CardModel_back")
                back1 = t.gameObject;
            if (t.gameObject.name == "CardModel_side")
                side1 = t.gameObject;
            if (t.gameObject.name == "SummonLinkpostLink")
                post = t.gameObject;
            if (t.gameObject.name == "CardLocal")
                cardLocal = t.gameObject;
            if (t.gameObject.name == "CardModel_frontAdd")
                frontAdd = t.gameObject;
            if (t.gameObject.name == "Particle")
                particle = t.gameObject;
            if (t.gameObject.name == "ParticlePre")
                particlePre = t.gameObject;

            if (t.gameObject.name == "BlackNormal")
                blackNormal = t.gameObject;
            if (t.gameObject.name == "SummonLinkmain03")
                main03 = t.gameObject;
            if (t.gameObject.name == "Group")
                group = t.gameObject;
            if (t.gameObject.name == "EtLinkGateSquare02")
                square02 = t.gameObject;
            if (t.gameObject.name == "LinkTrailIn01")
                linkTrail01 = t.gameObject;
            foreach (Transform tt in linkTrail01.transform)
            {

                if (tt.gameObject.name == "LinkTrailG01")
                    linkTrail0101 = tt.gameObject;

                if (tt.gameObject.name == "LinkTrailG02")
                {
                    linkTrail0102 = tt.gameObject;
                    if (!linkTrail0102.GetComponent<Animator>())
                        linkTrail0102.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG03")
                {
                    linkTrail0103 = tt.gameObject;
                    if (!linkTrail0103.GetComponent<Animator>())
                        linkTrail0103.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG04")
                {
                    linkTrail0104 = tt.gameObject;
                    if (!linkTrail0104.GetComponent<Animator>())
                        linkTrail0104.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG05")
                {
                    linkTrail0105 = tt.gameObject;
                    if (!linkTrail0105.GetComponent<Animator>())
                        linkTrail0105.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG06")
                {
                    linkTrail0106 = tt.gameObject;
                    if (!linkTrail0106.GetComponent<Animator>())
                        linkTrail0106.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG07")
                {
                    linkTrail0107 = tt.gameObject;
                    if (!linkTrail0107.GetComponent<Animator>())
                        linkTrail0107.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG08")
                {
                    linkTrail0108 = tt.gameObject;
                    if (!linkTrail0108.GetComponent<Animator>())
                        linkTrail0108.gameObject.AddComponent<Animator>();
                }
            }
            if (t.gameObject.name == "LinkTrailIn02")
                linkTrail02 = t.gameObject;
            foreach (Transform tt in linkTrail02.transform)
            {

                if (tt.gameObject.name == "LinkTrailG01")
                    linkTrail0201 = tt.gameObject;

                if (tt.gameObject.name == "LinkTrailG02")
                {
                    linkTrail0202 = tt.gameObject;
                    if (!linkTrail0202.GetComponent<Animator>())
                        linkTrail0202.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG03")
                {
                    linkTrail0203 = tt.gameObject;
                    if (!linkTrail0203.GetComponent<Animator>())
                        linkTrail0203.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG04")
                {
                    linkTrail0204 = tt.gameObject;
                    if (!linkTrail0204.GetComponent<Animator>())
                        linkTrail0204.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG05")
                {
                    linkTrail0205 = tt.gameObject;
                    if (!linkTrail0205.GetComponent<Animator>())
                        linkTrail0205.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG06")
                {
                    linkTrail0206 = tt.gameObject;
                    if (!linkTrail0206.GetComponent<Animator>())
                        linkTrail0206.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG07")
                {
                    linkTrail0207 = tt.gameObject;
                    if (!linkTrail0207.GetComponent<Animator>())
                        linkTrail0207.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG08")
                {
                    linkTrail0208 = tt.gameObject;
                    if (!linkTrail0208.GetComponent<Animator>())
                        linkTrail0208.gameObject.AddComponent<Animator>();
                }
            }
            if (t.gameObject.name == "LinkTrailIn03")
                linkTrail03 = t.gameObject;
            foreach (Transform tt in linkTrail03.transform)
            {

                if (tt.gameObject.name == "LinkTrailG01")
                    linkTrail0301 = tt.gameObject;

                if (tt.gameObject.name == "LinkTrailG02")
                {
                    linkTrail0302 = tt.gameObject;
                    if (!linkTrail0302.GetComponent<Animator>())
                        linkTrail0302.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG03")
                {
                    linkTrail0303 = tt.gameObject;
                    if (!linkTrail0303.GetComponent<Animator>())
                        linkTrail0303.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG04")
                {
                    linkTrail0304 = tt.gameObject;
                    if (!linkTrail0304.GetComponent<Animator>())
                        linkTrail0304.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG05")
                {
                    linkTrail0305 = tt.gameObject;
                    if (!linkTrail0305.GetComponent<Animator>())
                        linkTrail0305.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG06")
                {
                    linkTrail0306 = tt.gameObject;
                    if (!linkTrail0306.GetComponent<Animator>())
                        linkTrail0306.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG07")
                {
                    linkTrail0307 = tt.gameObject;
                    if (!linkTrail0307.GetComponent<Animator>())
                        linkTrail0307.gameObject.AddComponent<Animator>();
                }
                if (tt.gameObject.name == "LinkTrailG08")
                {
                    linkTrail0308 = tt.gameObject;
                    if (!linkTrail0308.GetComponent<Animator>())
                        linkTrail0308.gameObject.AddComponent<Animator>();
                }
            }
            if (t.gameObject.name == "line")
                line = t.gameObject;
            if (t.gameObject.name.StartsWith("frare0"))
            {
                var main = t.GetComponent<ParticleSystem>().main;
                main.playOnAwake = true;
            }
            if (t.gameObject.name == "Marker01")
            {
                marker01 = t.gameObject;
                marker01.AddComponent<Animator>();
            }
            if (t.gameObject.name == "Marker02")
            {
                marker02 = t.gameObject;
                marker02.AddComponent<Animator>();
            }
            if (t.gameObject.name == "Marker03")
            {
                marker03 = t.gameObject;
                marker03.AddComponent<Animator>();
            }
        }

        blackNormal.transform.localEulerAngles = new Vector3(70f, 0f, 0f);
        var go2 = ABLoader.LoadABFromFolder("timeline/summon/summonritual/summonritual01");
        GameObject front2 = go2;
        GameObject back2 = go2;
        GameObject side2 = go2;
        foreach (Transform t in go2.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.name == "CardModel_front")
                front2 = t.gameObject;
            if (t.gameObject.name == "CardModel_back")
                back2 = t.gameObject;
            if (t.gameObject.name == "CardModel_side")
                side2 = t.gameObject;
        }
        front1.GetComponent<Renderer>().material = front2.GetComponent<Renderer>().material;
        back1.GetComponent<Renderer>().material = back2.GetComponent<Renderer>().material;
        side1.GetComponent<Renderer>().material = side2.GetComponent<Renderer>().material;

        Destroy(go2);

        PlayableDirector director = post.GetComponent<PlayableDirector>();
        director.playOnAwake = true;
        director.playableAsset = summonLinkpostLinkTL;
        Dictionary<string, PlayableBinding> bindingDict = new Dictionary<string, PlayableBinding>();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_CardLocal"].sourceObject, cardLocal);
        director.SetGenericBinding(bindingDict["Animation_FrontAdd"].sourceObject, frontAdd);
        director.SetGenericBinding(bindingDict["Activation_FrontAdd"].sourceObject, frontAdd);
        director.SetGenericBinding(bindingDict["Activation_Particle"].sourceObject, particle);
        director.SetGenericBinding(bindingDict["Activation_ParticlePre"].sourceObject, particlePre);

        director = main03.GetComponent<PlayableDirector>();
        director.playableAsset = summonLinkmain03TL;
        director.playOnAwake = true;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_Group"].sourceObject, group);

        director = linkTrail01.GetComponent<PlayableDirector>();
        director.playableAsset = linkTrailIn01TL;
        director.playOnAwake = true;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_Trail0101"].sourceObject, linkTrail0101);
        director.SetGenericBinding(bindingDict["Animation_Trail0102"].sourceObject, linkTrail0102);
        director.SetGenericBinding(bindingDict["Animation_Trail0103"].sourceObject, linkTrail0103);
        director.SetGenericBinding(bindingDict["Animation_Trail0104"].sourceObject, linkTrail0104);
        director.SetGenericBinding(bindingDict["Animation_Trail0105"].sourceObject, linkTrail0105);
        director.SetGenericBinding(bindingDict["Animation_Trail0106"].sourceObject, linkTrail0106);
        director.SetGenericBinding(bindingDict["Animation_Trail0107"].sourceObject, linkTrail0107);
        director.SetGenericBinding(bindingDict["Animation_Trail0108"].sourceObject, linkTrail0108);

        director = linkTrail02.GetComponent<PlayableDirector>();
        director.playableAsset = linkTrailIn02TL;
        director.playOnAwake = true;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_Trail0201"].sourceObject, linkTrail0201);
        director.SetGenericBinding(bindingDict["Animation_Trail0202"].sourceObject, linkTrail0202);
        director.SetGenericBinding(bindingDict["Animation_Trail0203"].sourceObject, linkTrail0203);
        director.SetGenericBinding(bindingDict["Animation_Trail0204"].sourceObject, linkTrail0204);
        director.SetGenericBinding(bindingDict["Animation_Trail0205"].sourceObject, linkTrail0205);
        director.SetGenericBinding(bindingDict["Animation_Trail0206"].sourceObject, linkTrail0206);
        director.SetGenericBinding(bindingDict["Animation_Trail0207"].sourceObject, linkTrail0207);
        director.SetGenericBinding(bindingDict["Animation_Trail0208"].sourceObject, linkTrail0208);

        director = linkTrail03.GetComponent<PlayableDirector>();
        director.playableAsset = linkTrailIn03TL;
        director.playOnAwake = true;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_Trail0301"].sourceObject, linkTrail0301);
        director.SetGenericBinding(bindingDict["Animation_Trail0302"].sourceObject, linkTrail0302);
        director.SetGenericBinding(bindingDict["Animation_Trail0303"].sourceObject, linkTrail0303);
        director.SetGenericBinding(bindingDict["Animation_Trail0304"].sourceObject, linkTrail0304);
        director.SetGenericBinding(bindingDict["Animation_Trail0305"].sourceObject, linkTrail0305);
        director.SetGenericBinding(bindingDict["Animation_Trail0306"].sourceObject, linkTrail0306);
        director.SetGenericBinding(bindingDict["Animation_Trail0307"].sourceObject, linkTrail0307);
        director.SetGenericBinding(bindingDict["Animation_Trail0308"].sourceObject, linkTrail0308);

        director = go.GetComponent<PlayableDirector>();
        director.playableAsset = summonLink03_01TL;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_BlackNormal"].sourceObject, blackNormal);
        director.SetGenericBinding(bindingDict["Activation_Main03"].sourceObject, main03);
        director.SetGenericBinding(bindingDict["Activation_Post"].sourceObject, post);
        director.SetGenericBinding(bindingDict["Activation_LinkTrail01"].sourceObject, linkTrail01);
        director.SetGenericBinding(bindingDict["Activation_LinkTrail02"].sourceObject, linkTrail02);
        director.SetGenericBinding(bindingDict["Activation_LinkTrail03"].sourceObject, linkTrail03);
        director.SetGenericBinding(bindingDict["Animation_Square02"].sourceObject, square02);
        director.SetGenericBinding(bindingDict["Activation_Marker01"].sourceObject, marker01);
        director.SetGenericBinding(bindingDict["Animation_Marker0301"].sourceObject, marker01);
        director.SetGenericBinding(bindingDict["Activation_Marker02"].sourceObject, marker02);
        director.SetGenericBinding(bindingDict["Animation_Marker0302"].sourceObject, marker02);
        director.SetGenericBinding(bindingDict["Activation_Marker03"].sourceObject, marker03);
        director.SetGenericBinding(bindingDict["Animation_Marker0303"].sourceObject, marker03);
        director.SetGenericBinding(bindingDict["Activationl_Line"].sourceObject, line);

        TimelineAsset timelineAsset = summonLink03_01TL as TimelineAsset;
        TrackAsset track = timelineAsset.GetOutputTrack(2);//control_main03

        TimelineClip controlClip = null;
        foreach (var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "Main03Control";
            director.SetReferenceValue(new PropertyName("Main03Control"), main03);
        }

        track = timelineAsset.GetOutputTrack(4);//control_post
        controlClip = null;
        foreach (var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "PostLinkControl";
            director.SetReferenceValue(new PropertyName("PostLinkControl"), post);
        }
        Vector3 black = go.transform.Find("BlackNormal").localScale;
        go.transform.Find("BlackNormal").localScale = new Vector3(black.x * 10, black.y, black.z);

        return go;
    }

    GameObject SummonSynhro()
    {
        var go = ABLoader.LoadABFromFile("timeline/summon/summonsynchro/summonsynchro01");
        GameObject post = null;
        GameObject cardLocal = null;
        GameObject frontAdd = null;
        GameObject particle = null;
        GameObject black = null;
        GameObject speedline = null;
        GameObject speedlineWhite = null;
        GameObject starBG = null;
        GameObject centerFrare = null;
        GameObject star1 = null;
        GameObject star2 = null;
        GameObject star3 = null;
        GameObject star4 = null;
        GameObject star5 = null;
        GameObject star6 = null;
        GameObject star7 = null;
        GameObject star8 = null;
        GameObject star9 = null;
        GameObject star10 = null;
        GameObject star11 = null;
        GameObject marker = null;
        GameObject circle1 = null;
        GameObject circle2 = null;
        GameObject circle3 = null;
        foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.name == "SummonSynchroPostSynchro")
                post = t.gameObject;
            if (t.gameObject.name == "CardLocal")
                cardLocal = t.gameObject;
            if (t.gameObject.name == "CardModel_frontAdd")
                frontAdd = t.gameObject;
            if (t.gameObject.name == "Particle")
                particle = t.gameObject;
            if (t.gameObject.name == "Black")
                black = t.gameObject;
            if (t.gameObject.name == "SummonSynchroSpeedLine01")
                speedline = t.gameObject;
            if (t.gameObject.name == "SummonSynchroSpeedLine01White")
                speedlineWhite = t.gameObject;
            if (t.gameObject.name == "starBG")
                starBG = t.gameObject;
            if (t.gameObject.name == "CenterFrare")
                centerFrare = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel01_01")
                star1 = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel02_01")
                star2 = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel03_01")
                star3 = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel04_01")
                star4 = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel05_01")
                star5 = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel06_01")
                star6 = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel07_01")
                star7 = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel08_01")
                star8 = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel09_01")
                star9 = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel10_01")
                star10 = t.gameObject;
            if (t.gameObject.name == "SynchroStarLevel11_01")
                star11 = t.gameObject;
            if (t.gameObject.name == "Marker")
                marker = t.gameObject;
            if (t.gameObject.name == "SummonSynchroCircle01_01")
                circle1 = t.GetChild(0).GetChild(0).gameObject;
            if (t.gameObject.name == "SummonSynchroCircle02_01")
                circle2 = t.GetChild(0).GetChild(0).gameObject;
            if (t.gameObject.name == "SummonSynchroCircle03_01")
                circle3 = t.GetChild(0).GetChild(0).gameObject;
        }

        PlayableDirector director = post.GetComponent<PlayableDirector>();
        director.playOnAwake = true;
        director.playableAsset = summonSynchroPostSynchroTL;
        Dictionary<string, PlayableBinding> bindingDict = new Dictionary<string, PlayableBinding>();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_CardLocal"].sourceObject, cardLocal);
        director.SetGenericBinding(bindingDict["Animation_FrontAdd"].sourceObject, frontAdd);
        director.SetGenericBinding(bindingDict["Activation_FrontAdd"].sourceObject, frontAdd);
        director.SetGenericBinding(bindingDict["Activation_Particle"].sourceObject, particle);



        director = go.GetComponent<PlayableDirector>();
        director.playableAsset = summonSynchro01TL;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_Black"].sourceObject, black);
        director.SetGenericBinding(bindingDict["Animation_Speedline"].sourceObject, speedline);
        director.SetGenericBinding(bindingDict["Animation_SpeedlineWhite"].sourceObject, speedlineWhite);
        director.SetGenericBinding(bindingDict["Activation_starBG"].sourceObject, starBG);
        director.SetGenericBinding(bindingDict["Activation_centerFrare"].sourceObject, centerFrare);
        director.SetGenericBinding(bindingDict["Animation_Star1"].sourceObject, star1);
        director.SetGenericBinding(bindingDict["Activation_Star1"].sourceObject, star1);
        director.SetGenericBinding(bindingDict["Animation_Star2"].sourceObject, star2);
        director.SetGenericBinding(bindingDict["Activation_Star2"].sourceObject, star2);
        director.SetGenericBinding(bindingDict["Animation_Star3"].sourceObject, star3);
        director.SetGenericBinding(bindingDict["Activation_Star3"].sourceObject, star3);
        director.SetGenericBinding(bindingDict["Animation_Star4"].sourceObject, star4);
        director.SetGenericBinding(bindingDict["Activation_Star4"].sourceObject, star4);
        director.SetGenericBinding(bindingDict["Animation_Star5"].sourceObject, star5);
        director.SetGenericBinding(bindingDict["Activation_Star5"].sourceObject, star5);
        director.SetGenericBinding(bindingDict["Animation_Star6"].sourceObject, star6);
        director.SetGenericBinding(bindingDict["Activation_Star6"].sourceObject, star6);
        director.SetGenericBinding(bindingDict["Animation_Star7"].sourceObject, star7);
        director.SetGenericBinding(bindingDict["Activation_Star7"].sourceObject, star7);
        director.SetGenericBinding(bindingDict["Animation_Star8"].sourceObject, star8);
        director.SetGenericBinding(bindingDict["Activation_Star8"].sourceObject, star8);
        director.SetGenericBinding(bindingDict["Animation_Star9"].sourceObject, star9);
        director.SetGenericBinding(bindingDict["Activation_Star9"].sourceObject, star9);
        director.SetGenericBinding(bindingDict["Animation_Star10"].sourceObject, star10);
        director.SetGenericBinding(bindingDict["Activation_Star10"].sourceObject, star10);
        director.SetGenericBinding(bindingDict["Animation_Star11"].sourceObject, star11);
        director.SetGenericBinding(bindingDict["Activation_Star11"].sourceObject, star11);
        director.SetGenericBinding(bindingDict["Animation_Marker"].sourceObject, marker);
        director.SetGenericBinding(bindingDict["Animation_Cirecle1"].sourceObject, circle1);
        director.SetGenericBinding(bindingDict["Animation_Cirecle2"].sourceObject, circle2);
        director.SetGenericBinding(bindingDict["Animation_Cirecle3"].sourceObject, circle3);
        director.SetGenericBinding(bindingDict["Activation_Cirecle1"].sourceObject, circle1.transform.parent.parent.gameObject);
        director.SetGenericBinding(bindingDict["Activation_Cirecle2"].sourceObject, circle2.transform.parent.parent.gameObject);
        director.SetGenericBinding(bindingDict["Activation_Cirecle3"].sourceObject, circle3.transform.parent.parent.gameObject);

        Destroy(circle3);
        circle1.transform.parent.parent.localScale = new Vector3(2f, 2f, 2f);
        circle2.transform.parent.parent.localScale = new Vector3(2f, 2f, 2f);


        TimelineAsset timelineAsset = summonSynchro01TL as TimelineAsset;
        TrackAsset track = timelineAsset.GetOutputTrack(5);

        TimelineClip controlClip = null;
        foreach (var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "SynchroPost";
            director.SetReferenceValue(new PropertyName("SynchroPost"), post);
        }

        Vector3 black_ = go.transform.Find("BGOffset/Black").localScale;
        go.transform.Find("BGOffset/Black").localScale = new Vector3(black_.x * 10, black_.y, black_.z);

        return go;
}

    GameObject SummonXyz()
    {
        var go = ABLoader.LoadABFromFile("timeline/summon/summonxyz/summonxyz03_01");
        GameObject front1 = null;
        GameObject back1 = null;
        GameObject side1 = null;

        GameObject post = null;
        GameObject cardLocal = null;
        GameObject particle = null;
        GameObject black = null;
        GameObject galaxy = null;
        GameObject trail = null;
        GameObject xyzInMesh1 = null;
        GameObject xyzInMesh2 = null;
        GameObject xyzInMesh3 = null;
        GameObject flare1 = null;
        GameObject flare2 = null;
        GameObject flare3 = null;
        GameObject explosion = null;
        GameObject column = null;
        GameObject hemisphere = null;
        GameObject frare = null;
        GameObject frare01 = null;
        GameObject circleFrare = null;
        GameObject aurora01 = null;
        GameObject aurora02 = null;
        GameObject aurora03 = null;
        GameObject hole = null;
        GameObject spiral01 = null;
        GameObject spiral02 = null;
        GameObject spiral03 = null;
        GameObject particle01 = null;
        GameObject particle02 = null;
        GameObject particle03 = null;
        foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.name == "CardModel_front")
                front1 = t.gameObject;
            if (t.gameObject.name == "CardModel_back")
                back1 = t.gameObject;
            if (t.gameObject.name == "CardModel_side")
                side1 = t.gameObject;
            if (t.gameObject.name.StartsWith("SummonXYZShowUnitCard"))
                Destroy(t.gameObject);
            if (t.gameObject.name == "SummonXYZpostXYZ")
                post = t.gameObject;
            if (t.gameObject.name == "CardLocal")
                cardLocal = t.gameObject;
            if (t.gameObject.name == "CardParticle")
                particle = t.gameObject;
            if (t.gameObject.name == "Black")
                black = t.gameObject;

            if (t.gameObject.name == "SummonXYZGalaxy01")
                galaxy = t.gameObject;
            if (t.gameObject.name.StartsWith("XyzTrailIn"))
                trail = t.gameObject;
            if (t.gameObject.name == "XYZInMesh01")
                xyzInMesh1 = t.gameObject;
            if (t.gameObject.name == "XYZInMesh02")
                xyzInMesh2 = t.gameObject;
            if (t.gameObject.name == "XYZInMesh03")
                xyzInMesh3 = t.gameObject;
            if (t.gameObject.name == "CenterFlare01")
                flare1 = t.gameObject;
            if (t.gameObject.name == "CenterFlare02")
                flare2 = t.gameObject;
            if (t.gameObject.name == "CenterFlare03")
                flare3 = t.gameObject;
            if (t.gameObject.name == "SummonXYZexplosion01")
                explosion = t.gameObject;
            if (t.gameObject.name == "Column")
                column = t.gameObject;
            if (t.gameObject.name == "Hemisphere")
                hemisphere = t.gameObject;
            if (t.gameObject.name == "frare")
                frare = t.gameObject;
            if (t.gameObject.name == "frare01")
                frare01 = t.gameObject;
            if (t.gameObject.name == "CircleFrare")
                circleFrare = t.gameObject;
            if (t.gameObject.name == " Aurora01")
                aurora01 = t.gameObject;
            if (t.gameObject.name == " Aurora02")
                aurora02 = t.gameObject;
            if (t.gameObject.name == " Aurora03")
                aurora03 = t.gameObject;
            if (t.gameObject.name == "hole")
                hole = t.gameObject;
            if (t.gameObject.name == "SpiralAnim01")
                spiral01 = t.gameObject;
            if (t.gameObject.name == "SpiralAnim02")
                spiral02 = t.gameObject;
            if (t.gameObject.name == "SpiralAnim03")
                spiral03 = t.gameObject;
            if (t.gameObject.name == "particle01")
            {
                particle01 = t.gameObject;
                var main = particle01.GetComponent<ParticleSystem>().main;
                main.playOnAwake = true;
            }
            if (t.gameObject.name == "particle02")
            {
                particle02 = t.gameObject;
                var main = particle02.GetComponent<ParticleSystem>().main;
                main.playOnAwake = true;
            }
            if (t.gameObject.name == "particle03")
            {
                particle03 = t.gameObject;
                var main = particle03.GetComponent<ParticleSystem>().main;
                main.playOnAwake = true;
            }
        }
        var go2 = ABLoader.LoadABFromFolder("timeline/summon/summonritual/summonritual01");
        GameObject front2 = null;
        GameObject back2 = null;
        GameObject side2 = null;
        foreach (Transform t in go2.GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.name == "CardModel_front")
                front2 = t.gameObject;
            if (t.gameObject.name == "CardModel_back")
                back2 = t.gameObject;
            if (t.gameObject.name == "CardModel_side")
                side2 = t.gameObject;
        }
        front1.GetComponent<Renderer>().material = front2.GetComponent<Renderer>().material;
        back1.GetComponent<Renderer>().material = back2.GetComponent<Renderer>().material;
        side1.GetComponent<Renderer>().material = side2.GetComponent<Renderer>().material;

        Destroy(go2);

        black.transform.localEulerAngles = new Vector3(70f, 0f, 0f);
        black.transform.localScale = new Vector3(60f, 50f, 0f);

        //post
        PlayableDirector director = post.GetComponent<PlayableDirector>();
        director.playOnAwake = true;
        director.playableAsset = summonXYZpostXYZTL;
        Dictionary<string, PlayableBinding> bindingDict = new Dictionary<string, PlayableBinding>();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_CardLocal"].sourceObject, cardLocal);
        director.SetGenericBinding(bindingDict["Activation_Particle"].sourceObject, particle);


        //summonXYZGalaxy01TL
        director = galaxy.GetComponent<PlayableDirector>();
        director.playableAsset = summonXYZGalaxy01TL;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_ Aurora01"].sourceObject, aurora01);
        director.SetGenericBinding(bindingDict["Animation_ Aurora02"].sourceObject, aurora02);
        director.SetGenericBinding(bindingDict["Animation_ Aurora03"].sourceObject, aurora03);
        director.SetGenericBinding(bindingDict["Activation_ Aurora01"].sourceObject, aurora01);
        director.SetGenericBinding(bindingDict["Activation_ Aurora02"].sourceObject, aurora02);
        director.SetGenericBinding(bindingDict["Activation_ Aurora03"].sourceObject, aurora03);
        director.SetGenericBinding(bindingDict["Animation_SpiralAnim01"].sourceObject, spiral01);
        director.SetGenericBinding(bindingDict["Animation_SpiralAnim02"].sourceObject, spiral02);
        director.SetGenericBinding(bindingDict["Animation_SpiralAnim03"].sourceObject, spiral03);
        director.SetGenericBinding(bindingDict["Activation_Particle01"].sourceObject, particle01);
        director.SetGenericBinding(bindingDict["Activation_Particle02"].sourceObject, particle02);
        director.SetGenericBinding(bindingDict["Activation_Particle03"].sourceObject, particle03);

        director.playOnAwake = true;

        //xyzTrailIn03TL;
        bindingDict.Clear();
        director = trail.GetComponent<PlayableDirector>();
        director.playableAsset = xyzTrailIn03TL;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
#if UNITY_ANDROID
        director.SetGenericBinding(bindingDict["Animation_XYZInMesh01_android"].sourceObject, xyzInMesh1);
        director.SetGenericBinding(bindingDict["Animation_XYZInMesh02_android"].sourceObject, xyzInMesh2);
        director.SetGenericBinding(bindingDict["Animation_XYZInMesh03_android"].sourceObject, xyzInMesh3);
#else
        director.SetGenericBinding(bindingDict["Animation_XYZInMesh01"].sourceObject, xyzInMesh1);
        director.SetGenericBinding(bindingDict["Animation_XYZInMesh02"].sourceObject, xyzInMesh2);
        director.SetGenericBinding(bindingDict["Animation_XYZInMesh03"].sourceObject, xyzInMesh3);
#endif
        director.SetGenericBinding(bindingDict["Activation_XYZInMesh01"].sourceObject, xyzInMesh1);
        director.SetGenericBinding(bindingDict["Activation_XYZInMesh02"].sourceObject, xyzInMesh2);
        director.SetGenericBinding(bindingDict["Activation_XYZInMesh03"].sourceObject, xyzInMesh3);
        director.SetGenericBinding(bindingDict["Activation_Flare01"].sourceObject, flare1);
        director.SetGenericBinding(bindingDict["Activation_Flare02"].sourceObject, flare2);
        director.SetGenericBinding(bindingDict["Activation_Flare03"].sourceObject, flare3);
        director.playOnAwake = true;

        //summonXYZexplosion01TL
        bindingDict.Clear();
        director = explosion.GetComponent<PlayableDirector>();
        director.playableAsset = summonXYZexplosion01TL;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Activation_Column"].sourceObject, column);
        director.SetGenericBinding(bindingDict["Activation_Hemisphere"].sourceObject, hemisphere);
        director.SetGenericBinding(bindingDict["Activation_frare"].sourceObject, frare);
        director.SetGenericBinding(bindingDict["Activation_frare01"].sourceObject, frare01);
        director.SetGenericBinding(bindingDict["Activation_CircleFrare"].sourceObject, circleFrare);
        director.playOnAwake = true;



        //summonXYZ03_01TL
        director = go.GetComponent<PlayableDirector>();
        director.playableAsset = summonXYZ03_01TL;
        bindingDict.Clear();
        foreach (PlayableBinding pb in director.playableAsset.outputs)
        {
            if (!bindingDict.ContainsKey(pb.streamName))
                bindingDict.Add(pb.streamName, pb);
        }
        director.SetGenericBinding(bindingDict["Animation_Black"].sourceObject, black);
        director.SetGenericBinding(bindingDict["Activation_Hole"].sourceObject, hole);
        director.playOnAwake = true;


        TimelineAsset timelineAsset = summonXYZ03_01TL as TimelineAsset;
        TrackAsset track = timelineAsset.GetOutputTrack(1);//Explosion

        TimelineClip controlClip = null;
        foreach (var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "Explosion";
            director.SetReferenceValue(new PropertyName("Explosion"), explosion);
        }

        track = timelineAsset.GetOutputTrack(2);//Post
        controlClip = null;
        foreach (var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "postXYZ";
            director.SetReferenceValue(new PropertyName("postXYZ"), post);
        }

        track = timelineAsset.GetOutputTrack(3);//Trail
        controlClip = null;
        foreach (var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "trail";
            director.SetReferenceValue(new PropertyName("trail"), trail);
        }

        track = timelineAsset.GetOutputTrack(4);//Trail
        controlClip = null;
        foreach (var controlclip in track.GetClips())
        {
            controlClip = controlclip;
        }
        if (controlClip != null)
        {
            ControlPlayableAsset clip = controlClip.asset as ControlPlayableAsset;
            clip.sourceGameObject.exposedName = "galaxy";
            director.SetReferenceValue(new PropertyName("galaxy"), galaxy);
        }

        director.playOnAwake = true;

        Vector3 black_ = go.transform.Find("Black").localScale;
        go.transform.Find("Black").localScale = new Vector3(black_.x * 10, black_.y, black_.z);

        return go;
    }

    public Texture2D PendulumNum(int i)
    {
        switch (i)
        {
            case 0:
                return LPendulumNum00;
            case 1:
                return LPendulumNum01;
            case 2:
                return LPendulumNum02;
            case 3:
                return LPendulumNum03;
            case 4:
                return LPendulumNum04;
            case 5:
                return LPendulumNum05;
            case 6:
                return LPendulumNum06;
            case 7:
                return LPendulumNum07;
            case 8:
                return LPendulumNum08;
            case 9:
                return LPendulumNum09;
            case 10:
                return RPendulumNum00;
            case 11:
                return RPendulumNum01;
            case 12:
                return RPendulumNum02;
            case 13:
                return RPendulumNum03;
            case 14:
                return RPendulumNum04;
            case 15:
                return RPendulumNum05;
            case 16:
                return RPendulumNum06;
            case 17:
                return RPendulumNum07;
            case 18:
                return RPendulumNum08;
            case 19:
                return RPendulumNum09;

            default: 
                return LPendulumNum00;
        }
    }

    public bool TypeMatch(int type)
    {
        if((type & (int)CardType.Fusion) >0 && spType == "FUSION")
            return true;
        if ((type & (int)CardType.Synchro) > 0 && spType == "SYNCHRO")
            return true;
        if ((type & (int)CardType.Xyz) > 0 && spType == "XYZ")
            return true;
        if ((type & (int)CardType.Link) > 0 && spType == "LINK")
            return true;
        if ((type & (int)CardType.Ritual) > 0 && spType == "RITUAL")
            return true;
        return false;
    }

}