using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Slow : Item
{
    [Header("Additional Variable")]
    public float decelerationValue = 0.9f;

    void Awake()
    {
        id = 9;
        name = "Slow";

        pickable = false;
    }

    public override void Interact(GameObject _user)
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Player")) {
            collision.gameObject.GetComponent<Player>().ReduceExtraSpeed(decelerationValue);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            collision.gameObject.GetComponent<Player>().AddExtraSpeed(decelerationValue);
        }
    }
}
