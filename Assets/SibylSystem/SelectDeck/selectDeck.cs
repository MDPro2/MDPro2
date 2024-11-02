using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;
using static UnityEngine.Rendering.DebugUI;
using System.Text;

public class selectDeck : WindowServantSP
{
    private readonly cardPicLoader[] quickCards = new cardPicLoader[200];

    private UIDeckPanel deckPanel;


    private string deckSelected = "";

    private string preString = "";
    private UIInput searchInput;

    private UIselectableList superScrollView;
    //private SuperScrollView newSuperScrollView;
    private DiyScrollViewForDeckSelect newSuperScrollView;

    public bool deleting;
    UIButton delBtn;
    public List<string> deckToDel;

    public bool showingPickup;

    public PickupCard pickup;

    public override void initialize()
    {
        SetWindow(Program.I().remaster_deckManager);
        //deckPanel = gameObject.GetComponentInChildren<UIDeckPanel>();
        UIHelper.registEvent(gameObject, "exit_", onClickExit);

        //superScrollView = UIHelper.getByName<UIselectableList>(gameObject, "deck");

        //newSuperScrollView = new SuperScrollView
        newSuperScrollView = new DiyScrollViewForDeckSelect
            (
                UIHelper.getByName<UIPanel>(gameObject, "new_deck_panel"),
                UIHelper.getByName<UIScrollBar>(gameObject, "new_deck_bar"),
                itemOnDeckSelect,
                220,
                7,
                -780,
                0,
                240 + 20,
                220 + 20,
                140
            );

        //superScrollView.selectedAction = onSelected;
        //UIHelper.registEvent(gameObject, "sort_", onSort);
        //setSortLable();
        //UIHelper.registEvent(gameObject, "edit_", onEdit);
        UIHelper.registEvent(gameObject, "new_", onNew);

        delBtn = UIHelper.getByName<UIButton>(gameObject, "dispose_");
        EventDelegate.Add(delBtn.onClick, onDispose);

        //UIHelper.registEvent(gameObject, "copy_", onCopy);
        //UIHelper.registEvent(gameObject, "rename_", onRename);
        //UIHelper.registEvent(gameObject, "code_", onCode);
        searchInput = UIHelper.getByName<UIInput>(gameObject, "search_");
        UIHelper.registEvent(gameObject, "clean_", CleanSearch);

        //superScrollView.install();

        //for (var i = 0; i < quickCards.Length; i++)
        //{
        //    quickCards[i] = deckPanel.createCard();
        //    quickCards[i].relayer(i);
        //}

        //SetActiveFalse();

        deckToDel = new List<string>();
    }

    public override void ES_mouseUpRight()
    {
        base.ES_mouseUpRight();
        onClickExit();
    }

    private void CleanSearch()
    {
        searchInput.text = "";
    }

    private void onSearch()
    {
        printFile();
        //superScrollView.toTop();
    }

    private void onEdit()
    {
        if (!superScrollView.Selected()) return;
        if (!isShowed) return;
        KF_editDeck(superScrollView.selectedString);
    }

    private void returnToSelect()
    {
        Program.I().shiftToServant(Program.I().selectDeck);
        BGMHandler.ChangeBGM("menu");
    }

    public override void preFrameFunction()
    {
        base.preFrameFunction();
        if (searchInput.value != preString)
        {
            preString = searchInput.value;
            onSearch();
        }
    }

