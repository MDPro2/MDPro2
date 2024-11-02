using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainButtonHandler : MonoBehaviour
{
    gameInfo gameInfo;

    private UIButton btn;
    public Sprite duelButtonQE_0_auto_1;
    public Sprite duelButtonQE_0_auto_2;
    public Sprite duelButtonQE_0_auto_3;
    public Sprite duelButtonQE_0_auto_4;
    public Sprite duelButtonQE_0_on_1;
    public Sprite duelButtonQE_0_on_2;
    public Sprite duelButtonQE_0_on_3;
    public Sprite duelButtonQE_0_on_4;
    public Sprite duelButtonQE_0_off_1;
    public Sprite duelButtonQE_0_off_2;
    public Sprite duelButtonQE_0_off_3;
    public Sprite duelButtonQE_0_off_4;
    Sprite[] sprites = new Sprite[4];
    public int chainType = 0;
    private void Start()
    {
        gameInfo = Program.I().new_ui_gameInfo;

        btn = GetComponent<UIButton>();
        EventDelegate.Add(btn.onClick, OnBtnClick);
    }
    private void Update()
    {
        int i = 0;
        if (gameInfo.get_condition() == gameInfo.chainCondition.standard)
            i = 0;
        else if (gameInfo.get_condition() == gameInfo.chainCondition.smart)
            i = 1;
        else if (gameInfo.get_condition() == gameInfo.chainCondition.no)
            i = 2;
        if (chainType != i)
        {
            chainType = i;
            ChangeSprites(i);
        }
    }
    void OnBtnClick()
    {
        chainType = (chainType +1) % 3;
        ChangeSprites(chainType);
        ChangeChainCondition(chainType);
    }

    void ChangeSprites(int i)
    {
        switch (i)
        {
            case 0:
                sprites[0] = duelButtonQE_0_auto_1;
                sprites[1] = duelButtonQE_0_auto_2;
                sprites[2] = duelButtonQE_0_auto_3;
                sprites[3] = duelButtonQE_0_auto_4;
                break;
            case 1:
                sprites[0] = duelButtonQE_0_on_1;
                sprites[1] = duelButtonQE_0_on_2;
                sprites[2] = duelButtonQE_0_on_3;
                sprites[3] = duelButtonQE_0_on_4;
                break;
            case 2:
                sprites[0] = duelButtonQE_0_off_1;
                sprites[1] = duelButtonQE_0_off_2;
                sprites[2] = duelButtonQE_0_off_3;
                sprites[3] = duelButtonQE_0_off_4;
                break;
        }
        btn.normalSprite2D = sprites[0];
        btn.hoverSprite2D = sprites[1];
        btn.pressedSprite2D = sprites[2];
        btn.disabledSprite2D = sprites[3];
        btn.gameObject.SetActive(false);
        btn.gameObject.SetActive(true);
    }
    void ChangeChainCondition(int i)
    {
        switch (i)
        {
            case 0:
                gameInfo.set_condition(gameInfo.chainCondition.standard);
                break;
            case 1:
                gameInfo.set_condition(gameInfo.chainCondition.smart);
                break;
            case 2:
                gameInfo.set_condition(gameInfo.chainCondition.no);
                break;
        }
    }
}
