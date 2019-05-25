using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public const float MOVE_SPEED = 20f;

    private Vector3 target;
    private RoomLoader loader;

    void Start()
    {
        loader = GameObject.Find("RoomLoader").GetComponent<RoomLoader>();
        target = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, MOVE_SPEED * Time.deltaTime);
        if (transform.position.Equals(target))
        {
            loader.InActiveRooms();
        }
    }

    public void Move(Vector3 _target)
    {
        target = _target + new Vector3(0, 0, -10);
    }
}
