using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MoveObject
{
    private const float DEFAULT_SPEED = 1f;
    private const float RUSH_SPEED = DEFAULT_SPEED * 1.5f;

    void FixedUpdate()
    {
        PlayerRush();
        PlayerMove();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("你碰到了" + collider.name.ToString());
    }

    // 接触结束
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider != null)
        {
            Debug.Log("你离开了" + collider.name.ToString());
        }
    }

    // 控制人物移动
    private void PlayerMove()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        Vector3 vector = new Vector3(dx, dy);
        Move(vector);
    }

    // 控制人物冲刺
    private void PlayerRush()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            setSpeed(RUSH_SPEED);
        }
        else
        {
            setSpeed(DEFAULT_SPEED);
        }
    }
}
