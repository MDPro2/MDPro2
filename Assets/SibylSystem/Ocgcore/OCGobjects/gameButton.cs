using DG.Tweening;
using Percy;
using UnityEngine;

public class gameButton : OCGobject
{
    public gameCard cookieCard;

    public string cookieString;
    public GameObject gameObjectEvent;

    public string hint;

    public bool notCookie;

    public int response;

    public superButtonType type;

    public gameHiddenButton gameHiddenButton;

    public gameButton(int response, string hint, superButtonType type)
    {
        this.response = response;

        this.hint = hint;

        this.type = type;
    }

    public void show(Vector3 v)
    {
        if (gameObject == null)
        {
            gameObject = create(Program.I().new_ui_superButton, Program.I().camera_main_2d.ScreenToWorldPoint(v),
                Vector3.zero, false, Program.I().ui_main_2d);
            gameObjectEvent = UIHelper.getRealEventGameObject(gameObject);
            UIHelper.registEvent(gameObject, clicked);
            gameObject.GetComponent<iconSetForButton>().setTexture(type);
            gameObject.GetComponent<iconSetForButton>().setText(hint);
            gameObject.transform.localScale = Vector3.zero;
            gameObject.transform.DOScale(Vector3.one * 1.5f, 0.05f);
        }

        gameObject.transform.position = Program.I().camera_main_2d.ScreenToWorldPoint(v);
    }

    public void clicked()
    {
        if(response >= 0)
            Program.I().ocgcore.ES_gameButtonClicked(this);
        else if(response == -1)//super_button
        {
            gameHiddenButton.superButonOnClick(this);
        }
        else if (response == -2)//btn_confirm
        {
            Program.I().ocgcore.sendSelectedCards();
            Program.I().ocgcore.gameField.btn_decide.hide();
            Program.I().ocgcore.gameField.btn_confirm.hide();
            Program.I().ocgcore.gameField.btn_cancel.hide();
            Program.I().cardSelection.HideWithoutAction();
        }
        else if(response == -3)//btn_cancel
        {
            if(hint == "取消选择" || hint == "取消连锁")
            {
                var binaryMaster = new BinaryMaster();
                binaryMaster.writer.Write(-1);
                Program.I().ocgcore.sendReturn(binaryMaster.get());
            }
            else if(hint == "重新选择")
            {
                for (var i = 0; i < Program.I().ocgcore.allCardsInSelectMessage.Count; i++)
                {
                    Program.I().ocgcore.allCardsInSelectMessage[i].counterSELcount = 0;
                    Program.I().ocgcore.allCardsInSelectMessage[i].show_number(Program.I().ocgcore.allCardsInSelectMessage[i].counterSELcount);
                }
            }
            else if (hint == "取消操作")
            {
                Program.I().ocgcore.cancelSelectPlace();
            }
            else if (hint == "重新输入")
            {
                Program.I().ocgcore.RMSshow_input("AnnounceCard", InterString.Get("请输入关键字："), "");
            }
            Program.I().ocgcore.gameField.btn_decide.hide();
            Program.I().ocgcore.gameField.btn_confirm.hide();
            Program.I().ocgcore.gameField.btn_cancel.hide();
            Program.I().cardSelection.HideWithoutAction();
        }
        else if(response == -4)//btn_decide
        {
            Program.I().ocgcore.ES_cardClickedAndConfirmed(cookieCard);
            hide();
        }
    }

    public void hide()
    {
        destroy(gameObject, 0.2f, true, true);
        gameObject = null;
        gameObjectEvent = null;
    }
}