    public void KF_editDeck(string deckName)
    {
        //var path = "deck/" + deckName + ".ydk";
        //if (File.Exists(path))
        //{
        //    Config.Set("deckInUse", deckName);
        //    Program.I().deckManager.shiftCondition(DeckManager.Condition.editDeck);
        //    Program.I().shiftToServant(Program.I().deckManager);
        //    Program.I().deckManager.loadDeckFromYDK(path);
        //    Program.I().cardDescription.setTitle(deckName);
        //    Program.I().deckManager.setGoodLooking();
        //    Program.I().deckManager.returnAction =
        //        () =>
        //        {
        //            if (Program.I().deckManager.deckDirty)
        //                RMSshow_yesOrNoOrCancle(
        //                    "deckManager_returnAction"
        //                    , InterString.Get("要保存卡组的变更吗？")
        //                    , new messageSystemValue {hint = "yes", value = "yes"}
        //                    , new messageSystemValue {hint = "no", value = "no"}
        //                    , new messageSystemValue {hint = "cancle", value = "cancle"}
        //                );
        //            else
        //                returnToSelect();
        //        };
        //}
        var path = "deck/" + deckName + ".ydk";
        if (File.Exists(path))
        {
            Config.Set("deckInUse", deckName);
            Program.I().newDeckManager.shiftCondition(NewDeckManager.Condition.editDeck);
            Program.I().shiftToServant(Program.I().newDeckManager);
            Program.I().newDeckManager.loadDeckFromYDK(deckName);
            Program.I().newDeckManager.returnAction =
                () =>
                {
                    if (Program.I().newDeckManager.deckDirty 
                        ||
                        Program.I().newDeckManager.mono.deckName_.value != Program.I().newDeckManager.inDeckName
                        )
                        RMSshow_yesOrNoOrCancle(
                            "newDeckManager_returnAction"
                            , InterString.Get("要保存卡组的变更吗？")
                            , new messageSystemValue { hint = "yes", value = "yes" }
                            , new messageSystemValue { hint = "no", value = "no" }
                            , new messageSystemValue { hint = "cancle", value = "cancle" }
                        );
                    else
                        returnToSelect();
                };
        }
    }

    public override void ES_RMS(string hashCode, List<messageSystemValue> result)
    {
        base.ES_RMS(hashCode, result);
        if (hashCode == "deckManager_returnAction")
        {
            if (result[0].value == "yes")
                if (Program.I().deckManager.onSave())
                    returnToSelect();
            if (result[0].value == "no") returnToSelect();
        }
        if (hashCode == "newDeckManager_returnAction")
        {
            if (result[0].value == "yes")
                if (Program.I().newDeckManager.SaveDeck())
                    returnToSelect();
            if (result[0].value == "no") returnToSelect();
        }
        if (hashCode == "onNew")
            try
            {
                var path = $"deck/{result[0].value}.ydk";
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.Create(path).Close();
                //RMSshow_none(InterString.Get("「[?]」创建完毕。", result[0].value));

                string clipBoard = GUIUtility.systemCopyBuffer;
                if(clipBoard.Contains("#main"))
                    File.WriteAllText("deck/" + result[0].value + ".ydk", clipBoard, Encoding.UTF8);
                KF_editDeck(result[0].value);

                //superScrollView.selectedString = result[0].value;
                //printFile();
            }
            catch (Exception)
            {
                RMSshow_none(InterString.Get("创建卡组失败！请检查输入的文件名，以及文件夹权限。"));
            }

        if (hashCode == "onDispose")
            if (result[0].value == "yes")
            {
                try
                {
                    foreach (var deck in deckToDel)
                    {
                        File.Delete("deck/" + deck + ".ydk");
                        RMSshow_none(InterString.Get("「[?]」删除完毕。", deck));
                    }
                    printFile();
                    ResetDelButton();
                }
                catch (Exception)
                {
                    RMSshow_none(InterString.Get("删除卡组失败！请检查文件夹权限。"));
                }
            }
            else
            {
                ResetDelButton();
            }


        if (hashCode == "onCopy")
            try
            {
                var path = $"deck/{result[0].value}.ydk";
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.Copy("deck/" + superScrollView.selectedString + ".ydk", path);
                RMSshow_none(InterString.Get("「[?]」复制完毕。", superScrollView.selectedString));
                superScrollView.selectedString = result[0].value;
                printFile();
            }
            catch (Exception)
            {
                RMSshow_none(InterString.Get("复制卡组失败！请检查输入的文件名，以及文件夹权限。"));
            }

        if (hashCode == "onRename")
            try
            {
                var path = $"deck/{result[0].value}.ydk";
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                File.Move("deck/" + superScrollView.selectedString + ".ydk", path);
                RMSshow_none(InterString.Get("「[?]」重命名完毕。", superScrollView.selectedString));
                superScrollView.selectedString = result[0].value;
                printFile();
            }
            catch (Exception)
            {
                RMSshow_none(InterString.Get("重命名卡组失败！请检查输入的文件名，以及文件夹权限。"));
            }
    }

