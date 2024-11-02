using System;
using System.IO;
using System.Runtime.InteropServices;
using Percy;
using YGOSharp;
using GameMessage = YGOSharp.OCGWrapper.Enums.GameMessage;

public class PrecyOcg
{
    public static string HintInGame = smallYgopro.HintInGame;

    public static bool godMode;

    private static string error = "Error occurred.";

    private static IntPtr _buffer;

    private object locker = new object();

    public smallYgopro ygopro;

    public PrecyOcg()
    {
        _buffer = Marshal.AllocHGlobal(1024 * 256); // 256 KiB
        error = InterString.Get(
            "Error occurred! @nError occurred! @nError occurred! @nError occurred! @nError occurred! @nError occurred! @nYGOPro1旧版的录像崩溃了！您可以选择使用永不崩溃的新版录像。");
        ygopro = new smallYgopro(receiveHandler, cardHandler, scriptHandler, chatHandler);
        //ygopro.m_log = a => { Program.DEBUGLOG(a); };
    }

    public void dispose()
    {
        ygopro.dispose();
    }

    private void receiveHandler(byte[] buffer)
    {
        var bufferR = new byte[buffer.Length + 1];
        bufferR[0] = 1;
        buffer.CopyTo(bufferR, 1);
        TcpHelper.addDateJumoLine(bufferR);
    }

    public void startPuzzle(string path)
    {
        if (Program.I().ocgcore.isShowed == false)
        {
            Program.I().room.mode = 0;
            godMode = true;
            prepareOcgcore();
            Program.I().ocgcore.isFirst = true;
            Program.I().ocgcore.returnServant = Program.I().puzzleMode;
            if (!ygopro.startPuzzle(path))
            {
                Program.I().cardDescription.RMSshow_none(InterString.Get("游戏内部出错，请重试，文件名中不能包含中文。"));
                return;
            }

            //Config.ClientVersion = 0x233c;
            Program.I().shiftToServant(Program.I().ocgcore);
            Program.I().cardDescription.setTitle(path);
        }
    }

    public void startAI(string playerDek, string aiDeck, string aiScript, bool playerGo, bool unrand, int life,
        bool god, int rule)
    {
        if (Program.I().ocgcore.isShowed == false)
        {
            Program.I().room.mode = 0;
            Program.I().ocgcore.MasterRule = rule;
            godMode = god;
            prepareOcgcore();
            Program.I().ocgcore.lpLimit = life;
            Program.I().ocgcore.isFirst = playerGo;
            Program.I().ocgcore.returnServant = Program.I().aiRoom;
            if (!ygopro.startAI(playerDek, aiDeck, aiScript, playerGo, unrand, life, god, rule))
            {
                Program.I().cardDescription.RMSshow_none(InterString.Get("游戏内部出错，请重试，文件名中不能包含中文。"));
                return;
            }
            else
            {
                //Config.ClientVersion = 0x233c;
                Program.I().shiftToServant(Program.I().ocgcore);
            }
        }
    }


    private void prepareOcgcore()
    {
        Program.I().ocgcore.name_0 = Config.Get("name", "一秒一喵机会");
        Program.I().ocgcore.name_0_c = Program.I().ocgcore.name_0;
        Program.I().ocgcore.name_1 = "Percy AI";
        Program.I().ocgcore.name_1_c = "Percy AI";
        Program.I().ocgcore.name_0_tag = "---";
        Program.I().ocgcore.name_1_tag = "---";
        Program.I().ocgcore.timeLimit = 240;
        Program.I().ocgcore.lpLimit = 8000;
        Program.I().ocgcore.handler = response;
        Program.I().ocgcore.shiftCondition(Ocgcore.Condition.watch);
        Program.I().ocgcore.InAI = true;
    }

    public void response(byte[] resp)
    {
        UnityEngine.Debug.Log(BitConverter.ToString(resp));
        ygopro.response(resp);
    }

    private CardData cardHandler(long code)
    {
        var card = CardsManager.Get((int)code);
        var returnValue = new CardData
        {
            Code = card.Id,
            Alias = card.Alias,
            Attack = card.Attack,
            Attribute = card.Attribute,
            Defense = card.Defense,
            Level = card.Level,
            LScale = card.LScale,
            Race = card.Race,
            RScale = card.RScale,
            Type = card.Type,
            LinkMarker = card.LinkMarker
        };
        returnValue.ConvertLongToSetCode(card.Setcode);
        return returnValue;
    }

    private ScriptData scriptHandler(string filename)
    {
        byte[] content;
        ScriptData ret;
        ret.buffer = IntPtr.Zero;
        ret.len = 0;
        if (!filename.StartsWith("puzzle"))
        {
            var filename2 = filename.TrimStart('.', '/');
            foreach (var zip in GameZipManager.Zips)
                if (zip.ContainsEntry(filename2))
                {
                    var ms = new MemoryStream();
                    var e = zip[filename2];
                    e.Extract(ms);
                    content = ms.ToArray();
                    Marshal.Copy(content, 0, _buffer, content.Length);
                    ret.buffer = _buffer;
                    ret.len = content.Length;
                    break;
                }
        }
        else
        {
            if (File.Exists(filename))
            {
                content = File.ReadAllBytes(filename);
                Marshal.Copy(content, 0, _buffer, content.Length);
                ret.buffer = _buffer;
                ret.len = content.Length;
            }

        }
        return ret;
    }

    private void chatHandler(string result)
    {
        var p = new BinaryMaster();
        p.writer.Write((byte) GameMessage.sibyl_chat);
        result = result.Replace("Error Occurred.", error);
        p.writer.WriteUnicode(result, result.Length + 1);
        receiveHandler(p.get());
    }
}