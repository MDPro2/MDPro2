using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;

public class CardOnList : MonoBehaviour
{
    CardListHandler cardListHandler;

    UITexture pic;
    UI2DSprite sprite;
    UIButton button;
    UILabel levelNum;
    GameObject faceDown;
    GameObject chain_icon;
    UILabel chain_num;

    gameCard card;
    public GameObject go;

    public CardOnList(gameCard card)
    {
        this.card = card;
        go = Instantiate(Program.I().new_ui_cardList.cardOnListPrefab);
        go.transform.parent = Program.I().new_ui_cardList.grid;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        cardListHandler = Program.I().new_ui_cardList;

        pic = go.transform.Find("pic_").GetComponent<UITexture>();
        pic.mainTexture = card.gameObject_face.GetComponent<Renderer>().material.mainTexture;

        sprite = go.transform.Find("icon_").GetComponent<UI2DSprite>();
        levelNum = go.transform.Find("lv_").GetComponent<UILabel>();
        if ((card.get_data().Type & (uint)CardType.Monster) > 0)
        {
            if ((card.get_data().Type & (uint)CardType.Link) > 0)
            {
                sprite.sprite2D = cardListHandler.link;
                if(card.get_data().Level == 0)
                {
                    for (var i = 0; i < 32; i++)
                        if ((card.get_data().LinkMarker & (1 << i)) > 0)
                            card.get_data().Level++;
                }
            }
            else if ((card.get_data().Type & (uint)CardType.Xyz) > 0)
                sprite.sprite2D = cardListHandler.rank;
            levelNum.text = card.get_data().Level.ToString();
        }
        else
        {
            sprite.sprite2D = null;
            levelNum.text = "";
        }


        faceDown = go.transform.Find("face_down").gameObject;
        if((card.p.position & (uint)CardPosition.FaceUp) > 0)
            faceDown.SetActive(false);
        if((card.p.location & (uint)CardLocation.Overlay) > 0)
            faceDown.SetActive(false);
        if (card.get_data().Id == 0)
            faceDown.SetActive(false);

        button = go.transform.Find("button_cardOnList").GetComponent<UIButton>();
        EventDelegate.Add(button.onClick, OnClick);

        chain_icon = go.transform.Find("chain_icon").gameObject;
        chain_icon.SetActive(card.currentKuang == gameCard.kuangType.chaining);

        chain_num = go.transform.Find("chain_num").GetComponent<UILabel>();
        if (card.chains.Count > 0)
            chain_num.text = card.chains[card.chains.Count - 1].i.ToString();
        else
            chain_num.text = "1";
        if (card.p.controller == 0)
            chain_num.color = Color.cyan;
        else
            chain_num.color = Color.red;
        chain_num.gameObject.SetActive(card.currentKuang == gameCard.kuangType.chaining);
    }

    void OnClick()
    {
        if (card.get_data().Id != 0)
        {
            if(card.p.controller == 0)
                Program.I().cardDescription.setData(card.get_data(), GameTextureManager.myBack, card.tails.managedString, true);
            else
                Program.I().cardDescription.setData(card.get_data(), GameTextureManager.opBack, card.tails.managedString, true);
        }
    }
}