    private void onNew()
    {
        RMSshow_input("onNew", InterString.Get("请输入要创建的卡组名"), UIHelper.getTimeString());
    }

    private void onDispose()
    {
        //if (!superScrollView.Selected()) return;
        //var path = "deck/" + superScrollView.selectedString + ".ydk";
        //if (File.Exists(path))
        //    RMSshow_yesOrNo(
        //        "onDispose"
        //        , InterString.Get("确认删除「[?]」吗？", superScrollView.selectedString)
        //        , new messageSystemValue {hint = "yes", value = "yes"}
        //        , new messageSystemValue {hint = "no", value = "no"}
        //    );
        if(deleting == false)
        {
            deleting = true;
            delBtn.normalSprite2D = Program.I().buttonFrameRed;
            delBtn.hoverSprite2D = Program.I().buttonFrameRedHover;
            UIHelper.getByName<UILabel>(delBtn.gameObject, "label_").color = Color.red;
            delBtn.gameObject.SetActive(false);
            delBtn.gameObject.SetActive(true);
        }
        else
        {
            if(deckToDel.Count > 0)
            {
                if(deckToDel.Count == 1)
                {
                    var path = "deck/" + deckToDel[0] + ".ydk";
                    if (File.Exists(path))
                        RMSshow_yesOrNo(
                            "onDispose"
                            , InterString.Get("确认删除「[?]」吗？", deckToDel[0])
                            , new messageSystemValue { hint = "yes", value = "yes" }
                            , new messageSystemValue { hint = "no", value = "no" }
                        );
                }
                else
                {
                    RMSshow_yesOrNo(
                        "onDispose"
                        , InterString.Get("确认删除这些卡组吗？")
                        , new messageSystemValue { hint = "yes", value = "yes" }
                        , new messageSystemValue { hint = "no", value = "no" }
                    );
                }
            }
            else
                ResetDelButton();
        }
    }

    private void ResetDelButton()
    {
        deleting = false;
        delBtn.normalSprite2D = Program.I().buttonFrameGreen;
        delBtn.hoverSprite2D = Program.I().buttonFrameGreenHover;
        UIHelper.getByName<UILabel>(delBtn.gameObject, "label_").color = new Color(0.78f, 1, 0, 1);
        foreach(var check in gameObject.transform.GetComponentsInChildren<UI2DSprite>())
        {
            if(check.name == "mark_")
                check.gameObject.SetActive(false);
        }
        deckToDel.Clear();
        delBtn.gameObject.SetActive(false);
        delBtn.gameObject.SetActive(true);
    }


    private void onCopy()
    {
        if (!superScrollView.Selected()) return;
        var path = "deck/" + superScrollView.selectedString + ".ydk";
        if (File.Exists(path))
        {
            var newname = InterString.Get("[?]的副本", superScrollView.selectedString);
            var newnamer = newname;
            var i = 1;
            while (File.Exists("deck/" + newnamer + ".ydk"))
            {
                newnamer = newname + i;
                i++;
            }

            RMSshow_input("onCopy", InterString.Get("请输入复制后的卡组名"), newnamer);
        }
    }

    private void onRename()
    {
        if (!superScrollView.Selected()) return;
        var path = "deck/" + superScrollView.selectedString + ".ydk";
        if (File.Exists(path)) RMSshow_input("onRename", InterString.Get("新的卡组名"), superScrollView.selectedString);
    }

