using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

public class DeckOnSelect : MonoBehaviour
{
    public int id;
    public string deckName;
    public int code_1;
    public int code_2;
    public int code_3;
    public string case_;
    public string protector;

    public UIButton btn;
    public UILabel deckName_;

    public Animator animator1;
    public Animator animator2;
    public Animator animator3;

    public UI2DSprite checkOn;

    bool hover;


    async void Start()
    {
        EventDelegate.Add(btn.onClick, OnClick);
        StartRefresh();
    }

    public void StartRefresh()
    {
        try
        {
            if (refreshDeck != null)
                StopCoroutine(refreshDeck);
            refreshDeck = Refresh();
            StartCoroutine(refreshDeck);
        }
        catch { }
    }

    IEnumerator refreshDeck;

    IEnumerator Refresh()
    {
        deckName_.text = deckName;
        Sprite caseSprite = Resources.Load<Sprite>("Texture/DeckCase/DeckCase" + selectDeck.AddZero(case_) + "_L");
        if (caseSprite != null)
            UIHelper.getByName<UI2DSprite>(gameObject, "case_").sprite2D = caseSprite;

        Texture2D protector2D = Resources.Load<Texture2D>("Texture/Protector/ProtectorIcon107" + selectDeck.AddZero(protector));
        if (protector2D == null)
            protector2D = Resources.Load<Texture2D>("Texture/Protector/ProtectorIcon1070001");
        UIHelper.getByName<UITexture>(gameObject, "card_1").mainTexture = protector2D;
        UIHelper.getByName<UITexture>(gameObject, "card_2").mainTexture = protector2D;
        UIHelper.getByName<UITexture>(gameObject, "card_3").mainTexture = protector2D;

        yield return new WaitForSeconds(0.01f * (id *3));
        RefreshCard1(protector2D);
        yield return new WaitForSeconds(0.01f);
        RefreshCard2(protector2D);
        yield return new WaitForSeconds(0.01f);
        RefreshCard3(protector2D);
    }

    private async void RefreshCard1(Texture2D protector2D)
    {
        UIHelper.getByName<UITexture>(gameObject, "card_1").mainTexture = await GameTextureManager.GetCardPicture(code_1, protector2D);
    }
    private async void RefreshCard2(Texture2D protector2D)
    {
        UIHelper.getByName<UITexture>(gameObject, "card_2").mainTexture = await GameTextureManager.GetCardPicture(code_2, protector2D);
    }
    private async void RefreshCard3(Texture2D protector2D)
    {
        UIHelper.getByName<UITexture>(gameObject, "card_3").mainTexture = await GameTextureManager.GetCardPicture(code_3, protector2D);
    }

    private void Update()
    {
        if(Program.pointedCollider == btn.GetComponent<Collider>() && hover == false)
        {
            hover = true;
            if(Program.I().selectDeck.showingPickup == false)
            {
                animator1.SetBool("Hover", true);
                animator2.SetBool("Hover", true);
                animator3.SetBool("Hover", true);
            }
            SEHandler.PlayInternalAudio("se_sys/SE_DECK_CARD_SELECT", 0.5f);
        }
        else if (Program.pointedCollider != btn.GetComponent<Collider>())
        {
            hover = false;
            if (animator1.GetBool("Hover") && Program.I().selectDeck.showingPickup == false)
            {
                animator1.SetBool("Hover", false);
                animator2.SetBool("Hover", false);
                animator3.SetBool("Hover", false);
            }
        }
    }

    private void OnClick()
    {
        if(Program.I().selectDeck.deleting)
        {
            if (checkOn.gameObject.activeSelf)
            {
                checkOn.gameObject.SetActive(false);
                Program.I().selectDeck.deckToDel.Remove(deckName_.text);
            }
            else
            {
                checkOn.gameObject.SetActive(true);
                Program.I().selectDeck.deckToDel.Add(deckName_.text);
            }
        }
        else
        {
            Program.I().selectDeck.KF_editDeck(deckName_.text);
        }
    }
}
