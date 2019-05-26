using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player_BackPack : MonoBehaviour
{
    public int maxStorageAmount;

    [Header("Item Group")]
    public GameObject[] backpack;
    public GameObject[] item_Group;
    public Text[] text_StorageAmount;
    public int[] item_StorageAmount;
    public int[] item_Id;
    public string[] item_Name;
    public string[] item_Description;

    public int numberOfItem;

    [Header("Operation Vairable")]
    public bool inSelected = false;
    public int selectedIndex = -1;

    public bool inUsed = false;

    public bool inOperated = false;
    public float operateDelayTime = 0.5f;

    [Header("Composite Vairable")]
    public string[] compositeTable;
    public bool inMultipleSelected = false;
    public List<int> multipleSelectIndex;

    public const float MULTIPLESELECTUSAGETIME = 1.0f;
    public float timer_MultipleSelectUsageTime = MULTIPLESELECTUSAGETIME;

    public const float COMPOSEITEMUSAGETIME = 1.0f;
    public float timer_ComposeItemUsageTime = COMPOSEITEMUSAGETIME;

    public Image progressRing;

    void Start()
    {

        backpack = new GameObject[maxStorageAmount];
        text_StorageAmount = new Text[maxStorageAmount];
        item_Group = new GameObject[maxStorageAmount];
        item_StorageAmount = new int[maxStorageAmount];
        item_Id = new int[maxStorageAmount];
        item_Name = new string[maxStorageAmount];
        item_Description = new string[maxStorageAmount];

        numberOfItem = 0;

        for (int i = 1; i <= maxStorageAmount; i++) {
            string n = "Box_" + i.ToString();
            backpack[i - 1] = GameObject.Find(n); 
        }

        int j = 0;
        foreach (GameObject t in backpack) {
            text_StorageAmount[j++] = t.GetComponentInChildren<Text>();
        }

        progressRing = GameObject.Find("ProgressRing").GetComponent<Image>();
    }

    void Update()
    {
        if (!inUsed)
        {
            SelectItem();
            if(!inMultipleSelected)
            DiscardItem();
        }
        UseItem();
        ComposeItem();
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
    public bool AddItem(GameObject _item, int _maxStorageAmount, int _id, string _name, string _description) {
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
                GameObject item = LoadItemToBackpack(_id);
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
                item_Id[j] = _id;
                item_Name[j] = _name;
                item_Description[j] = _description;

                numberOfItem++;
                return true;
            }
        }

        return false;
    }
    // 添加道具(针对带特殊参数的key类道具)
    public bool AddItem(GameObject _item, int _maxStorageAmount, int _id , string _name, string _description,int _value)
    {
        // 尝试寻找空位置
        for (int j = 0; j < maxStorageAmount; j++)
        {
            if (item_Group[j] == null)
            {
                GameObject item = LoadItemToBackpack(_id);
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
                item_Id[j] = _id;
                item_Name[j] = _name;
                item_Description[j] = _description;

                numberOfItem++;
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

        numberOfItem--;

        ResetSelectedPrompt();
    }

    // 丢弃物品(包含特殊处理)
    public void DiscardItem() {
        if (inSelected) {
            if (Input.GetKeyDown(KeyCode.G))
            {
                GameObject item = LoadItemToScene(item_Id[selectedIndex]);
                // 需要把物体的加到Room里
                Room room = GameObject.Find("RoomLoader").GetComponent<RoomLoader>().GetPlayerRoom();
                item.transform.parent = room.transform;
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
    public GameObject LoadItemToBackpack(int _id) {
        switch (_id)
        {
            case 0:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Backpack_Item/Item_Food"));
                }
            case 1:
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
    public GameObject LoadItemToScene(int _id) {
        switch (_id)
        {
            case 0:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/Food"));
                }
            case 1:
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
            if (Input.GetKey(KeyCode.E) && !inOperated) {
                inUsed = true;
                item_Group[selectedIndex].SendMessage("Use", this.gameObject, SendMessageOptions.DontRequireReceiver);
            } else if (Input.GetKeyUp(KeyCode.E)) {
                inOperated = false;
                inUsed = false;
                item_Group[selectedIndex].SendMessage("Use_Reset", this.gameObject, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    // 物品使用成功 (是否消耗当前物品)
    public void UseSucceed(bool _delete = true)
    {
        inOperated = true;
        inUsed = false;

        if(_delete)
        ReflashItemAmount(selectedIndex, 0);
    }

    // 刷新物品数量 _opr:1->增加 0->减少 数量默认减少1
    public void ReflashItemAmount(int _index,int _opr ,int _amount = 1) {
        if (_opr == 1)
        {
            item_StorageAmount[_index] += _amount;
            if(item_StorageAmount[_index] >= 2)
                text_StorageAmount[_index].text = item_StorageAmount[_index].ToString();
        }
        else {
            item_StorageAmount[_index] -= _amount;
            if (item_StorageAmount[_index] < 2)
                text_StorageAmount[_index].text = null;
            else {
                text_StorageAmount[_index].text = item_StorageAmount[_index].ToString();
            }

            if (item_StorageAmount[_index] <= 0) {
                RemoveItem(_index);
            }
        }

    }

    // 选择物品
    public void SelectItem() {
        // 长按 进入/退出 选择模式(物品数量必须大于1)
        if (numberOfItem > 1 && (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.Alpha3) || Input.GetKey(KeyCode.Alpha4)))
        {
            if (timer_MultipleSelectUsageTime <= 0)
            {
                if (!inMultipleSelected)
                {
                    EnterMultipleSelectMode();
                }
                else {
                    ExitMultipleSelectMode();
                }

                ResetTimer();
            }
            else
            {
                // 延迟进度环
                if (timer_MultipleSelectUsageTime <= (MULTIPLESELECTUSAGETIME - operateDelayTime))
                {
                    progressRing.fillAmount = ((MULTIPLESELECTUSAGETIME - operateDelayTime) - timer_MultipleSelectUsageTime) / (MULTIPLESELECTUSAGETIME - operateDelayTime);
                }
                timer_MultipleSelectUsageTime -= Time.deltaTime;
            }
        }

        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Alpha4)) ResetTimer();
        
        // 选择单个物品
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Selected(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Selected(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Selected(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Selected(4);
        }
    }

    // 选中物品
    public void Selected(int _index) 
    {
        //多选时 选择
        if (inMultipleSelected && item_Group[_index - 1] != null)
        {
            if (multipleSelectIndex.Contains(_index - 1))
            {
                // 暂用 取消多选选中提示
                backpack[_index - 1].GetComponent<Image>().color = Color.white;

                multipleSelectIndex.Remove(_index - 1);
            }
            else
            {
                // 暂用 多选选中提示
                backpack[_index - 1].GetComponent<Image>().color = Color.blue;

                multipleSelectIndex.Add(_index - 1);
            }
            return;
        }

        // 单选时重设选择提示
        ResetSelectedPrompt();

        //单选时选择
        if (inSelected)
        {
            item_Group[selectedIndex].SendMessage("Use_Reset", this.gameObject, SendMessageOptions.DontRequireReceiver);

            if (item_Group[_index - 1] != null && selectedIndex != _index - 1)
            {
                selectedIndex = _index - 1;

                // 暂用 UI选中提示
                backpack[_index - 1].GetComponent<Image>().color = Color.red;
                return;
            }
            inSelected = !inSelected; selectedIndex = -1;

            // 暂用 UI取消选中提示
            backpack[_index - 1].GetComponent<Image>().color = Color.white;
        }
        else
        {
            if (item_Group[_index - 1] == null) return;

            inSelected = !inSelected; selectedIndex = _index - 1;

            // 暂用 UI选择提示
            backpack[_index - 1].GetComponent<Image>().color = Color.red;
        }
    }

    // 重设所有选中提示
    public void ResetSelectedPrompt() {
        for (int i = 0; i < maxStorageAmount; i++) {
            backpack[i].GetComponent<Image>().color = Color.white;
        }
    }

    // 合成物品
    public void ComposeItem() {
        if (!inMultipleSelected) {
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            if (timer_ComposeItemUsageTime <= 0)
            {
                ResetTimer();

                // 检索合成表
                string compositeInfo = null;

                // 未选择大于 1 个的对象
                if (multipleSelectIndex.Count <= 1) {
                    Debug.Log("必须选择大于 1 个的对象");
                    ExitMultipleSelectMode();
                    return;
                }

                // 选择了包含重复 id 物品
                List<int> checkGroup = new List<int>();
                for (int k = 0; k < multipleSelectIndex.Count; k++) {
                    if (checkGroup.Contains(item_Id[multipleSelectIndex[k]]))
                    {
                        //TODO Reduce RestorageAmount
                        Debug.Log("无对应合成信息");
                        ExitMultipleSelectMode();
                        return;
                    }
                    else {
                        checkGroup.Add(item_Id[multipleSelectIndex[k]]);
                    }
                }

                // 检索合成信息
                switch (multipleSelectIndex.Count)
                {
                    case 2:
                        {
                            for (int i = 0; i < compositeTable.Length; i++)
                            {
                                if (compositeTable[i].Split(' ').Length - 1 != multipleSelectIndex.Count)
                                    continue;

                                if (compositeTable[i].Contains(item_Id[multipleSelectIndex[0]] + "-") && compositeTable[i].Contains(item_Id[multipleSelectIndex[1]] + "-"))
                                {
                                    Debug.Log("Find : " + compositeTable[i]);
                                    compositeInfo = compositeTable[i];
                                    break;
                                }
                            }
                            break;
                        }
                    case 3:
                        {
                            for (int i = 0; i < compositeTable.Length; i++)
                            {
                                if (compositeTable[i].Split(' ').Length - 1 != multipleSelectIndex.Count)
                                    continue;

                                if (compositeTable[i].Contains(item_Id[multipleSelectIndex[0]] + "-") && compositeTable[i].Contains(item_Id[multipleSelectIndex[1]] + "-") && compositeTable[i].Contains(item_Id[multipleSelectIndex[2]] + "-"))
                                {
                                    Debug.Log("Find : " + compositeTable[i]);
                                    compositeInfo = compositeTable[i];
                                    break;
                                }
                            }
                            break;
                        }
                    case 4:
                        {
                            for (int i = 0; i < compositeTable.Length; i++)
                            {
                                if (compositeTable[i].Split(' ').Length - 1 != multipleSelectIndex.Count)
                                    continue;

                                if (compositeTable[i].Contains(item_Id[multipleSelectIndex[0]] + "-") && compositeTable[i].Contains(item_Id[multipleSelectIndex[1]] + "-") && compositeTable[i].Contains(item_Id[multipleSelectIndex[3]] + "-") && compositeTable[i].Contains(item_Id[multipleSelectIndex[4]] + "-"))
                                {
                                    Debug.Log("Find : " + compositeTable[i]);
                                    compositeInfo = compositeTable[i];
                                    break;
                                }
                            }
                            break;
                        }
                }

                // 未找到合成信息
                if (compositeInfo == null)
                {
                    //TODO Reduce RestorageAmount
                    Debug.Log("无对应合成信息");
                    ExitMultipleSelectMode();
                    return;
                }

                // 开始数量检测
                string[] amount = compositeInfo.Split(' ');
                for (int i = 0; i < amount.Length - 1; i++)
                {
                    int itemId = int.Parse(amount[i].Split('-')[0]);
                    int itemAmount = int.Parse(amount[i].Split('-')[1]);

                    for (int j = 0; j < multipleSelectIndex.Count; j++)
                    {
                        if (item_Id[multipleSelectIndex[j]] != itemId) continue;

                        // 需求量不足
                        if (item_StorageAmount[multipleSelectIndex[j]] < itemAmount)
                        {
                            //TODO Reduce RestorageAmount
                            Debug.Log("材料不足");
                            ExitMultipleSelectMode();
                            return;
                        }
                    }
                }

                // 消耗材料合成
                for (int i = 0; i < amount.Length - 1; i++)
                {
                    int itemId = int.Parse(amount[i].Split('-')[0]);
                    int itemAmount = int.Parse(amount[i].Split('-')[1]);

                    for (int j = 0; j < multipleSelectIndex.Count; j++)
                    {
                        if (item_Id[multipleSelectIndex[j]] != itemId) continue;
                        ReflashItemAmount(multipleSelectIndex[j], 0, itemAmount);
                        break;
                    }
                }

                // 生成合成物品(临时使用Key代替)
                Debug.Log("合成成功 结果:" + amount[2].Split('=')[0] + " 数量: " + amount[2].Split('=')[1]);

                for (int i = 0; i < int.Parse(amount[2].Split('=')[1]); i++)
                {
                    GameObject item = LoadItemToScene(int.Parse(amount[2].Split('=')[0]));
                    Room room = GameObject.Find("RoomLoader").GetComponent<RoomLoader>().GetPlayerRoom();
                    item.transform.parent = room.transform;
                    item.transform.position = this.gameObject.transform.position;

                    item.SendMessage("Interact", this.gameObject, SendMessageOptions.DontRequireReceiver);
                }

                ExitMultipleSelectMode();
            }
            else {
                // 延迟进度环
                if (timer_ComposeItemUsageTime <= (COMPOSEITEMUSAGETIME - operateDelayTime))
                {
                    progressRing.fillAmount = ((COMPOSEITEMUSAGETIME - operateDelayTime) - timer_ComposeItemUsageTime) / (COMPOSEITEMUSAGETIME - operateDelayTime);
                }
                timer_ComposeItemUsageTime -= Time.deltaTime;
            }
        }

        if (Input.GetKeyUp(KeyCode.E)) { ResetTimer(); }
    }

    // 进入\退出 多选择模式
    public void EnterMultipleSelectMode() {
        inMultipleSelected = true;

        ResetSelectedPrompt();
        ResetTimer();

        //将单选的选项转移至多选中
        if (inSelected) {
            Selected(selectedIndex + 1);
        }

        inSelected = false;
        selectedIndex = -1;
    }
    public void ExitMultipleSelectMode() {
        multipleSelectIndex.Clear();
        inMultipleSelected = false;

        ResetSelectedPrompt();
        ResetTimer();
    }

    // 重设定时器
    private void ResetTimer() {
        progressRing.fillAmount = 0;
        timer_ComposeItemUsageTime = COMPOSEITEMUSAGETIME;
        timer_MultipleSelectUsageTime = MULTIPLESELECTUSAGETIME;
    }
}