    private void onCode()
    {
        if (!superScrollView.Selected()) return;
        var path = "deck/" + superScrollView.selectedString + ".ydk";
        if (File.Exists(path))
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN //编译器、Windows
            Process.Start("notepad.exe", path);
#elif UNITY_STANDALONE_OSX //Mac OS X
                System.Diagnostics.Process.Start("open", "-e " + path);
#elif UNITY_STANDALONE_LINUX //Linux
                System.Diagnostics.Process.Start("gedit", path);
#endif
        }
    }

    private void setSortLable()
    {
        if (Config.Get(UIHelper.sort, "1") == "1")
            UIHelper.trySetLableText(gameObject, "sort_", InterString.Get("时间排序"));
        else
            UIHelper.trySetLableText(gameObject, "sort_", InterString.Get("名称排序"));
    }

    private void onSort()
    {
        if (Config.Get(UIHelper.sort, "1") == "1")
            Config.Set(UIHelper.sort, "0");
        else
            Config.Set(UIHelper.sort, "1");
        setSortLable();
        printFile();
    }

    private void onSelected()
    {
        if (deckSelected == superScrollView.selectedString) onEdit();
        deckSelected = superScrollView.selectedString;
        printSelected();
    }

    private void printSelected()
    {
        // GameTextureManager.clearUnloaded();
        Deck deck;
        DeckManager.FromYDKtoCodedDeck("deck/" + deckSelected + ".ydk", out deck);
        var mainAll = 0;
        var mainMonster = 0;
        var mainSpell = 0;
        var mainTrap = 0;
        var sideAll = 0;
        var sideMonster = 0;
        var sideSpell = 0;
        var sideTrap = 0;
        var extraAll = 0;
        var extraFusion = 0;
        var extraLink = 0;
        var extraSync = 0;
        var extraXyz = 0;
        var currentIndex = 0;

        var hangshu = UIHelper.get_decklieshuArray(deck.Main.Count);
        foreach (var item in deck.Main)
        {
            mainAll++;
            var c = CardsManager.Get(item);
            if ((c.Type & (uint) CardType.Monster) > 0) mainMonster++;
            if ((c.Type & (uint) CardType.Spell) > 0) mainSpell++;
            if ((c.Type & (uint) CardType.Trap) > 0) mainTrap++;
            quickCards[currentIndex].code = item;
            var v = UIHelper.get_hang_lieArry(mainAll - 1, hangshu);
            quickCards[currentIndex].transform.localPosition = new Vector3
            (
                -176.3f + UIHelper.get_left_right_indexZuo(0, 352f, (int) v.y, hangshu[(int) v.x], 10)
                ,
                161.6f - v.x * 60f
                ,
                0
            );
            if (currentIndex <= 198) currentIndex++;
        }

        foreach (var item in deck.Side)
        {
            sideAll++;
            var c = CardsManager.Get(item);
            if ((c.Type & (uint) CardType.Monster) > 0) sideMonster++;
            if ((c.Type & (uint) CardType.Spell) > 0) sideSpell++;
            if ((c.Type & (uint) CardType.Trap) > 0) sideTrap++;
            quickCards[currentIndex].code = item;
            quickCards[currentIndex].transform.localPosition = new Vector3
            (
                -176.3f + UIHelper.get_left_right_indexZuo(0, 352f, sideAll - 1, deck.Side.Count, 10)
                ,
                -181.1f
                ,
                0
            );
            if (currentIndex <= 198) currentIndex++;
        }

        foreach (var item in deck.Extra)
        {
            extraAll++;
            var c = CardsManager.Get(item);
            if ((c.Type & (uint) CardType.Fusion) > 0) extraFusion++;
            if ((c.Type & (uint) CardType.Synchro) > 0) extraSync++;
            if ((c.Type & (uint) CardType.Xyz) > 0) extraXyz++;
            if ((c.Type & (uint) CardType.Link) > 0) extraLink++;
            quickCards[currentIndex].code = item;
            quickCards[currentIndex].transform.localPosition = new Vector3
            (
                -176.3f + UIHelper.get_left_right_indexZuo(0, 352f, extraAll - 1, deck.Extra.Count, 10)
                ,
                -99.199f
                ,
                0
            );
            if (currentIndex <= 198) currentIndex++;
        }

        while (true)
        {
            quickCards[currentIndex].clear();
            if (currentIndex <= 198)
                currentIndex++;
            else
                break;
        }

        deckPanel.leftMain.text = GameStringHelper._zhukazu + mainAll;
        deckPanel.leftExtra.text = GameStringHelper._ewaikazu + extraAll;
        deckPanel.leftSide.text = GameStringHelper._fukazu + sideAll;
        deckPanel.rightMain.text = GameStringHelper._guaishou + mainMonster + " " + GameStringHelper._mofa + mainSpell +
                                   " " + GameStringHelper._xianjing + mainTrap;
        deckPanel.rightExtra.text = GameStringHelper._ronghe + extraFusion + " " + GameStringHelper._tongtiao +
                                    extraSync + " " + GameStringHelper._chaoliang + extraXyz + " " +
                                    GameStringHelper._lianjie + extraLink;
        deckPanel.rightSide.text = GameStringHelper._guaishou + sideMonster + " " + GameStringHelper._mofa + sideSpell +
                                   " " + GameStringHelper._xianjing + sideTrap;
    }

    public override void show()
    {
        base.show();
        printFile();

        ResetDelButton();
        if(pickup != null)
            pickup.Show();

        //superScrollView.toTop();
        //superScrollView.selectedString = Config.Get("deckInUse", "miaowu");
        //printSelected();
        //Program.charge();
    }

    public override void hide()
    {
        //if (isShowed)
        //    if (superScrollView.Selected())
        //        Config.Set("deckInUse", superScrollView.selectedString);
        base.hide();
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_04");
    }

    private void printFile()
    {
        if (newSuperScrollView != null)
        {
            var args = new List<string[]>();
            List<string> decks = UIHelper.GetDecks(searchInput.value).ToList();
            for (int i = 0; i < decks.Count; i++)
            {
                string[] arg = new string[7];
                arg[0] = i.ToString();
                arg[1] = decks[i];

                Deck deck2 = new Deck("deck/" + decks[i] + ".ydk");

                if (deck2.Pickup.Count > 2)
                    arg[4] = deck2.Pickup[2].ToString();
                else
                    arg[4] = "0";
                if (deck2.Pickup.Count > 1)
                    arg[3] = deck2.Pickup[1].ToString();
                else
                    arg[3] = "0";
                if (deck2.Pickup.Count > 0)
                    arg[2] = deck2.Pickup[0].ToString();
                else
                    arg[2] = "0";

                if(deck2.Case.Count > 0)
                    arg[5] = deck2.Case[0].ToString();
                else
                    arg[5] = "1";

                if (deck2.Protector.Count > 0)
                    arg[6] = deck2.Protector[0].ToString();
                else
                    arg[6] = "1070001";
                args.Add(arg);
            }
            newSuperScrollView.Print(args);
        }
    }

    public static string AddZero(string code)
    {
        string returnValue = "";
        if (code.Length == 1)
            returnValue = "000" + code;
        else if (code.Length == 2)
            returnValue = "00" + code;
        else if (code.Length == 3)
            returnValue = "0" + code;
        else if (code.Length > 4)
            returnValue = code.Substring(1, 4) + "_" + code.Substring(0, 1);
        else
            returnValue = code;
        return returnValue;
    }

    private GameObject itemOnDeckSelect(string[] args)
    {
        var returnValue = create(Program.I().new_ui_deckOnSearchList, Vector3.zero, Vector3.zero, false, Program.I().ui_back_ground_2d);
        var deckOnSelect = returnValue.GetComponent<DeckOnSelect>();
        deckOnSelect.id = int.Parse(args[0]);
        deckOnSelect.deckName = args[1];
        deckOnSelect.code_1 = int.Parse(args[2]);
        deckOnSelect.code_2 = int.Parse(args[3]);
        deckOnSelect.code_3 = int.Parse(args[4]);
        deckOnSelect.case_ = args[5];
        deckOnSelect.protector = args[6];

        return returnValue;
    }

    CancellationTokenSource source = new CancellationTokenSource();

    private async void LoadPickupTexture(GameObject go, int card1, int card2, int card3, Texture2D protector, int id)
    {
        await Task.Delay(10 * (id * 3));
        UIHelper.getByName<UITexture>(go, "card_1").mainTexture = await GameTextureManager.GetCardPictureWithProtector(card1, protector);
        await Task.Delay(10 * (id * 3 + 1));
        UIHelper.getByName<UITexture>(go, "card_2").mainTexture = await GameTextureManager.GetCardPictureWithProtector(card2, protector);
        await Task.Delay(10 * (id * 3 + 2));
        UIHelper.getByName<UITexture>(go, "card_3").mainTexture = await GameTextureManager.GetCardPictureWithProtector(card3, protector);
    }

    public void onClickExit()
    {
        if (Program.exitOnReturn)
            Program.I().menu.onClickExit();
        else
        {
            Program.I().shiftToServant(Program.I().menu);
        }
    }
}