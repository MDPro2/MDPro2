using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using YGOSharp;

public class CardOnManager : MonoBehaviour
{
    public int id;
    public int code;
    public bool mark;
    public bool isExtraCard;
    public Card data;

    Vector3 cardPosition;

    public Collider collider;
    bool hover;
    public bool clicked;
    bool pressing;
    public bool pressed;
    public bool draging;
    public bool draged;
    bool covered;
    Vector3 clickedPosition;
    bool dead;

    public UI2DSprite hoverLight_;
    public UI2DSprite limit_;
    public UI2DSprite pickup_;
    public bool pickupMode;
    public bool pickedUp;
    public bool bookMode;

    async void Start()
    {
        GetComponent<UITexture>().mainTexture =
            await GameTextureManager.GetCardPictureWithProtector(code, GameTextureManager.unknown);

        GetComponent<UITexture>().depth = id * 3;
        limit_.depth = id * 3 + 1;
        hoverLight_.depth = id * 3 +2;

        data = CardsManager.GetCard(code);
        if(data != null)
        {
            isExtraCard = data.IsExtraCard();

            RefreshLimitIcon();
        }
        GetPosition();
    }

    private void Update()
    {
        if(dead) return;

        if (Program.pointedCollider == collider)
        {
            hover = true;
            if (Program.InputGetMouseButtonDown_0)
            {
                pressed = true;
                clickedPosition = Input.mousePosition;
            }
            if (Program.InputGetMouseButton_0 && pressed && draging == false)
            {
                pressing = true;
                if ((clickedPosition - Input.mousePosition).magnitude > 5)
                {
                    draging = true;
                    draged = true;
                }
            }
            if(pressing && draging)
            {
                pressing = false;
                if(Program.I().newDeckManager.mono.tab_history.isShowed && id == 9999)
                    Program.I().newDeckManager.ShowDescription(code, false);
                else
                    Program.I().newDeckManager.ShowDescription(code);
            }

            if (Program.InputGetMouseButton_0 == false)
            {
                pressing = false;
                draging = false;
            }
            if (Program.InputGetMouseButtonUp_0)
            {
                if (pressed && draged == false)
                {
                    clicked = true;
                }
                if (Program.coveredCollider != null && draged)
                {
                    MoveCard();
                }
                if(Program.coveredCollider == null && Program.eventCollider != null && draged)
                {
                    MoveCardToZone();
                }
                if (Program.coveredCollider == null && Program.eventCollider == null && draged && id ==9999)
                {
                    DeleteThis();
                }
            }
            if (Program.InputGetMouseButtonUp_1 && pickupMode == false)
            {
                if(Program.I().newDeckManager.condition == NewDeckManager.Condition.changeSide)
                {
                    if (id < 200)
                        MoveThisTo(3);
                    else if(isExtraCard)
                        MoveThisTo(2);
                    else
                        MoveThisTo(1);
                    Program.I().newDeckManager.RefreshTable();
                    Program.I().newDeckManager.RefreshLabel();
                }
                else
                {
                    Program.I().newDeckManager.ShowDescription(code);
                    DeleteThis();
                }
            }
            if (Input.GetMouseButtonUp(2) 
                && pickupMode == false 
                && Program.I().newDeckManager.condition != NewDeckManager.Condition.changeSide
                )
            {
                Program.I().newDeckManager.AddCardByCode(code);
            }
        }
        else if (Program.coveredCollider == collider)
        {
            hover = false;
            pressed = false;
            if (Program.InputGetMouseButton_0)
                covered = true;
            else
            {
                pressing = false;
                draging = false;
                covered = false;
            }
        }
        else
        {
            hover = false;
            pressed = false;
            covered = false;
            //if ((transform.localPosition - cardPosition).magnitude > 3)
            //{
            //    draged = true;
            //}
            if (Program.InputGetMouseButton_0 == false)
            {
                pressing = false;
                draging = false;
            }
            if (id == 9999)
            {
                DeleteThis();
            }
        }

        if (draging && pickupMode == false)
        {
            transform.position = Program.I().camera_back_ground_2d.ScreenToWorldPoint(Input.mousePosition);
            transform.localScale = Vector3.one * 1.2f;
            GetComponent<UITexture>().depth = id + 1000;
            limit_.depth = id + 1001;
            hoverLight_.depth = id + 1002;
        }

        if (pressing && draging == false)
        {
            hoverLight_.color = Color.black;
            hoverLight_.alpha = 0.2f;
        }
        else if (hover && !draging || covered)
        {
            hoverLight_.color = Color.white;
            hoverLight_.alpha = 0.2f;
        }
        else
        {
            hoverLight_.color = Color.white;
            hoverLight_.alpha = 0;
        }

        if (clicked)
        {
            clicked = false;
            SEHandler.PlayInternalAudio("se_sys/SE_MENU_DECIDE", 0.7f);
            if (pickupMode)
            {
                Pickup();
            }
            else
                Program.I().newDeckManager.ShowDescription(code);
        }

        if(draging)
        {
            
        }
        if (draging == false && draged)
        {
            draged = false;
            RefreshPosition();
        }
    }

