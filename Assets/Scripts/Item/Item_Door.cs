using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Door : Item
{
    public bool locked = false;
    public bool open = false;
    public int pairingValue;

    private Item_Door connect;
    private float MIN_DISTANCE = 1f;

    void Start()
    {
        // TODO animate
        SpriteRenderer sr = gameObject.transform.GetComponent<SpriteRenderer>();
        if (open)
        {
            sr.sprite = Resources.Load("Image/Item/door_open", typeof(Sprite)) as Sprite;
        }
        else
        {
            sr.sprite = Resources.Load("Image/Item/door", typeof(Sprite)) as Sprite;
        }
        pickable = false;
    }

    public bool Lock(int _pairingValue)
    {
        if (pairingValue == _pairingValue)
        {
            // LOCK
            return true;
        }
        return false;
    }

    public bool Unlock(int _pairingValue)
    {
        if (pairingValue == _pairingValue)
        {
            Open(true);
            return true;
        }
        return false;
    }

    private void Open(bool hasKey)
    {
        if (locked)
        {
            if (hasKey)
            {
                pairingValue = -1;
            }
            else
            {
                // TODO fail need key
                Debug.Log("door locked");
                return;
            }
        }
        SetOpen(true);
    }

    private void Close(GameObject _user)
    {
        if ((transform.position - _user.transform.position).sqrMagnitude > MIN_DISTANCE * MIN_DISTANCE)
        {
            SetOpen(false);
        }
    }

    // 互动效果
    public override void Interact(GameObject _user)
    {
        Debug.Log("Interact door");
        if (GetOpen())
        {
            if (false)
            {
                // TODO monster attacking QTE
            }
            else
            {
                Close(_user);
            }
        }
        else
        {
            Open(false);
        }
    }

    public bool GetOpen()
    {
        return open;
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
        open = connect.open = _open;
    }

    public void SetLock(bool _locked)
    {
        locked = connect.locked = _locked;
    }

    public void SetConnect(Item_Door door)
    {
        connect = door;
    }
}
