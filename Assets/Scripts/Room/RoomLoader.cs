using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    static public int ROOM_SIZE = 24;
    private int UNIT_SIZE = 22;

    public List<Room> allRooms;
    public List<Room> activeRooms;

    public Room playerRoom;
    public MainCamera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        initRooms();
    }

    // 初始化房间
    public void initRooms()
    {
        allRooms = new List<Room>();
        activeRooms = new List<Room>();

        // find all rooms
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag == "Room")
            {
                GameObject roomObject = transform.GetChild(i).gameObject;
                Room room = roomObject.GetComponent<Room>();
                allRooms.Add(room);
            }
        }

        // 这里需要执行两次，先设定门的状态
        foreach (Room room in allRooms)
        {
            room.CheckRoom();
        }

        foreach (Room room in allRooms)
        {
            room.CheckDoor();
        }

        // init
        if (playerRoom == null)
        {
            Debug.LogError("未设置出生房间");
        }

        ActiveRoom(playerRoom, new Vector3(0, 0, 0));
        ActiveAroundRoom();
    }

    // 激活某个房间
    public void ActiveRoom(Room center, Vector3 pos)
    {
        Debug.Log("Active" + center.name);
        center.gameObject.SetActive(true);
        activeRooms.Add(center);
        center.transform.position = pos;
    }

    // 激活主房间周围的所有房间
    public void ActiveAroundRoom()
    {
        Vector3 pos = playerRoom.transform.position;
        if (playerRoom.up && !activeRooms.Contains(playerRoom.up))
        {
            ActiveRoom(playerRoom.up, pos + new Vector3(0, UNIT_SIZE, 0));
        }

        if (playerRoom.down && !activeRooms.Contains(playerRoom.down))
        {
            ActiveRoom(playerRoom.down, pos + new Vector3(0, -UNIT_SIZE, 0));
        }

        if (playerRoom.left && !activeRooms.Contains(playerRoom.left))
        {
            ActiveRoom(playerRoom.left, pos + new Vector3(-UNIT_SIZE, 0, 0));
        }

        if (playerRoom.right && !activeRooms.Contains(playerRoom.right))
        {
            ActiveRoom(playerRoom.right, pos + new Vector3(UNIT_SIZE, 0, 0));
        }
    }

    // 停止所有不在活跃列表的房间
    public void InActiveRooms()
    {
        foreach (Room room in allRooms)
        {
            if (!activeRooms.Contains(room) && room.gameObject.activeSelf)
            {
                room.gameObject.SetActive(false);
            }
        }
    }

    // 玩家进入一个房间时，调用
    public void Enter(Room room, Item_Door door)
    {
        Debug.Log("进入房间" + room.name);
        if (playerRoom == room)
        {
            // 第一次加载房间
            return;
        }

        activeRooms.Clear();
        Vector3 target = new Vector3();

        if (playerRoom.up == room)
        {
            Vector3 vector = new Vector3(0, UNIT_SIZE, 0);
            target = playerRoom.transform.position + vector;
        }
        else if (playerRoom.down == room)
        {
            Vector3 vector = new Vector3(0, -UNIT_SIZE, 0);
            target = playerRoom.transform.position + vector;
        }
        else if (playerRoom.left == room)
        {
            Vector3 vector = new Vector3(-UNIT_SIZE, 0, 0);
            target = playerRoom.transform.position + vector;
        }
        else if (playerRoom.right == room)
        {
            Vector3 vector = new Vector3(UNIT_SIZE, 0, 0);
            target = playerRoom.transform.position + vector;
        }
        else
        {
            Debug.LogError("房间进入错误" + room.name);
            return;
        }
        // 更新玩家位置
        playerRoom = room;
        // 激活房间
        ActiveRoom(room, target);
        // 移动镜头
        mainCamera.Move(target);
    }

    public Room GetPlayerRoom()
    {
        return playerRoom;
    }
    public void SetPlayerRoom(Room _room) {
        playerRoom = _room;
    }

    // 计算两个房间之间的图距离，不连通则返回-1
    public static int Distance(Room roomFrom, Room roomTo)
    {
        return 0;
    }
}
