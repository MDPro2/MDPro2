using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DiyScrollView
{
    public float heightOfEach;
    public int numOfEachLine;
    public int maxCreateNumInOnePanel;

    public int startX;
    public int startY;
    public int cellX;
    public int cellY;

    public Func<string[], GameObject> itemOnListProducer;

    public bool printed;
    public int hideLines;
    public float dummyDown;
    public class Item
    {
        public string[] Args;
        public GameObject gameObject;
    }
    public List<Item> items = new List<Item>();
    public List<GameObject> dummys = new List<GameObject>();

    public UIPanel panel;
    public UIScrollBar scrollBar;
    public UIScrollView uIScrollView;

    public DiyScrollView
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
    )
    {
        this.panel = panel;
        this.scrollBar = scrollBar;
        this.itemOnListProducer = itemOnListProducer;
        this.heightOfEach = heightOfEach;
        this.numOfEachLine = numOfEachLine;
        this.startX = startX;
        this.startY = startY;
        this.cellX = cellX;
        this.cellY = cellY;
        this.dummyDown = dummyDown;
        Install();
    }

    public void Install()
    {
        uIScrollView = panel.gameObject.AddComponent<UIScrollView>();
        uIScrollView.can_be_draged = false;
        uIScrollView.disableDragIfFits = true;
        uIScrollView.movement = UIScrollView.Movement.Vertical;
        uIScrollView.contentPivot = UIWidget.Pivot.TopLeft;
        uIScrollView.dragEffect = UIScrollView.DragEffect.Momentum;
        uIScrollView.verticalScrollBar = scrollBar;
        UIHelper.registEvent(scrollBar, () => OnScrollBarChange(false));
        scrollBar.value = 0;
    }

    public virtual void OnScrollBarChange(bool forced = false)
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
        List<GameObject> cards = new List<GameObject>(); ;
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
                        //startY - ((i + outOfRangeLines * numOfEachLine) / numOfEachLine) * cellY - panel.GetViewSize().y,
                        items[cards.Count - numOfEachLine].gameObject.transform.localPosition.y - cellY,
                        0
                    );
                    
                    if (i + (outOfRangeLines + showingLines - 1) * numOfEachLine >= items.Count)
                        items[i].gameObject.SetActive(false);
                    else
                    {
                        items[i].gameObject.GetComponent<CardOnBook>().id = i;
                        items[i].gameObject.GetComponent<CardOnBook>().code = int.Parse(items[i + (outOfRangeLines + showingLines - 1) * numOfEachLine].Args[1]);
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
                items[i].gameObject.transform.localPosition = new Vector3
                (
                    items[i].gameObject.transform.localPosition.x,
                    items[0].gameObject.transform.localPosition.y + cellY,
                    0
                );
                items[i].gameObject.GetComponent<CardOnBook>().id = i - (cards.Count - numOfEachLine);
                items[i].gameObject.GetComponent<CardOnBook>().code = int.Parse(items[(i + numOfEachLine - cards.Count) + outOfRangeLines * numOfEachLine].Args[1]);
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
                        items[i].gameObject.GetComponent<CardOnBook>().id = i;
                        items[i].gameObject.GetComponent<CardOnBook>().code = int.Parse(items[i + outOfRangeLines * numOfEachLine].Args[1]);
                    }
                }
            }
        }
        hideLines = outOfRangeLines;
    }

    public void Clear()
    {
        panel.transform.DestroyChildren();
        items.Clear();
        DestroyDummys();
        hideLines = 0;
        scrollBar.value = 0;
    }

    public void ToTop()
    {
        DOTween.To(() => scrollBar.value, x => scrollBar.value = x, 0, 0.2f);
    }

    public void Print(List<string[]> tasks)
    {
        Clear();
        for (int i = 0; i < tasks.Count; i++)
        {
            var it = new Item();
            it.Args = tasks[i];
            it.gameObject = null;
            items.Add(it);
        }
        CreateDummys();

        float heightOfPanel = panel.GetViewSize().y;
        int maxShowLines = (int)(heightOfPanel / cellY) + 1;
        maxCreateNumInOnePanel = maxShowLines * numOfEachLine;

        int max = maxCreateNumInOnePanel > tasks.Count ? tasks.Count : maxCreateNumInOnePanel;
        for (int i = 0; i < max; i++)
        {
            CreateItem(i);
        }
        ToTop();
    }

    private void CreateItem(int i)
    {
        if (items[i].gameObject == null)
        {
            items[i].gameObject = itemOnListProducer(items[i].Args);
            items[i].gameObject.transform.SetParent(panel.gameObject.transform, false);
            items[i].gameObject.transform.localPosition = new Vector3(startX + i % numOfEachLine * cellX, startY - (i / numOfEachLine) * cellY, 0);
            var boxCollider = items[i].gameObject.transform.GetComponentInChildren<BoxCollider>();
            if (boxCollider != null) boxCollider.gameObject.AddComponent<UIDragScrollView>().scrollView = uIScrollView;
        }
    }

    public void CreateDummys()
    {
        GameObject head = new GameObject();
        head.layer = panel.gameObject.layer;
        head.transform.SetParent(panel.transform);
        head.transform.localScale = Vector3.one;
        head.AddComponent<UIWidget>().height = cellY;
        head.GetComponent<UIWidget>().width = cellX;
        head.transform.localPosition = new Vector3(startX + 0 % numOfEachLine * cellX, startY - (0 / numOfEachLine) * cellY, 0);
        dummys.Add(head);

        GameObject tail = new GameObject();
        tail.layer = panel.gameObject.layer;
        tail.transform.SetParent(panel.transform);
        tail.transform.localScale = Vector3.one;
        tail.AddComponent<UIWidget>().height = cellY;
        tail.GetComponent<UIWidget>().width = cellX;
        tail.transform.localPosition = new Vector3(startX + (items.Count - 1) % numOfEachLine * cellX, startY - ((items.Count - 1) / numOfEachLine) * cellY - dummyDown, 0);
        dummys.Add(tail);

        printed = true;
    }
    private void DestroyDummys()
    {
        foreach (GameObject go in dummys)
            UnityEngine.Object.Destroy(go);
        dummys.Clear();
        printed = false;
    }
}
