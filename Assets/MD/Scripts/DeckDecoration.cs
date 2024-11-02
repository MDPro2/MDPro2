using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckDecoration : MonoBehaviour
{
    public List<AppearanceSubMenu> subMenus = new List<AppearanceSubMenu>();
    public AppearanceSubMenu case_;
    public AppearanceSubMenu protector_;
    public AppearanceSubMenu field_;
    public AppearanceSubMenu grave_;
    public AppearanceSubMenu stand_;
    public AppearanceSubMenu mate_;
    public AppearanceSubMenu pickup_;

    UIWidget mainWindow_;
    UIButton exit_;

    public bool isShowed;

    private void Awake()
    {
        mainWindow_ = transform.GetChild(0).GetComponent<UIWidget>();
        mainWindow_.alpha = 0f;
        exit_ = UIHelper.getByName<UIButton>(gameObject, "exit_");
        EventDelegate.Add(exit_.onClick, Hide);

        subMenus.Add(case_);
        subMenus.Add(protector_);
        subMenus.Add(field_);
        subMenus.Add(grave_);
        subMenus.Add(stand_);
        subMenus.Add(mate_);
        subMenus.Add(pickup_);
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(int deckCase, int protector, int field, int grave, int avatarBase, int mate)
    {
        isShowed = true;
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_03");
        DOTween.To(() => mainWindow_.alpha, x => mainWindow_.alpha = x, 1f, 0.2f);

        Program.I().newDeckManager.mono.mainWindow_.alpha = 0;
        foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
            card.Visible(false);

        case_.PreselectByCode(deckCase);
        protector_.PreselectByCode(protector);
        field_.PreselectByCode(field);
        grave_.PreselectByCode(grave);
        stand_.PreselectByCode(avatarBase);
        mate_.PreselectByCode(mate);

        foreach (var menu in subMenus)
            menu.Selectable(true);
        pickup_.Show();
        subMenus[0].Show();
    }

    public void Hide()
    {
        isShowed = false;
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_04");
        DOTween.To(() => mainWindow_.alpha, x => mainWindow_.alpha = x, 0f, 0.2f).OnComplete(() => 
        {
            gameObject.SetActive(false);
        });


        pickup_.Hide(true);
        Program.I().newDeckManager.mono.mainWindow_.alpha = 1;
        foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
            card.Visible(true);

        Program.I().newDeckManager.deck.Case[0] = case_.GetSelectedCode();
        Program.I().newDeckManager.deck.Protector[0] = protector_.GetSelectedCode();
        Program.I().newDeckManager.deck.Field[0] = field_.GetSelectedCode();
        Program.I().newDeckManager.deck.Grave[0] = grave_.GetSelectedCode();
        Program.I().newDeckManager.deck.Stand[0] = stand_.GetSelectedCode();
        Program.I().newDeckManager.deck.Mate[0] = mate_.GetSelectedCode();
        Program.I().newDeckManager.RefreshDecorationIcons();
        Program.I().newDeckManager.deckDirty = true;
    }
}
