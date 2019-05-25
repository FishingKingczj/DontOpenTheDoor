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

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("Player"))
        {
            RoomLoader loader = GameObject.Find("RoomLoader").GetComponent<RoomLoader>();
            loader.Enter(this, null);
        }
    }


    public void CheckRoom()
    {
        if (up && up.down != this)
        {
            Debug.LogError("房间" + up.name + "and" + name + "上方连接有误");
        }
        if (down && down.up != this)
        {
            Debug.LogError("房间" + up.name + "and" + name + "下方连接有误");
        }
        if (left && left.right != this)
        {
            Debug.LogError("房间" + up.name + "and" + name + "左方连接有误");
        }
        if (right && right.left != this)
        {
            Debug.LogError("房间" + up.name + "and" + name + "右方连接有误");
        }
    }

    public void CheckDoor()
    {
        Item_Door door_up = transform.Find("DoorUp").GetComponent<Item_Door>();
        if (!up)
            Destroy(door_up.gameObject);
        else
        {
            Item_Door another = up.transform.Find("DoorDown").GetComponent<Item_Door>();
            InitDoor(door_up, another);
        }

        Item_Door door_down = transform.Find("DoorDown").GetComponent<Item_Door>();
        if (!down)
            Destroy(door_down.gameObject);
        else
        {
            Item_Door another = down.transform.Find("DoorUp").GetComponent<Item_Door>();
            InitDoor(door_down, another);
        }

        Item_Door door_left = transform.Find("DoorLeft").GetComponent<Item_Door>();
        if (!left)
            Destroy(door_left.gameObject);
        else
        {
            Item_Door another = left.transform.Find("DoorRight").GetComponent<Item_Door>();
            InitDoor(door_left, another);
        }

        Item_Door door_right = transform.Find("DoorRight").GetComponent<Item_Door>();
        if (!right)
            Destroy(door_right.gameObject);
        else
        {
            Item_Door another = right.transform.Find("DoorLeft").GetComponent<Item_Door>();
            InitDoor(door_right, another);
        }
    }

    public void InitDoor(Item_Door door1, Item_Door door2)
    {
        if (door1.locked != door2.locked)
        {
            door1.locked = door2.locked = door2.locked && door2.locked;
            Debug.LogWarning("房间" + name + "门" + door1.name + "连接有误");
        }
        if (door1.open != door2.open)
        {
            door1.open = door2.open = door2.open && door2.open;
            Debug.LogWarning("房间" + name + "门" + door1.name + "连接有误");
        }
        if (door1.pairingValue != door2.pairingValue)
        {
            door1.pairingValue = door2.pairingValue;
            Debug.LogError("房间" + name + "门" + door1.name + "连接有误");
        }
        door1.SetConnect(door2);
        door2.SetConnect(door1);
    }
}
