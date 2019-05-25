using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Door : Item
{
    [Header("Additional Varible")]
    public int pairingValue;

    void Start()
    {
        pickable = false;
    }

    public bool Open(int _pairingValue) {
        if (pairingValue == _pairingValue) {
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }
}
