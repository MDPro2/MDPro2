using Ionic.Zip;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class GameTextureManager
{
    private static bool bLock;
    public static Texture2D myBack;
    public static Texture2D opBack;
    public static Texture2D unknown;
    public static Texture2D attack;
    public static Texture2D negated;
    public static Texture2D bar;
    public static Texture2D exBar;
    public static Texture2D lp;
    public static Texture2D time;
    public static Texture2D L;
    public static Texture2D R;
    public static Texture2D Chain;
    public static Texture2D Mask;
    public static Texture2D N;
    public static Texture2D LINK;
    public static Texture2D LINKm;

    public static Color chainColor = Color.white;

    internal static void initialize()
    {
        attack = UIHelper.GetTexture2D("texture/duel/attack.png");
        unknown = UIHelper.GetTexture2D("texture/duel/unknown.jpg");
        negated = UIHelper.GetTexture2D("texture/duel/negated.png");
        bar = UIHelper.GetTexture2D("texture/duel/healthBar/bg.png");
        exBar = UIHelper.GetTexture2D("texture/duel/healthBar/excited.png");
        time = UIHelper.GetTexture2D("texture/duel/healthBar/t.png");
        lp = UIHelper.GetTexture2D("texture/duel/healthBar/lp.png");
        L = UIHelper.GetTexture2D("texture/duel/L.png");
        R = UIHelper.GetTexture2D("texture/duel/R.png");
        LINK = UIHelper.GetTexture2D("texture/duel/link.png");
        LINKm = UIHelper.GetTexture2D("texture/duel/linkMask.png");
        Chain = UIHelper.GetTexture2D("texture/duel/chain.png");
        Mask = UIHelper.GetTexture2D("texture/duel/mask.png");
        N = new Texture2D(10, 10);
        for (var i = 0; i < 10; i++)
        for (var a = 0; a < 10; a++)
            N.SetPixel(i, a, new Color(0, 0, 0, 0));
        N.Apply();
        ColorUtility.TryParseHtmlString(File.ReadAllText("texture/duel/chainColor.txt"), out chainColor);
        
        //RefreshBack();
    }

    public static void RefreshBack()
    {
        string protectorSetting = Config.Get("Protector0", "自定义");
        if (protectorSetting != "自定义" && protectorSetting != "卡组设置")
        {
            Sprite sprite = Program.I().appearance.GetSprite("Protector", Config.Get("Protector0", "自定义"));
            myBack = sprite.texture;
        }
        else if (protectorSetting == "卡组设置")
        {
            var path = "deck/" + Config.Get("deckInUse", "") + ".ydk";
            var deck = new YGOSharp.Deck(path);
            if (deck == null)
                myBack = Resources.Load<Texture2D>("Texture/Protector/ProtectorIcon1070001");
            else
                myBack = Resources.Load<Texture2D>("Texture/Protector/ProtectorIcon107" + selectDeck.AddZero(deck.Protector[0].ToString()));
        }
        else
            myBack = UIHelper.GetTexture2D("texture/duel/me.jpg");

        if (Config.Get("Protector1", "自定义") != "自定义")
        {
            Sprite sprite = Program.I().appearance.GetSprite("Protector", Config.Get("Protector1", "自定义"));
            opBack = sprite.texture;
        }
        else
            opBack = UIHelper.GetTexture2D("texture/duel/opponent.jpg");
    }

    public static void clearAll()
    {
        loadedPicture.Clear();
        loadedCloseUp.Clear();
    }

    public static Dictionary<int, Task<Texture2D>> loadedPicture = new Dictionary<int, Task<Texture2D>>();
    private static readonly Dictionary<int, Task<Texture2D>> loadedCloseUp = new Dictionary<int, Task<Texture2D>>();
    private static readonly Dictionary<string, Texture2D> loadedUI = new Dictionary<string, Texture2D>();

    public static Task<Texture2D> GetCardPicture(int code, int player =0)
    {
        return GetCardPicture(code, player == 0 ? myBack : opBack);
    }

    public static Task<Texture2D> GetCardPictureWithProtector(int code, Texture2D protector)
    {
        return GetCardPicture(code, protector);
    }

    public static async Task<Texture2D> GetCardPicture(int code, Texture2D zero)
    {
        if (code == 0) return zero;
        if (loadedPicture.TryGetValue(code, out var cached)) return await cached;

        foreach (ZipFile zip in GameZipManager.Zips)
        {
            if (zip.Name.ToLower().EndsWith("script.zip"))
                continue;
            foreach (string file in zip.EntryFileNames)
            {
                foreach (var extname in new[] { ".png", ".jpg" })
                {
                    var path = $"pics/{code}{extname}";
                    if (file.ToLower() == path)
                    {
                        var result = UIHelper.GetTexture2DFromZipAsync(zip, file);
                        loadedPicture.Add(code, result);
                        return await result;
                    }
                }
            }
        }

        foreach (var extname in new[] {".png", ".jpg"})
        {
            foreach (var folder in new[] {"expansions", "diy"})
            {
                var ex_path = $"{folder}/pics/{code}{extname}";
                if (File.Exists(ex_path))
                {
                    var result = UIHelper.GetTexture2DAsync(ex_path);
                    loadedPicture.Add(code, result);
                    return await result;
                }
            }
            var path = $"picture/card/{code}{extname}";
            if (File.Exists(path))
            {
                var result = UIHelper.GetTexture2DAsync(path);
                loadedPicture.Add(code, result);
                return await result;
            }
        }
        return unknown;
    }

    public static Texture2D GetCardPictureNow(int code, bool returnUnknown = true)
    {
        foreach (var extname in new[] { ".png", ".jpg" })
        {
            foreach (var folder in new[] {"expansions", "diy"})
            {
                var ex_path = $"{folder}/pics/{code}{extname}";
                if (File.Exists(ex_path))
                {
                    var result = UIHelper.GetTexture2D(ex_path);
                    return result;
                }
            }
            var path = $"picture/card/{code}{extname}";
            if (File.Exists(path))
            {
                var result = UIHelper.GetTexture2D(path);
                return result;
            }
        }

        foreach (ZipFile zip in GameZipManager.Zips)
        {
            if (zip.Name.ToLower().EndsWith("script.zip"))
                continue;
            foreach (string file in zip.EntryFileNames)
            {
                foreach (var extname in new[] { ".png", ".jpg" })
                {
                    var path = $"pics/{code}{extname}";
                    if (file.ToLower() == path)
                    {
                        var result = UIHelper.GetTexture2DFromZip(zip, file);
                        return result;
                    }
                }
            }
        }
        if(returnUnknown)
            return unknown;
        else
            return null;
    }


    public static async Task<Texture2D> GetCardCloseUp(int code)
    {
        if (loadedCloseUp.TryGetValue(code, out var cached)) return await cached;
        var path = $"picture/closeup/{code}.png";
        if (File.Exists(path))
        {
            var result = UIHelper.GetTexture2DAsync(path);
            loadedCloseUp.Add(code, result);
            return await result;
        }

        return N;
    }

    public static async void CacheCardCloseUp(int code)
    {
        await GetCardCloseUp(code);
    }

    public static Texture2D GetUI(string name)
    {
        var path = $"texture/ui/{name}.png";
        if (loadedUI.TryGetValue(path, out var cached)) return cached;
        var result = UIHelper.GetTexture2D(path);
        loadedUI.Add(path, result);
        return result;
    }
}