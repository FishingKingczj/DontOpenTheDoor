using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : ScrollRect
{
    public Player player;
    private Vector2 forceVector;
    protected float radius;
    protected float radius_Rush = 0.5f;

    public void FixedUpdate()
    {
        if (player.GetInEscape()) {
            return;
        }

        if (forceVector != Vector2.zero) {
            player.PlayerMove(new Vector3(forceVector.x, forceVector.y, 0));
        }
    }


    protected override void Start()
    {
        base.Start();
        player = GameObject.Find("player").GetComponent<Player>();
        forceVector = Vector2.zero;
        radius = (transform as RectTransform).sizeDelta.x * 0.5f; // 计算摇杆块的半径
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
}