    public void RefreshLimitIcon()
    {
        if (bookMode)
            limit_.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().null_;
        else
        {
            int limit = Program.I().newDeckManager.currentBanlist.GetQuantity(code);
            if (limit == 3)
                limit_.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().null_;
            else if (limit == 2)
                limit_.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().limit2;
            else if (limit == 1)
                limit_.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().limit1;
            else if (limit == 0)
                limit_.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().ban;
        }
    }

    public void RefreshPositionInstant()
    {
        transform.localScale = Vector3.one;
        transform.localPosition = GetPosition();
        GetComponent<UITexture>().depth = id * 3;
        limit_.depth = id * 3 + 1;
        hoverLight_.depth = id * 3 + 2;
    }

    bool moving;
    public void RefreshPosition()
    {
        if (moving)
            return;
        moving = true;
        transform.DOScale(Vector3.one, 0.1f);
        transform.DOLocalMove(GetPosition(), 0.1f).OnComplete
                (() =>
                {
                    GetComponent<UITexture>().depth = id * 3;
                    limit_.depth = id * 3 + 1;
                    hoverLight_.depth = id * 3 + 2;
                    moving = false;
                });
    }

    Vector3 GetPosition()
    {
        float startX = -420;
        float endX = 264;
        float cellX = 76;
        float cellY = 110;

        if(id == 9999)
        {
            return Program.I().camera_back_ground_2d.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (id > 199)//side
        {
            int realID = id - 200;
            int sideCount = Program.I().newDeckManager.sideCount;
            if (sideCount > 10)
                cardPosition.x = startX + ((endX - startX) / (sideCount - 1)) * realID;
            else
                cardPosition.x = startX + cellX * realID;
            cardPosition.y = -405;

        }
        else if (id > 99)//extra
        {
            int realID = id - 100;
            int extraCount = Program.I().newDeckManager.extraCount;
            if (extraCount > 10)
                cardPosition.x = startX + ((endX - startX) / (extraCount - 1)) * realID;
            else
                cardPosition.x = startX + cellX * realID;
            cardPosition.y = -240;
        }
        else//main
        {
            int extraCardInX = 0;
            int mainCount = Program.I().newDeckManager.mainCount;
            if (mainCount > 40)
            {
                if ((mainCount - 40) % 4 == 0)
                    extraCardInX = (mainCount - 40) / 4;
                else
                    extraCardInX = (mainCount - 40) / 4 + 1;

                cardPosition.x = startX + ((endX - startX) / (10 + extraCardInX - 1) * (id % (10 + extraCardInX)));
                cardPosition.y = 255 - cellY * (id / (10 + extraCardInX));
            }
            else
            {
                cardPosition.x = startX + cellX * (id % 10);
                cardPosition.y = 255 - cellY * (id / 10);
            }
        }
        return cardPosition;
    }
    
    void ReturnDelete()
    {
        if (id == 9999)
            DeleteThis();
    }

    public void MoveCard()
    {
        CardOnManager targetCard = Program.coveredCollider.GetComponent<CardOnManager>();
        int bufferID = targetCard.id;

        if (CheckMovable(this, targetCard) == false)
            return;
        bool refresh = false;

        if((id < 100 && bufferID < 100) 
            || (id > 199 && id<300 && bufferID > 199) 
            || ( id > 99 && id < 200 && bufferID > 99 && bufferID < 200)
            )
        {
            foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
            {
                if (card.id > id)
                    card.id -= 1;
            }
            id = bufferID;
            foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
            {
                if (card.gameObject == gameObject) continue;
                if (card.id >= id)
                    card.id += 1;
            }
            refresh = true;
        }
        else if(id < 100 && bufferID >199)
        {
            if (Program.I().newDeckManager.sideCount >= 15 && Program.I().newDeckManager.condition == NewDeckManager.Condition.editDeck)
                return;
            else
            {
                Program.I().newDeckManager.sideCount++;
                Program.I().newDeckManager.mainCount--;
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id > id && card.id < 100)
                        card.id -= 1;
                }
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id >= bufferID)
                        card.id += 1;
                }
                id = bufferID;
                refresh = true;
            }
        }
        else if (id > 199 && id < 300 && bufferID < 99)
        {
            if (Program.I().newDeckManager.mainCount >= 60 && Program.I().newDeckManager.condition == NewDeckManager.Condition.editDeck)
                return;
            else
            {
                Program.I().newDeckManager.sideCount--;
                Program.I().newDeckManager.mainCount++;
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id > id)
                        card.id -= 1;
                }
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id >= bufferID && card.id < 100)
                        card.id += 1;
                }
                id = bufferID;
                refresh = true;
            }
        }
        else if (id > 99 && id < 200 && bufferID > 199)
        {
            if (Program.I().newDeckManager.sideCount >= 15 && Program.I().newDeckManager.condition == NewDeckManager.Condition.editDeck)
                return;
            else
            {
                Program.I().newDeckManager.sideCount++;
                Program.I().newDeckManager.extraCount--;
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id > id 
                        &&
                        card.id > 99
                        &&
                        card.id < 200                        
                        )
                        card.id -= 1;
                }
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id >= bufferID)
                        card.id += 1;
                }
                id = bufferID;
                refresh = true;
            }
        }
        else if (id >199 && id < 300 && bufferID > 99 && bufferID < 200)
        {
            if (Program.I().newDeckManager.extraCount >= 15 && Program.I().newDeckManager.condition == NewDeckManager.Condition.editDeck)
                return;
            else
            {
                Program.I().newDeckManager.sideCount--;
                Program.I().newDeckManager.extraCount++;
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id > id)
                        card.id -= 1;
                }
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id >= bufferID 
                        && 
                        card.id > 99
                        &&
                        card.id < 200
                        )
                        card.id += 1;
                }
                id = bufferID;
                refresh = true;
            }
        }
        else if(id == 9999)
        {
            if (Program.I().newDeckManager.CheckBanlistAvail(code) == false)
            {
                ReturnDelete();
                return;
            }

            if(bufferID < 100)
            {
                if (Program.I().newDeckManager.mainCount >= 60 || isExtraCard)
                {
                    if(Program.I().newDeckManager.extraCount < 15 && isExtraCard)
                    {
                        AddThisTo(2);
                        refresh = true;
                    }
                    else if (Program.I().newDeckManager.sideCount < 15)
                    {
                        AddThisTo(3);
                        refresh = true;
                    }
                    else
                    {
                        ReturnDelete();
                        return;
                    }
                }
                else
                {
                    foreach(var card in Program.I().newDeckManager.mono.cardsOnManager)
                    {
                        if(card.id >= bufferID && card.id < 100)
                            card.id++;
                    }
                    Program.I().newDeckManager.mono.cardsOnManager.Add(this);
                    Program.I().newDeckManager.mainCount++;
                    id = bufferID;
                    refresh = true;
                }
            }
            else if(bufferID > 99 && bufferID < 200)
            {
                if (Program.I().newDeckManager.extraCount >= 15 || isExtraCard == false)
                {
                    if(Program.I().newDeckManager.mainCount < 60 && isExtraCard == false)
                    {
                        AddThisTo(1);
                        refresh = true;
                    }
                    else if (Program.I().newDeckManager.sideCount < 15)
                    {
                        AddThisTo(3);
                        refresh = true;
                    }
                    else
                    {
                        ReturnDelete();
                        return;
                    }
                }
                else
                {
                    foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                    {
                        if (card.id >= bufferID && card.id > 99 && card.id < 200)
                            card.id++;
                    }
                    Program.I().newDeckManager.mono.cardsOnManager.Add(this);
                    Program.I().newDeckManager.extraCount++;
                    id = bufferID;
                    refresh = true;
                }
            }
            else if (bufferID > 199)
            {
                if (Program.I().newDeckManager.sideCount >= 15)
                {
                    if(Program.I().newDeckManager.mainCount < 60 && isExtraCard == false)
                    {
                        AddThisTo(1);
                        refresh = true;
                    }
                    else if (Program.I().newDeckManager.extraCount < 15 && isExtraCard)
                    {
                        AddThisTo(2);
                        refresh = true;
                    }
                    else
                    {
                        ReturnDelete();
                        return;
                    }
                }
                else
                {
                    foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                    {
                        if (card.id >= bufferID && card.id > 199)
                            card.id++;
                    }
                    Program.I().newDeckManager.mono.cardsOnManager.Add(this);
                    Program.I().newDeckManager.sideCount++;
                    id = bufferID;
                    refresh = true;
                }
            }
        }
        if (refresh)
        {
            Program.I().newDeckManager.RefreshTable(gameObject);
            Program.I().newDeckManager.RefreshLabel();
            Program.I().newDeckManager.deckDirty = true;
        }
    }

    private void MoveCardToZone()
    {
        GameObject eventGo = Program.eventGameObject;
        bool refresh = false;
        if (eventGo.name == "event_main")
        {
            if (id < 100)
                return;
            else if (isExtraCard || Program.I().newDeckManager.mainCount >= 60)
            {
                if (id == 9999)
                {
                    if (Program.I().newDeckManager.CheckBanlistAvail(code) == false)
                    {
                        ReturnDelete();
                        return;
                    }
                    if (Program.I().newDeckManager.extraCount < 15)
                    {
                        AddThisTo(2);
                        refresh = true;
                    }
                    else if (Program.I().newDeckManager.sideCount < 15)
                    {
                        AddThisTo(3);
                        refresh = true;
                    }
                    else
                    {
                        ReturnDelete();
                        return;
                    }
                }
                else if (Program.I().newDeckManager.condition == NewDeckManager.Condition.changeSide)
                {
                    if (id > 199 && isExtraCard == false)
                    {
                        Program.I().newDeckManager.sideCount--;
                        Program.I().newDeckManager.mainCount++;
                        foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                        {
                            if (card.id > id)
                                card.id -= 1;
                        }
                        id = Program.I().newDeckManager.mainCount - 1;
                        refresh = true;
                    }
                }
                else
                    return;
            }
            else if (id < 300)
            {
                Program.I().newDeckManager.sideCount--;
                Program.I().newDeckManager.mainCount++;
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id > id)
                        card.id -= 1;
                }
                id = Program.I().newDeckManager.mainCount - 1;
                refresh = true;
            }
            else if (id == 9999)
            {
                if (Program.I().newDeckManager.CheckBanlistAvail(code) == false)
                {
                    ReturnDelete();
                    return;
                }
                AddThisTo(1);
                refresh = true;
            }
        }
        else if (eventGo.name == "event_extra")
        {
            if ((id > 99 && id < 200))
                return;
            else if (isExtraCard == false || Program.I().newDeckManager.extraCount >= 15)
            {
                if (id == 9999)
                {
                    if (Program.I().newDeckManager.CheckBanlistAvail(code) == false)
                    {
                        ReturnDelete();
                        return;
                    }
                    if (Program.I().newDeckManager.mainCount < 60)
                    {
                        AddThisTo(1);
                        refresh = true;
                    }
                    else if (Program.I().newDeckManager.sideCount < 15)
                    {
                        AddThisTo(3);
                        refresh = true;
                    }
                    else
                    {
                        ReturnDelete();
                        return;
                    }
                }
                else if (Program.I().newDeckManager.condition == NewDeckManager.Condition.changeSide)
                {
                    if (isExtraCard && id > 199)
                    {
                        Program.I().newDeckManager.sideCount--;
                        Program.I().newDeckManager.extraCount++;
                        foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                        {
                            if (card.id > id)
                                card.id -= 1;
                        }
                        id = Program.I().newDeckManager.extraCount - 1 + 100;
                        refresh = true;
                    }
                }
                else
                    return;
            }
            else if (id < 300)
            {
                Program.I().newDeckManager.sideCount--;
                Program.I().newDeckManager.extraCount++;
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id > id)
                        card.id -= 1;
                }
                id = Program.I().newDeckManager.extraCount - 1 + 100;
                refresh = true;
            }
            else if (id == 9999)
            {
                if (Program.I().newDeckManager.CheckBanlistAvail(code) == false)
                {
                    ReturnDelete();
                    return;
                }
                AddThisTo(2);
                refresh = true;
            }
        }
        else if (eventGo.name == "event_side")
        {
            if (id > 199 && id < 300)
                return;
            else if (Program.I().newDeckManager.sideCount >= 15)
            {
                if (id == 9999)
                {
                    if (Program.I().newDeckManager.CheckBanlistAvail(code) == false)
                    {
                        ReturnDelete();
                        return;
                    }
                    if (isExtraCard && Program.I().newDeckManager.extraCount < 15)
                    {
                        AddThisTo(2);
                        refresh = true;
                    }
                    else if (isExtraCard = false && Program.I().newDeckManager.mainCount < 60)
                    {
                        AddThisTo(1);
                        refresh = true;
                    }
                    else
                    {
                        ReturnDelete();
                        return;
                    }
                }
                else if (Program.I().newDeckManager.condition == NewDeckManager.Condition.changeSide)
                {
                    if (id < 100)
                    {
                        Program.I().newDeckManager.sideCount++;
                        Program.I().newDeckManager.mainCount--;
                        foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                        {
                            if (card.id > id
                                &&
                                card.id < 100
                                )
                                card.id -= 1;
                        }
                        id = Program.I().newDeckManager.sideCount - 1 + 200;
                        refresh = true;
                    }
                    else if (id > 99 && id < 200)
                    {
                        Program.I().newDeckManager.sideCount++;
                        Program.I().newDeckManager.extraCount--;
                        foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                        {
                            if (
                                card.id > id
                                && card.id > 99
                                && card.id < 200
                                )
                                card.id -= 1;
                        }
                        id = Program.I().newDeckManager.sideCount - 1 + 200;
                        refresh = true;
                    }
                    else
                        return;
                }
            }
            else if (id < 100)
            {
                Program.I().newDeckManager.sideCount++;
                Program.I().newDeckManager.mainCount--;
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id > id
                        &&
                        card.id < 100
                        )
                        card.id -= 1;
                }
                id = Program.I().newDeckManager.sideCount - 1 + 200;
                refresh = true;
            }
            else if (id > 99 && id < 200)
            {
                Program.I().newDeckManager.sideCount++;
                Program.I().newDeckManager.extraCount--;
                foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
                {
                    if (card.id > id
                        &&
                        card.id > 99
                        &&
                        card.id < 200
                        )
                        card.id -= 1;
                }
                id = Program.I().newDeckManager.sideCount - 1 + 200;
                refresh = true;
            }
            else if (id == 9999)
            {
                if (Program.I().newDeckManager.CheckBanlistAvail(code) == false)
                {
                    ReturnDelete();
                    return;
                }
                AddThisTo(3);
                refresh = true;
            }

        }
        if (refresh)
        {
            Program.I().newDeckManager.RefreshTable(gameObject);
            Program.I().newDeckManager.RefreshLabel();
            Program.I().newDeckManager.deckDirty = true;
        }
    }

    void AddThisTo(int zone)
    {
        if (zone == 1)
        {
            id = Program.I().newDeckManager.mainCount;
            Program.I().newDeckManager.mono.cardsOnManager.Add(this);
            Program.I().newDeckManager.mainCount++;
        }
        else if (zone == 2)
        {
            id = Program.I().newDeckManager.extraCount + 100;
            Program.I().newDeckManager.mono.cardsOnManager.Add(this);
            Program.I().newDeckManager.extraCount++;
        }
        else if (zone == 3)
        {
            id = Program.I().newDeckManager.sideCount + 200;
            Program.I().newDeckManager.mono.cardsOnManager.Add(this);
            Program.I().newDeckManager.sideCount++;
        }
    }
    void MoveThisTo(int zone)
    {
        if(id < 100)
        {
            Program.I().newDeckManager.mainCount--;
            foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
            {
                if (card.id > id && card.id < 100)
                    card.id--;
            }
        }
        else if (id < 200)
        {
            Program.I().newDeckManager.extraCount--;
            foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
            {
                if (card.id > id && card.id > 99 && card.id < 200)
                    card.id--;
            }
        }
        else
        {
            Program.I().newDeckManager.sideCount--;
            foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
            {
                if (card.id > id && card.id > 199)
                    card.id--;
            }
        }

        if (zone == 1)
        {
            id = Program.I().newDeckManager.mainCount;
            Program.I().newDeckManager.mainCount++;
        }
        else if (zone == 2)
        {
            id = Program.I().newDeckManager.extraCount + 100;
            Program.I().newDeckManager.extraCount++;
        }
        else if (zone == 3)
        {
            id = Program.I().newDeckManager.sideCount + 200;
            Program.I().newDeckManager.sideCount++;
        }
    }

    private bool CheckMovable(CardOnManager card1, CardOnManager card2)
    {
        bool returnValue = false;
        int type_1 = 0;
        if (card1.id == 9999)
            type_1 = 0;
        else if (card1.id > 199)
            type_1 = 3;
        else if (card1.id > 99)
            type_1 = 2;
        else
            type_1 = 1;

        int type_2 = 0;
        if (card2.id > 199)
            type_2 = 3;
        else if (card2.id > 99)
            type_2 = 2;
        else
            type_2 = 1;

        if(type_1 == 3)
        {
            if(type_2 == 2 && card1.isExtraCard)
                returnValue = true;
            if(type_2 == 1 && card1.isExtraCard == false)
                returnValue = true;
        }

        if (type_1 == 0 || type_2 ==3)
        {
            returnValue = true;
        }
        if (type_1 == type_2)
            returnValue = true;

        return returnValue;
    }

    public void DeleteThis()
    {
        if (Program.I().newDeckManager.condition == NewDeckManager.Condition.changeSide)
            return;

        if(id == 9999)
        {
            dead = true;
            draged = false;

            GetComponent<UITexture>().depth = 1000;
            limit_.depth = 1001;
            transform.localScale = Vector3.one * 1.5f;

            Destroy(gameObject, 0.6f);
            if(Program.I().newDeckManager.mono.tab_search.isShowed)
                transform.DOLocalMove(new Vector3(625, -50, 0), 0.3f);
            else
                transform.DOLocalMove(new Vector3(420, 395, 0), 0.3f);
            transform.DOScale(Vector3.one * 1.5f, 0.3f).OnComplete(() => 
            {
                transform.DOScale(Vector3.one, 0.3f);
                DOTween.To(() => GetComponent<UITexture>().alpha, x => GetComponent<UITexture>().alpha = x, 0, 0.3f);
            });
            return;
        }
        else if (id < 100)
        {
            Program.I().newDeckManager.mainCount--;
            foreach(var card in Program.I().newDeckManager.mono.cardsOnManager)
            {
                if (card.id > id 
                    && 
                    card.id < 100
                    )
                    card.id--;
            }
        }
        else if (id > 99 && id < 200)
        {
            Program.I().newDeckManager.extraCount--;
            foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
            {
                if (card.id > id 
                    &&
                    card.id > 99
                    &&
                    card.id < 200
                    )
                    card.id--;
            }
        }
        else if (id > 199 && id < 300)
        {
            Program.I().newDeckManager.sideCount--;
            foreach (var card in Program.I().newDeckManager.mono.cardsOnManager)
            {
                if (card.id > id
                    &&
                    card.id > 199
                    &&
                    card.id < 300
                    )
                    card.id--;
            }
        }
        dead = true;
        Program.I().newDeckManager.mono.cardsOnManager.Remove(this);
        Program.I().newDeckManager.RefreshTable();
        Program.I().newDeckManager.RefreshLabel();
        Program.I().newDeckManager.deckDirty = true;

        GetComponent<UITexture>().depth = 1000;
        limit_.depth = 1001;
        SEHandler.PlayInternalAudio("se_sys/SE_DECK_MINUS", 0.8f);

        Destroy(gameObject, 0.6f);
        if (Program.I().newDeckManager.mono.tab_search.isShowed)
            transform.DOLocalMove(new Vector3(625, -50, 0), 0.3f);
        else
            transform.DOLocalMove(new Vector3(420, 395, 0), 0.3f);
        transform.DOScale(Vector3.one * 1.5f, 0.3f).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.3f);
            DOTween.To(() => GetComponent<UITexture>().alpha, x => GetComponent<UITexture>().alpha = x, 0, 0.3f);
        });
    }

    public void Visible(bool visible)
    {
        if (visible)
            GetComponent<UITexture>().alpha = 1;
        else
            GetComponent<UITexture>().alpha = 0;
    }

    public void EnterPickupMode()
    {
        pickupMode = true;
        pickedUp = false;
        limit_.alpha = 0;
        GetComponent<UIDragObject>().enabled = false;
        pickup_.depth = id * 3 + 1;
    }

    public void ExitPickupMode()
    {
        DOTween.To(() => pickup_.alpha, x => pickup_.alpha =x, 0, 0.3f).OnComplete(() => 
        {
            pickupMode = false;
            GetComponent<UIDragObject>().enabled = true;
        });
        DOTween.To(() => limit_.alpha, x => limit_.alpha = x, 1, 0.3f);
    }
    public void ExitPickupModeInstant()
    {
        pickupMode = false;
        pickup_.alpha = 0;
        limit_.alpha = 1;
    }
    public void Pickup()
    {
        if (pickedUp)
        {
            pickedUp = false;
            pickup_.alpha = 0;
            Program.I().newDeckManager.pickupCount--;
        }
        else if(Program.I().newDeckManager.pickupCount <3)
        {
            pickedUp=true;
            pickup_.alpha = 1;
            Program.I().newDeckManager.pickupCount++;
        }
    }
}
