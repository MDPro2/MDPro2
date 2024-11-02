using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class SelectServer : WindowServantSP
{
    private GameObject faceShow = null;

    private UIInput inputIP;
    private UIInput inputPort;
    private UIInput inputPsw;
    private UIInput inputVersion;
    private UIPopupList list;
    private UIButton preIp;

    public string name = "";

    public override void initialize()
    {
        SetWindow(Program.I().new_ui_selectServer);
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        UIHelper.registEvent(gameObject, "face_", onClickFace);
        UIHelper.registEvent(gameObject, "join_", onClickJoin);
        name = Config.Get("name", "一秒一喵机会");
        UIHelper.getByName<UIInput>(gameObject, "name_").value = name;
        list = UIHelper.getByName<UIPopupList>(gameObject, "history_");
        UIHelper.registEvent(gameObject, "history_", onSelected);
        inputIP = UIHelper.getByName<UIInput>(gameObject, "ip_");
        inputPort = UIHelper.getByName<UIInput>(gameObject, "port_");
        inputPsw = UIHelper.getByName<UIInput>(gameObject, "psw_");
        inputVersion = UIHelper.getByName<UIInput>(gameObject, "version_");
        inputVersion.value = Config.ClientVersion.ToString();
        //添加选择服务器
        preIp = UIHelper.getByName<UIButton>(gameObject, "_preIP");
        EventDelegate.Add(preIp.onClick, OnPreIPClick);
        preIp.gameObject.SetActive(CheckPreIPShow());
        SetActiveFalse();
    }

    /// <summary>
    /// 检查是否该显示预设服务器
    /// </summary>
    /// <returns></returns>
    private bool CheckPreIPShow()
    {
        FileInfo fileInfo = new FileInfo($"config/preHosts.conf");
        if (!fileInfo.Exists)
        {
            return false;
        }
        List<(string, string, string)> preIpInfo = new List<(string, string, string)>();
        
        using (var reader = fileInfo.OpenText())
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var group = line.Split('|');
                if (group.Length >= 3)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 点击选择预设服务器键出现的事件
    /// </summary>
    private void OnPreIPClick()
    {
        FileInfo fileInfo = new FileInfo($"config/preHosts.conf");
        if (!fileInfo.Exists)
        {
            return;
        }
        List<(string, string, string)> preIpInfo = new List<(string, string, string)>();
        using (var reader = fileInfo.OpenText())
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var group = line.Split('|');
                if (group.Length >= 3)
                {
                    preIpInfo.Add((group[0],group[1],group[2]));
                }
            }
        }
        if (preIpInfo.Count == 0)
        {
            return;
        }
        var values = new List<(string, Action)>();
        for (var i = 0; i < preIpInfo.Count; i++)
        {
            var stringGroup = preIpInfo[i];
            values.Add(($"{stringGroup.Item1}({stringGroup.Item2}:{stringGroup.Item3})", () =>
            {
                inputIP.value = stringGroup.Item2;
                inputPort.value = stringGroup.Item3;
                RMSshow_clear();
            }));
        }
        values.Add((InterString.Get("取消(预设服务器可以在config/preHosts.conf中修改内容)"), () =>
        {
            RMSshow_clear();
        }));

        RMSshow_singleChoice("NULL", values);
    }

    private void onSelected()
    {
        if (list != null) readString(list.value);
    }

    private void readString(string str)
    {
        var remain = "";
        string ip = "", port = "", psw = "";
        string[] splited;
        splited = str.Split(":");
        try
        {
            ip = splited[0];
            remain = splited[1];
        }
        catch (Exception)
        {
        }

        splited = remain.Split(" ");
        try
        {
            port = splited[0];
            psw = splited[1];
        }
        catch (Exception)
        {
        }

        inputIP.value = ip;
        inputPort.value = port;
        inputPsw.value = psw;
    }

    public override void show()
    {
        base.show();
        Program.I().room.RMSshow_clear();
        printFile(true);
        Program.charge();
        Program.I().ocgcore.returnServant = Program.I().selectServer;
    }

    private void printFile(bool first)
    {
        list.Clear();
        if (File.Exists("config/hosts.conf") == false) File.Create("config/hosts.conf").Close();
        var txtString = File.ReadAllText("config/hosts.conf");
        var lines = txtString.Replace("\r", "").Split("\n");
        for (var i = 0; i < lines.Length; i++)
        {
            lines[i] = Regex.Replace(lines[i], "^\\(.*\\)", ""); // remove old version
            if (i == 0)
                if (first)
                    readString(lines[i]);
            list.AddItem(lines[i]);
        }
    }

    private void onClickExit()
    {
        if (Program.exitOnReturn)
            Program.I().menu.onClickExit();
        else
            Program.I().shiftToServant(Program.I().menu);
        if (TcpHelper.tcpClient != null)
            if (TcpHelper.tcpClient.Connected)
                TcpHelper.tcpClient.Close();
    }

    private void onClickJoin()
    {
        if (!isShowed) return;
        var Name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        var ipString = UIHelper.getByName<UIInput>(gameObject, "ip_").value;
        var portString = UIHelper.getByName<UIInput>(gameObject, "port_").value;
        var pswString = UIHelper.getByName<UIInput>(gameObject, "psw_").value;
        var versionString = UIHelper.getByName<UIInput>(gameObject, "version_").value;
        KF_onlineGame(Name, ipString, portString, versionString, pswString);
    }

    public void KF_onlineGame(string Name, string ipString, string portString, string versionString,
        string pswString = "")
    {
        name = Name;
        Config.Set("name", name);
        if (ipString == "" || portString == "")
        {
            RMSshow_onlyYes("", InterString.Get("非法输入！请检查输入的主机名。"), null);
        }
        else
        {
            if (name != "")
            {
                var fantasty = ipString + ":" + portString + " " + pswString;
                list.items.Remove(fantasty);
                list.items.Insert(0, fantasty);
                list.value = fantasty;
                if (list.items.Count > 5) list.items.RemoveAt(list.items.Count - 1);
                var all = "";
                for (var i = 0; i < list.items.Count; i++) all += list.items[i] + "\r\n";
                File.WriteAllText("config/hosts.conf", all);
                printFile(false);
                new Thread(() => { TcpHelper.join(ipString, name, portString, pswString, versionString); }).Start();
            }
            else
            {
                RMSshow_onlyYes("", InterString.Get("昵称不能为空。"), null);
            }
        }
    }

    private void onClickFace()
    {
        name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        RMSshow_face("showFace", name);
        Config.Set("name", name);
    }
}