using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_RemoteKey : Item
{
    [Header("Additional Variable")]
    public GameObject triggerPosition;
    public GameObject targetDoor;

    void Awake()
    {
        id = 3;
        name = "RemoteKey";
        description = "这是个远程钥匙,魔法打开远处的门";

        pickable = true;

        maxStorageAmount = 1;

        if (targetDoor == null) {
            Debug.LogWarning("无目标门");
        }
    }

    public override void Effect(GameObject _user)
    {
        targetDoor.GetComponent<Item_Door>().Unlock();
        targetDoor.GetComponent<Item_Door>().Open();
    }

    public override void Use(GameObject _user)
    {
        if (targetDoor == null) {
            Debug.LogError("无目标门");
        }

        if (triggerPosition == null)
        {
            Effect(_user);
            _user.GetComponent<Player_BackPack>().UseSucceed();
        }
        else {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_user.transform.position, Player.interaction_Range);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject == triggerPosition)
                    {
                        Effect(_user);
                        _user.GetComponent<Player_BackPack>().UseSucceed();
                    }
                }
            }
        }
    }

    public void SetTargetDoor(GameObject _targetDoor) {
        targetDoor = _targetDoor;
    }
    public void SetTriggerPosition(GameObject _triggerPosition) {
        triggerPosition = _triggerPosition;
    }

    public GameObject GetTargetDoor() { return targetDoor; }
    public GameObject GetTriggerPosition() { return triggerPosition; }
}
