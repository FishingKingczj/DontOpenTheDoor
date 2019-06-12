using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterEvent : MonoBehaviour
{
    public Room enter;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Contains("Player"))
        {
            RoomLoader loader = GameObject.Find("RoomLoader").GetComponent<RoomLoader>();
            loader.Enter(enter);
        }
    }
}
