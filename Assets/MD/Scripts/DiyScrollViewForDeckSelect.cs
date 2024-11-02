using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DiyScrollViewForDeckSelect : DiyScrollView
{
    public DiyScrollViewForDeckSelect
    (
        UIPanel panel,
        UIScrollBar scrollBar,
        Func<string[], GameObject> itemOnListProducer,
        float heightOfEach,
        int numOfEachLine,
        int startX,
        int startY,
        int cellX,
        int cellY,
        float dummyDown
    ) : base (panel, scrollBar, itemOnListProducer, heightOfEach, numOfEachLine, startX, startY, cellX, cellY, dummyDown)
    {
    }

    public override void OnScrollBarChange(bool forced = false)
    {
        if (printed == false || items.Count == 0)
            return;
        double fullY = cellY * Math.Ceiling((float)items.Count / numOfEachLine) + dummyDown - panel.GetViewSize().y;
        if (fullY <= 0)
            return;
        double move = fullY * scrollBar.value;
        if (forced)
            move = 0;
        int outOfRangeLines = (int)Math.Floor(move / cellY);

        if (hideLines == outOfRangeLines)
            return;
        List<GameObject> cards = new List<GameObject>();
        foreach (var item in items)
        {
            if (item.gameObject != null)
                cards.Add(item.gameObject);
        }
        int showingLines = cards.Count % numOfEachLine == 0 ? cards.Count / numOfEachLine : cards.Count / numOfEachLine + 1;
        if (outOfRangeLines - hideLines == 1)
        {
            for (int i = 0; i < numOfEachLine; i++)
            {
                if (items[i].gameObject != null)
                {
                    items[i].gameObject.SetActive(true);
                    items[i].gameObject.transform.localPosition = new Vector3
                    (
                        items[i].gameObject.transform.localPosition.x,
                        items[cards.Count - numOfEachLine].gameObject.transform.localPosition.y - cellY,
                        0
                    );

                    if (i + (outOfRangeLines + showingLines - 1) * numOfEachLine >= items.Count)
                        items[i].gameObject.SetActive(false);
                    else
                    {
                        if (PickupCard.showing)
                        {
                            foreach (var animator in items[i].gameObject.GetComponentsInChildren<Animator>())
                                animator.SetBool("Hover", true);
                        }
                        items[i].gameObject.GetComponent<DeckOnSelect>().id = i;
                        items[i].gameObject.GetComponent<DeckOnSelect>().deckName = items[i + (outOfRangeLines + showingLines - 1) * numOfEachLine].Args[1];
                        items[i].gameObject.GetComponent<DeckOnSelect>().code_1 = int.Parse(items[i + (outOfRangeLines + showingLines - 1) * numOfEachLine].Args[2]);
                        items[i].gameObject.GetComponent<DeckOnSelect>().code_2 = int.Parse(items[i + (outOfRangeLines + showingLines - 1) * numOfEachLine].Args[3]);
                        items[i].gameObject.GetComponent<DeckOnSelect>().code_3 = int.Parse(items[i + (outOfRangeLines + showingLines - 1) * numOfEachLine].Args[4]);
                        items[i].gameObject.GetComponent<DeckOnSelect>().case_ = items[i + (outOfRangeLines + showingLines - 1) * numOfEachLine].Args[5];
                        items[i].gameObject.GetComponent<DeckOnSelect>().protector = items[i + (outOfRangeLines + showingLines - 1) * numOfEachLine].Args[6];
                        items[i].gameObject.GetComponent<DeckOnSelect>().StartRefresh();
                    }
                }
            }
            for (int i = 0; i < cards.Count; i++)
            {
                if (i < cards.Count - numOfEachLine)
                    items[i].gameObject = cards[i + numOfEachLine].gameObject;
                else
                    items[i].gameObject = cards[i + numOfEachLine - cards.Count].gameObject;
            }
        }
        else if (hideLines - outOfRangeLines == 1)
        {
            for (int i = cards.Count - numOfEachLine; i < cards.Count; i++)
            {
                items[i].gameObject.SetActive(true);
                if (PickupCard.showing)
                {
                    foreach (var animator in items[i].gameObject.GetComponentsInChildren<Animator>())
                        animator.SetBool("Hover", true);
                }
                items[i].gameObject.transform.localPosition = new Vector3
                (
                    items[i].gameObject.transform.localPosition.x,
                    items[0].gameObject.transform.localPosition.y + cellY,
                    0
                );
                items[i].gameObject.GetComponent<DeckOnSelect>().id = i - (cards.Count - numOfEachLine);
                items[i].gameObject.GetComponent<DeckOnSelect>().deckName = items[(i + numOfEachLine - cards.Count) + outOfRangeLines * numOfEachLine].Args[1];
                items[i].gameObject.GetComponent<DeckOnSelect>().code_1 = int.Parse(items[(i + numOfEachLine - cards.Count) + outOfRangeLines * numOfEachLine].Args[2]);
                items[i].gameObject.GetComponent<DeckOnSelect>().code_2 = int.Parse(items[(i + numOfEachLine - cards.Count) + outOfRangeLines * numOfEachLine].Args[3]);
                items[i].gameObject.GetComponent<DeckOnSelect>().code_3 = int.Parse(items[(i + numOfEachLine - cards.Count) + outOfRangeLines * numOfEachLine].Args[4]);
                items[i].gameObject.GetComponent<DeckOnSelect>().case_ = items[(i + numOfEachLine - cards.Count) + outOfRangeLines * numOfEachLine].Args[5];
                items[i].gameObject.GetComponent<DeckOnSelect>().protector = items[(i + numOfEachLine - cards.Count) + outOfRangeLines * numOfEachLine].Args[6];
                items[i].gameObject.GetComponent<DeckOnSelect>().StartRefresh();
            }
            for (int i = 0; i < cards.Count; i++)
            {
                if (i < numOfEachLine)
                    items[i].gameObject = cards[i + (cards.Count - numOfEachLine)].gameObject;
                else
                    items[i].gameObject = cards[i - numOfEachLine].gameObject;
            }
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].gameObject != null)
                {
                    items[i].gameObject.transform.localPosition = new Vector3
                    (
                        items[i].gameObject.transform.localPosition.x,
                        startY - ((i + outOfRangeLines * numOfEachLine) / numOfEachLine) * cellY,
                        0
                    );
                    if (i + outOfRangeLines * numOfEachLine >= items.Count)
                        items[i].gameObject.SetActive(false);
                    else
                    {
                        items[i].gameObject.SetActive(true);
                        if (PickupCard.showing)
                        {
                            foreach (var animator in items[i].gameObject.GetComponentsInChildren<Animator>())
                                animator.SetBool("Hover", true);
                        }
                        items[i].gameObject.GetComponent<DeckOnSelect>().id = i;
                        items[i].gameObject.GetComponent<DeckOnSelect>().deckName = items[i + outOfRangeLines * numOfEachLine].Args[1];
                        items[i].gameObject.GetComponent<DeckOnSelect>().code_1 = int.Parse(items[i + outOfRangeLines * numOfEachLine].Args[2]);
                        items[i].gameObject.GetComponent<DeckOnSelect>().code_2 = int.Parse(items[i + outOfRangeLines * numOfEachLine].Args[3]);
                        items[i].gameObject.GetComponent<DeckOnSelect>().code_3 = int.Parse(items[i + outOfRangeLines * numOfEachLine].Args[4]);
                        items[i].gameObject.GetComponent<DeckOnSelect>().case_ = items[i + outOfRangeLines * numOfEachLine].Args[5];
                        items[i].gameObject.GetComponent<DeckOnSelect>().protector = items[i + outOfRangeLines * numOfEachLine].Args[6];
                        items[i].gameObject.GetComponent<DeckOnSelect>().StartRefresh();
                    }
                }
            }
        }
        hideLines = outOfRangeLines;
    }
}
