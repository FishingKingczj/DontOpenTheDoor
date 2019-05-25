using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Key : Item
{
    private const float DEFAULT_ENERGYRESTORE = 10.0f;

    [Header("Additional Varible")]
    public int pairingValue;

    void Start()
    {
        name = "Key";
        description = "这是个钥匙";

        pickable = true;

        maxStorageAmount = 1;
    }

    public override void Interact(GameObject _user)
    {
        if (pickable == true)
        {
            if (_user.GetComponent<Player_BackPack>().AddItem(this.gameObject, maxStorageAmount, name, description,pairingValue))
                Destroy(this.gameObject);
            else
            {
                Debug.Log("背包已满");
            }
        }
    }

    public override void Use_Reset() { }

    public override void Use(GameObject _user)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_user.transform.position, _user.GetComponent<Player>().interaction_Range);
        if (colliders.Length > 0)
        {
            foreach (Collider2D t in colliders)
            {
                if (t.tag == "Item_Door")
                {
                    if(t.GetComponent<Item_Door>().Open(pairingValue))
                    _user.GetComponent<Player_BackPack>().UseSucceed();
                    return;
                }
            }
        }
    }

    // 设置钥匙配对值
    public void SetPairingValue(int _value) { pairingValue = _value; }
    public int GetPairingValue() { return pairingValue; }
}
