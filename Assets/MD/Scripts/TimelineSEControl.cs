using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineSEControl : MonoBehaviour
{
    public PlayableAsset seRutual1;
    public PlayableAsset seRutual2;
    public PlayableAsset seRutual3;
    public PlayableAsset seFusion1;
    public PlayableAsset seFusion2;
    public PlayableAsset seFusion3;
    public PlayableAsset seSynchro;
    public PlayableAsset seXyz1;
    public PlayableAsset seXyz2;
    public PlayableAsset seXyz3;
    public PlayableAsset seLink1;
    public PlayableAsset seLink2;
    public PlayableAsset seLink3;
    public PlayableAsset seLink4;
    public PlayableAsset seLink5;
    public PlayableAsset seLink6;


    PlayableDirector director;
    public GameObject summoncard;
    GameObject showunlitcard;
    static int showUnlitCardFrames = 80;
    static int summonFrames1 = 108;
    static int summonFrames2 = 182;
    static int cutinFrames = 96;
    static bool cutin;
    static string type;
    static float time_move;
    static float time_land;
    static int level;
    gameCard card;
    bool cutinPlayed;
    public void Play(GameObject showCard, bool hasCutin, string type, int materialConut, float time_move, gameCard card)
    {
        cutinPlayed = false;
        this.card = card;
        TimelineSEControl.type = type;
        director = GetComponent<PlayableDirector>();
        if (type == "RITUAL")
        {
            showUnlitCardFrames = 0;
            summonFrames1 = 282;
            summonFrames2 = 353;
            if(materialConut == 1)
                director.playableAsset = seRutual1;
            if (materialConut == 2)
                director.playableAsset = seRutual2;
            if (materialConut > 2)
                director.playableAsset = seRutual3;
        }
        else if (type == "FUSION")
        {
            showUnlitCardFrames = 80;
            summonFrames1 = 108;
            summonFrames2 = 182;
            if (materialConut == 1)
                director.playableAsset = seFusion1;
            if (materialConut == 2)
                director.playableAsset = seFusion2;
            if (materialConut > 2)
                director.playableAsset = seFusion3;
        }
        else if (type == "XYZ")
        {
            showUnlitCardFrames = 0;
            summonFrames1 = 220;
            summonFrames2 = 300;
            if (materialConut == 1)
                director.playableAsset = seXyz1;
            if (materialConut == 2)
                director.playableAsset = seXyz2;
            if (materialConut > 2)
                director.playableAsset = seXyz3;
        }
        else if (type == "LINK")
        {
            showUnlitCardFrames = 0;
            summonFrames1 = 217;
            summonFrames2 = 288;          
            if (card.get_data().Level > 1)
            {
                summonFrames1 += 30;
                summonFrames2 += 30;
            }
            if (card.get_data().Level > 2)
            {
                summonFrames1 += 30;
                summonFrames2 += 30;
            }
            switch (card.get_data().Level)
            {
                case 1:
                    director.playableAsset = seLink1;
                    break;
                case 2:
                    director.playableAsset = seLink2;
                    break;
                case 3:
                    director.playableAsset = seLink3;
                    break;
                case 4:
                    director.playableAsset = seLink4;
                    break;
                case 5:
                    director.playableAsset = seLink5;
                    break;
                case 6:
                    director.playableAsset = seLink6;
                    break;
                default:
                    director.playableAsset = seLink6;
                    break;
            }            
        }
        else if (type == "SYNCHRO")
        {
            showUnlitCardFrames = 0;
            summonFrames1 = 245;
            summonFrames2 = 300;
            director.playableAsset = seSynchro;
        }
        director.time = 0;
        director.Play();
        summoncard = card.gameObject;
        showunlitcard = showCard;
        cutin = hasCutin;
        TimelineSEControl.time_move = time_move;
        TimelineSEControl.time_land = 10 / 60f;
        level = card.get_data().Level;
    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0) && director.time < (showUnlitCardFrames + summonFrames1) / 60f && director.time > 0.1f)
        {
            if (Program.I().setting.isShowed)
                return;
            if (Program.I().cardDetail.isShowed)
                return;
            CutinLoader cl = GameObject.Find("Program").GetComponent<CutinLoader>();
            cl.LoadCutin();
            cutinPlayed = true;
            CardAnimation ca = summoncard.GetComponent<CardAnimation>();
            ca.StopAllCoroutines();
            if (cutin)
            {
                ca.PlayDelayedAnimation("card_rise", cutinFrames / 60f);
                ca.PlayDelayedAnimation("card_land", cutinFrames / 60f + time_move);
            }
            else
            {
                ca.PlayDelayedAnimation("card_rise", (summonFrames2 - summonFrames1) / 60f);
                ca.PlayDelayedAnimation("card_land", (summonFrames2 - summonFrames1) / 60f + time_move);
            }
            iTween[] iTweens = summoncard.GetComponents<iTween>();
            iTween.Pause(summoncard);
            foreach (iTween t in iTweens)
            {
                if (cutin)
                    t.delay = cutinFrames / 60f;
                else
                    t.delay = (summonFrames2 - summonFrames1) / 60f;
                t.enabled = false;
                t.enabled = true;
            }

            if (cutin)
                Ocgcore.MessageBeginTime = Program.TimePassed() + (int)((cutinFrames / 60f + time_move + time_land - Ocgcore.landDelay) * 1000);
            else
                Ocgcore.MessageBeginTime = Program.TimePassed() + (int)(((summonFrames2 - summonFrames1) / 60f + time_move + time_land - Ocgcore.landDelay) * 1000);

            director.time = (showUnlitCardFrames + summonFrames1) / 60f;
            BlackBehaviour.FadeOut(0.2f);
            Destroy(showunlitcard);
            SEHandler.Stop();
            GameObject sa = GameObject.Find("main_3d/SummonAnimation").transform.GetChild(0).gameObject;
            sa.SetActive(true);
            sa.GetComponent<PlayableDirector>().time = summonFrames1 / 60f;
            sa.GetComponent<PlayableDirector>().Play();
            Destroy(sa.transform.parent.gameObject, (summonFrames2 - summonFrames1) / 60f);

            //this.enabled = false;

        }
        if (director.time > (showUnlitCardFrames + summonFrames1) / 60f)
        {
            if (!cutinPlayed)
            {
                CutinLoader cl = GameObject.Find("Program").GetComponent<CutinLoader>();
                cl.LoadCutin();
                BlackBehaviour.FadeOut(0.2f);
            }
        }
        if(type == "RITUAL")
            if (director.time > (director.duration - 1f))
            {
                card.gameObject.transform.position = new Vector3(0, 65f, -26.142f);
                card.gameObject.transform.eulerAngles = new Vector3(-20f, 0f, 0f);
                this.enabled = false;
            }
        if (type == "FUSION")
            if (director.time > (director.duration - 1.5f))
            {
                card.gameObject.transform.position = new Vector3(0, 65f, -26.142f);
                card.gameObject.transform.eulerAngles = new Vector3(-20f, 0f, 0f);
                this.enabled = false;
            }
        if (type == "SYNCHRO")
            if (director.time > (director.duration - 1.7f))
            {
                card.gameObject.transform.position = new Vector3(0, 65f, -26.142f);
                card.gameObject.transform.eulerAngles = new Vector3(-20f, 0f, 0f);
                this.enabled = false;
            }
        if (type == "XYZ")
            if (director.time > (director.duration - 1.5f))
            {
                card.gameObject.transform.position = new Vector3(0, 65f, -26.142f);
                card.gameObject.transform.eulerAngles = new Vector3(-20f, 0f, 0f);
                this.enabled = false;
            }
        if (type == "LINK")
            if (director.time > (director.duration - 1.5f))
            {
                card.gameObject.transform.position = new Vector3(0, 65f, -26.142f);
                card.gameObject.transform.eulerAngles = new Vector3(-20f, 0f, 0f);
                this.enabled = false;
            }
    }
}
