using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_SecretLock : Item
{
    [Header("Additional Variable")]
    public int key = -1;
    public bool inTrigger = false;

    public Item_SecretLockManager manager;

    private void Awake()
    {
        id = 11;
        name = "SecreLock";

        pickable = false;

        if (key == -1) { Debug.LogWarning(this.gameObject.name + " 并未赋予Key值"); }
    }

    public void Reset()
    {
        if (inTrigger)
        {
            inTrigger = false;
            this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
    }

    public override void Interact(GameObject _user)
    {
        if (!inTrigger) {
            if (manager.Pair(key, _user))
            {
                inTrigger = true;
                this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            }
        }
    }
}
