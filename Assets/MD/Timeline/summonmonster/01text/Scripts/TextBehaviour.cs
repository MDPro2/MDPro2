using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBehaviour : MonoBehaviour
{
    public string cardName = "³à×ÓÄÎÂä";
    public int atk = 3000;
    public int def = 2500;
    public string type = "level";
    public int level = 12;

    private void Start()
    {
        string _atk = atk.ToString();
        if (atk == -2) _atk = "?";
        string _def = def.ToString();
        if (def == -2) _def = "?";
        this.transform.Find("SummonMonster_Name/tex_NameBg/MosterName Text_TMP").GetComponent<TextMesh>().text = cardName;
        if(type == "link") this.transform.Find("SummonMonster_Name/tex_StatusBg/MosterPara Text").GetComponent<TextMesh>().text = string.Format("ATK {0}", _atk);
        else this.transform.Find("SummonMonster_Name/tex_StatusBg/MosterPara Text").GetComponent<TextMesh>().text = string.Format("ATK {0}  DEF {1}", _atk, _def);
    }
}
