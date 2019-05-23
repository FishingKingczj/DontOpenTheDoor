using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterA : Monster
{
    private const float DEFAULT_SPEED = 0.8f;

    void Start()
    {
        setChasing(true);
        setSpeed(DEFAULT_SPEED);
        target = GameObject.Find("player").transform;
    }

    void FixedUpdate()
    {
        if (chasing)
            Chase();
    }

    // 怪物追逐角色
    private void Chase()
    {
        float dx = target.position.x - transform.position.x;
        if (dx > 1)
            dx = 1;
        else if (dx < -1)
            dx = -1;
        float dy = target.position.y - transform.position.y;
        if (dy > 1)
            dy = 1;
        else if (dy < -1)
            dy = -1;

        Vector3 vector = new Vector3(dx, dy);
        Move(vector);
    }
}
