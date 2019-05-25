using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Door : Item
{
    [Header("Additional Varible")]
    public bool inLocked = false;
    public bool inOpened = false;
    public int pairingValue;

    private Item_Door connect;
    private const float MIN_DISTANCE = 1f;

    void Start()
    {
        // TODO animate
        SpriteRenderer sr = gameObject.transform.GetComponent<SpriteRenderer>();
        if (inOpened)
        {
            sr.sprite = Resources.Load("Image/Item/door_open", typeof(Sprite)) as Sprite;
        }
        else
        {
            sr.sprite = Resources.Load("Image/Item/door", typeof(Sprite)) as Sprite;
        }
        pickable = false;
    }

    // 锁门
    public bool GetLock()
    {
        return inLocked;
    }

    public void Lock()
    {
        inLocked = connect.inLocked = true;
    }

    public void Unlock()
    {
        inLocked = connect.inLocked = false;
    }

    // 钥匙匹配
    public bool Pair(int _pairingValue)
    {
        if (pairingValue == _pairingValue)
        {
            return true;
        }
        return false;
    }

    // 开关门
    private void Open()
    {
        SetOpen(true);
    }

    private void Close()
    {

        SetOpen(false);
    }

    public void SetOpen(bool _open)
    {
        SpriteRenderer sr = gameObject.transform.GetComponent<SpriteRenderer>();
        SpriteRenderer cnsr = connect.gameObject.transform.GetComponent<SpriteRenderer>();
        Sprite sprite;
        if (_open)
        {
            // TODO animation
            sprite = Resources.Load("Image/Item/door_open", typeof(Sprite)) as Sprite;
        }
        else
        {
            // TODO animation
            sprite = Resources.Load("Image/Item/door", typeof(Sprite)) as Sprite;
        }
        sr.sprite = sprite;
        cnsr.sprite = sprite;

        gameObject.GetComponent<BoxCollider2D>().isTrigger = _open;
        connect.GetComponent<BoxCollider2D>().isTrigger = _open;
        inOpened = connect.inOpened = _open;
    }

    public bool GetOpen()
    {
        return inOpened;
    }

    // 互动效果
    public override void Interact(GameObject _user)
    {
        if (pickable == false)
        {
            if (GetLock())
            {
                Debug.Log("这门锁上了");
            }
            else
            {
                if (GetOpen())
                {
                    if (false)
                    {
                        // TODO monster attacking QTE
                    }
                    else if ((transform.position - _user.transform.position).sqrMagnitude > MIN_DISTANCE * MIN_DISTANCE)
                        Close();
                }
                else
                {
                    Open();
                }
            }
        }
    }

    public void SetConnect(Item_Door door)
    {
        connect = door;
    }
}
