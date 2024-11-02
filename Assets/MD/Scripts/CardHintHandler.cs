using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;

public class CardHintHandler : MonoBehaviour
{
    public gameCard card;
    Card data_raw;
    bool changed;

    float atk;
    float def;
    string atk_pre = "";
    string atk_after = "";
    string def_pre = "";
    string def_after = "";
    string greaterFormat = "<#05C5F7><size=45>";
    string lessFormat = "<#FB3E08><size=45>";
    string grayFormat = "<#888888><size=30>";

    public void OnEnable()
    {
        atk = -3;
        def = -3;
    }
    public void setHint()
    {
        string raw = "";

        data_raw = CardsManager.Get(card.get_data().Id);

        if ((card.get_data().Type & (uint)CardType.Link) > 0)
        {
            if (card.get_data().Attack > data_raw.Attack)
            {
                atk_pre = greaterFormat;
                atk_after = "</size></color>";
            }
            else if (card.get_data().Attack < data_raw.Attack)
            {
                atk_pre = lessFormat;
                atk_after = "</size></color>";
            }
            else
            {
                atk_pre = "<size=45>";
                atk_after = "</size>";
            }
            raw = atk_pre + card.get_data().Attack + atk_after;
            set_text(raw.Replace("-2", "?"));
        }
        else
        {
            if ((card.p.position & (int)CardPosition.Attack) > 0)
            {
                if (card.get_data().Attack > data_raw.Attack)
                {
                    atk_pre = greaterFormat;
                    atk_after = "</size></color>";
                }
                else if (card.get_data().Attack < data_raw.Attack)
                {
                    atk_pre = lessFormat;
                    atk_after = "</size></color>";
                }
                else
                {
                    atk_pre = "<size=45>";
                    atk_after = "</size>";
                }
                def_pre = grayFormat;
                def_after = "</size></color>";
            }
            else
            {
                atk_pre = grayFormat;
                atk_after = "</size></color>";
                if (card.get_data().Defense > data_raw.Defense)
                {
                    def_pre = greaterFormat;
                    def_after = "</size></color>";
                }
                else if (card.get_data().Defense < data_raw.Defense)
                {
                    def_pre = lessFormat;
                    def_after = "</size></color>";
                }
                else
                {
                    def_pre = "<size=45>";
                    def_after = "</size>";
                }
            }
            raw = atk_pre + card.get_data().Attack + atk_after + "<size=45>" + "/" + "</size>" + def_pre + card.get_data().Defense + def_after;
            set_text(raw.Replace("-2", "?"));
        }
    }

    void set_text(string hint)
    {
        changed = true;

        if(atk == -3)
            atk = data_raw.Attack;
        if(def == -3)
            def = data_raw.Defense;

        if(atk != card.get_data().Attack)
        {
            DOTween.To(() => atk, x => atk = x, card.get_data().Attack, changeTime);
            if ((card.p.position & (uint)CardPosition.Attack) > 0)
            {
                if (atk > card.get_data().Attack)
                {
                    SEHandler.PlaySingle("SE_DUEL/SE_DEBUFF_ATTACK");
                    var decoration = ABLoader.LoadABFromFile("effects/eff_prm/fxp_cardparm_down_001");
                    decoration.transform.position = card.gameObject.transform.position;
                    Destroy(decoration, 2f);
                }
                if (atk < card.get_data().Attack)
                {
                    SEHandler.PlaySingle("SE_DUEL/SE_BUFF_ATTACK");
                    var decoration = ABLoader.LoadABFromFile("effects/eff_prm/fxp_cardparm_up_001");
                    decoration.transform.position = card.gameObject.transform.position;
                    Destroy(decoration, 2f);
                }
            }
        }
        if ((card.get_data().Type & (uint)CardType.Link) == 0 &&def != card.get_data().Defense)
        {
            DOTween.To(() => def, x => def = x, card.get_data().Defense, changeTime);
            if (def > card.get_data().Defense)
            {
                //SEHandler.PlaySingle("SE_DUEL/SE_DEBUFF_ATTACK");
                //var decoration = ABLoader.LoadAB("effects/eff_prm/fxp_cardparm_down_001");
                //decoration.transform.position = card.gameObject.transform.position;
                //Destroy(decoration, 2f);
            }
            else
            {
                //SEHandler.PlaySingle("SE_DUEL/SE_BUFF_ATTACK");
                //var decoration = ABLoader.LoadAB("effects/eff_prm/fxp_cardparm_up_001");
                //decoration.transform.position = card.gameObject.transform.position;
                //Destroy(decoration, 2f);
            }
        }
    }
    float changeTime = 0.6f;
    float time = 0f;
    private void Update()
    {
        if (changed)
        {
            if ((card.get_data().Type & (uint)CardType.Link) > 0)
                GetComponent<TextMeshPro>().text = atk_pre + (int)atk + atk_after;
            else
                GetComponent<TextMeshPro>().text = atk_pre + (int)atk + atk_after + "<size=45>" + "/" + "</size>" + def_pre + (int)def + def_after;

            time += Time.deltaTime;
            if(time > changeTime)
            {
                changed = false;
                time = 0f;
                atk = card.get_data().Attack;
                if ((card.get_data().Type & (uint)CardType.Link) > 0)
                    GetComponent<TextMeshPro>().text = atk_pre + (int)atk + atk_after;
                else
                {
                    def = card.get_data().Defense;
                    GetComponent<TextMeshPro>().text = atk_pre + (int)atk + atk_after + "<size=45>" + "/" + "</size>" + def_pre + (int)def + def_after;
                }
            }
        }
    }
}
