  using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveObject : MonoBehaviour
{
    // 速度系数，决定所有物体移动的基准系数
    private const float SPEED_FACTOR = 0.1f;

    private float moveSpeed;

    // 角色移动
    protected void Move(Vector3 vector)
    {
        transform.position = vector * moveSpeed * SPEED_FACTOR + transform.position;
        Turn(vector.x, vector.y);
    }

    // 角色转向，tx/ty的正负表示朝向
    protected void Turn(float tx, float ty)
    {
        if (Math.Abs(tx) > Math.Abs(ty))
        {
            if (tx > 0)
                transform.rotation = Quaternion.Euler(0, 0, 270);
            else if (tx < 0)
                transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            if (ty > 0)
                transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (ty < 0)
                transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }

    // 设置角色的速度
    public void setSpeed(float speed)
    {
        moveSpeed = speed;
    }

	// 返回角色移动速度
	public float GetSpeed(){
		return moveSpeed;
	}
}
