using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AppearanceSubMenu : MonoBehaviour
{
    public UILabel title;
    public UIWidget content;
    UI2DSprite menuBase;
    Collider collider;


    public bool selected;
    bool selectable;
    bool hover;
    void Awake()
    {
        try
        {
            collider = GetComponent<BoxCollider>();
            menuBase = GetComponent<UI2DSprite>();
            title = transform.GetChild(0).GetComponent<UILabel>();
            content = transform.GetChild(1).GetComponent<UIWidget>();
            content.alpha = 0f;
        }
        catch { }
    }

    private void Update()
    {
        if(Program.pointedCollider == collider)
        {
            if (!hover && !selected && selectable)
            {
                hover = true;
                menuBase.sprite2D = Program.I().appearance.hover;
                menuBase.border = new Vector4(15, 15, 15, 15);
            }

            if (Program.InputGetMouseButtonUp_0 && selectable)
            {
                if(!selected)
                {
                    Show(true);
                }
                SEHandler.PlayInternalAudio("se_sys/SE_MENU_SELECT_01");
            }
        }
        else
        {
            if (hover)
            {
                hover = false;
                if (!selected)
                {
                    menuBase.sprite2D = Program.I().appearance.normal;
                    menuBase.border = new Vector4(7, 5, 3, 5);
                }
            }
        }
    }

    public void Show(bool save = false)
    {
        selected = true;
        collider.enabled = true;

        menuBase.sprite2D = Program.I().appearance.selected;
        menuBase.border = new Vector4(15, 15, 15, 15);

        title.color = Color.black;
        if (gameObject.name != "Pickup")
            content.alpha = 1f;
        else
        {
            Program.I().newDeckManager.mono.mainWindow_.alpha = 1f;
            foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
            {
                card.Visible(true);
                card.EnterPickupMode();
            }
            Program.I().newDeckManager.mono.top_.SetActive(false);
            Program.I().newDeckManager.mono.left_.SetActive(false);
            Program.I().newDeckManager.mono.right_.SetActive(false);

            Program.I().newDeckManager.pickupCount = 0;
            foreach (var pickupCode in Program.I().newDeckManager.deck.Pickup)
            {
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if(card.code == pickupCode && card.pickedUp == false)
                    {
                        card.Pickup();
                        break;
                    }
                }
            }
        }

        foreach(var menu in Program.I().appearance.subMenus)
            if(menu.gameObject != gameObject)
            {
                menu.Hide();
            }
        foreach (var menu in Program.I().deckDecoration.subMenus)
            if (menu.gameObject != gameObject)
            {
                menu.Hide(save);
            }
    }

    public void Preselect(string itemName)
    {
        foreach(var item in GetComponentsInChildren<AppearanceItem>())
            if(item.labelName.text == itemName)
            {
                item.OnSelect();
                break;
            }
    }

    public void PreselectByCode(int code)
    {
        foreach(var item in GetComponentsInChildren<AppearanceItem>())
            if(item.name == code.ToString())
            {
                item.OnSelect();
                break;
            }
    }

    public void Selectable(bool selectable)
    {
        this.selectable = selectable;
        if (selectable)
        {
            title.color = Color.white;
        }
        else
        {
            title.color = Color.gray;
        }
    }

    public void Hide(bool save = false)
    {
        selected = false;

        menuBase.sprite2D = Program.I().appearance.normal;
        menuBase.border = new Vector4(7, 5, 3, 5);
        if(selectable)
            title.color = Color.white;
        else
            title.color = Color.gray;
        if(gameObject.name != "Pickup")
            content.alpha = 0f;
        else
        {
            Program.I().newDeckManager.mono.mainWindow_.alpha = 0f;

            if(save)
            {
                Program.I().newDeckManager.deck.Pickup.Clear();
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.pickedUp)
                        Program.I().newDeckManager.deck.Pickup.Add(card.code);
                    card.Visible(false);
                    if(Program.I().deckDecoration.isShowed)
                        card.ExitPickupModeInstant();
                    else
                        card.ExitPickupMode();
                }
            }
            else
            {
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    card.Visible(false);
                    card.ExitPickupMode();
                }
            }
            Program.I().newDeckManager.mono.top_.SetActive(true);
            Program.I().newDeckManager.mono.left_.SetActive(true);
            Program.I().newDeckManager.mono.right_.SetActive(true);
        }
    }

    public void Save()
    {
        bool saved = false;
        foreach (var item in GetComponentsInChildren<AppearanceItem>())
            if (item.selected)
            {
                Config.Set(name + Program.I().appearance.player, item.labelName.text);
                saved = true;
            }
        if(!saved)
            Config.Set(name + Program.I().appearance.player, transform.GetChild(2).name);
    }
    public int GetSelectedCode()
    {
        int returnValue = 0;
        foreach (var item in GetComponentsInChildren<AppearanceItem>())
            if (item.selected)
            {
                returnValue = int.Parse(item.name);
            }
        return returnValue;
    }
    public void ItemDeck(bool show)
    {
        UI2DSprite itemDeck = UIHelper.getByName<UI2DSprite>(gameObject, "itemDeck");
        if(itemDeck != null)
        {
            if (show)
                itemDeck.alpha = 1;
            else
                itemDeck.alpha = 0;
        }
    }
}
