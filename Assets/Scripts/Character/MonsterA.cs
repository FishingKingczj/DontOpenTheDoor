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
}
