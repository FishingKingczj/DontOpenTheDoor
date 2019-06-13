using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Cabinet : Item
{
    [Header("Additional Variable")]
    public bool inOpened = false;

    [Range(0,1)]
    public float initiallyVolume = 0.5f;
    [Range(0,100)]
    public float itemProbability = 50.0f;

    public GameObject itemPosition;
    public List<int> itemPool;

    public AudioSource audio;
    public AudioClip sound;

    private void Awake()
    {
        id = 10;
        name = "Cabinet";

        pickable = false;

        audio = this.gameObject.AddComponent<AudioSource>();
        //sound = Resources.Load<AudioClip>("Audio/Item/Cabinet/Audio_1");
        audio.clip = sound;

        audio.playOnAwake = false;
        audio.loop = true;
        audio.Play();
        audio.rolloffMode = AudioRolloffMode.Linear;
        audio.volume = initiallyVolume;
        audio.minDistance = 1;
        audio.maxDistance = 15.0f;
        audio.spatialBlend = 1.0f;

        if (itemPosition == null) Debug.LogError("无道具生成点");
    }

    public override void Interact(GameObject _user)
    {
        if (inOpened)
        {

        }
        else {
            Open();
        }
    }

    public void Open() {
        inOpened = true;
        
        //TODO animation
        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/Item/箱子-打开");

        audio.loop = false;
        audio.Stop();

        // 不可再互动
        this.gameObject.tag = "Untagged";

        // 概率创建道具或吓人的东西
        if (Random.Range(0, 101) < itemProbability)
        {
            if (itemPool.Count == 0) { Debug.LogError("道具池为空!");return; }

            int itemIndex = Random.Range(0, itemPool.Count);
            int itemId = itemPool[itemIndex];
            GameObject go = ItemLoader.LoadItemToScene(itemId);
            go.transform.SetParent(this.gameObject.transform.parent);

            go.transform.position = itemPosition.transform.position;
        }
        else {
            GameObject image = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Buff/Image_Phantom"));
        }
    }

    public void Close() {
        inOpened = false;

        // TODO Animation
        this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/Item/Cabinet");
    }
}
