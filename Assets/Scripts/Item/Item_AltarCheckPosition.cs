using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_AltarCheckPosition : Item
{
    [Header("Additional Variable")]
    public GameObject altar_Object;
    public Item_Altar altar;
    public bool inTrigger = false;

    public GameObject currentItem;
    public int currentItemId;

    public void Start()
    {
        altar_Object = this.transform.parent.gameObject;
        altar = this.transform.parent.GetComponent<Item_Altar>();
    }

    public void FixedUpdate()
    {
        if (inTrigger) {
            if (currentItem == null) {
                inTrigger = false;
                altar.CancelOperated(currentItemId);
                SetCurrentItemId(-1);
            }
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, 0.3f);
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i].tag.Contains("Item"))
                    continue;
                else
                {
                    colliders[i].gameObject.SendMessage("SendId", altar_Object, SendMessageOptions.DontRequireReceiver);
                    colliders[i].gameObject.SendMessage("SendId", this.gameObject, SendMessageOptions.DontRequireReceiver);
                    currentItem = colliders[i].gameObject;
                    inTrigger = true;
                    break;
                }
            }
        }
    }

    // 破坏物体
    public void DestroyItem() {
        Destroy(currentItem);
        SetCurrentItemId(-1);
    }

    public void SetInTrigger(bool _value) {
        inTrigger = _value;
    }

    // 设置/获取 当前物品Id
    public void SetCurrentItemId(int _value) {
        currentItemId = _value;
    }
    public int GetCurrentItemId() {
        return currentItemId;
    }

    public override void ReceiveId(int _id)
    {
        SetCurrentItemId(_id);
    }
}
