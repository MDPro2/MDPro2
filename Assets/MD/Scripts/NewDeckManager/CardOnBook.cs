using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;

public class CardOnBook : MonoBehaviour
{
    public int id;
    private int _code;
    public int code
    {
        get { return _code; }
        set 
        {
            if(value != _code)
            {
                _code = value;
                if(refreshCard !=  null)
                    StopCoroutine(refreshCard);
                refreshCard = Refresh();
                StartCoroutine(refreshCard);
            }
        }
    }
    public Card data;

    public UIButton btn_;
    public UI2DSprite limit_;
    public UI2DSprite dot_1;
    public UI2DSprite dot_2;
    public UI2DSprite dot_3;

    public Collider collider;

    Vector3 clickInPosition;

    private void Start()
    {
        EventDelegate.Add(btn_.onClick, OnClick);
    }

    private void Update()
    {
        if (Program.pointedCollider == collider &&
            Program.InputGetMouseButtonDown_0)
        {
            clickInPosition = Input.mousePosition;
        }
        if (Program.pointedCollider == collider 
            && Program.InputGetMouseButtonUp_1 
            && Program.I().newDeckManager.condition != NewDeckManager.Condition.changeSide
            )
        {
            AddThisCard();
        }
        if (Program.pointedCollider != collider || Program.InputGetMouseButton_0 == false)
        {
            clickInPosition = Vector3.zero;
        }
        if (clickInPosition != Vector3.zero 
            && (clickInPosition - Input.mousePosition).magnitude > 5
            && Program.I().newDeckManager.condition != NewDeckManager.Condition.changeSide
            )
        {
            CreateCard();
            clickInPosition = Vector3.zero;
        }
    }

