using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEvent : MonoBehaviour
{
    private GameObject player;
    private GameObject roomLoader;
    private GameObject room;
    public static int deathGiven = 0;

    private GameObject findRoom(string roomName)
    {
        return roomLoader.transform.FindChild(roomName).gameObject;
    }

    public static void die() {
        Debug.Log("die");
        SceneManager.LoadScene(0);
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player");
        roomLoader = GameObject.Find("RoomLoader");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
