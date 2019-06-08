using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Altar : Item
{
    [Header("Additinoal Variable")]
    public List<int> currentItemSequence;
    public List<int> checkPositionItemId;

    public List<GameObject> checkPosition;
    public List<int> targetItemSequence;

    public GameObject itemPotition;
    public int itemId;

    private void Awake()
    {
        id = 12;
        name = "Altar";

        pickable = false;

        if (targetItemSequence.Count != checkPosition.Count) { Debug.LogError("目标物品序列与检测位置数量不一致"); }

        currentItemSequence = targetItemSequence;
    }

    // 重写id接受函数
    public override void ReceiveId(int _id)
    {
        if (currentItemSequence.Contains(_id)) {
            currentItemSequence.Remove(_id);
            checkPositionItemId.Add(_id);
        }

        // 销毁目标物品并生成新物品
        if (currentItemSequence.Count == 0) {
            GameObject go = ItemLoader.LoadItemToScene(itemId);
            go.transform.SetParent(this.gameObject.transform);
            go.transform.position = itemPotition.transform.position;

            for (int i = 0; i < checkPosition.Count; i++) {
                checkPosition[i].GetComponent<Item_AltarCheckPosition>().DestroyItem();
            }
        }
    }

    // 撤销目标物品
    public void CancelOperated(int _id) {
        if(targetItemSequence.Contains(_id))
        currentItemSequence.Add(_id);
    }
}
