using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopMoveY : MonoBehaviour
{
    public float range;
    public float time;
    float startY;

    private void Start()
    {
        startY = GetComponent<RectTransform>().anchoredPosition.y;
        MoveUp();
    }

    void MoveUp()
    {
        GetComponent<RectTransform>().DOAnchorPosY(startY + range, time).SetEase(Ease.InOutCubic).OnComplete(() => {MoveDown();});
    }
    void MoveDown()
    {
        GetComponent<RectTransform>().DOAnchorPosY(startY - range, time).SetEase(Ease.InOutCubic).OnComplete(() => {MoveUp();});
    }
}
