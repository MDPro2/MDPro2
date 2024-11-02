using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;

public class VoiceHandler : MonoBehaviour
{
    public static AudioSource audioSource;
    public static AudioSource audioSource2;
    public static float voiceTime;
    public static bool occupy;
    public static string character1 = "V0001";
    public static string character2 = "V0001";

    static AssetBundle ab1;
    static AssetBundle ab2;

    public static VoiceHandler instance;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource2 = GameObject.Find("Voice2").GetComponent<AudioSource>();
        instance = this;
        ABInit();
    }

    public static float voice1StopTime;
    public static float voice2StopTime;
    private void LateUpdate()
    {
        if (audioSource.isPlaying)
            voice1StopTime = 0;
        else
            voice1StopTime += Time.deltaTime;
        if (audioSource2.isPlaying)
            voice2StopTime = 0;
        else
            voice2StopTime += Time.deltaTime;
        if(voice1StopTime > 0.3f && audioSource.isPlaying == false)
        {
            Program.I().duelCommentMe.Hide();
            if (Program.I().setting.setting.Voice.value)
                Program.I().ocgcore.gameInfo.SetCharacter0Face();
        }
        if(voice2StopTime > 0.3f && audioSource2.isPlaying == false)
        {
            Program.I().duelCommentOp.Hide();
            if (Program.I().setting.setting.Voice.value)
                Program.I().ocgcore.gameInfo.SetCharacter1Face();
        }
    }
    public static VoiceHandler I()
    {
        return instance;
    }

    public void ABInit()
    {
        StartCoroutine(VoiceHandler.ABInitAsync());
    }

    static bool loading;
    public static IEnumerator ABInitAsync()
    {
        if (loading)
            yield break;
        loading = true;
        character1 = NameMap(Config.Get("Character0", "暗游戏"));
        character2 = NameMap(Config.Get("Character1", "暗游戏"));
        if(ab1 != null)
            ab1.Unload(false);
        if (ab2 != null)
            ab2.Unload(false);
        AssetBundleCreateRequest _ab1= AssetBundle.LoadFromFileAsync(Boot.root + "assetbundle/voice/" + character1.ToLower());
        yield return _ab1;
        ab1 = _ab1.assetBundle;

        if (character2 == character1)
            ab2 = ab1;
        else
        {
            AssetBundleCreateRequest _ab2 = AssetBundle.LoadFromFileAsync(Boot.root + "assetbundle/voice/" + character2.ToLower());
            yield return _ab2;
            ab2 = _ab2.assetBundle;
        }
        loading = false;
    }

    public static void PlayVoice(AudioClip clip, bool me = true, string content = "")
    {
        if(me)
        {
            audioSource.clip = clip;
            audioSource.Play();
            Program.I().duelCommentMe.Speak(content);
            Program.I().ocgcore.gameInfo.SetCharacter0Face(3);
        }
        else
        {
            audioSource2.clip = clip;
            audioSource2.Play();
            Program.I().duelCommentOp.Speak(content);
            Program.I().ocgcore.gameInfo.SetCharacter1Face(3);
        }
    }

    public static bool PlayCardLine(uint controller, string type, gameCard card, bool sleep, bool firstOnField = true, uint location = (uint)CardLocation.Onfield, int chain = 1, bool summonline = false)
    {
        if (!Program.I().setting.setting.Voice.value)
        {
            voiceTime = 0;
            return false;
        }
        string character = controller == 0 ? NameMap(Config.Get("Character0", "暗游戏")) : NameMap(Config.Get("Character1", "暗游戏"));
        int voiceId = VoiceMap.Map(character, type, card, firstOnField, location, chain);
        if (voiceId <= -1)
            return false;

        bool onlyPlayOne = false;
        if(voiceId >= 1000)
        {
            onlyPlayOne = true;
            voiceId -= 1000;
        }
        bool hand = false;
        if (voiceId >= 100)
        {
            hand = true;
            voiceId -= 100;
        }
        string typeId = "";
        switch (type)
        {
            case "summon":
                typeId = "13";
                break;
            case "attack":
                typeId = "19";
                break;
            case "activate":
                typeId = "08";
                break;
            case "activateM":
                typeId = "09";
                break;
        }
        string clipname;
        if (voiceId > 9)
            clipname = string.Format("{0}_{1}_00_{2}_0", character, typeId, voiceId);
        else
            clipname = string.Format("{0}_{1}_00_0{2}_0", character, typeId, voiceId);

        AudioClip clip = GetClipFromAB( controller, clipname);

        if (clip != null)
        {
            if (type == "summon")
            {
                if(summonline)
                    PlayGroup(controller, clip, sleep, onlyPlayOne, hand, true, card);
                else
                    PlayGroup(controller, clip, sleep, onlyPlayOne, hand, true);
            }
            else
                PlayGroup(controller, clip, sleep, onlyPlayOne, hand, false);
            return true;
        }
        else
            return false;
    }

    static void PlayGroup(uint controller, AudioClip clip0, bool sleep, bool onlyPlayOne = false, bool hand = false,bool summon = false, gameCard card = null, bool showText = true)
    {
        AudioClip clip = clip0;
        List<AudioClip> audioClips = new List<AudioClip>();
        if (hand)
        {
            var clipFromHand = GetClipFromAB(controller, clip0.name.Substring(0,6) + "07_00_00_0");
            audioClips.Add(clipFromHand);
        }
        if(card != null && card.get_data().Id != 0)
        {
            int voiceId = VoiceMap.Map(clip0.name.Substring(0,5), "summonline", card, true, (uint)CardLocation.Onfield, 1);
            if( voiceId >= 0)
            {
                string sl = "";
                if (voiceId > 9)
                    sl = clip0.name.Substring(0, 5) + "_37_00_" + voiceId + "_0";
                if (voiceId <= 9)
                    sl = clip0.name.Substring(0, 5) + "_37_00_0" + voiceId + "_0";
                AudioClip clipSL = GetClipFromAB(controller, sl);
                int k = 0;
                while (clipSL != null)
                {
                    audioClips.Add(clipSL);
                    k++;
                    clipSL = GetClipFromAB(controller, clipSL.name.Substring(0, 15) + k);
                }
            }
        }
        audioClips.Add(clip);
        if (!onlyPlayOne)
        {
            int i = 0;
            while (clip != null)
            {
                i++;
                clip = GetClipFromAB(controller, clip0.name.Substring(0, 15) + i);
                if (clip != null)
                    audioClips.Add(clip);
            }
        }

        //List<float> datas = new List<float>();
        //for (int j = 0; j < audioClips.Count; j++)
        //{
        //    float[] data = new float[audioClips[j].samples * audioClips[j].channels];
        //    audioClips[j].GetData(data, 0);
        //    datas.AddRange(data);
        //    audioClips[j].UnloadAudioData();
        //}
        //float[] clipdatas = datas.ToArray();
        //AudioClip result = AudioClip.Create("Combine", clipdatas.Length, 1, 44100, false);
        //result.SetData(clipdatas, 0);

        //if (controller == 0)
        //{
        //    audioSource.clip = result;
        //    audioSource.Play();
        //}
        //else
        //{
        //    audioSource2.clip = result;
        //    audioSource2.Play();
        //}
        float fullTime = 0;
        foreach (var _clip in audioClips)
            fullTime += _clip.length;

        if (sleep)
            Program.I().ocgcore.Sleep((int)(fullTime * 60));
        voiceTime = fullTime;

        float waitTime = 0;
        foreach (AudioClip c in audioClips)
        {
            VoiceLine voiceLine = VoiceLineHandler.GetVoiceLine(controller, c.name);
            if (controller == 0)
            {
                Program.I().duelCommentMe.DelaySpeak(waitTime, voiceLine == null ? "" : voiceLine.text, c, showText, c == audioClips[audioClips.Count - 1]);
                int face = voiceLine == null ? 0 : voiceLine.face;
                if (face == 0) face = 3;
                Program.I().ocgcore.gameInfo.SetCharacter0Face(face);
            }
            else
            {
                Program.I().duelCommentOp.DelaySpeak(waitTime, voiceLine == null ? "" : voiceLine.text, c, showText, c == audioClips[audioClips.Count - 1]);
                int face = voiceLine == null ? 0 : voiceLine.face;
                if (face == 0) face = 3;
                Program.I().ocgcore.gameInfo.SetCharacter1Face(face);
            }
            waitTime += c.length;
        }
    }

    public static void PlayFixedLine(uint controller ,string line, bool sleep = true, string cardName = "", bool showText = true)
    {
        if (!Program.I().setting.setting.Voice.value)
        {
            voiceTime = 0;
            return;
        }
        if (IsSummon(line) & occupy)
            return;
        AudioClip clip;
        List<AudioClip> clips = new List<AudioClip>();
        if (controller == 0)
        {
            clip = ab1.LoadAsset<AudioClip>(NameMap(Config.Get("Character0", "暗游戏")) + LineMap(line));
            int i = 0;
            while(clip != null)
            {
                clips.Add(clip);
                i++;
                clip = ab1.LoadAsset<AudioClip>((NameMap(Config.Get("Character0", "暗游戏")) + LineMap(line)).Substring(0, 13) + i + "_0");
            }
            if(clips.Count > 0)
            {
                if(line == "MyTurn" || line == "TurnEnd")
                {
                    int num = clips.Count / 3;

                    if (Program.I().ocgcore.life_0 > Program.I().ocgcore.life_1 
                        && 
                        Program.I().ocgcore.life_0 > Program.I().ocgcore.lpLimit / 2 
                        && 
                        Program.I().ocgcore.life_1 < Program.I().ocgcore.lpLimit / 2)
                    {
                        clip = clips[UnityEngine.Random.Range(0, num)];
                    }
                    else if(Program.I().ocgcore.life_0 < Program.I().ocgcore.life_1 
                        && 
                        Program.I().ocgcore.life_0 < Program.I().ocgcore.lpLimit / 2
                        &&
                        Program.I().ocgcore.life_1 > Program.I().ocgcore.lpLimit / 2
                        )
                    {
                        clip = clips[UnityEngine.Random.Range(num * 2, clips.Count)];
                    }
                    else 
                    {
                        clip = clips[UnityEngine.Random.Range(num, num * 2)];
                    }
                }
                else
                {
                    clip = clips[UnityEngine.Random.Range(0, clips.Count)];
                }
            }
        }
        else
        {
            clip = ab2.LoadAsset<AudioClip>(NameMap(Config.Get("Character1", "暗游戏")) + LineMap(line));
            int i = 0;
            while (clip != null)
            {
                clips.Add(clip);
                i++;
                clip = ab2.LoadAsset<AudioClip>((NameMap(Config.Get("Character1", "暗游戏")) + LineMap(line)).Substring(0, 13) + i + "_0");
            }
            if (clips.Count > 0)
            {
                if (line == "MyTurn" || line == "TurnEnd")
                {
                    int num = clips.Count / 3;

                    if (Program.I().ocgcore.life_0 < Program.I().ocgcore.life_1
                        &&
                        Program.I().ocgcore.life_0 < Program.I().ocgcore.lpLimit / 2
                        &&
                        Program.I().ocgcore.life_1 > Program.I().ocgcore.lpLimit / 2)
                    {
                        clip = clips[UnityEngine.Random.Range(0, num)];
                    }
                    else if (Program.I().ocgcore.life_0 > Program.I().ocgcore.life_1
                        &&
                        Program.I().ocgcore.life_0 > Program.I().ocgcore.lpLimit / 2
                        &&
                        Program.I().ocgcore.life_1 < Program.I().ocgcore.lpLimit / 2
                        )
                    {
                        clip = clips[UnityEngine.Random.Range(num * 2, clips.Count)];
                    }
                    else
                    {
                        clip = clips[UnityEngine.Random.Range(num, num * 2)];
                    }
                }
                else
                {
                    clip = clips[UnityEngine.Random.Range(0, clips.Count)];
                }
            }
        }
        if (clip == null && line == "LinkSummon")
            PlayFixedLine(controller, "SpecialSummon", sleep, cardName, false);
        if (clip == null && line == "SetPendulumScale")
            PlayFixedLine(controller, "ActivateMagic", sleep, cardName, false);
        if (clip == null && line == "ActivatePendulum")
            PlayFixedLine(controller, "Activate", sleep, cardName, false);
        if (clip != null)
            PlayGroup(controller, clip, sleep, false, false, false, null, false);

        if(showText)
        {
            bool query = true;
            string text = "";
            VoiceLine voiceLine = null;
            if (clip != null)
            {
                voiceLine = VoiceLineHandler.GetVoiceLine(controller, clip.name);
                if (clip.name.Substring(6, 2) == "07")
                {
                    if (clip.name.EndsWith("_00_00_0") == false)
                        query = false;
                }
                else if (clip.name.Substring(6, 2) == "11")
                    query = false;
                else if (clip.name.Substring(6, 2) == "17")
                    query = false;
                else if (clip.name.Substring(6, 2) == "18")
                    query = false;
                if (query && voiceLine != null)
                    text = voiceLine.text;
            }

            if (controller == 0)
            {
                if (text != "")
                    Program.I().duelCommentMe.Speak(text);
                else
                    Program.I().duelCommentMe.Speak(Line2CommentMap(line, cardName));
                int face = 0;
                if (clip != null && voiceLine != null)
                    face = voiceLine.face;
                if (face == 0) face = 3;
                Program.I().ocgcore.gameInfo.SetCharacter0Face(face);
                voice1StopTime = 0;
            }
            else
            {
                if (text != "")
                    Program.I().duelCommentOp.Speak(text);
                else
                    Program.I().duelCommentOp.Speak(Line2CommentMap(line, cardName));
                int face = 0;
                if(clip != null && voiceLine != null)
                    face = voiceLine.face;
                if (face == 0) face = 3;
                Program.I().ocgcore.gameInfo.SetCharacter1Face(face);
                voice2StopTime = 0;
            }
        }
    }

    public static string NameMap(string name)
    {
        switch (name)
        {
            case "暗游戏":
                return "V0001";
            case "海马濑人":
                return "V0002";
            case "城之内克也":
                return "V0003";
            case "孔雀舞":
                return "V0004";
            case "真崎杏子":
                return "V0005";
            case "武藤游戏":
                return "V0006";
            case "昆虫羽蛾":
                return "V0007";
            case "恐龙龙崎":
                return "V0008";
            case "梶木渔太":
                return "V0009";
            case "暗马利克":
                return "V0010";
            case "暗貘良":
                return "V0011";
            case "盗贼基斯":
                return "V0012";
            case "伊西丝·伊修达尔":
                return "V0013";
            case "利希德":
                return "V0014";
            case "贝卡斯":
                return "V0015";
            case "武藤双六":
                return "V0016";
            case "海马圭平":
                return "V0017";
            case "迷宫兄弟":
                return "V0019";
            case "迷宫兄弟·兄":
                return "V0020";
            case "迷宫兄弟·弟":
                return "V0021";
            case "潘多拉":
                return "V0022";
            case "鬼骨冢":
                return "V0023";
            case "超能絽场":
                return "V0024";
            case "本田广人":
                return "V0029";
            case "被操纵的城之内":
                return "V0030";
            case "光暗假面":
                return "V0031";
            case "光之假面":
                return "V0032";
            case "暗之假面":
                return "V0033";
            case "御伽龙儿":
                return "V0034";

            case "游城十代":
                return "V0101";
            case "凯撒亮":
                return "V0102";
            case "爱得·菲尼克斯":
                return "V0103";
            case "约翰·安德森":
                return "V0104";
            case "万丈目准":
                return "V0105";
            case "天上院明日香":
                return "V0106";
            case "库洛诺斯·德·梅迪奇":
                return "V0107";
            case "尤贝尔":
                return "V0109";
            case "三泽大地":
                return "V0110";
            case "丸藤翔":
                return "V0112";
            case "迪拉诺剑山":
                return "V0113";
            case "斋王琢磨":
                return "V0114";
            case "游城十代/尤贝尔":
                return "V0115";
            case "早乙女礼":
                return "V0140";
            case "奥斯辛奥布赖恩":
                return "V0153";
            case "吉姆·克劳戴尔·库克":
                return "V0154";
            case "霸王十代":
                return "V0155";

            case "不动游星":
                return "V0201";
            case "杰克·阿特拉斯":
                return "V0202";
            case "克罗·霍根":
                return "V0203";
            case "十六夜秋":
                return "V0204";
            case "龙亚":
                return "V0205";
            case "龙可":
                return "V0206";
            case "牛尾哲":
                return "V0207";
            case "DS鬼柳京介":
                return "V0208";
            case "安提诺米":
                return "V0209";
            case "DS卡莉渚":
                return "V0210";
            case "DS雷克斯·哥德温":
                return "V0211";
            case "鬼柳京介":
                return "V0213";
            case "卡莉渚":
                return "V0214";
            case "普拉西多":
                return "V0215";
            case "阿波利亚":
                return "V0216";
            case "帕拉多克斯":
                return "V0217";
            case "Z-ONE":
                return "V0218";

            case "DSOD海马濑人":
                return "V0301";
            case "DSOD海马圭平":
                return "V0302";
            case "DSOD武藤游戏":
                return "V0303";
            case "DSOD城之内克也":
                return "V0304";
            case "DSOD真崎杏子":
                return "V0305";
            case "蓝神":
                return "V0306";
            case "塞拉":
                return "V0307";
            case "塞拉-高维":
                return "V0308";
            case "百济木":
                return "V0309";
            case "DSOD貘良了":
                return "V0315";

            case "九十九游马/阿斯特拉尔":
                return "V0401";
            case "神代凌牙":
                return "V0404";
            case "武田铁男":
                return "V0405";
            case "观月小鸟":
                return "V0406";
            case "天城快斗":
                return "V0407";
            case "III":
                return "V0409";
            case "IV":
                return "V0410";
            case "V":
                return "V0411";
            case "神代璃绪":
                return "V0412";
            case "ZEXAL":
                return "V0415";
            case "神月安娜":
                return "V0418";
            case "基拉古":
                return "V0419";
            case "阿里特":
                return "V0420";
            case "巴利安·基拉古":
                return "V0441";
            case "巴利安·阿里特":
                return "V0442";

            case "榊游矢":
                return "V0501";
            case "柊柚子":
                return "V0502";
            case "权现坂升":
                return "V0503";
            case "泽渡慎吾":
                return "V0504";
            case "赤马零儿":
                return "V0505";
            case "游斗":
                return "V0506";
            case "紫云院素良":
                return "V0507";
            case "黑咲隼":
                return "V0508";
            case "游吾":
                return "V0510";
            case "塞瑞娜":
                return "V0512";

            case "PlayerMaker&Ai":
                return "V0601";
            case "SoulBurner":
                return "V0602";
            case "Go鬼塚":
                return "V0603";
            case "BlueAngel":
                return "V0604";
            case "Revolver":
                return "V0605";
            case "GhostGirl":
                return "V0610";
            case "BraveMax":
                return "V0849";
            case "汉诺骑士":
                return "V0850";

            case "导游小姐":
                return "V9995";

            default:
                return "V0001";
        }
    }

    static bool IsSummon(string line)
    {
        switch (line)
        {
            case "Summon":
                return true;
            case "SummonInDefence":
                return true;
            case "SpecialSummon":
                return true;
            case "Release":
                return true;
            case "AdvanceSummon":
                return true;
            case "FusionSummon":
                return true;
            case "RitualSummon":
                return true;
            case "SynchroSummon":
                return true;
            case "XyzSummon":
                return true;
            case "LinkSummon":
                return true;
            default: 
                return false;
        }
    }

    static string LineMap(string line)
    {
        switch (line)
        {
            default:
                return "";
            case "Start":
                return "_01_00_00_0";
            case "Duel":
                return "_02_00_00_0";
            case "MyTurn":
                return "_03_00_00_0";
            case "Draw":
                return "_04_00_00_0";
            case "DestinyDraw":
                return "_05_00_00_0";
            case "Now":
                return "_06_01_00_0";
            case "Naive":
                return "_06_01_01_0";
            case "Naive2":
                return "_06_01_02_0";
            case "Counter":
                return "_06_01_03_0";
            case "FromHand":
                return "_07_00_00_0";
            case "ActivateMagic":
                return "_07_01_00_0";
            case "ActivateQuickPlayMagic":
                return "_07_02_00_0";
            case "ActivateContinuousMagic":
                return "_07_03_00_0";
            case "ActivateEquipMagic":
                return "_07_04_00_0";
            case "ActivateRitualMagic":
                return "_07_05_00_0";
            case "ActivateTrap":
                return "_07_06_00_0";
            case "ActivateContinuousTrap":
                return "_07_07_00_0";
            case "ActivateCounterTrap":
                return "_07_08_00_0";
            case "OpenSetCard":
                return "_07_09_00_0";
            case "ActivateMonsterEffect":
                return "_07_10_00_0";
            case "ActivateFieldMagic":
                return "_07_11_00_0";
            case "ActivateEffect":
                return "_07_12_00_0";
            case "SetPendulumScale":
                return "_07_13_00_0";
            case "ActivatePendulum":
                return "_07_14_00_0";
            case "ActivateEffects":
                return "_08_00_00_0";
            case "ActivateMonsterEffects":
                return "_09_00_00_0";
            case "PreSummon":
                return "_10_00_00_0";
            case "Summon":
                return "_11_00_00_0";
            case "SummonInDefence":
                return "_11_01_00_0";
            case "SpecialSummon":
                return "_11_02_00_0";
            case "Release":
                return "_11_03_00_0";
            case "AdvanceSummon":
                return "_11_04_00_0";
            case "FusionSummon":
                return "_11_05_00_0";
            case "RitualSummon":
                return "_11_06_00_0";
            case "SynchroSummon":
                return "_11_07_00_0";
            case "XyzSummon":
                return "_11_08_00_0";
            case "PendulumSummon":
                return "_11_09_00_0";
            case "LinkSummon":
                return "_11_10_00_0";
            case "SummonMonsters":
                return "_13_10_00_0";
            case "BattlePhase":
                return "_14_00_00_0";
            case "PreAttack":
                return "_15_00_00_0";
            case "LastAttack":
                return "_16_00_00_0";
            case "Attack":
                return "_17_00_00_0";
            case "DirectAttack":
                return "_18_00_00_0";
            case "MonstersAttack":
                return "_19_00_00_0";
            case "SetCard":
                return "_20_00_00_0";
            case "SetMonster":
                return "_20_01_00_0";
            case "TurnEnd":
                return "_21_00_00_0";
            case "GetDamage":
                return "_22_00_00_0";
            case "LostRoar":
                return "_23_00_00_0";
            case "PayCost":
                return "_24_00_00_0";
            case "GetHugeDamage":
                return "_25_00_00_0";
            case "AfterDamage":
                return "_26_00_00_0";
            case "Adversity":
                return "_27_00_00_0";
            case "Victory":
                return "_28_00_00_0";
            case "Defeat":
                return "_29_00_00_0";
            case "Chat":
                return "_30_00_00_0";
            case "Nani":
                return "_31_00_00_0";
            case "TitleCall":
                return "_32_00_00_0";
            case "Chat2":
                return "_33_00_00_0";
            case "Chat3":
                return "_34_00_00_0";
            case "Chat4":
                return "_35_00_00_0";
            case "Tag":
                return "_36_00_00_0";
            case "SummonLines":
                return "_37_00_00_0";
            case "RidingDuel":
                return "_38_00_00_0";
            case "CoinEffect":
                return "_39_00_00_0";
            case "CoinEffect2":
                return "_40_00_00_0";
            case "PreDimensionDuel":
                return "_41_00_00_0";
            case "DimensionDuel":
                return "_42_00_00_0";
            case "HenShinn":
                return "_43_00_00_0";
            case "ActionDuel":
                return "_44_00_00_0";
            case "ActionCard":
                return "_45_00_00_0";
            case "Reincarnation":
                return "_46_00_00_0";
            case "AfterReincarnation":
                return "_47_00_00_0";
        }
    }

    static string Line2CommentMap(string line, string cardName)
    {
        switch (line)
        {
            default:
                return "";
            case "Start":
                //return "_01_00_00_0";
                return "快来决斗吧！";
            case "Duel":
                //return "_02_00_00_0";
                return "决斗！";
            case "MyTurn":
                //return "_03_00_00_0";
                return "我的回合";
            case "Draw":
                //return "_04_00_00_0";
                return "抽卡";
            case "DestinyDraw":
                //return "_05_00_00_0";
                return "命运一抽！";
            case "Now":
                //return "_06_01_00_0";
                return "这瞬间";
            case "Naive":
                //return "_06_01_01_0";
                return "那可说不准";
            case "Naive2":
                //return "_06_01_02_0";
                return "来了";
            case "Counter":
                //return "_06_01_03_0";
                return "那可不好说";
            case "FromHand":
                //return "_07_00_00_0";
                return "从手卡";
            case "ActivateMagic":
                //return "_07_01_00_0";
                return "发动魔法卡 " + cardName;
            case "ActivateQuickPlayMagic":
                //return "_07_02_00_0";
                return "发动速攻魔法 " + cardName;
            case "ActivateContinuousMagic":
                //return "_07_03_00_0";
                return "发动永续魔法 " + cardName;
            case "ActivateEquipMagic":
                //return "_07_04_00_0";
                return "发动装备魔法 " + cardName;
            case "ActivateRitualMagic":
                //return "_07_05_00_0";
                return "发动仪式魔法 " + cardName;
            case "ActivateTrap":
                //return "_07_06_00_0";
                return "发动陷阱卡 " + cardName;
            case "ActivateContinuousTrap":
                //return "_07_07_00_0";
                return "发动永续陷阱 ";
            case "ActivateCounterTrap":
                //return "_07_08_00_0";
                return "发动反击陷阱 ";
            case "OpenSetCard":
                //return "_07_09_00_0";
                return "打开盖卡 ";
            case "ActivateMonsterEffect":
                //return "_07_10_00_0";
                return "发动" + cardName + "的怪兽效果";
            case "ActivateFieldMagic":
                //return "_07_11_00_0";
                return "发动场地魔法 " + cardName;
            case "ActivateEffect":
                //return "_07_12_00_0";
                return "发动" + cardName + "的效果";
            case "SetPendulumScale":
                //return "_07_13_00_0";
                return "设置灵摆刻度";
            case "ActivatePendulum":
                //return "_07_14_00_0";
                return "发动" + cardName + "的灵摆效果";
            case "ActivateEffects":
                //return "_08_00_00_0";
                return "发动" + cardName + "的效果";
            case "ActivateMonsterEffects":
                //return "_09_00_00_0";
                return "发动" + cardName + "的效果";
            case "PreSummon":
                //return "_10_00_00_0";
                return "出来吧";
            case "Summon":
                //return "_11_00_00_0";
                return "召唤 " + cardName;
            case "SummonInDefence":
                //return "_11_01_00_0";
                return "守备表示召唤 " + cardName;
            case "SpecialSummon":
                //return "_11_02_00_0";
                return "特殊召唤 " + cardName;
            case "Release":
                //return "_11_03_00_0";
                return "解放" + cardName;
            case "AdvanceSummon":
                //return "_11_04_00_0";
                return "上级召唤 " + cardName;
            case "FusionSummon":
                //return "_11_05_00_0";
                return "融合召唤 " + cardName;
            case "RitualSummon":
                //return "_11_06_00_0";
                return "仪式召唤 " + cardName;
            case "SynchroSummon":
                //return "_11_07_00_0";
                return "同调召唤 " + cardName;
            case "XyzSummon":
                //return "_11_08_00_0";
                return "超量召唤 " + cardName;
            case "PendulumSummon":
                //return "_11_09_00_0";
                return "灵摆召唤怪兽";
            case "LinkSummon":
                //return "_11_10_00_0";
                return "连接召唤 " + cardName;
            case "SummonMonsters":
                //return "_13_10_00_0";
                return "召唤 " + cardName;
            case "BattlePhase":
                //return "_14_00_00_0";
                return "进战阶";
            case "PreAttack":
                //return "_15_00_00_0";
                return "上吧";
            case "LastAttack":
                //return "_16_00_00_0";
                return "这下就结束了！";
            case "Attack":
                //return "_17_00_00_0";
                return cardName + " 攻击！";
            case "DirectAttack":
                //return "_18_00_00_0";
                return cardName + " 直接攻击！";
            case "MonstersAttack":
                //return "_19_00_00_0";
                return cardName + " 攻击！";
            case "SetCard":
                //return "_20_00_00_0";
                return "盖1张卡";
            case "SetMonster":
                //return "_20_01_00_0";
                return "盖1只怪兽";
            case "TurnEnd":
                //return "_21_00_00_0";
                return "回合结束";
            case "GetDamage":
                //return "_22_00_00_0";
                return "额……";
            case "LostRoar":
                //return "_23_00_00_0";
                return "额……";
            case "PayCost":
                //return "_24_00_00_0";
                return "额……";
            case "GetHugeDamage":
                //return "_25_00_00_0";
                return "啊！！";
            case "AfterDamage":
                //return "_26_00_00_0";
                return "还没完";
            case "Adversity":
                //return "_27_00_00_0";
                return "这么下去的话……";
            case "Victory":
                //return "_28_00_00_0";
                return "我赢了！";
            case "Defeat":
                //return "_29_00_00_0";
                return "我输了……";
            case "Chat":
                //return "_30_00_00_0";
                return "说骚话";
            case "Nani":
                //return "_31_00_00_0";
                return "纳尼！？";
            case "TitleCall":
                //return "_32_00_00_0";
                return "Yu-Gi-Oh! Duel Links";
            case "Chat2":
                //return "_33_00_00_0";
                return "说骚话";
            case "Chat3":
                //return "_34_00_00_0";
                return "说骚话";
            case "Chat4":
                //return "_35_00_00_0";
                return "说骚话";
            case "Tag":
                //return "_36_00_00_0";
                return "加油";
            case "SummonLines":
                //return "_37_00_00_0";
                return "中二的召唤词";
            case "RidingDuel":
                //return "_38_00_00_0";
                return "骑乘决斗，加速！！";
            case "CoinEffect":
                return "_39_00_00_0";
            case "CoinEffect2":
                return "_40_00_00_0";
            case "PreDimensionDuel":
                return "_41_00_00_0";
            case "DimensionDuel":
                return "_42_00_00_0";
            case "HenShinn":
                return "_43_00_00_0";
            case "ActionDuel":
                return "_44_00_00_0";
            case "ActionCard":
                return "_45_00_00_0";
            case "Reincarnation":
                return "_46_00_00_0";
            case "AfterReincarnation":
                return "_47_00_00_0";
        }
    }

    public static AudioClip GetClip(string p, bool threeD = true, bool stream = true)
    {
        var path = "sound/" + p + ".ogg";
        if (File.Exists(path) == false) path = "sound/" + p + ".wav";
        if (File.Exists(path) == false) path = "sound/" + p + ".mp3";
        if (File.Exists(path) == false) return null;
        path = Environment.CurrentDirectory.Replace("\\", "/") + "/" + path;
        path = "file:///" + path;
        WWW www = new WWW(path);
        return www.GetAudioClip(threeD, stream);
    }

    static AudioClip GetClipFromAB(uint controller, string name)
    {
        if( controller == 0 )
            return ab1.LoadAsset<AudioClip>(name);
        else
            return ab2.LoadAsset<AudioClip>(name);
    }
}