    private IEnumerator refreshCard;
    IEnumerator Refresh()
    {
        data = CardsManager.GetCard(code);
        if (data == null)
            data = new Card();
        limit_.alpha = 0;
        dot_1.alpha = 0;
        dot_2.alpha = 0;
        dot_3.alpha = 0;

        if ((data.Type & (uint)CardType.Monster) > 0)
        {
            if ((data.Type & (uint)CardType.Pendulum) > 0)
            {
                if ((data.Type & (uint)CardType.Ritual) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_p_ritual.texture;
                else if ((data.Type & (uint)CardType.Fusion) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_p_fusion.texture;
                else if ((data.Type & (uint)CardType.Synchro) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_p_synchro.texture;
                else if ((data.Type & (uint)CardType.Xyz) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_p_xyz.texture;
                else if ((data.Type & (uint)CardType.Normal) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_p_normal.texture;
                else if ((data.Type & (uint)CardType.Effect) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_p_effect.texture;
            }
            else
            {
                if ((data.Type & (uint)CardType.Ritual) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_ritual.texture;
                else if ((data.Type & (uint)CardType.Fusion) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_fusion.texture;
                else if ((data.Type & (uint)CardType.Synchro) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_synchro.texture;
                else if ((data.Type & (uint)CardType.Xyz) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_xyz.texture;
                else if ((data.Type & (uint)CardType.Link) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_link.texture;
                else if ((data.Type & (uint)CardType.Normal) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_normal.texture;
                else if ((data.Type & (uint)CardType.Effect) > 0)
                    GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_effect.texture;
            }
        }
        else if ((data.Type & (uint)CardType.Spell) > 0)
            GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_spell.texture;
        else
            GetComponent<UITexture>().mainTexture = Program.I().newDeckManager.mono.card_trap.texture;

        float waitTime = 0.005f;

#if !UNITY_EDITOR && UNITY_ANDROID
        waitTime = 0.02f;
#endif
        yield return new WaitForSeconds(id * waitTime);

        RefreshFace();
        RefreshLimitIcon();
        ShowDot();
    }

    GameObject CreateCard(int id = 9999)
    {
        GameObject card = Instantiate(Program.I().newDeckManager.mono.cardOnManager);
        CardOnManager cardOnManager = card.GetComponent<CardOnManager>();
        cardOnManager.code = code;
        cardOnManager.id = id;
        card.transform.parent = Program.I().newDeckManager.mono.cardsContainer;
        card.transform.position = Program.I().camera_back_ground_2d.ScreenToWorldPoint(Input.mousePosition);
        if(id == 9999)
            cardOnManager.pressed = true;

        card.transform.localScale = Vector3.one * 1.2f;

        return card;
    }

    void AddThisCard()
    {
        if (Program.I().newDeckManager.CheckBanlistAvail(code) == false)
            return;

        bool added = false;
        GameObject card = null;
        if (data.IsExtraCard())
        {
            if(Program.I().newDeckManager.extraCount < 15)
            {
                card = CreateCard(Program.I().newDeckManager.extraCount + 100);
                Program.I().newDeckManager.extraCount++;
                added = true;
            }
            else if (Program.I().newDeckManager.sideCount < 15)
            {
                card = CreateCard(Program.I().newDeckManager.sideCount + 200);
                Program.I().newDeckManager.sideCount++;
                added = true;
            }
        }
        else
        {
            if (Program.I().newDeckManager.mainCount < 60)
            {
                card = CreateCard(Program.I().newDeckManager.mainCount);
                Program.I().newDeckManager.mainCount++;
                added = true;
            }
            else if (Program.I().newDeckManager.sideCount < 15)
            {
                card = CreateCard(Program.I().newDeckManager.sideCount + 200);
                Program.I().newDeckManager.sideCount++;
                added = true;
            }
        }
        if (added)
        {
            SEHandler.PlayInternalAudio("se_sys/SE_DECK_PLUS");
            Program.I().newDeckManager.mono.cardsOnManager.Add(card.GetComponent<CardOnManager>());
            Program.I().newDeckManager.RefreshTable();
            Program.I().newDeckManager.RefreshLabel();
        }
    }

    private async void RefreshFace()
    {
        GetComponent<UITexture>().mainTexture = await GameTextureManager.GetCardPicture(code);
    }

    public void RefreshLimitIcon()
    {
        limit_.alpha = 1.0f;
        int limit = Program.I().newDeckManager.currentBanlist.GetQuantity(code);
        if (limit == 3)
            limit_.alpha = 0;
        else if (limit == 2)
            limit_.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().limit2;
        else if (limit == 1)
            limit_.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().limit1;
        else if (limit == 0)
            limit_.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().ban;
    }

    void OnClick()
    {
        if(Program.I().newDeckManager.mono.tab_history.isShowed)
            Program.I().newDeckManager.ShowDescription(code, false);
        else
            Program.I().newDeckManager.ShowDescription(code);
    }

    public void ShowDot()
    {
        int count = Program.I().newDeckManager.CountCard(code);
        int max = Program.I().newDeckManager.currentBanlist.GetQuantity(code);
        switch (count)
        {
            case 0:
                dot_1.alpha = 0;
                dot_2.alpha = 0;
                dot_3.alpha = 0;
                break;
            case 1:
                dot_1.alpha = 1;
                dot_2.alpha = 0;
                dot_3.alpha = 0;
                dot_1.transform.localPosition = new Vector3(0, -65, 0);
                if(max == 0)
                    dot_1.color = Color.red;
                else if (max == 1)
                    dot_1.color = Color.yellow;
                else
                    dot_1.color = Color.white;
                break;
            case 2:
                dot_1.alpha = 1;
                dot_2.alpha = 1;
                dot_3.alpha = 0;
                dot_1.transform.localPosition = new Vector3(-5, -65, 0);
                dot_2.transform.localPosition = new Vector3(5, -65, 0);
                if(max < 2)
                {
                    dot_1.color = Color.red;
                    dot_2.color = Color.red;
                }
                else if(max == 2)
                {
                    dot_1.color = Color.yellow;
                    dot_2.color = Color.yellow;
                }
                else
                {
                    dot_1.color = Color.white;
                    dot_2.color = Color.white;
                }
                break;
            case 3:
                dot_1.alpha = 1;
                dot_2.alpha = 1;
                dot_3.alpha = 1;
                dot_1.transform.localPosition = new Vector3(-10, -65, 0);
                dot_2.transform.localPosition = new Vector3(0, -65, 0);
                dot_3.transform.localPosition = new Vector3(10, -65, 0);
                if(max < 3)
                {
                    dot_1.color = Color.red;
                    dot_2.color = Color.red;
                    dot_3.color = Color.red;
                }
                else
                {
                    dot_1.color = Color.yellow;
                    dot_2.color = Color.yellow;
                    dot_3.color = Color.yellow;
                }
                break;
        }
    }
}
