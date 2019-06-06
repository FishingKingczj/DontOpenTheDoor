using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : ScrollRect
{
    public bool controllable;
    public float timer_ChangeMoveDir = 0.5f;

    public float autoMove_x = 0;
    public float autoMove_y = 0;

    public Player player;
    private Vector2 forceVector;
    protected float radius;
    protected float radius_Rush = 0.5f;

    public void FixedUpdate()
    {
        if (player.GetInEscape()) {
            // QTE测试用
            if (Input.GetKey(KeyCode.D))
            {
                forceVector.x = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                forceVector.x = -1;
            }
            else
                forceVector.x = 0;


            return;
        }

        // 可控制状态下玩家进行操作
        if (controllable)
        {
            if(forceVector != Vector2.zero)
            player.PlayerMove(new Vector3(forceVector.x, forceVector.y, 0));
        }
        else {
            AutoMove();
        }
    }


    protected override void Start()
    {
        base.Start();
        player = GameObject.Find("player").GetComponent<Player>();
        forceVector = Vector2.zero;
        radius = (transform as RectTransform).sizeDelta.x * 0.5f; // 计算摇杆块的半径

        controllable = true;
        CreateAutoMovementDir();
    }   

    public override void OnDrag(PointerEventData eventData)
    {

        base.OnDrag(eventData);
        var contentPostion = this.content.anchoredPosition;


        //越界
        if (contentPostion.magnitude > radius)
        {
            contentPostion = contentPostion.normalized * radius;
            SetContentAnchoredPosition(contentPostion);
        }

        forceVector = contentPostion.normalized;


        //奔跑判定
        if (contentPostion.magnitude > radius * radius_Rush)
        {
            player.PlayerRush(true);
        }
        else
        {
            player.PlayerRush(false);
        }

        player.SetInMoved(true);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        forceVector = Vector2.zero;

        player.SetInMoved(false);
    }

    public Vector2 GetVector() {
        return forceVector;
    }
    public void SetControllable(bool _value) {
        controllable = _value;
    }

    public void AutoMove() {
        if (timer_ChangeMoveDir <= 0)
        {
            timer_ChangeMoveDir = 1.0f;
            CreateAutoMovementDir();
        }
        else
        {
            timer_ChangeMoveDir -= Time.deltaTime;
        }

        player.PlayerMove(new Vector3(autoMove_x, autoMove_y, 0));
    }

    public void CreateAutoMovementDir() {
        autoMove_x = Random.Range(-1.0f, 1.0f);
        autoMove_y = Random.Range(-1.0f, 1.0f);
    }
}