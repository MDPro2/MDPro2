using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class Setting : WindowServant2D
{
    public LAZYsetting setting;
    private UIPopupList _screen;
    private UIPopupList _aa;
    private UIPopupList _fps;

    public float bgmVol;
    public float seVol;
    public float voiceVol;
    public override void initialize()
    {
        gameObject = SetWindow(this, Program.I().new_ui_setting);
        setting = gameObject.GetComponent<LAZYsetting>();

        _screen = UIHelper.getByName<UIPopupList>(gameObject, "screen_");
        _aa = UIHelper.getByName<UIPopupList>(gameObject, "aa_");
        _aa.value = Config.Get("aa_", "无");
        _fps = UIHelper.getByName<UIPopupList>(gameObject, "fps_");
        _fps.value = Config.Get("fps_", "60");
        changeAA();
        changeFPS();
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        UIHelper.registEvent(gameObject, "screen_", resizeScreen);
        UIHelper.registEvent(gameObject, "aa_", changeAA);
        UIHelper.registEvent(gameObject, "fps_", changeFPS);
        UIHelper.registEvent(gameObject, "full_", resizeScreen);
        UIHelper.getByName<UIToggle>(gameObject, "full_").value = Screen.fullScreen;
        UIHelper.getByName<UIToggle>(gameObject, "ignoreWatcher_").value =
            UIHelper.fromStringToBool(Config.Get("ignoreWatcher_", "0"));
        UIHelper.getByName<UIToggle>(gameObject, "ignoreOP_").value =
            UIHelper.fromStringToBool(Config.Get("ignoreOP_", "0"));
        UIHelper.getByName<UIToggle>(gameObject, "smartSelect_").value =
            UIHelper.fromStringToBool(Config.Get("smartSelect_", "1"));
        UIHelper.getByName<UIToggle>(gameObject, "autoChain_").value =
            UIHelper.fromStringToBool(Config.Get("autoChain_", "0"));
        UIHelper.getByName<UIToggle>(gameObject, "handPosition_").value =
            UIHelper.fromStringToBool(Config.Get("handPosition_", "1"));
        UIHelper.getByName<UIToggle>(gameObject, "handmPosition_").value =
            UIHelper.fromStringToBool(Config.Get("handmPosition_", "1"));
        UIHelper.getByName<UIToggle>(gameObject, "spyer_").value = UIHelper.fromStringToBool(Config.Get("spyer_", "0"));
        UIHelper.getByName<UIToggle>(gameObject, "timming_").value = UIHelper.fromStringToBool(Config.Get("timming_", "0"));
        UIHelper.getByName<UIToggle>(gameObject, "confirmLeft_").value = UIHelper.fromStringToBool(Config.Get("confirmLeft_", "0"));
        UIHelper.getByName<UIToggle>(gameObject, "showFPS_").value = UIHelper.fromStringToBool(Config.Get("showFPS_", "0"));
        UIHelper.getByName<UIToggle>(gameObject, "resize_").canChange = false;
        if (QualitySettings.GetQualityLevel() < 3)
            UIHelper.getByName<UIToggle>(gameObject, "high_").value = false;
        else
            UIHelper.getByName<UIToggle>(gameObject, "high_").value = true;
        UIHelper.registEvent(gameObject, "ignoreWatcher_", save);
        UIHelper.registEvent(gameObject, "ignoreOP_", save);
        UIHelper.registEvent(gameObject, "smartSelect_", save);
        UIHelper.registEvent(gameObject, "autoChain_", save);
        UIHelper.registEvent(gameObject, "handPosition_", save);
        UIHelper.registEvent(gameObject, "handmPosition_", save);
        UIHelper.registEvent(gameObject, "spyer_", save);
        UIHelper.registEvent(gameObject, "timming_", save);
        UIHelper.registEvent(gameObject, "confirmLeft_", save);
        UIHelper.registEvent(gameObject, "showFPS_", save);
        UIHelper.registEvent(gameObject, "high_", save);

        //Program.go(2000, readVales);
        var collection = gameObject.GetComponentsInChildren<UIToggle>();
        for (var i = 0; i < collection.Length; i++)
            if (collection[i].name.Length > 0 && collection[i].name[0] == '*')
            {
                if (collection[i].name == "*MonsterCloud")
                    collection[i].value = UIHelper.fromStringToBool(Config.Get(collection[i].name, "0"));
                else
                    collection[i].value = UIHelper.fromStringToBool(Config.Get(collection[i].name, "1"));
            }

        UIHelper.registEvent(setting.mouseEffect.gameObject, onchangeMouse);
        UIHelper.registEvent(setting.cloud.gameObject, onchangeCloud);
        UIHelper.registEvent(setting.Vpedium.gameObject, onCP);
        UIHelper.registEvent(setting.Vfield.gameObject, onCP);
        UIHelper.registEvent(setting.Vlink.gameObject, onCP);

        onchangeMouse();
        onchangeCloud();
        SetScreenSizeValue();

        setting.sliderSeVol.forceValue(int.Parse(Config.Get("seVol_", "750")) / 1000f);
        setting.sliderBgmVol.forceValue(int.Parse(Config.Get("bgmVol_", "500")) / 1000f);
        setting.sliderVoiceVol.forceValue(int.Parse(Config.Get("voiceVol_", "500")) / 1000f);
        EventDelegate.Add(setting.sliderBgmVol.onChange, BgmVolChange);
        EventDelegate.Add(setting.sliderSeVol.onChange, SeVolChange);
        EventDelegate.Add(setting.sliderVoiceVol.onChange, VoiceVolChange);
        bgmVol = setting.sliderBgmVol.value;
        seVol = setting.sliderSeVol.value;
        voiceVol = setting.sliderVoiceVol.value;

        UIHelper.registEvent(gameObject, "appearance_1", SetAppearance1);
        UIHelper.registEvent(gameObject, "appearance_2", SetAppearance2);
        UIHelper.registEvent(gameObject, "charater_1", SelectCharacter1);
        UIHelper.registEvent(gameObject, "charater_2", SelectCharacter2);

        setting.character0.text = Config.Get("Character0", "暗游戏");
        setting.character1.text = Config.Get("Character1", "暗游戏");
        setting.field0.text = Config.Get("Field0", "随机");
        setting.field1.text = Config.Get("Field1", "随机");
    }

    void changeFPS()
    {
        if (_fps.value == "无限制")
            Application.targetFrameRate = -1;
        else
            Application.targetFrameRate = int.Parse(_fps.value);
    }
    void changeAA()
    {
        var cameraData = Program.I().main_camera.GetUniversalAdditionalCameraData();
        switch (_aa.value)
        {
            case "无":
                cameraData.antialiasing = AntialiasingMode.None;
                break;
            case "FXAA":
                cameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                break;
            case "SMAA":
                cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                //cameraData.antialiasingQuality = AntialiasingQuality.High;
                break;
        }
        if(_aa.value == "无")
        {
            cameraData.antialiasing = AntialiasingMode.None;
        }
    }

    public void RefreshAppearance()
    {
        LoadAssets.CreateField1(Config.Get("Field0", "随机"));
        LoadAssets.CreateField2(Config.Get("Field1", "随机"));
        if (Program.I().ocgcore.gameField != null && Program.I().ocgcore.gameField.gameObject != null)
        {
            foreach (var btn in Program.I().ocgcore.gameField.gameHiddenButtons)
                btn.GetGraveMaterial();
            Program.I().ocgcore.gameInfo.SetFace();
        }

        Program.I().setting.setting.field0.text = Config.Get("Field0", "随机");
        Program.I().setting.setting.field1.text = Config.Get("Field1", "随机");
    }


    void SetAppearance1()
    {
        Program.I().appearance.gameObject.SetActive(true);
        Program.I().appearance.Show(0);
    }
    void SetAppearance2()
    {
        Program.I().appearance.gameObject.SetActive(true);
        Program.I().appearance.Show(1);
    }

    void SelectCharacter1()
    {
        Program.I().character.gameObject.SetActive(true);
        Program.I().character.Show(0);
    }
    void SelectCharacter2()
    {
        Program.I().character.gameObject.SetActive(true);
        Program.I().character.Show(1);
    }


    void BgmVolChange()
    {
        bgmVol = setting.sliderBgmVol.value;
        BGMHandler.ChangeBgmVol(bgmVol);
    }
    void SeVolChange()
    {
        seVol = setting.sliderSeVol.value;
        GameObject.Find("SE").GetComponent<AudioSource>().volume = seVol;
        GameObject.Find("SE_Timeline/SE1").GetComponent<AudioSource>().volume = seVol;
        GameObject.Find("SE_Timeline/SE2").GetComponent<AudioSource>().volume = seVol;
        GameObject.Find("SE_Timeline/SE3").GetComponent<AudioSource>().volume = seVol;
        GameObject.Find("SE_Timeline/SE4").GetComponent<AudioSource>().volume = seVol;
    }
    void VoiceVolChange()
    {
        voiceVol = setting.sliderVoiceVol.value;
        GameObject.Find("Voice").GetComponent<AudioSource>().volume = voiceVol;
        GameObject.Find("Voice2").GetComponent<AudioSource>().volume = voiceVol;
    }

    private void readVales()
    {
        try
        {
            setting.sliderSeVol.forceValue(int.Parse(Config.Get("seVol_", "750")) / 1000f);
            setting.sliderBgmVol.forceValue(int.Parse(Config.Get("bgmVol_", "500")) / 1000f);
            setting.sliderVoiceVol.forceValue(int.Parse(Config.Get("voiceVol_", "500")) / 1000f);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void onchangeCloud()
    {
        Program.MonsterCloud = setting.cloud.value;
    }

    public void onchangeMouse()
    {
        Program.I().mouseParticle.SetActive(setting.mouseEffect.value);
    }

    public void SetScreenSizeValue()
    {
        var target = $"{Screen.width} x {Screen.height}";
        if (_screen.value != target) _screen.value = target;

        _screen.items = /*(Screen.fullScreen ? Screen.resolutions : WindowResolutions())*/
            WindowResolutions()
            .Select(r => $"{r.width} x {r.height}")
            .Distinct()
            .ToList();
    }

    private static IEnumerable<Resolution> WindowResolutions()
    {
        var resolutions = new List<Resolution>();
        var max = Screen.resolutions.Last();

        foreach (var resolution in Screen.resolutions)
        {
            resolutions.Add(resolution);
        }
#if !UNITY_EDITOR && UNITY_ANDROID
        resolutions.Add(new Resolution { width = max.height * 3 / 4, height = max.height * 3 / 4 });
        resolutions.Add(new Resolution { width = max.height * 2 / 4, height = max.height * 2 / 4 });
        resolutions.Add(new Resolution { width = max.height * 1 / 4, height = max.height * 1 / 4 });
#else
        resolutions.Add(new Resolution { width = max.width, height = max.width * 9 / 20 });
        resolutions.Add(new Resolution { width = max.width, height = max.width * 9 / 18 });
        resolutions.Add(new Resolution { width = max.width, height = max.width * 9 / 16 });
#endif
        return resolutions;
    }

    private void onCP()
    {
        try
        {
            Program.I().ocgcore.realize(true);
        }
        catch (Exception e)
        {
        }
    }

    public float vol()
    {
        return UIHelper.getByName<UISlider>(gameObject, "seVol_").value;
    }

    public override void preFrameFunction()
    {
        base.preFrameFunction();
    }

    private void onClickExit()
    {
        hide();
    }

    public override void hide()
    {
        isShowed = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        DOTween.To(() => gameObject.transform.GetChild(1).GetComponent<UIPanel>().alpha, x => gameObject.transform.GetChild(1).GetComponent<UIPanel>().alpha = x, 0, 0.3f);
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_04");
        Program.I().ocgcore.gameInfo.SetFace();
    }
    public override void show()
    {
        isShowed = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        DOTween.To(() => gameObject.transform.GetChild(1).GetComponent<UIPanel>().alpha, x => gameObject.transform.GetChild(1).GetComponent<UIPanel>().alpha = x, 1, 0.3f);
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_03");
    }

    private void resizeScreen()
    {
        if (UIHelper.isMaximized())
            UIHelper.RestoreWindow();

        var mats = UIHelper.getByName<UIPopupList>(gameObject, "screen_").value
            .Split(new[] { " x " }, StringSplitOptions.RemoveEmptyEntries);
        Assert.IsTrue(mats.Length == 2);
        Screen.SetResolution(int.Parse(mats[0]), int.Parse(mats[1]),
            UIHelper.getByName<UIToggle>(gameObject, "full_").value);
        Program.go(100, () =>
        {
            SetScreenSizeValue();
            Program.I().fixScreenProblems();
        });
    }

    public void saveWhenQuit()
    {
        Config.Set("seVol_", ((int)(UIHelper.getByName<UISlider>(gameObject, "seVol_").value * 1000)).ToString());
        Config.Set("bgmVol_", ((int)(UIHelper.getByName<UISlider>(gameObject, "bgmVol_").value * 1000)).ToString());
        Config.Set("voiceVol_", ((int)(UIHelper.getByName<UISlider>(gameObject, "voiceVol_").value * 1000)).ToString());
        var collection = gameObject.GetComponentsInChildren<UIToggle>();
        for (var i = 0; i < collection.Length; i++)
            if (collection[i].name.Length > 0 && collection[i].name[0] == '*')
                Config.Set(collection[i].name, UIHelper.fromBoolToString(collection[i].value));
        Config.Set("showoffATK", setting.showoffATK.value);
        Config.Set("showoffStar", setting.showoffStar.value);
        Config.Set("maximize_", UIHelper.fromBoolToString(UIHelper.isMaximized()));
        Config.Set("aa_", _aa.value);
        Config.Set("fps_", _fps.value);
    }

    public void save()
    {
        Config.Set("ignoreWatcher_",
            UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "ignoreWatcher_").value));
        Config.Set("ignoreOP_", UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "ignoreOP_").value));
        Config.Set("smartSelect_",
            UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "smartSelect_").value));
        Config.Set("autoChain_",
            UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "autoChain_").value));
        Config.Set("handPosition_",
            UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "handPosition_").value));
        Config.Set("handmPosition_",
            UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "handmPosition_").value));
        Config.Set("spyer_", UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "spyer_").value));
        Config.Set("timming_", UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "timming_").value));
        Config.Set("confirmLeft_", UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "confirmLeft_").value));
        Config.Set("showFPS_", UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "showFPS_").value));
        if (UIHelper.getByName<UIToggle>(gameObject, "high_").value)
            QualitySettings.SetQualityLevel(5);
        else
            QualitySettings.SetQualityLevel(0);
    }

    public float soundValue()
    {
        return UIHelper.getByName<UISlider>(gameObject, "seVol_").value;
    }
}
