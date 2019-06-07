using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Transporter : Item
{
    [Header("Additional Varible")]
    public GameObject startPosition;
    public GameObject endPosition;

    void Awake()
    {
        id = 5;
        name = "Transporter";
        description = "这是个传送器,通往指定区域";

        pickable = true;

        maxStorageAmount = 1;

        if (endPosition == null) {
            Debug.LogError("无传送目标位置");
        }
    }

    public override void Effect(GameObject _user)
    {
        if (endPosition.transform.parent == null) {
            Debug.LogError("传送目标位置放置错误");
            return;
        }

        Room r = endPosition.transform.parent.GetComponent<Room>();

        if (r == GameObject.Find("RoomLoader").GetComponent<RoomLoader>().GetPlayerRoom()) {
            _user.transform.position = endPosition.transform.position;
            return;
        }

        GameObject.Find("RoomLoader").GetComponent<RoomLoader>().SetPlayerRoom(r);
        GameObject.Find("RoomLoader").GetComponent<RoomLoader>().initRooms();
        GameObject.Find("RoomLoader").GetComponent<RoomLoader>().InActiveRooms();

        Camera.main.gameObject.transform.position = _user.transform.position;
        Camera.main.gameObject.GetComponent<MainCamera>().SetCurrentCenterPoint(r.transform.position);

        _user.transform.position = endPosition.transform.position;
    }

    public override void Use(GameObject _user)
    {
        // 无需起点传送
        if (startPosition == null)
        {
            Effect(_user);
            _user.GetComponent<Player_BackPack>().UseSucceed();
        }
        // 需要起点传送
        else
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_user.transform.position, Player.interaction_Range);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject == startPosition)
                    {
                        Effect(_user);
                        _user.GetComponent<Player_BackPack>().UseSucceed();
                    }
                }
            }

        }
    }

    public GameObject GetEndPosition() { return endPosition; }
    public GameObject GetStartPosition() { return startPosition; }

    public void SetEndPosition(GameObject _endPosition) { endPosition = _endPosition; }
    public void SetStartPosition(GameObject _startPosition) { startPosition = _startPosition; }
}
