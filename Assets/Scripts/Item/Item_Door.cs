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

    // 内部测试变量
    [Header("Test Varible")]
    public string direction;
    public Item_Door nextDoor;
    public Item_Door lastDoor;
    private const float MIN_DISTANCE = 1f;

    void Awake()
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
        SetLocked(true);
    }

    public void Unlock()
    {
        SetLocked(false);
    }

    private void SetLocked(bool _lock)
    {
        if (inLocked != _lock)
        {
            inLocked = _lock;
            nextDoor.SetLocked(_lock);
            if (lastDoor)
                lastDoor.SetLocked(_lock);
        }
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
        if (inOpened != _open)
        {
            SpriteRenderer sr = gameObject.transform.GetComponent<SpriteRenderer>();
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

            gameObject.GetComponent<BoxCollider2D>().isTrigger = _open;
            inOpened = _open;
            nextDoor.SetOpen(_open);
            if (lastDoor)
                lastDoor.SetOpen(_open);
        }
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
                    // 和门之间的距离
                    else
                    {
                        Debug.Log((transform.position - _user.transform.position).sqrMagnitude);
                        if ((transform.position - _user.transform.position).sqrMagnitude > MIN_DISTANCE * MIN_DISTANCE)
                            Close();
                    }
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
        nextDoor = door;
        door.lastDoor = this;
    }

    public void Disable()
    {
        enabled = false;
        GetComponent<Renderer>().enabled = false;
    }
}
