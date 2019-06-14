using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public const float MOVE_SPEED = 15f;
    public const float FOLLOW_SPEED = 5.0f;

    private Vector3 target;
    private RoomLoader loader;
    public bool refresh;

    private GameObject player;
    public Camera camera;
    public Vector3 currentRoomCenterPoint;
    public Vector3 nextRoomPoint;
    public bool inMoving = false;

    void Start()
    {
        loader = GameObject.Find("RoomLoader").GetComponent<RoomLoader>();
        target = transform.position;

        player = GameObject.Find("player");
        camera = this.GetComponent<Camera>();
        currentRoomCenterPoint = new Vector3(0, 0,-10);
        nextRoomPoint = Vector3.zero;

    }

    void Update()
    {
       Follow();
    }
    public void Follow() {
        float interpolation = FOLLOW_SPEED * Time.deltaTime;

        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, player.transform.position.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, player.transform.position.x, interpolation);
        position.z = -10.0f;

        this.transform.position = position;
    }

    public void SetCurrentCenterPoint(Vector3 _v) {
        currentRoomCenterPoint = _v;
    }
}
