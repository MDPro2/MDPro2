using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhaseButtonHandler : MonoBehaviour
{
    Transform commonPart;
    Transform playerPart;
    Transform opponentPart;
    TextMeshPro textMain;
    TextMeshPro textBelow;
    TextMeshPro textAbove;

    Material playerMaterial;
    Material opponentMaterial;
    Collider collider_;
    bool hover;
    float mouseOver;
    int turns = -1;

    private void Start()
    {
        commonPart = transform.Find("CommonPart");

        textMain = commonPart.Find("TextMain").gameObject.AddComponent<TextMeshPro>();
        textMain.text = "Main1";
        textMain.fontSize = 16f;
        textMain.alignment = TextAlignmentOptions.Center;
        textMain.GetComponent<TextContainer>().height = 4;
        textMain.color = new Color(1, 1, 1, 0.75f);
        textMain.outlineColor = new Color(1, 1, 1, 0);

        textAbove = commonPart.Find("TextAbove").gameObject.AddComponent<TextMeshPro>();
        textAbove.text = "Turn1";
        textAbove.fontSize = 12f;
        textAbove.alignment = TextAlignmentOptions.Top;
        textAbove.GetComponent<TextContainer>().height = 4;
        textAbove.color = new Color(1, 1, 1, 0.75f);
        textAbove.outlineColor = new Color(1, 1, 1, 0);

        textBelow = commonPart.Find("TextBelow").gameObject.AddComponent<TextMeshPro>();
        textBelow.text = "01";
        textBelow.fontSize = 10f;
        textBelow.alignment = TextAlignmentOptions.Bottom;
        textBelow.GetComponent<TextContainer>().height = 4;
        textBelow.color = new Color(1, 1, 1, 0.75f);
        textBelow.outlineColor = new Color(1, 1, 1, 0);

        playerPart = transform.Find("PlayerPart");
        opponentPart = transform.Find("OpponentPart");
        playerMaterial = playerPart.GetComponent<Renderer>().material;
        opponentMaterial = opponentPart.GetComponent<Renderer>().material;
        opponentMaterial.SetFloat("_SwitchTurn", 1);
        collider_ = commonPart.GetComponent<Collider>();
    }


    private void Update()
    {
        if (PhaseUIBehaviour.clickable)
        {
            playerMaterial.SetFloat("_Active", 1);
            //Click
            if (Program.pointedCollider == collider_ && Input.GetMouseButtonUp(0))
            {
                PhaseUIBehaviour.show();
                SEHandler.PlayInternalAudio("se_sys/SE_MENU_DECIDE");
            }
            if (Program.pointedCollider == collider_ && Input.GetMouseButton(0))
                playerMaterial.SetFloat("_PressButton", 1);
            else
                playerMaterial.SetFloat("_PressButton", 0);
            //MouseOver
            if (Program.pointedCollider == collider_ && !hover)
            {
                hover = true;
                DOTween.To(() => mouseOver, x => mouseOver = x, 1, 0.2f);
            }
            else if (Program.pointedCollider != collider_ && hover)
            {
                hover = false;
                DOTween.To(() => mouseOver, x => mouseOver = x, 0, 0.2f);
            }
            playerMaterial.SetFloat("_MouseOver", mouseOver);
        }
        else
        {
            playerMaterial.SetFloat("_Active", 0);
        }
        //TurnChange
        if(turns != Program.I().ocgcore.turns)
        {
            turns = Program.I().ocgcore.turns;
            commonPart.localScale = Vector3.zero;
            commonPart.DOScale(Vector3.one, 0.3f);
            if (Program.I().ocgcore.myTurn)
            {
                playerPart.DOScale(Vector3.one, 0.3f);
                opponentPart.DOScale(Vector3.one * 0.1f, 0.3f);
            }
            else
            {
                playerPart.DOScale(Vector3.one * 0.1f, 0.3f);
                opponentPart.DOScale(Vector3.one, 0.3f);
            }
        }
        //Text
        textMain.text = Program.I().ocgcore.MD_phaseString;
        textAbove.text = "Turn" + Program.I().ocgcore.turns;
        textBelow.text = Program.I().ocgcore.MD_battleString;
    }
}
