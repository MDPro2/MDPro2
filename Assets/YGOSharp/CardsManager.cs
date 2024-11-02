using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System;
using System.IO;
using System.Text.RegularExpressions;
using YGOSharp.OCGWrapper.Enums;
using System.Diagnostics;
using System.Linq;
using SevenZip;
using UnityEngine;

namespace YGOSharp
{
    internal static class CardsManager
    {
        public static IDictionary<int, Card> _cards = new Dictionary<int, Card>();

        public static string nullName = "";

        public static string nullString = "";

        internal static void initialize(string databaseFullPath)
        {
            nullName = InterString.Get("欢迎使用MDPro2宵版");
            nullString = @"
因@赤子奈落@不再更新MDPro2，本客户端由@今晚有宵夜吗@继承并更新发布

（感谢白炽星大佬为MDPro2更新提供帮助）

快捷键功能：
F4：开关卡片描述；
F5：开关决斗时的UI；

MDPro2官方交流群：167092257"


;
            using (SqliteConnection connection = new SqliteConnection("Data Source=" + databaseFullPath))
            {
                connection.Open();

                using (IDbCommand command =
                    new SqliteCommand("SELECT datas.*, texts.* FROM datas,texts WHERE datas.id=texts.id;", connection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LoadCard(reader);
                        }
                    }
                }
            }
        }

        internal static Card GetCard(int id)
        {
            if (_cards.ContainsKey(id))
                return _cards[id].clone();
            return null;
        }

        internal static Card GetCardRaw(int id)
        {
            if (_cards.ContainsKey(id))
                return _cards[id];
            return null;
        }

        internal static Card Get(int id)
        {
            Card returnValue = new Card();
            if (id > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    returnValue = GetCard(id - i);
                    if (returnValue != null)
                    {
                        break;
                    }
                }

                if (returnValue == null)
                {
                    returnValue = new Card();
                }
            }

