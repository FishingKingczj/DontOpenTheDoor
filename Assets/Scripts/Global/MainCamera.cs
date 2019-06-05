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
        if (!inMoving)
            Follow();
        else {
            transform.position = Vector3.MoveTowards(transform.position, nextRoomPoint, MOVE_SPEED * Time.deltaTime);
            if (transform.position.Equals(nextRoomPoint) && refresh)
            {
                loader.InActiveRooms();
                loader.ActiveAroundRoom();
                inMoving = false;
            }
        }
    }

    public void Move(Vector3 _target)
    {
        target = _target + new Vector3(0, 0, -10);
        
        refresh = true;
        inMoving = true;

        // 2 为房间之间的重叠距离
        if (target.x > currentRoomCenterPoint.x)
        {
            nextRoomPoint = new Vector3(this.transform.position.x + (2 + camera.orthographicSize) + 1, this.transform.position.y, -10);
        }
        else if (target.x < currentRoomCenterPoint.x)
        {
            nextRoomPoint = new Vector3(this.transform.position.x - (2 + camera.orthographicSize) - 1, this.transform.position.y, -10);
        }
        else if (target.y > currentRoomCenterPoint.y)
        {
            nextRoomPoint = new Vector3(this.transform.position.x, this.transform.position.y + (2 + camera.orthographicSize) + 1, -10);
        }
        else if (target.y < currentRoomCenterPoint.y)
        {
            nextRoomPoint = new Vector3(this.transform.position.x, this.transform.position.y - (2 + camera.orthographicSize) - 1, -10);
        }

        currentRoomCenterPoint = target;
    }

    public void Follow() {
        float interpolation = FOLLOW_SPEED * Time.deltaTime;

        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, player.transform.position.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, player.transform.position.x, interpolation);
        position.z = -10.0f;

        position.x = Mathf.Clamp(position.x, currentRoomCenterPoint.x + -(RoomLoader.ROOM_SIZE / 2) + camera.orthographicSize, currentRoomCenterPoint.x + (RoomLoader.ROOM_SIZE / 2) - camera.orthographicSize);
        position.y = Mathf.Clamp(position.y, currentRoomCenterPoint.y + -(RoomLoader.ROOM_SIZE / 2) + camera.orthographicSize, currentRoomCenterPoint.y + (RoomLoader.ROOM_SIZE / 2) - camera.orthographicSize);

        this.transform.position = position;
    }
}
