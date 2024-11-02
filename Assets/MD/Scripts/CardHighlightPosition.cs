using Percy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHighlightPosition : MonoBehaviour
{
    public gameCard card;

    void Update()
    {
        if (card != null && card.highlightOn)
        {
            if ((card.p.position & (uint)CardPosition.FaceDown) > 0 && (card.p.location & (uint)CardLocation.Onfield) > 0)
                transform.localPosition = new Vector3(0f, 0f, -0.1f);
            else
                transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }
}
