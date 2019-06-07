using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Room up;
    public Room down;
    public Room left;
    public Room right;

    private Item_Door doorUp;
    private Item_Door doorDown;
    private Item_Door doorLeft;
    private Item_Door doorRight;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Contains("Player"))
        {
            RoomLoader loader = GameObject.Find("RoomLoader").GetComponent<RoomLoader>();
            loader.Enter(this, null);
        }
    }

    // 检查房间的连接，关闭不存在的门
    public void CheckRoom()
    {
        // 初始化门
        doorUp = transform.Find("DoorUp").GetComponent<Item_Door>();
        doorDown = transform.Find("DoorDown").GetComponent<Item_Door>();
        doorLeft = transform.Find("DoorLeft").GetComponent<Item_Door>();
        doorRight = transform.Find("DoorRight").GetComponent<Item_Door>();

        if (!up)
        {
            doorUp.Disable();
        }
        if (!down)
        {
            doorDown.Disable();
        }
        if (!left)
        {
            doorLeft.Disable();
        }
        if (!right)
        {
            doorRight.Disable();
        }
    }

    public void CheckDoor()
    {
        if (up)
        {
            Item_Door upConnect = up.transform.Find("DoorDown").GetComponent<Item_Door>();
            LinkDoor(doorUp, upConnect);
        }

        if (down)
        {
            Item_Door downConnect = down.transform.Find("DoorUp").GetComponent<Item_Door>();
            LinkDoor(doorDown, downConnect);
        }

        if (left)
        {
            Item_Door leftConnect = left.transform.Find("DoorRight").GetComponent<Item_Door>();
            LinkDoor(doorLeft, leftConnect);
        }

        if (right)
        {
            Item_Door rightConnect = right.transform.Find("DoorLeft").GetComponent<Item_Door>();
            LinkDoor(doorRight, rightConnect);
        }
    }

    public void LinkDoor(Item_Door door1, Item_Door door2)
    {
        Debug.Log(door1.direction);
        if (!door2.enabled)
        {
            Debug.LogError("房间" + name + "门" + door1.direction + "对面没有门！");
        }
        else
        {
            if (door1.GetLock() != door2.GetLock())
            {
                Debug.LogError("房间" + name + "门" + door1.direction + "锁设置不一致");
            }
            if (door1.GetOpen() != door2.GetOpen())
            {
                Debug.LogError("房间" + name + "门" + door1.direction + "开关设置不一致");
            }
            if (door1.pairingValue != door2.pairingValue)
            {
                Debug.LogError("房间" + name + "门" + door1.direction + "钥匙设置不一致");
            }
            // 单向连接
            door1.SetConnect(door2);
        }
    }
}
