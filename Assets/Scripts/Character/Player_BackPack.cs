using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_BackPack : MonoBehaviour
{
    public int maxStorageAmount;

    [Header("Item Group")]
    public GameObject[] backpack;
    public GameObject[] item_Group;
    public Text[] text_StorageAmount;
    public int[] item_StorageAmount;
    public string[] item_Name;
    public string[] item_Description;

    [Header("Operation Vairable")]
    public bool inSelected = false;
    public int selectedIndex = -1;

    public bool inUsed = false;

    void Start()
    {
        backpack = new GameObject[maxStorageAmount];
        text_StorageAmount = new Text[maxStorageAmount];
        item_Group = new GameObject[maxStorageAmount];
        item_StorageAmount = new int[maxStorageAmount];
        item_Name = new string[maxStorageAmount];
        item_Description = new string[maxStorageAmount];

        for (int i = 1; i <= maxStorageAmount; i++) {
            string n = "Box_" + i.ToString();
            backpack[i - 1] = GameObject.Find(n); 
        }

        int j = 0;
        foreach (GameObject t in backpack) {
            text_StorageAmount[j++] = t.GetComponentInChildren<Text>();
        }
    }

    void Update()
    {
        UseItem();
        if (!inUsed)
        {
            SelectItem();
            DiscardItem();
        }
    }

    private void OnGUI()
    {
        // 测试用 显示选择的物品信息
        if (inSelected)
        {
            GUI.TextField(new Rect(Screen.width / 2.0f - (Screen.height / 1.5f / 2.0f), Screen.height / 9.5f, Screen.height / 1.5f, Screen.height / 8), item_Name[selectedIndex] + '\n' + item_Description[selectedIndex]);
        }
    }

    // 添加道具 true->背包有余位 false->背包已满
    public bool AddItem(GameObject _item, int _maxStorageAmount, string _name, string _description) {
        // 尝试寻找重复道具并叠加
        for (int i = 0; i < maxStorageAmount; i++) {
            if (item_Name[i] == _name && item_StorageAmount[i] < _maxStorageAmount)
            {
                ReflashItemAmount(i,1);
                return true;
            }
        }

        // 尝试寻找空位置
        for (int j = 0; j < maxStorageAmount; j++) {
            if (item_Group[j] == null)
            {
                GameObject item = LoadItemToBackpack(_name);
                if (item == null) return false;

                // 物品信息更新
                item.transform.parent = backpack[j].transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                float width = backpack[j].GetComponent<RectTransform>().sizeDelta.x;
                float height = backpack[j].GetComponent<RectTransform>().sizeDelta.y;
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 10, height - 10);
                item.transform.SetAsFirstSibling();

                item_Group[j] = item;
                ReflashItemAmount(j,1);
                item_Name[j] = _name;
                item_Description[j] = _description;
                return true;
            }
        }

        return false;
    }
    // 添加道具(针对带特殊参数的key类道具)
    public bool AddItem(GameObject _item, int _maxStorageAmount, string _name, string _description,int _value)
    {
        // 尝试寻找空位置
        for (int j = 0; j < maxStorageAmount; j++)
        {
            if (item_Group[j] == null)
            {
                GameObject item = LoadItemToBackpack(_name);
                if (item == null) return false;

                // 物品信息更新
                item.transform.parent = backpack[j].transform;
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                float width = backpack[j].GetComponent<RectTransform>().sizeDelta.x;
                float height = backpack[j].GetComponent<RectTransform>().sizeDelta.y;
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 10, height - 10);
                item.transform.SetAsFirstSibling();

                // 特殊物品处理
                if (_name.Contains("Key")) {
                    item.GetComponent<Item_Key>().pairingValue = _value;
                }

                item_Group[j] = item;
                ReflashItemAmount(j, 1);
                item_Name[j] = _name;
                item_Description[j] = _description;
                return true;
            }
        }

        return false;
    }

    // 删除物品
    public void RemoveItem(int _index) {
        inSelected = false;
        selectedIndex = -1;

        Destroy(item_Group[_index]);

        item_Group[_index] = null;
        item_StorageAmount[_index] = 0;
        item_Name[_index] = null;
        item_Description[_index] = null;
    }

    // 丢弃物品(包含特殊处理)
    public void DiscardItem() {
        if (inSelected) {
            if (Input.GetKeyDown(KeyCode.G))
            {
                GameObject item = LoadItemToScene(item_Name[selectedIndex]);
                item.transform.position = this.gameObject.transform.position;

                // 特殊处理
                if (item_Name[selectedIndex].Contains("Key"))
                {
                    item.GetComponent<Item_Key>().SetPairingValue(item_Group[selectedIndex].GetComponent<Item_Key>().GetPairingValue());
                }

                ReflashItemAmount(selectedIndex, 0);
            }
        }
    }

    // 加载物品(生成到道具栏)
    public GameObject LoadItemToBackpack(string _name) {
        switch (_name)
        {
            case "Food":
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Backpack_Item/Item_Food"));
                }
            case "Key":
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Backpack_Item/Item_Key"));
                }
            default:
                {
                    return null;
                }
        }
    }

    // 加载物品(生成到场景)
    public GameObject LoadItemToScene(string _name) {
        switch (_name)
        {
            case "Food":
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/Food"));
                }
            case "Key":
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/Key"));
                }
            default:
                {
                    return null;
                }
        }
    }

    // 使用物品
    public void UseItem() {
        if (inSelected)
        {
            if (Input.GetKey(KeyCode.E)) {
                inUsed = true;
                item_Group[selectedIndex].SendMessage("Use", this.gameObject, SendMessageOptions.DontRequireReceiver);
            } else if (Input.GetKeyUp(KeyCode.E)) {
                inUsed = false;
                item_Group[selectedIndex].SendMessage("Use_Reset", this.gameObject, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    // 物品使用成功
    public void UseSucceed()
    {
        inUsed = false;
        ReflashItemAmount(selectedIndex, 0);
    }

    // 刷新物品数量 1->增加 0->减少
    public void ReflashItemAmount(int _index,int _opr) {
        if (_opr == 1)
        {
            item_StorageAmount[_index]++;
            if(item_StorageAmount[_index] >= 2)
                text_StorageAmount[_index].text = item_StorageAmount[_index].ToString();
        }
        else {
            item_StorageAmount[_index]--;
            if (item_StorageAmount[_index] < 2)
                text_StorageAmount[_index].text = null;
            else {
                text_StorageAmount[_index].text = item_StorageAmount[_index].ToString();
            }

            if (item_StorageAmount[_index] == 0) {
                RemoveItem(_index);
            }
        }

    }

    // 选择物品
    public void SelectItem() {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (inSelected) {
                item_Group[selectedIndex].SendMessage("Use_Reset", this.gameObject, SendMessageOptions.DontRequireReceiver);
                
                if (item_Group[0] != null && selectedIndex != 0) {
                    selectedIndex = 0;
                    return;
                }
                inSelected = !inSelected; selectedIndex = -1;
            }
            else {
                if (item_Group[0] == null) return;

                inSelected = !inSelected;selectedIndex = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (inSelected) {
                item_Group[selectedIndex].SendMessage("Use_Reset", this.gameObject, SendMessageOptions.DontRequireReceiver);

                if (item_Group[1] != null && selectedIndex != 1) {
                    selectedIndex = 1;
                    return;
                }
                inSelected = !inSelected; selectedIndex = -1;
            }
            else {
                if (item_Group[1] == null) return;

                inSelected = !inSelected; selectedIndex = 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (inSelected) {
                item_Group[selectedIndex].SendMessage("Use_Reset", this.gameObject, SendMessageOptions.DontRequireReceiver);

                if (item_Group[2] != null && selectedIndex != 2) {
                    selectedIndex = 2;
                    return;
                }
                inSelected = !inSelected; selectedIndex = -1;
            }
            else {
                if (item_Group[2] == null) return;

                inSelected = !inSelected; selectedIndex = 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (inSelected) {
                item_Group[selectedIndex].SendMessage("Use_Reset", this.gameObject, SendMessageOptions.DontRequireReceiver);

                if (item_Group[3] != null && selectedIndex != 3)
                {
                    selectedIndex = 3;
                    return;
                }
                inSelected = !inSelected; selectedIndex = -1;
            }
            else {
                if (item_Group[3] == null) return;

                inSelected = !inSelected; selectedIndex = 3;
            }
        }
    }
}
