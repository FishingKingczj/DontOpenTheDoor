using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Door : Item
{
    [Header("Additional Varible")]
    public int type = 3;
    public bool inLocked = false;
    public bool inOpened = false;
    public int pairingValue;

    // 内部测试变量
    [Header("Test Varible")]
    public string direction;
    public Item_Door nextDoor;
    public Item_Door lastDoor;
    private const float MIN_DISTANCE = 1f;

    private Sprite openSprite;
    private Sprite closeSprite;

    private Vector3 POSITION_OPEN_UP = new Vector3(-7f / 4.28f, 0, 0);
    private Vector3 SCALE_OPEN_UP = new Vector3(4.28f, 4.28f, 1);

    private Vector3 POSITION_OPEN_DOWN = new Vector3(-6.5f / 4, 0, 1);
    private Vector3 SCALE_OPEN_DOWN = new Vector3(4, 4, 1);

    private Vector3 POSITION_OPEN_RIGHT = new Vector3(-0.45f, 0.94f, 0);
    private Vector3 SCALE_OPEN_RIGHT = new Vector3(1.15f, 1.15f, 1);

    private Vector3 POSITION_OPEN_LEFT = new Vector3(0.45f, 0.92f, 0);
    private Vector3 SCALE_OPEN_LEFT = new Vector3(1.15f, 1.15f, 1);

    void Awake()
    {
        String doorStr = "door" + type;
        if (direction.Equals("left") || direction.Equals("right"))
        {
            doorStr += direction;
        }
        openSprite = Resources.Load("Image/Scene/" + doorStr + "open", typeof(Sprite)) as Sprite;
        closeSprite = Resources.Load("Image/Scene/" + doorStr, typeof(Sprite)) as Sprite;

        id = 2;
        name = "Door";

        pickable = false;

        if (inOpened)
        {
            OpenTransform();
        }
        else
        {
            CloseTransform();
        }
        pickable = false;
    }

    private void OpenTransform()
    {
        if (!enabled)
        {
            Dialog.ShowDialog("This is locked. You need a key");
            return;
        }
        Transform renderer = gameObject.transform.Find("Renderer");
        SpriteRenderer sr = renderer.GetComponent<SpriteRenderer>();
        sr.sprite = openSprite;
        if (direction.Equals("up"))
        {
            renderer.position = gameObject.transform.position + POSITION_OPEN_UP;
            renderer.localScale = SCALE_OPEN_UP;
        }
        else if (direction.Equals("down"))
        {
            renderer.position = gameObject.transform.position + POSITION_OPEN_DOWN;
            renderer.localScale = SCALE_OPEN_DOWN;
        }
        else if (direction.Equals("left"))
        {
            renderer.position = gameObject.transform.position + POSITION_OPEN_LEFT;
            renderer.localScale = SCALE_OPEN_LEFT;
        }
        else if (direction.Equals("right"))
        {
            renderer.position = gameObject.transform.position + POSITION_OPEN_RIGHT;
            renderer.localScale = SCALE_OPEN_RIGHT;
        }
        else
        {
            Debug.LogError("Door bad direction");
        }
    }

    private void CloseTransform()
    {
        if (!enabled)
        {
            return;
        }
        Transform renderer = gameObject.transform.Find("Renderer");
        SpriteRenderer sr = renderer.GetComponent<SpriteRenderer>();
        sr.sprite = closeSprite;
        renderer.position = gameObject.transform.position;
        renderer.localScale = new Vector3(1, 1, 1);
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
    public void Open()
    {
        SetOpen(true);
    }

    public void Close()
    {

        SetOpen(false);
    }

    public void SetOpen(bool _open)
    {
        if (inOpened != _open)
        {
            if (_open)
            {
                OpenTransform();
            }
            else
            {
                CloseTransform();
            }

            gameObject.GetComponent<Collider2D>().isTrigger = _open;
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
        Debug.Log("Door Interact");
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
        transform.Find("Renderer").GetComponent<Renderer>().enabled = false;
    }
}
