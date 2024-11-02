using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;

public class CheckAnimation : MonoBehaviour
{
    public bool isShowed;
    public bool isPlaying;
    UIWidget mainWindow;
    private UIselectableList superScrollView;
    CutinLoader cl;
    string selectedString;
    public static List<int> cutins = new List<int>();
    void Start()
    {
        mainWindow = gameObject.transform.GetChild(0).GetChild(0).GetComponent<UIWidget>();
        UIHelper.registEvent(gameObject, "exit_", hide);
        UIHelper.registEvent(gameObject, "play_", PlayAnimations);
        UIHelper.registEvent(gameObject, "play_type", PlayAnimationsByType);
        superScrollView = gameObject.GetComponentInChildren<UIselectableList>();
        superScrollView.selectedAction = onSelected;
        superScrollView.install();
        cl = GameObject.Find("Program").GetComponent<CutinLoader>();

        printFile();
        ListForFilter();
        gameObject.SetActive(false);
    }

    DirectoryInfo[] spineInfos = null;

    void ListForFilter()
    {
        foreach (DirectoryInfo dirInfo in spineInfos)
            cutins.Add(int.Parse(dirInfo.Name.Replace("u", "")));
    }
    void printFile()
    {
        var args = new List<string[]>();
        spineInfos = new DirectoryInfo(Boot.root + "assetbundle/spine").GetDirectories();
        for (var i = 0; i < spineInfos.Length; i++)
            superScrollView.add(spineInfos[i].Name + "-" +CardsManager.GetCard(int.Parse(spineInfos[i].Name.Replace("u", ""))).Name);
    }

    void PlayAnimationsByType()
    {
        StartCoroutine(PlayType());
    }

    IEnumerator PlayType()
    {
        isPlaying = true;
        mainWindow.alpha = 0f;
        for (int i = 0; i < spineInfos.Length; i++)
        {
            Card card = CardsManager.Get(int.Parse(spineInfos[i].Name.Replace("u", "")));
            if ((card.Type & (uint)CardType.Normal) > 0 && (card.Type & (uint)CardType.Pendulum) == 0)
            {
                PlayCutin(spineInfos[i].Name);
                yield return new WaitForSeconds(1.6f);
            }
        }
        for (int i = 0; i < spineInfos.Length; i++)
        {
            Card card = CardsManager.Get(int.Parse(spineInfos[i].Name.Replace("u", "")));
            if ((card.Type & (uint)CardType.Effect) > 0 
                && 
                (card.Type & (uint)CardType.Ritual) == 0
                &&
                (card.Type & (uint)CardType.Fusion) == 0
                &&
                (card.Type & (uint)CardType.Synchro) == 0
                &&
                (card.Type & (uint)CardType.Xyz) == 0
                &&
                (card.Type & (uint)CardType.Pendulum) == 0
                &&
                (card.Type & (uint)CardType.Link) == 0
                )
            {
                PlayCutin(spineInfos[i].Name);
                yield return new WaitForSeconds(1.6f);
            }
        }
        for (int i = 0; i < spineInfos.Length; i++)
        {
            Card card = CardsManager.Get(int.Parse(spineInfos[i].Name.Replace("u", "")));
            if ((card.Type & (uint)CardType.Ritual) > 0 && (card.Type & (uint)CardType.Pendulum) == 0)
            {
                PlayCutin(spineInfos[i].Name);
                yield return new WaitForSeconds(1.6f);
            }
        }
        for (int i = 0; i < spineInfos.Length; i++)
        {
            Card card = CardsManager.Get(int.Parse(spineInfos[i].Name.Replace("u", "")));
            if ((card.Type & (uint)CardType.Fusion) > 0 && (card.Type & (uint)CardType.Pendulum) == 0)
            {
                PlayCutin(spineInfos[i].Name);
                yield return new WaitForSeconds(1.6f);
            }
        }
        for (int i = 0; i < spineInfos.Length; i++)
        {
            Card card = CardsManager.Get(int.Parse(spineInfos[i].Name.Replace("u", "")));
            if ((card.Type & (uint)CardType.Synchro) > 0 && (card.Type & (uint)CardType.Pendulum) == 0)
            {
                PlayCutin(spineInfos[i].Name);
                yield return new WaitForSeconds(1.6f);
            }
        }
        for (int i = 0; i < spineInfos.Length; i++)
        {
            Card card = CardsManager.Get(int.Parse(spineInfos[i].Name.Replace("u", "")));
            if ((card.Type & (uint)CardType.Xyz) > 0 && (card.Type & (uint)CardType.Pendulum) == 0)
            {
                PlayCutin(spineInfos[i].Name);
                yield return new WaitForSeconds(1.6f);
            }
        }
        for (int i = 0; i < spineInfos.Length; i++)
        {
            Card card = CardsManager.Get(int.Parse(spineInfos[i].Name.Replace("u", "")));
            if ((card.Type & (uint)CardType.Pendulum) > 0)
            {
                PlayCutin(spineInfos[i].Name);
                yield return new WaitForSeconds(1.6f);
            }
        }
        for (int i = 0; i < spineInfos.Length; i++)
        {
            Card card = CardsManager.Get(int.Parse(spineInfos[i].Name.Replace("u", "")));
            if ((card.Type & (uint)CardType.Link) > 0 && (card.Type & (uint)CardType.Pendulum) == 0)
            {
                PlayCutin(spineInfos[i].Name);
                yield return new WaitForSeconds(1.6f);
            }
        }
        StopPlaying();
    }

    void PlayAnimations()
    {
        if (!superScrollView.Selected())
        {
            Program.PrintToChat("请先选中一个动画。");
            return;
        }
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        isPlaying = true;
        mainWindow.alpha = 0f;

        int selected = superScrollView.selectedIndex;
        for (int i =selected; i < spineInfos.Length; i++)
        {
            PlayCutin(spineInfos[i].Name);
            yield return new WaitForSeconds(1.6f);
        }
        StopPlaying();
    }

    public void StopPlaying()
    {
        StopAllCoroutines();
        mainWindow.alpha = 1f;
        isPlaying = false;
    }

    void onSelected()
    {
        if (selectedString == superScrollView.selectedString)
        {
            PlayCutin(superScrollView.selectedString.Split("-")[0]);
        }
        selectedString = superScrollView.selectedString;
    }

    void PlayCutin(string cutinName)
    {
        int id = int.Parse(cutinName.Replace("u", ""));
        CutinLoader.id = id;
        Card card = CardsManager.GetCard(id);
        CutinLoader.level = card.Level;
        CutinLoader.attribute = card.Attribute;
        CutinLoader.type = card.Type;
        CutinLoader.cardName = card.Name;
        CutinLoader.atk = card.Attack;
        CutinLoader.def = card.Defense;
        CutinLoader.controller = 0;

        cl.LoadCutin();
    }

    public void hide()
    {
        isShowed = false;
        Program.I().menu.show();
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_04");
        BGMHandler.PlayBGM("menu");
        gameObject.SetActive(false);
    }

    public void show()
    {
        isShowed = true;
        isPlaying = false;
        mainWindow.alpha = 1.0f;
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_03");
        BGMHandler.PlayBGM("climax");
    }
}
