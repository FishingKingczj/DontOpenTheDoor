using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Candel : Item
{
    void Awake()
    {
        id = 6;
        name = "Candel";
        description = "这是一根蜡烛";

        pickable = true;

        maxStorageAmount = 5;
    }

    public override void Effect(GameObject _user)
    {
        GameObject item = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/SceneItem_Candle"));
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
