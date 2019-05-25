using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    private int UNIT_SIZE = 22;

    private List<Room> allRooms;
    private List<Room> activeRooms;

    public Room playerRoom;
    public MainCamera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        initRooms();
    }

    // 初始化房间
    private void initRooms()
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

        foreach (Room room in allRooms)
        {
            Debug.Log("Check" + room.name);
            room.CheckRoom();
            room.CheckDoor();
        }

        // init
        if (playerRoom == null)
        {
            Debug.LogError("未设置出生房间");
        }

        ActiveAroundRoom(playerRoom, new Vector3(0, 0, 0));
    }

    // 激活某个房间周围的所有房间
    private void ActiveAroundRoom(Room center, Vector3 pos)
    {
        Debug.Log("Active" + center.name);
        center.gameObject.SetActive(true);
        activeRooms.Add(center);
        center.transform.position = pos;

        if (!activeRooms.Contains(center.up))
        {
            center.up.gameObject.SetActive(true);
            activeRooms.Add(center.up);
            center.up.transform.position = pos + new Vector3(0, UNIT_SIZE, 0);
        }

        if (!activeRooms.Contains(center.down))
        {
            center.down.gameObject.SetActive(true);
            activeRooms.Add(center.down);
            center.down.transform.position = pos + new Vector3(0, -UNIT_SIZE, 0);
        }

        if (!activeRooms.Contains(center.left))
        {
            center.left.gameObject.SetActive(true);
            activeRooms.Add(center.left);
            center.left.transform.position = pos + new Vector3(-UNIT_SIZE, 0, 0);
        }

        if (!activeRooms.Contains(center.right))
        {
            center.right.gameObject.SetActive(true);
            activeRooms.Add(center.right);
            center.right.transform.position = pos + new Vector3(UNIT_SIZE, 0, 0);
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
        ActiveAroundRoom(room, target);
        // 移动镜头
        mainCamera.Move(target);
    }

    // 计算两个房间之间的图距离，不连通则返回-1
    public static int Distance(Room roomFrom, Room roomTo)
    {
        return 0;
    }
}
