﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster2 : MoveObject
{
    protected Transform target;
    protected bool chasing;

    public void setChasing(bool _chasing)
    {
        chasing = _chasing;
    }
}
