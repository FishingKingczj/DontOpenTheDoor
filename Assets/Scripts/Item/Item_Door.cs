using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Door : Item
{
    [Header("Additional Varible")]
    public int pairingValue;

    public bool inOpened;
    public bool inLocked;

    void Start()
    {
        pickable = false;
    }

    public bool Pair(int _pairingValue) {
        if (pairingValue == _pairingValue) {
            return true;
        }
        return false;
    }

    public override void Interact(GameObject _user)
    {
        if (pickable == false)
        {
            if (inLocked)
            {
                Debug.Log("这门锁上了");
            }
            else {
                if (inOpened)
                {
                    Close();
                    Debug.Log("你关上了门");
                }
                else {
                    Open();
                    Debug.Log("你打开了门");
                }
            }
        }
    }

    public void Open() { inOpened = true; }
    public void Close() { inOpened = false; }

    public void Lock() { inLocked = true; }
    public void Unlock() { inLocked = false; }
}
