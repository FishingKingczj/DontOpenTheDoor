using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Dictionary<int, NpcState> npcStates = new Dictionary<int, NpcState>();
    public Dictionary<int, StateTransfor> npcStateTransfor = new Dictionary<int, StateTransfor>();

    private string player_npcState = "inRoom";
    private GameObject player;
    private GameObject roomLoader;
    private GameObject room;
    private int stateID = 0;
    public float npcTalkScale = 1.0f;
    public int achievementID = -1;

    public string stateText;
    public string transforText;

    private GameObject findRoom(string roomName) {
        return roomLoader.transform.Find(roomName).gameObject;
    }
    private void detectionLayer() {
        //if (achievementID == player.GetComponent<Player>().achievementID) return;

        achievementID = player.GetComponent<Player>().achievementID;
        //检测player和NPC的关系
        if (room.transform.name.Equals(roomLoader.GetComponent<RoomLoader>().playerRoom.name)) {
            /*if (Vector2.Distance((Vector2)player.transform.position, (Vector2)transform.position) < npcTalkScale)
                player_npcState = Player_NPCState.inTalkScale;
            else*/
                player_npcState = Player_NPCState.inRoom;
        }
        else
            player_npcState = Player_NPCState.outRoom;
        //检测NPC是否状态发生改变
        if (!npcStateTransfor.ContainsKey(stateID)) return;
        StateTransfor stateTransfor = npcStateTransfor[stateID];
        if (stateTransfor.situation.Equals(player_npcState) && stateTransfor.parameter == achievementID) {
            stateID = stateTransfor.nextID;
            NpcState state = npcStates[stateID];
            room = findRoom(state.roomID);
            this.transform.SetParent(room.transform);
            this.transform.localPosition = new Vector3(state.localX, state.localY, this.transform.localPosition.z);
        }  
    }

    //展示NPC对话和赠送礼物
    private void giveItem() {
        NpcState state = npcStates[stateID];
        if (state.gift  == -1) return;
        GameObject gift = ItemLoader.LoadItemToScene(state.gift);
        gift.transform.SetParent(this.transform);
        gift.transform.position = player.transform.position;
    }
    private void showDialog() {
        NpcState state = npcStates[stateID];
        Dialog.ShowDialog(state.dialog);
    }

    public void interact() {
        showDialog();
        giveItem();
        player.GetComponent<Player>().achievementID = npcStates[stateID].achievementID;
        achievementID = player.GetComponent<Player>().achievementID;
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player");
        roomLoader = GameObject.Find("RoomLoader");
        
        string[] lines = ReadInText.readin(stateText);
        var count = 0;
        int i = 0;
        while (count < lines.Length) {
            npcStates.Add(i, new NpcState(int.Parse(lines[count+0]), lines[count + 1], float.Parse(lines[count + 2]), float.Parse(lines[count + 3]), lines[count + 4], int.Parse(lines[count + 5]), int.Parse(lines[count + 6])));
            count = count + 7;
            i++;
        }

        lines = ReadInText.readin(transforText);
        count = 0;
        i = 0;
        while (count < lines.Length)
        {
            npcStateTransfor.Add(i, new StateTransfor(int.Parse(lines[count + 0]), int.Parse(lines[count + 1]), lines[count + 2], int.Parse(lines[count + 3])));
            count = count + 4;
            i++;
        }
        //npcStateTransfor.Add(0,new StateTransfor(0,1,Player_NPCState.inRoom,10));
        //npcStateTransfor.Add(1, new StateTransfor(1, 2, Player_NPCState.outRoom, 20));

        NpcState state = npcStates[stateID];
        room = findRoom(state.roomID);
        this.transform.SetParent(room.transform);
        this.transform.localPosition = new Vector3(state.localX, state.localY, this.transform.localPosition.z);


    }

    // Update is called once per frame
    void Update()
    {
        detectionLayer();
    }
}

public class NpcState {
    public int ID;
    public string roomID;
    public float localX;
    public float localY;
    public string dialog;
    public int gift= -1;
    public int achievementID;
    public NpcState(int _ID, string _roomID, float _localX, float _localY,string _dialog,int _gift,int _achievement) {
        ID = _ID;
        roomID = _roomID;
        localX = _localX;
        localY = _localY;
        dialog = _dialog;
        gift = _gift;
        achievementID = _achievement;
    }
}

public class StateTransfor {
    public int ID;
    public int nextID;
    public string situation;
    public int parameter = 0;
    public StateTransfor(int _ID,int _nextID,string _situation,int _parameter) {
        ID = _ID;
        nextID = _nextID;
        situation = _situation;
        parameter = _parameter;
    }
}

public static class Player_NPCState {
    public static string inRoom = "inRoom";
    public static string outRoom = "outRoom";
    public static string inTalkScale = "inTalkScale";
}