            return returnValue;
        }

        private static void LoadCard(IDataRecord reader)
        {
            Card card = new Card(reader);
            if (!_cards.ContainsKey(card.Id))
            {
                _cards.Add(card.Id, card);
            }
        }

        internal static void updateSetNames()
        {
            foreach (var item in _cards)
            {
                Card card = item.Value;
                card.strSetName = GameStringHelper.getSetName(card.Setcode);
            }
        }

        internal static List<Card> NewSearch
            (
            string getName,
            List<List<string>> filters,
            Banlist banlist
            )
        {
            List<Card> returnValue = new List<Card>();
            string[] strings = getName.Split(' ');

            foreach (var item in _cards)
            {
                Card card = item.Value;
                if ((card.Type & (uint)CardType.Token) == 0)
                {
                    bool pass = true;
                    foreach (string s in strings)
                    {
                        if (s.StartsWith("@"))
                        {
                            if (Regex.Replace(card.strSetName, s.Substring(1, s.Length - 1), "miaowu", RegexOptions.IgnoreCase) != card.strSetName)
                            {
                            }
                            else
                            {
                                pass = false;
                                break;
                            }
                        }
                        else if (
                        s == ""
                        || Regex.Replace(card.Name, s, "miaowu", RegexOptions.IgnoreCase) != card.Name
                        || Regex.Replace(card.Desc, s, "miaowu", RegexOptions.IgnoreCase) != card.Desc
                        || Regex.Replace(card.strSetName, s, "miaowu", RegexOptions.IgnoreCase) != card.strSetName
                        || card.Id.ToString() == s
                        )
                        {
                        }
                        else
                        {
                            pass = false;
                            break;
                        }
                    }
                    if (pass)
                    {
                        if (filters == null)
                        {
                            returnValue.Add(card);
                        }
                        else
                        {
                            pass = false;
                            int voidCount = 0;
                            foreach (var s_category in filters[0])
                            {
                                if (s_category == "")
                                    voidCount++;
                            }
                            if (voidCount == filters[0].Count)
                                pass = true;
                            if (pass == false)
                            {
                                foreach (var s_category in filters[0])
                                {
                                    if (s_category != "" && GameStringHelper.GetType(card).Contains(s_category))
                                    {
                                        if (s_category == "魔法")
                                        {
                                            if ((card.Type & (uint)CardType.Spell) > 0)
                                                pass = true;
                                        }
                                        else
                                            pass = true;
                                        break;
                                    }
                                }
                            }
                            if (pass)
                            {
                                pass = false;
                                voidCount = 0;
                                foreach (var s_attribute in filters[1])
                                {
                                    if (s_attribute == "")
                                        voidCount++;
                                }
                                if (voidCount == filters[1].Count)
                                    pass = true;
                                if (pass == false)
                                {
                                    foreach (var s_attribute in filters[1])
                                    {
                                        if (s_attribute != "" && GameStringHelper.attribute(card.Attribute) == s_attribute)
                                        {
                                            pass = true;
                                            break;
                                        }
                                    }
                                }
                                if (pass)
                                {
                                    pass = false;
                                    voidCount = 0;
                                    foreach (var s_spellType in filters[2])
                                    {
                                        if (s_spellType == "")
                                            voidCount++;
                                    }
                                    if (voidCount == filters[2].Count)
                                        pass = true;
                                    if (pass == false)
                                    {
                                        foreach (var s_spellType in filters[2])
                                        {
                                            if (s_spellType != "" && GameStringHelper.GetSpellType(card) == s_spellType)
                                            {
                                                pass = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (pass)
                                    {
                                        pass = false;
                                        voidCount = 0;
                                        foreach (var s_race in filters[3])
                                        {
                                            if (s_race == "")
                                                voidCount++;
                                        }
                                        if (voidCount == filters[3].Count)
                                            pass = true;
                                        if (pass == false)
                                        {
                                            foreach (var s_race in filters[3])
                                            {
                                                if (s_race != "" && GameStringHelper.race(card.Race) + "族" == s_race)
                                                {
                                                    pass = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (pass)
                                        {
                                            pass = false;
                                            voidCount = 0;
                                            foreach (var s_ability in filters[4])
                                            {
                                                if (s_ability == "")
                                                    voidCount++;
                                            }
                                            if (voidCount == filters[4].Count)
                                                pass = true;
                                            if (pass == false)
                                            {
                                                foreach (var s_ability in filters[4])
                                                {
                                                    if (
                                                        (s_ability != "" && GameStringHelper.GetType(card).Contains(s_ability))
                                                        ||
                                                        (s_ability == "效果以外" && GameStringHelper.GetType(card).Contains("效果") == false && (card.Type & (uint)CardType.Monster) > 0)
                                                        )
                                                    {
                                                        pass = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (pass)
                                            {
                                                pass = false;
                                                voidCount = 0;
                                                foreach (var s_limit in filters[5])
                                                {
                                                    if (s_limit == "")
                                                        voidCount++;
                                                }
                                                if (voidCount == filters[5].Count)
                                                    pass = true;
                                                if (pass == false)
                                                {
                                                    foreach (var s_limit in filters[5])
                                                    {
                                                        if (
                                                            (banlist != null && s_limit == "禁止" && banlist.GetQuantity(card.Id) == 0)
                                                            ||
                                                            (banlist != null && s_limit == "限制" && banlist.GetQuantity(card.Id) == 1)
                                                            ||
                                                            (banlist != null && s_limit == "准限制" && banlist.GetQuantity(card.Id) == 2)
                                                            ||
                                                            (banlist != null && s_limit == "无限制" && banlist.GetQuantity(card.Id) == 3)
                                                            ||
                                                            (banlist == null && s_limit == "无限制")
                                                            )
                                                        {
                                                            pass = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (pass)
                                                {
                                                    pass = false;
                                                    voidCount = 0;
                                                    foreach (var s_ot in filters[6])
                                                    {
                                                        if (s_ot == "")
                                                            voidCount++;
                                                    }
                                                    if (voidCount == filters[6].Count)
                                                        pass = true;
                                                    if (pass == false)
                                                    {
                                                        foreach (var s_ot in filters[6])
                                                        {
                                                            if (
                                                                (s_ot == "OCG" && (card.Ot & 1) == 1)
                                                                ||
                                                                (s_ot == "TCG" && (card.Ot & 2) == 2)
                                                                ||
                                                                (s_ot == "简体中文" && (card.Ot & 8) == 8)
                                                                ||
                                                                (s_ot == "自定义" && (card.Ot & 4) == 4)
                                                                ||
                                                                (s_ot == "无独有" && (card.Ot & 3) == 3)
                                                                ||
                                                                (s_ot == "OCG独有" && (card.Ot & 1) == 1 && (card.Ot & 2) == 0)
                                                                ||
                                                                (s_ot == "TCG独有" && (card.Ot & 2) == 2 && (card.Ot & 1) == 0)
                                                                )
                                                            {
                                                                pass = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    if (pass)
                                                    {
                                                        pass = false;
                                                        voidCount = 0;
                                                        foreach (var s_effect in filters[7])
                                                        {
                                                            if (s_effect == "")
                                                                voidCount++;
                                                        }
                                                        if (voidCount == filters[7].Count)
                                                            pass = true;
                                                        if (pass == false)
                                                        {
                                                            foreach (var s_effect in filters[7])
                                                            {
                                                                if (
                                                                    (s_effect == "魔陷破坏" && (card.Category & 1) == 1)
                                                                    ||
                                                                    (s_effect == "怪兽破坏" && (card.Category & 2) == 2)
                                                                    ||
                                                                    (s_effect == "卡片除外" && (card.Category & 2 << 1) == 2 << 1)
                                                                    ||
                                                                    (s_effect == "送去墓地" && (card.Category & 2 << 2) == 2 << 2)
                                                                    ||
                                                                    (s_effect == "返回手卡" && (card.Category & 2 << 3) == 2 << 3)
                                                                    ||
                                                                    (s_effect == "返回卡组" && (card.Category & 2 << 4) == 2 << 4)
                                                                    ||
                                                                    (s_effect == "手卡破坏" && (card.Category & 2 << 5) == 2 << 5)
                                                                    ||
                                                                    (s_effect == "卡组破坏" && (card.Category & 2 << 6) == 2 << 6)
                                                                    ||
                                                                    (s_effect == "抽卡辅助" && (card.Category & 2 << 7) == 2 << 7)
                                                                    ||
                                                                    (s_effect == "卡组检索" && (card.Category & 2 << 8) == 2 << 8)
                                                                    ||
                                                                    (s_effect == "卡片回收" && (card.Category & 2 << 9) == 2 << 9)
                                                                    ||
                                                                    (s_effect == "表示形式" && (card.Category & 2 << 10) == 2 << 10)
                                                                    ||
                                                                    (s_effect == "控制权" && (card.Category & 2 << 11) == 2 << 11)
                                                                    ||
                                                                    (s_effect == "攻守变化" && (card.Category & 2 << 12) == 2 << 12)
                                                                    ||
                                                                    (s_effect == "穿刺伤害" && (card.Category & 2 << 13) == 2 << 13)
                                                                    ||
                                                                    (s_effect == "多次攻击" && (card.Category & 2 << 14) == 2 << 14)
                                                                    ||
                                                                    (s_effect == "攻击限制" && (card.Category & 2 << 15) == 2 << 15)
                                                                    ||
                                                                    (s_effect == "直接攻击" && (card.Category & 2 << 16) == 2 << 16)
                                                                    ||
                                                                    (s_effect == "特殊召唤" && (card.Category & 2 << 17) == 2 << 17)
                                                                    ||
                                                                    (s_effect == "衍生物" && (card.Category & 2 << 18) == 2 << 18)
                                                                    ||
                                                                    (s_effect == "种族相关" && (card.Category & 2 << 19) == 2 << 19)
                                                                    ||
                                                                    (s_effect == "属性相关" && (card.Category & 2 << 20) == 2 << 20)
                                                                    ||
                                                                    (s_effect == "LP伤害" && (card.Category & 2 << 21) == 2 << 21)
                                                                    ||
                                                                    (s_effect == "LP回复" && (card.Category & 2 << 22) == 2 << 22)
                                                                    ||
                                                                    (s_effect == "破坏耐性" && (card.Category & 2 << 23) == 2 << 23)
                                                                    ||
                                                                    (s_effect == "效果耐性" && (card.Category & 2 << 24) == 2 << 24)
                                                                    ||
                                                                    (s_effect == "指示物" && (card.Category & 2 << 25) == 2 << 25)
                                                                    ||
                                                                    (s_effect == "幸运" && (card.Category & 2 << 26) == 2 << 26)
                                                                    ||
                                                                    (s_effect == "融合相关" && (card.Category & 2 << 27) == 2 << 27)
                                                                    ||
                                                                    (s_effect == "同调相关" && (card.Category & 2 << 28) == 2 << 28)
                                                                    ||
                                                                    (s_effect == "超量相关" && (card.Category & 2 << 29) == 2 << 29)
                                                                    ||
                                                                    (s_effect == "效果无效" && (card.Category & 2 << 30) == 2 << 30)
                                                                    )
                                                                {
                                                                    pass = true;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        if (pass)
                                                        {
                                                            pass = false;
                                                            voidCount = 0;
                                                            foreach (var s_cutin in filters[8])
                                                            {
                                                                if (s_cutin == "")
                                                                    voidCount++;
                                                            }
                                                            if (voidCount == filters[8].Count)
                                                                pass = true;
                                                            if (pass == false)
                                                            {
                                                                foreach (var s_cutin in filters[8])
                                                                {
                                                                    if (
                                                                        (s_cutin == "有" && CheckAnimation.cutins.Contains(card.Id))
                                                                        ||
                                                                        (s_cutin == "无" && CheckAnimation.cutins.Contains(card.Id) == false)
                                                                        )
                                                                    {
                                                                        pass = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            if (pass)
                                                            {
                                                                pass = false;
                                                                voidCount = 0;
                                                                foreach (var s_number in filters[9])
                                                                {
                                                                    if (s_number == "")
                                                                        voidCount++;
                                                                }
                                                                if (voidCount == filters[9].Count)
                                                                    pass = true;
                                                                if (pass == false)
                                                                {
                                                                    int lv_min = filters[9][0] == "" ? -233 : int.Parse(filters[9][0]);
                                                                    int lv_max = filters[9][1] == "" ? -233 : int.Parse(filters[9][1]);
                                                                    int atk_min = filters[9][2] == "" ? -233 : int.Parse(filters[9][2]);
                                                                    int atk_max = filters[9][3] == "" ? -233 : int.Parse(filters[9][3]);
                                                                    int def_min = filters[9][4] == "" ? -233 : int.Parse(filters[9][4]);
                                                                    int def_max = filters[9][5] == "" ? -233 : int.Parse(filters[9][5]);
                                                                    int scale_min = filters[9][6] == "" ? -233 : int.Parse(filters[9][6]);
                                                                    int scale_max = filters[9][7] == "" ? -233 : int.Parse(filters[9][7]);
                                                                    int year_min = filters[9][8] == "" ? -233 : int.Parse(filters[9][8]);
                                                                    int year_max = filters[9][9] == "" ? -233 : int.Parse(filters[9][9]);
                                                                    if (judgeint(atk_min, atk_max, card.Attack))
                                                                    {
                                                                        if (judgeint(def_min, def_max, card.Defense))
                                                                        {
                                                                            if (judgeint(lv_min, lv_max, card.Level))
                                                                            {
                                                                                if (judgeint(scale_min, scale_max, card.LScale))
                                                                                {
                                                                                    if (judgeint(year_min, year_max, card.year))
                                                                                        pass = true;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                if (pass)
                                                                {
                                                                    returnValue.Add(card);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        public static List<string> GetMiddleStrings(string str, string start, string end)
        {
            List<string> returnValue = new List<string>();
            Regex reg = new Regex("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))", RegexOptions.RightToLeft);
            while (reg.Match(str).Value != "")
            {
                string s = reg.Match(str).Value;
                returnValue.Add(s);
                str = str.Replace(start + s + end, "");
            }
            return returnValue;
        }

        public static List<string> setNameTail = new List<string>
        {
            "、",
            "卡",
            "怪兽",
            "魔法",
            "陷阱",
            "通常",
            "效果怪兽",
            "融合",
            "仪式",
            "灵魂",
            "同盟",
            "二重",
            "调整",
            "同调",
            "衍生物",
            "速攻",
            "永续",
            "装备",
            "场地",
            "反击",
            "反转",
            "卡通",
            "超量",
            "灵摆",
            "连接",
        };
        public static List<string> setNameHead = new List<string>
        {
            "带有"
        };

        static List<string> GetSetNamesInDescription(string input)
        {
            List <string> returnValue = new List<string>();
            foreach(string s in setNameHead)
            {
                List<string> setNames = GetMiddleStrings(input, s + "「", "」");
                for (int i = 0; i < setNames.Count; i++)
                {
                    if (returnValue.Contains(setNames[i]) == false)
                        returnValue.Add(setNames[i]);
                }
            }
            foreach (string s in setNameTail)
            {
                List<string> setNames = GetMiddleStrings(input, "「", "」" + s);
                for (int i = 0; i < setNames.Count; i++)
                {
                    if (returnValue.Contains(setNames[i]) == false)
                        returnValue.Add(setNames[i]);
                }
            }
            return returnValue;
        }

        internal static List<Card> RelatedSearch(int code)
        {
            var cards = new List<Card>();
            var card = GetCard(code);
            if (card == null)
                return cards;
            card.strSetName = GameStringHelper.getSetName(card.Setcode);

            List<string> names = new List<string>();
            names.Add(card.Name);
            List<string> setNames = GetSetNamesInDescription(card.Desc);
            if (setNames.Contains(card.strSetName) == false)
                setNames.Add(card.strSetName);

            var matches = GetMiddleStrings(card.Desc, "「", "」");
            foreach (var match in matches)
                if (names.Contains(match.ToString()) == false)
                    names.Add(match.ToString());
            foreach(string s in setNames)
                if(names.Contains(s))
                    names.Remove(s);

            names.Remove("");
            setNames.Remove("");

            List<int> setCodes = new List<int>();
            foreach (var s in setNames)
                setCodes.Add(GameStringHelper.GetSetNameCode(s));

            foreach (var item in _cards)
            {
                if (card.Id != item.Value.Id && (item.Value.Type & (uint)CardType.Token) == 0)
                {
                    bool pass = false;
                    foreach (var n in names)
                    {
                        if (
                            Regex.Replace(item.Value.Name, n, "miaowu", RegexOptions.IgnoreCase) != item.Value.Name
                            || Regex.Replace(item.Value.Desc, "「" + n + "」", "miaowu", RegexOptions.IgnoreCase) != item.Value.Desc
                            || Regex.Replace(item.Value.strSetName, n, "miaowu", RegexOptions.IgnoreCase) != item.Value.strSetName
                            )
                        {
                            pass = true; 
                            break;
                        }
                    }
                    if (pass == false)
                    {
                        for (int i = 0; i < setNames.Count; i++)
                        {
                            if (
                                Regex.Replace(item.Value.Desc, "「" + setNames[i] + "」", "miaowu", RegexOptions.IgnoreCase) != item.Value.Desc
                                //|| Regex.Replace(item.Value.strSetName, setNames[i], "miaowu", RegexOptions.IgnoreCase) != item.Value.strSetName
                                || (setCodes[i] != 0)
                                    && (setCodes[i] - item.Value.Setcode == 0 || ~Math.Abs(setCodes[i] - item.Value.Setcode) == 0x999)
                                )
                            {
                                pass = true;
                                break;
                            }
                        }
                    }
                    if (pass)
                        cards.Add(item.Value);
                }
            }
            cards.Sort(comparisonOfCard());
            return cards;
        }

        internal static List<Card> searchAdvanced(
            string getName,
            int getLevel,
            int getAttack,
            int getDefence,
            int getP,
            int getYear,
            int getLevel_UP,
            int getAttack_UP,
            int getDefence_UP,
            int getP_UP,
            int getYear_UP,
            int getOT,
            string getPack,
            int getBAN,
            Banlist banlist,
            uint getTypeFilter,
            uint getTypeFilter2,
            uint getRaceFilter,
            uint getAttributeFilter,
            uint getCatagoryFilter
        )
        {
            List<Card> returnValue = new List<Card>();
            foreach (var item in _cards)
            {
                Card card = item.Value;
                if ((card.Type & (uint)CardType.Token) == 0)
                {
                    if (getName == ""
                        || Regex.Replace(card.Name, getName, "miaowu", RegexOptions.IgnoreCase) != card.Name
                        || Regex.Replace(card.Desc, getName, "miaowu", RegexOptions.IgnoreCase) != card.Desc
                        || Regex.Replace(card.strSetName, getName, "miaowu", RegexOptions.IgnoreCase) != card.strSetName
                        || card.Id.ToString() == getName
                    )
                    {
                        if (((card.Type & getTypeFilter) == getTypeFilter || getTypeFilter == 0)
                            && ((card.Type == getTypeFilter2
                                 || getTypeFilter == (UInt32)CardType.Monster) &&
                                (card.Type & getTypeFilter2) == getTypeFilter2
                                || getTypeFilter2 == 0))
                        {
                            if ((card.Race & getRaceFilter) > 0 || getRaceFilter == 0)
                            {
                                if ((card.Attribute & getAttributeFilter) > 0 || getAttributeFilter == 0)
                                {
                                    if (((card.Category & getCatagoryFilter)) == getCatagoryFilter ||
                                        getCatagoryFilter == 0)
                                    {
                                        if (judgeint(getAttack, getAttack_UP, card.Attack))
                                        {
                                            if (judgeint(getDefence, getDefence_UP, card.Defense))
                                            {
                                                if (judgeint(getLevel, getLevel_UP, card.Level))
                                                {
                                                    if (judgeint(getP, getP_UP, card.LScale))
                                                    {
                                                        if (judgeint(getYear, getYear_UP, card.year))
                                                        {
                                                            if (getBAN == -233 || banlist == null ||
                                                                banlist.GetQuantity(card.Id) == getBAN)
                                                            {
                                                                if (getOT == -233 || (getOT & card.Ot) == getOT)
                                                                {
                                                                    if (getPack == "" || card.packFullName == getPack)
                                                                    {
                                                                        returnValue.Add(card);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            nameInSearch = getName;
            returnValue.Sort(comparisonOfCard());
            nameInSearch = "";
            return returnValue;
        }

        public static string nameInSearch = "";

        static bool judgeint(int min, int max, int raw)
        {
            bool re = true;
            if (min == -233 && max == -233)
            {
                re = true;
            }

            if (min == -233 && max != -233)
            {
                re = max == raw;
            }

            if (min != -233 && max == -233)
            {
                re = min == raw;
            }

            if (min != -233 && max != -233)
            {
                re = min <= raw && raw <= max;
            }

            return re;
        }

        internal static List<Card> search(
            string getName,
            List<int> getsearchCode
        )
        {
            List<Card> returnValue = new List<Card>();
            foreach (var item in _cards)
            {
                Card card = item.Value;
                if (getName == ""
                    || Regex.Replace(card.Name, getName, "miaowu", RegexOptions.IgnoreCase) != card.Name
                    //|| Regex.Replace(card.Desc, getName, "miaowu", RegexOptions.IgnoreCase) != card.Desc
                    || Regex.Replace(card.strSetName, getName, "miaowu", RegexOptions.IgnoreCase) != card.strSetName
                    || card.Id.ToString() == getName
                )
                {
                    if (getsearchCode.Count == 0 || is_declarable(card, getsearchCode))
                    {
                        returnValue.Add(card);
                    }
                }
            }

            nameInSearch = getName;
            returnValue.Sort(comparisonOfCard());
            nameInSearch = "";
            return returnValue;
        }

        private static bool is_declarable(Card card, List<int> getsearchCode)
        {
            Stack<int> stack = new Stack<int>();
            for (int i = 0; i < getsearchCode.Count; i++)
            {
                switch (getsearchCode[i])
                {
                    case (int)searchCode.OPCODE_ADD:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            stack.Push(lhs + rhs);
                        }

                        break;
                    case (int)searchCode.OPCODE_SUB:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            stack.Push(lhs - rhs);
                        }

                        break;
                    case (int)searchCode.OPCODE_MUL:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            stack.Push(lhs * rhs);
                        }

                        break;
                    case (int)searchCode.OPCODE_DIV:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            stack.Push(lhs / rhs);
                        }

                        break;
                    case (int)searchCode.OPCODE_AND:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            bool b0 = rhs != 0;
                            bool b1 = lhs != 0;
                            if (b0 && b1)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }

                        break;
                    case (int)searchCode.OPCODE_OR:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            bool b0 = rhs != 0;
                            bool b1 = lhs != 0;
                            if (b0 || b1)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }

                        break;
                    case (int)searchCode.OPCODE_NEG:
                        if (stack.Count >= 1)
                        {
                            int rhs = stack.Pop();
                            stack.Push(-rhs);
                        }

                        break;
                    case (int)searchCode.OPCODE_NOT:
                        if (stack.Count >= 1)
                        {
                            int rhs = stack.Pop();
                            bool b0 = rhs != 0;
                            if (b0)
                            {
                                stack.Push(0);
                            }
                            else
                            {
                                stack.Push(1);
                            }
                        }

                        break;
                    case (int)searchCode.OPCODE_ISCODE:
                        if (stack.Count >= 1)
                        {
                            int code = stack.Pop();
                            bool b0 = code == card.Id;
                            if (b0)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }

                        break;
                    case (int)searchCode.OPCODE_ISSETCARD:
                        if (stack.Count >= 1)
                        {
                            if (IfSetCard(stack.Pop(), card.Setcode))
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }

                        break;
                    case (int)searchCode.OPCODE_ISTYPE:
                        if (stack.Count >= 1)
                        {
                            if ((stack.Pop() & card.Type) > 0)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }

                        break;
                    case (int)searchCode.OPCODE_ISRACE:
                        if (stack.Count >= 1)
                        {
                            if ((stack.Pop() & card.Race) > 0)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }

                        break;
                    case (int)searchCode.OPCODE_ISATTRIBUTE:
                        if (stack.Count >= 1)
                        {
                            if ((stack.Pop() & card.Attribute) > 0)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }

                        break;
                    default:
                        stack.Push(getsearchCode[i]);
                        break;
                }
            }

            if (stack.Count != 1 || stack.Pop() == 0)
                return false;
            return
                card.Id == (int)TwoNameCards.CARD_MARINE_DOLPHIN
                ||
                card.Id == (int)TwoNameCards.CARD_TWINKLE_MOSS
                ||
                (!(card.Alias != 0)
                 && ((card.Type & ((int)CardType.Monster + (int)CardType.Token)))
                 != ((int)CardType.Monster + (int)CardType.Token));
        }

        public static bool IfSetCard(int setCodeToAnalyse, long setCodeFromCard)
        {
            bool res = false;
            int settype = setCodeToAnalyse & 0xfff;
            int setsubtype = setCodeToAnalyse & 0xf000;
            long sc = setCodeFromCard;
            while (sc != 0)
            {
                if ((sc & 0xfff) == settype && (sc & 0xf000 & setsubtype) == setsubtype)
                    res = true;
                sc = sc >> 16;
            }

            return res;
        }

        internal static Comparison<Card> comparisonOfCard()
        {
            return (left, right) =>
            {
                int a = 1;
                if (left.Name == nameInSearch && right.Name != nameInSearch)
                {
                    a = -1;
                }
                else if (right.Name == nameInSearch && left.Name != nameInSearch)
                {
                    a = 1;
                }
                else
                {
                    if ((left.Type & 7) < (right.Type & 7))
                    {
                        a = -1;
                    }
                    else if ((left.Type & 7) > (right.Type & 7))
                    {
                        a = 1;
                    }
                    else
                    {
                        //if ((left.Type >> 3) > (right.Type >> 3))
                        //{
                        //    a = 1;
                        //}
                        //else if ((left.Type >> 3) < (right.Type >> 3))
                        //{
                        //    a = -1;
                        //}
                        if ((left.Type & 0x58020f0) < (right.Type & 0x58020f0))
                        {
                            a = -1;
                        }
                        else if ((left.Type & 0x58020f0) > (right.Type & 0x58020f0))
                        {
                            a = 1;
                        }
                        else
                        {
                            if (left.Level > right.Level)
                            {
                                a = -1;
                            }
                            else if (left.Level < right.Level)
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.Attack > right.Attack)
                                {
                                    a = -1;
                                }
                                else if (left.Attack < right.Attack)
                                {
                                    a = 1;
                                }
                                else
                                {
                                    if (left.Attribute > right.Attribute)
                                    {
                                        a = 1;
                                    }
                                    else if (left.Attribute < right.Attribute)
                                    {
                                        a = -1;
                                    }
                                    else
                                    {
                                        if (left.Race > right.Race)
                                        {
                                            a = 1;
                                        }
                                        else if (left.Race < right.Race)
                                        {
                                            a = -1;
                                        }
                                        else
                                        {
                                            if (left.Category > right.Category)
                                            {
                                                a = 1;
                                            }
                                            else if (left.Category < right.Category)
                                            {
                                                a = -1;
                                            }
                                            else
                                            {
                                                if (left.Id > right.Id)
                                                {
                                                    a = 1;
                                                }
                                                else if (left.Id < right.Id)
                                                {
                                                    a = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return a;
            };
        }
        internal static Comparison<Card> comparisonOfCardReverse()
        {
            return (left, right) =>
            {
                int a = 1;
                if (left.Name == nameInSearch && right.Name != nameInSearch)
                {
                    a = -1;
                }
                else if (right.Name == nameInSearch && left.Name != nameInSearch)
                {
                    a = 1;
                }
                else
                {
                    if ((left.Type & 7) < (right.Type & 7))
                    {
                        a = -1;
                    }
                    else if ((left.Type & 7) > (right.Type & 7))
                    {
                        a = 1;
                    }
                    else
                    {
                        //if ((left.Type >> 3) > (right.Type >> 3))
                        //{
                        //    a = -1;
                        //}
                        //else if ((left.Type >> 3) < (right.Type >> 3))
                        //{
                        //    a = 1;
                        //}
                        if ((left.Type & 0x58020f0) < (right.Type & 0x58020f0))
                        {
                            a = 1;
                        }
                        else if ((left.Type & 0x58020f0) > (right.Type & 0x58020f0))
                        {
                            a = -1;
                        }
                        else
                        {
                            if (left.Level > right.Level)
                            {
                                a = -1;
                            }
                            else if (left.Level < right.Level)
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.Attack > right.Attack)
                                {
                                    a = -1;
                                }
                                else if (left.Attack < right.Attack)
                                {
                                    a = 1;
                                }
                                else
                                {
                                    if (left.Attribute > right.Attribute)
                                    {
                                        a = 1;
                                    }
                                    else if (left.Attribute < right.Attribute)
                                    {
                                        a = -1;
                                    }
                                    else
                                    {
                                        if (left.Race > right.Race)
                                        {
                                            a = 1;
                                        }
                                        else if (left.Race < right.Race)
                                        {
                                            a = -1;
                                        }
                                        else
                                        {
                                            if (left.Category > right.Category)
                                            {
                                                a = 1;
                                            }
                                            else if (left.Category < right.Category)
                                            {
                                                a = -1;
                                            }
                                            else
                                            {
                                                if (left.Id > right.Id)
                                                {
                                                    a = 1;
                                                }
                                                else if (left.Id < right.Id)
                                                {
                                                    a = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return a;
            };
        }
        internal static Comparison<Card> comparisonOfCard_ATK_Down()
        {
            return (left, right) =>
            {
                int a = 1;
                if (left.Name == nameInSearch && right.Name != nameInSearch)
                {
                    a = -1;
                }
                else if (right.Name == nameInSearch && left.Name != nameInSearch)
                {
                    a = 1;
                }
                else
                {
                    if ((left.Type & 7) < (right.Type & 7))
                    {
                        a = -1;
                    }
                    else if ((left.Type & 7) > (right.Type & 7))
                    {
                        a = 1;
                    }
                    else
                    {
                        if (left.Attack > right.Attack)
                        {
                            a = -1;
                        }
                        else if (left.Attack < right.Attack)
                        {
                            a = 1;
                        }
                        else
                        {
                            //if ((left.Type >> 3) > (right.Type >> 3))
                            //{
                            //    a = 1;
                            //}
                            //else if ((left.Type >> 3) < (right.Type >> 3))
                            //{
                            //    a = -1;
                            //}
                            if ((left.Type & 0x58020f0) < (right.Type & 0x58020f0))
                            {
                                a = -1;
                            }
                            else if ((left.Type & 0x58020f0) > (right.Type & 0x58020f0))
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.Level > right.Level)
                                {
                                    a = -1;
                                }
                                else if (left.Level < right.Level)
                                {
                                    a = 1;
                                }
                                else
                                {
                                    if (left.Attribute > right.Attribute)
                                    {
                                        a = 1;
                                    }
                                    else if (left.Attribute < right.Attribute)
                                    {
                                        a = -1;
                                    }
                                    else
                                    {
                                        if (left.Race > right.Race)
                                        {
                                            a = 1;
                                        }
                                        else if (left.Race < right.Race)
                                        {
                                            a = -1;
                                        }
                                        else
                                        {
                                            if (left.Category > right.Category)
                                            {
                                                a = 1;
                                            }
                                            else if (left.Category < right.Category)
                                            {
                                                a = -1;
                                            }
                                            else
                                            {
                                                if (left.Id > right.Id)
                                                {
                                                    a = 1;
                                                }
                                                else if (left.Id < right.Id)
                                                {
                                                    a = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return a;
            };
        }
        internal static Comparison<Card> comparisonOfCard_ATK_Up()
        {
            return (left, right) =>
            {
                int a = 1;
                if (left.Name == nameInSearch && right.Name != nameInSearch)
                {
                    a = -1;
                }
                else if (right.Name == nameInSearch && left.Name != nameInSearch)
                {
                    a = 1;
                }
                else
                {
                    if ((left.Type & 7) < (right.Type & 7))
                    {
                        a = -1;
                    }
                    else if ((left.Type & 7) > (right.Type & 7))
                    {
                        a = 1;
                    }
                    else
                    {
                        if (left.Attack > right.Attack)
                        {
                            a = 1;
                        }
                        else if (left.Attack < right.Attack)
                        {
                            a = -1;
                        }
                        else
                        {
                            //if ((left.Type >> 3) > (right.Type >> 3))
                            //{
                            //    a = 1;
                            //}
                            //else if ((left.Type >> 3) < (right.Type >> 3))
                            //{
                            //    a = -1;
                            //}
                            if ((left.Type & 0x58020f0) < (right.Type & 0x58020f0))
                            {
                                a = -1;
                            }
                            else if ((left.Type & 0x58020f0) > (right.Type & 0x58020f0))
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.Level > right.Level)
                                {
                                    a = 1;
                                }
                                else if (left.Level < right.Level)
                                {
                                    a = -1;
                                }
                                else
                                {
                                    if (left.Attribute > right.Attribute)
                                    {
                                        a = 1;
                                    }
                                    else if (left.Attribute < right.Attribute)
                                    {
                                        a = -1;
                                    }
                                    else
                                    {
                                        if (left.Race > right.Race)
                                        {
                                            a = 1;
                                        }
                                        else if (left.Race < right.Race)
                                        {
                                            a = -1;
                                        }
                                        else
                                        {
                                            if (left.Category > right.Category)
                                            {
                                                a = 1;
                                            }
                                            else if (left.Category < right.Category)
                                            {
                                                a = -1;
                                            }
                                            else
                                            {
                                                if (left.Id > right.Id)
                                                {
                                                    a = 1;
                                                }
                                                else if (left.Id < right.Id)
                                                {
                                                    a = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return a;
            };
        }
        internal static Comparison<Card> comparisonOfCard_DEF_Down()
        {
            return (left, right) =>
            {
                int a = 1;
                if (left.Name == nameInSearch && right.Name != nameInSearch)
                {
                    a = -1;
                }
                else if (right.Name == nameInSearch && left.Name != nameInSearch)
                {
                    a = 1;
                }
                else
                {
                    if ((left.Type & 7) < (right.Type & 7))
                    {
                        a = -1;
                    }
                    else if ((left.Type & 7) > (right.Type & 7))
                    {
                        a = 1;
                    }
                    else
                    {
                        if (left.Defense > right.Defense)
                        {
                            a = -1;
                        }
                        else if (left.Defense < right.Defense)
                        {
                            a = 1;
                        }
                        else
                        {
                            //if ((left.Type >> 3) > (right.Type >> 3))
                            //{
                            //    a = 1;
                            //}
                            //else if ((left.Type >> 3) < (right.Type >> 3))
                            //{
                            //    a = -1;
                            //}
                            if ((left.Type & 0x58020f0) < (right.Type & 0x58020f0))
                            {
                                a = -1;
                            }
                            else if ((left.Type & 0x58020f0) > (right.Type & 0x58020f0))
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.Level > right.Level)
                                {
                                    a = -1;
                                }
                                else if (left.Level < right.Level)
                                {
                                    a = 1;
                                }
                                else
                                {
                                    if (left.Attribute > right.Attribute)
                                    {
                                        a = 1;
                                    }
                                    else if (left.Attribute < right.Attribute)
                                    {
                                        a = -1;
                                    }
                                    else
                                    {
                                        if (left.Race > right.Race)
                                        {
                                            a = 1;
                                        }
                                        else if (left.Race < right.Race)
                                        {
                                            a = -1;
                                        }
                                        else
                                        {
                                            if (left.Category > right.Category)
                                            {
                                                a = 1;
                                            }
                                            else if (left.Category < right.Category)
                                            {
                                                a = -1;
                                            }
                                            else
                                            {
                                                if (left.Id > right.Id)
                                                {
                                                    a = 1;
                                                }
                                                else if (left.Id < right.Id)
                                                {
                                                    a = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return a;
            };
        }
        internal static Comparison<Card> comparisonOfCard_DEF_Up()
        {
            return (left, right) =>
            {
                int a = 1;
                if (left.Name == nameInSearch && right.Name != nameInSearch)
                {
                    a = -1;
                }
                else if (right.Name == nameInSearch && left.Name != nameInSearch)
                {
                    a = 1;
                }
                else
                {
                    if ((left.Type & 7) < (right.Type & 7))
                    {
                        a = -1;
                    }
                    else if ((left.Type & 7) > (right.Type & 7))
                    {
                        a = 1;
                    }
                    else
                    {
                        if (left.Defense > right.Defense)
                        {
                            a = 1;
                        }
                        else if (left.Defense < right.Defense)
                        {
                            a = -1;
                        }
                        else
                        {
                            //if ((left.Type >> 3) > (right.Type >> 3))
                            //{
                            //    a = 1;
                            //}
                            //else if ((left.Type >> 3) < (right.Type >> 3))
                            //{
                            //    a = -1;
                            //}
                            if ((left.Type & 0x58020f0) < (right.Type & 0x58020f0))
                            {
                                a = -1;
                            }
                            else if ((left.Type & 0x58020f0) > (right.Type & 0x58020f0))
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.Level > right.Level)
                                {
                                    a = 1;
                                }
                                else if (left.Level < right.Level)
                                {
                                    a = -1;
                                }
                                else
                                {
                                    if (left.Attribute > right.Attribute)
                                    {
                                        a = 1;
                                    }
                                    else if (left.Attribute < right.Attribute)
                                    {
                                        a = -1;
                                    }
                                    else
                                    {
                                        if (left.Race > right.Race)
                                        {
                                            a = 1;
                                        }
                                        else if (left.Race < right.Race)
                                        {
                                            a = -1;
                                        }
                                        else
                                        {
                                            if (left.Category > right.Category)
                                            {
                                                a = 1;
                                            }
                                            else if (left.Category < right.Category)
                                            {
                                                a = -1;
                                            }
                                            else
                                            {
                                                if (left.Id > right.Id)
                                                {
                                                    a = 1;
                                                }
                                                else if (left.Id < right.Id)
                                                {
                                                    a = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return a;
            };
        }
        internal static Comparison<Card> comparisonOfCard_LV_Down()
        {
            return (left, right) =>
            {
                int a = 1;
                if (left.Name == nameInSearch && right.Name != nameInSearch)
                {
                    a = -1;
                }
                else if (right.Name == nameInSearch && left.Name != nameInSearch)
                {
                    a = 1;
                }
                else
                {
                    if ((left.Type & 7) < (right.Type & 7))
                    {
                        a = -1;
                    }
                    else if ((left.Type & 7) > (right.Type & 7))
                    {
                        a = 1;
                    }
                    else
                    {
                        if (left.Level > right.Level)
                        {
                            a = -1;
                        }
                        else if (left.Level < right.Level)
                        {
                            a = 1;
                        }
                        else
                        {
                            //if ((left.Type >> 3) > (right.Type >> 3))
                            //{
                            //    a = 1;
                            //}
                            //else if ((left.Type >> 3) < (right.Type >> 3))
                            //{
                            //    a = -1;
                            //}
                            if ((left.Type & 0x58020f0) < (right.Type & 0x58020f0))
                            {
                                a = -1;
                            }
                            else if ((left.Type & 0x58020f0) > (right.Type & 0x58020f0))
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.Attack > right.Attack)
                                {
                                    a = -1;
                                }
                                else if (left.Attack < right.Attack)
                                {
                                    a = 1;
                                }
                                else
                                {
                                    if (left.Attribute > right.Attribute)
                                    {
                                        a = 1;
                                    }
                                    else if (left.Attribute < right.Attribute)
                                    {
                                        a = -1;
                                    }
                                    else
                                    {
                                        if (left.Race > right.Race)
                                        {
                                            a = 1;
                                        }
                                        else if (left.Race < right.Race)
                                        {
                                            a = -1;
                                        }
                                        else
                                        {
                                            if (left.Category > right.Category)
                                            {
                                                a = 1;
                                            }
                                            else if (left.Category < right.Category)
                                            {
                                                a = -1;
                                            }
                                            else
                                            {
                                                if (left.Id > right.Id)
                                                {
                                                    a = 1;
                                                }
                                                else if (left.Id < right.Id)
                                                {
                                                    a = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return a;
            };
        }
        internal static Comparison<Card> comparisonOfCard_LV_Up()
        {
            return (left, right) =>
            {
                int a = 1;
                if (left.Name == nameInSearch && right.Name != nameInSearch)
                {
                    a = -1;
                }
                else if (right.Name == nameInSearch && left.Name != nameInSearch)
                {
                    a = 1;
                }
                else
                {
                    if ((left.Type & 7) < (right.Type & 7))
                    {
                        a = -1;
                    }
                    else if ((left.Type & 7) > (right.Type & 7))
                    {
                        a = 1;
                    }
                    else
                    {
                        if (left.Level > right.Level)
                        {
                            a = 1;
                        }
                        else if (left.Level < right.Level)
                        {
                            a = -1;
                        }
                        else
                        {
                            //if ((left.Type >> 3) > (right.Type >> 3))
                            //{
                            //    a = 1;
                            //}
                            //else if ((left.Type >> 3) < (right.Type >> 3))
                            //{
                            //    a = -1;
                            //}
                            if ((left.Type & 0x58020f0) < (right.Type & 0x58020f0))
                            {
                                a = -1;
                            }
                            else if ((left.Type & 0x58020f0) > (right.Type & 0x58020f0))
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.Attack > right.Attack)
                                {
                                    a = 1;
                                }
                                else if (left.Attack < right.Attack)
                                {
                                    a = -1;
                                }
                                else
                                {
                                    if (left.Attribute > right.Attribute)
                                    {
                                        a = 1;
                                    }
                                    else if (left.Attribute < right.Attribute)
                                    {
                                        a = -1;
                                    }
                                    else
                                    {
                                        if (left.Race > right.Race)
                                        {
                                            a = 1;
                                        }
                                        else if (left.Race < right.Race)
                                        {
                                            a = -1;
                                        }
                                        else
                                        {
                                            if (left.Category > right.Category)
                                            {
                                                a = 1;
                                            }
                                            else if (left.Category < right.Category)
                                            {
                                                a = -1;
                                            }
                                            else
                                            {
                                                if (left.Id > right.Id)
                                                {
                                                    a = 1;
                                                }
                                                else if (left.Id < right.Id)
                                                {
                                                    a = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return a;
            };
        }
    }

    internal static class PacksManager
    {
        public class packName
        {
            public string fullName;
            public string shortName;
            public int year;
            public int month;
            public int day;
        }

        public static List<packName> packs = new List<packName>();

        static Dictionary<string, string> pacDic = new Dictionary<string, string>();

        internal static void initialize(string databaseFullPath)
        {
            using (SqliteConnection connection = new SqliteConnection("Data Source=" + databaseFullPath))
            {
                connection.Open();
                using (IDbCommand command = new SqliteCommand("SELECT pack.* FROM pack;", connection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                int Id = (int) reader.GetInt64(0);
                                Card c = CardsManager.GetCardRaw(Id);
                                if (c != null)
                                {
                                    string temp = reader.GetString(1);
                                    c.packFullName = reader.GetString(2);
                                    string[] mats = temp.Split("-");
                                    if (mats.Length > 1)
                                        c.packShortNam = mats[0];
                                    else
                                        c.packShortNam = c.packFullName.Length > 10 ? c.packFullName.Substring(0, 10) + "..." : c.packFullName;
                                    c.reality = reader.GetString(3);
                                    temp = reader.GetString(4);
                                    mats = temp.Split("/");
                                    if (mats.Length == 3)
                                    {
                                        c.month = int.Parse(mats[0]);
                                        c.day = int.Parse(mats[1]);
                                        c.year = int.Parse(mats[2]);
                                    }
                                    mats = temp.Split("-");
                                    if (mats.Length == 3)
                                    {
                                        c.year = int.Parse(mats[0]);
                                        c.month = int.Parse(mats[1]);
                                        c.day = int.Parse(mats[2]);
                                    }
                                    c.packFullName = c.year + "-" + c.month.ToString().PadLeft(2, '0') + "-" + c.day.ToString().PadLeft(2, '0') + " " + c.packShortNam;

                                    if (!pacDic.ContainsKey(c.packFullName))    
                                    {
                                        pacDic.Add(c.packFullName, c.packShortNam);
                                        packName p = new packName();
                                        p.day = c.day;
                                        p.year = c.year;
                                        p.month = c.month;
                                        p.fullName = c.packFullName;
                                        p.shortName = c.packShortNam;
                                        packs.Add(p);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
        }

        internal static void initializeSec()
        {
            packs.Sort((left, right) =>
            {
                if (left.year > right.year)
                {
                    return -1;
                }

                if (left.year < right.year)
                {
                    return 1;
                }

                if (left.month > right.month)
                {
                    return -1;
                }

                if (left.month < right.month)
                {
                    return 1;
                }

                if (left.day > right.day)
                {
                    return -1;
                }

                if (left.day < right.day)
                {
                    return 1;
                }

                return 1;
            });
        }
    }
}