using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MoveObject
{
    protected Transform target;
    protected bool chasing;

    public void setChasing(bool _chasing)
    {
        chasing = _chasing;
    }

    protected void Chase()
    {
        float dx = target.position.x - transform.position.x;
        float dy = target.position.y - transform.position.y;
        Vector3 vector = new Vector3(dx, dy);
        vector.Normalize();
        Move(vector);
    }
}
