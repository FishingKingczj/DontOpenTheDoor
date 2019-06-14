using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Candel : Item
{
    void Awake()
    {
        id = 6;
        name = "Candel";
        description = "This is a candel.";

        pickable = true;

        maxStorageAmount = 5;
    }

    public override void Effect(GameObject _user)
    {
        GameObject item = ItemLoader.LoadItemToScene(id);
        Room room = GameObject.Find("RoomLoader").GetComponent<RoomLoader>().GetPlayerRoom();
        item.transform.parent = room.transform;
        item.transform.position = _user.gameObject.transform.position;
    }

    public override void Use(GameObject _user)
    {
        Effect(_user);
        _user.GetComponent<Player_BackPack>().UseSucceed();
    }
}
