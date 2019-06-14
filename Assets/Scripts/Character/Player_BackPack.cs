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

    public List<int> selectIndex;

    public bool inUsed = false;

    public bool inOperated = false;

    public Vector2 interact_Pivot;
    public float interact_Radius;

    [Header("Composite Vairable")]
    public string[] compositeTable;
    public bool inCompositeMode = false;

    public const float COMPOSEITEMUSAGETIME = 1.0f;
    public float timer_ComposeItemUsageTime = COMPOSEITEMUSAGETIME;

    void Start()
    {
        //获取互动按钮轴心和半径
        var obj = GameObject.Find("Canvas_UI").transform.Find("Button_Interact").gameObject;
        interact_Pivot = obj.GetComponent<Transform>().position;
        interact_Radius = obj.GetComponent<RectTransform>().sizeDelta.x * obj.GetComponent<RectTransform>().localScale.x;

        //interact_Pivot = GameObject.Find("Button_Interact").GetComponent<Transform>().position;
        //interact_Radius = GameObject.Find("Button_Interact").GetComponent<RectTransform>().sizeDelta.x * GameObject.Find("Button_Interact").GetComponent<RectTransform>().localScale.x;

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
    }

    void FixedUpdate()
    {
        #if UNITY_ANDROID
            UseItem(1); // 移动端(带任意整形参数) 电脑端(无参)
            ComposeItem(1);// 移动端(带任意整形参数) 电脑端(无参)
        #endif

        #if UNITY_IPHONE
        #endif

        #if UNITY_STANDALONE_WIN
            UseItem();
            ComposeItem();
            SelectItem();
            DiscardItem();
        #endif
    }

    // 添加道具 true->背包有余位 false->背包已满 (含特殊处理)
    public bool AddItem(GameObject _item, int _maxStorageAmount, int _id, string _name, string _description) {
        // 尝试寻找重复道具并叠加
        for (int i = 0; i < maxStorageAmount; i++) {
            if (item_Name[i] == _name && item_StorageAmount[i] < _maxStorageAmount)
            {
                ReflashItemAmount(i,1);
                //拾取成功 破坏源物品
                _item.SendMessage("DestoryItem");
                return true;
            }
        }

        // 尝试寻找空位置
        for (int j = 0; j < maxStorageAmount; j++) {
            if (item_Group[j] == null)
            {
                GameObject item = ItemLoader.LoadItemToBackpack(_id);
                if (item == null) return false;

                // 物品信息更新
                item.transform.SetParent(backpack[j].transform);
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                float width = backpack[j].GetComponent<RectTransform>().sizeDelta.x;
                float height = backpack[j].GetComponent<RectTransform>().sizeDelta.y;
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 10, height - 10);
                item.transform.SetAsFirstSibling();

                // 特殊物品处理
                if (_name.Equals("Key"))
                {
                    item.GetComponent<Item_Key>().pairingValue = _item.GetComponent<Item_Key>().GetPairingValue();
                }
                else if (_name.Equals("Transporter"))
                {
                    item.GetComponent<Item_Transporter>().SetStartPosition(_item.GetComponent<Item_Transporter>().GetStartPosition());
                    item.GetComponent<Item_Transporter>().SetEndPosition(_item.GetComponent<Item_Transporter>().GetEndPosition());
                }
                else if (_name.Equals("RemoteKey"))
                {
                    item.GetComponent<Item_RemoteKey>().SetTargetDoor(_item.GetComponent<Item_RemoteKey>().GetTargetDoor());
                    item.GetComponent<Item_RemoteKey>().SetTriggerPosition(_item.GetComponent<Item_RemoteKey>().GetTriggerPosition());
                }

                item_Group[j] = item;
                ReflashItemAmount(j,1);
                item_Id[j] = _id;
                item_Name[j] = _name;
                item_Description[j] = _description;

                numberOfItem++;

                //拾取成功 破坏源物品
                _item.SendMessage("DestoryItem");
                return true;
            }
        }

        return false;
    }

    // 删除物品
    public void RemoveItem(int _index) {
        selectIndex.Remove(_index);
        inSelected = false;

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
        if (inUsed || selectIndex.Count != 1) return;

        if (inSelected) {
            if (Input.GetKeyDown(KeyCode.G))
            {
                GameObject item = ItemLoader.LoadItemToScene(item_Id[selectIndex[0]]);
                // 需要把物体的加到Room里
                Room room = GameObject.Find("RoomLoader").GetComponent<RoomLoader>().GetPlayerRoom();
                item.transform.parent = room.transform;
                item.transform.position = this.gameObject.transform.position;

                // 特殊处理
                if (item_Name[selectIndex[0]].Equals("Key"))
                {
                    item.GetComponent<Item_Key>().SetPairingValue(item_Group[selectIndex[0]].GetComponent<Item_Key>().GetPairingValue());
                }
                else if (item_Name[selectIndex[0]].Equals("Transporter"))
                {
                    item.GetComponent<Item_Transporter>().SetStartPosition(item_Group[selectIndex[0]].GetComponent<Item_Transporter>().GetStartPosition());
                    item.GetComponent<Item_Transporter>().SetEndPosition(item_Group[selectIndex[0]].GetComponent<Item_Transporter>().GetEndPosition());
                }
                else if (item_Name[selectIndex[0]].Equals("RemoteKey")) {
                    item.GetComponent<Item_RemoteKey>().SetTargetDoor(item_Group[selectIndex[0]].GetComponent<Item_RemoteKey>().GetTargetDoor());
                    item.GetComponent<Item_RemoteKey>().SetTriggerPosition(item_Group[selectIndex[0]].GetComponent<Item_RemoteKey>().GetTriggerPosition());
                }

                ReflashItemAmount(selectIndex[0], 0);
            }
        }
    }

    // 丢弃物品(包含特殊处理) (移动端)
    public void DiscardItem(int _index)
    {
        if (inUsed || selectIndex.Count != 1) return;
        
        if (inSelected && selectIndex.Count == 1)
        {
            GameObject item = ItemLoader.LoadItemToScene(item_Id[selectIndex[0]]);
            // 需要把物体的加到Room里
            Room room = GameObject.Find("RoomLoader").GetComponent<RoomLoader>().GetPlayerRoom();
            
            item.transform.parent = room.transform;
            item.transform.position = this.gameObject.transform.position;

            // 特殊处理
            if (item_Name[selectIndex[0]].Equals("Key"))
            {
                item.GetComponent<Item_Key>().SetPairingValue(item_Group[selectIndex[0]].GetComponent<Item_Key>().GetPairingValue());
            }
            else if (item_Name[selectIndex[0]].Equals("Transporter"))
            {
                item.GetComponent<Item_Transporter>().SetStartPosition(item_Group[selectIndex[0]].GetComponent<Item_Transporter>().GetStartPosition());
                item.GetComponent<Item_Transporter>().SetEndPosition(item_Group[selectIndex[0]].GetComponent<Item_Transporter>().GetEndPosition());
            }
            else if (item_Name[selectIndex[0]].Equals("RemoteKey"))
            {
                item.GetComponent<Item_RemoteKey>().SetTargetDoor(item_Group[selectIndex[0]].GetComponent<Item_RemoteKey>().GetTargetDoor());
                item.GetComponent<Item_RemoteKey>().SetTriggerPosition(item_Group[selectIndex[0]].GetComponent<Item_RemoteKey>().GetTriggerPosition());
            }

            ReflashItemAmount(selectIndex[0], 0);
        }
    }

    // 使用物品
    public void UseItem() {
        
        if (inSelected && selectIndex.Count == 1)
        {
            if (Input.GetKey(KeyCode.E) && !inOperated) {
                inUsed = true;
                item_Group[selectIndex[0]].SendMessage("Use", this.gameObject, SendMessageOptions.DontRequireReceiver);
            } else if (Input.GetKeyUp(KeyCode.E)) {
                inOperated = false;
                inUsed = false;
                item_Group[selectIndex[0]].SendMessage("Use_Reset", this.gameObject, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    // 使用物品(移动端)
    public void UseItem(int _index)
    {
        if (inSelected && selectIndex.Count == 1)
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    float x = Input.GetTouch(i).position.x;
                    float y = Input.GetTouch(i).position.y;

                    if ((x > interact_Pivot.x - interact_Radius) && (x < interact_Pivot.x + interact_Radius) && (y > interact_Pivot.y - interact_Radius) && (y < interact_Pivot.y + interact_Radius))
                    {
                        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId) && !inOperated)
                        {
                            inUsed = true;
                            item_Group[selectIndex[0]].SendMessage("Use", this.gameObject, SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }
        }
    }

    // 物品使用成功 (是否消耗当前物品)
    public void UseSucceed(bool _delete = true)
    {
        inOperated = true;
        inUsed = false;

        if(_delete)
        ReflashItemAmount(selectIndex[0], 0);
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
                Dialog.CloseDialog();
                RemoveItem(_index);
                GameObject.Find("Canvas_UI").transform.Find("Button_Discard").gameObject.SetActive(false);
            }
        }

    }

    // 选择物品
    public void SelectItem() {
        if (inUsed) return;

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
        if (inUsed || inCompositeMode) return;

        else {
            Dialog.CloseDialog();
        }

        if (item_Group[_index - 1] != null) {
            // 选择
            if (!selectIndex.Contains(_index - 1))
            {
                // 选中提示
                Color c = backpack[_index - 1].GetComponent<Image>().color;
                c.a = 1;
                backpack[_index - 1].GetComponent<Image>().color = c;

                selectIndex.Add(_index - 1);
            }
            // 取消选择
            else {

                // 取消选中提示
                Color c = backpack[_index - 1].GetComponent<Image>().color;
                c.a = 0;
                backpack[_index - 1].GetComponent<Image>().color = c;

                selectIndex.Remove(_index - 1);
            }
        }

        if (selectIndex.Count != 0) inSelected = true;
        else inSelected = false;

        // 调用丢弃按钮与文本框
        if (selectIndex.Count == 1)
        {
            GameObject.Find("Canvas_UI").transform.Find("Button_Discard").gameObject.SetActive(true);
            Dialog.ShowDialog(item_Description[selectIndex[0]]);
        }
        else {
            GameObject.Find("Canvas_UI").transform.Find("Button_Discard").gameObject.SetActive(false);
            Dialog.CloseDialog();
        }

        // 根据选择数切换按钮
        if (selectIndex.Count >= 2)
        {
            GameObject.Find("Canvas_UI").transform.Find("Button_Compose").gameObject.SetActive(true);
        }
        else {
            GameObject.Find("Canvas_UI").transform.Find("Button_Compose").gameObject.SetActive(false);
        }
    }

    // 重设所有选中提示
    public void ResetSelectedPrompt() {
        for (int i = 0; i < maxStorageAmount; i++) {
            Color c = backpack[i].GetComponent<Image>().color;
            c.a = 0;
            backpack[i].GetComponent<Image>().color = c;
        }
    }
        
    // 合成物品
    public void ComposeItem() {
        if (selectIndex.Count < 2) return;

        if (Input.GetKey(KeyCode.E))
        {
            if (timer_ComposeItemUsageTime <= 0)
            {
                CancelOperation();

                // 检索合成表
                string compositeInfo = null;

                // 未选择大于 1 个的对象
                if (selectIndex.Count <= 1) {
                    Debug.Log("必须选择大于 1 个的对象");
                    ExitMultipleSelectMode();
                    return;
                }

                // 选择了包含重复 id 物品
                List<int> checkGroup = new List<int>();
                for (int k = 0; k < selectIndex.Count; k++) {
                    if (checkGroup.Contains(item_Id[selectIndex[k]]))
                    {
                        //TODO Reduce RestorageAmount
                        Debug.Log("无对应合成信息");
                        ExitMultipleSelectMode();
                        return;
                    }
                    else {
                        checkGroup.Add(item_Id[selectIndex[k]]);
                    }
                }

                // 检索合成信息
                switch (selectIndex.Count)
                {
                    case 2:
                        {
                            for (int i = 0; i < compositeTable.Length; i++)
                            {
                                if (compositeTable[i].Split(' ').Length - 1 != selectIndex.Count)
                                    continue;

                                if (compositeTable[i].Contains(item_Id[selectIndex[0]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[1]] + "-"))
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
                                if (compositeTable[i].Split(' ').Length - 1 != selectIndex.Count)
                                    continue;

                                if (compositeTable[i].Contains(item_Id[selectIndex[0]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[1]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[2]] + "-"))
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
                                if (compositeTable[i].Split(' ').Length - 1 != selectIndex.Count)
                                    continue;

                                if (compositeTable[i].Contains(item_Id[selectIndex[0]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[1]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[3]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[4]] + "-"))
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

                    for (int j = 0; j < selectIndex.Count; j++)
                    {
                        if (item_Id[selectIndex[j]] != itemId) continue;

                        // 需求量不足
                        if (item_StorageAmount[selectIndex[j]] < itemAmount)
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

                    for (int j = 0; j < selectIndex.Count; j++)
                    {
                        if (item_Id[selectIndex[j]] != itemId) continue;
                        ReflashItemAmount(selectIndex[j], 0, itemAmount);
                        break;
                    }
                }

                // 生成合成物品(临时使用Key代替)
                Debug.Log("合成成功 结果:" + amount[2].Split('=')[0] + " 数量: " + amount[2].Split('=')[1]);

                for (int i = 0; i < int.Parse(amount[2].Split('=')[1]); i++)
                {
                    GameObject item = ItemLoader.LoadItemToScene(int.Parse(amount[2].Split('=')[0]));
                    Room room = GameObject.Find("RoomLoader").GetComponent<RoomLoader>().GetPlayerRoom();
                    item.transform.parent = room.transform;
                    item.transform.position = this.gameObject.transform.position;

                    item.SendMessage("Interact", this.gameObject, SendMessageOptions.DontRequireReceiver);
                }

                ExitMultipleSelectMode();
            }
            else {
                // 延迟进度环
                if (timer_ComposeItemUsageTime <= (COMPOSEITEMUSAGETIME))
                {
                    ProgressRing.Use(((COMPOSEITEMUSAGETIME) - timer_ComposeItemUsageTime) / (COMPOSEITEMUSAGETIME));
                }
                timer_ComposeItemUsageTime -= Time.deltaTime;
                inCompositeMode = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.E)) { CancelOperation(); }
    }

    // 合成物品(移动端)
    public void ComposeItem(int _index)
    {
        if (selectIndex.Count < 2) return;

        if (Input.touchCount > 0)
        {
            for (int q = 0; q < Input.touchCount; q++)
            {
                float x = Input.GetTouch(q).position.x;
                float y = Input.GetTouch(q).position.y;

                if ((x > interact_Pivot.x - interact_Radius) && (x < interact_Pivot.x + interact_Radius) && (y > interact_Pivot.y - interact_Radius) && (y < interact_Pivot.y + interact_Radius))
                {
                    if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(q).fingerId))
                    {
                        if (timer_ComposeItemUsageTime <= 0)
                        {
                            // 检索合成表
                            string compositeInfo = null;

                            // 选择了包含重复 id 物品
                            List<int> checkGroup = new List<int>();
                            for (int k = 0; k < selectIndex.Count; k++)
                            {
                                if (checkGroup.Contains(item_Id[selectIndex[k]]))
                                {
                                    //TODO Reduce RestorageAmount
                                    Debug.Log("无对应合成信息");
                                    ExitMultipleSelectMode();
                                    return;
                                }
                                else
                                {
                                    checkGroup.Add(item_Id[selectIndex[k]]);
                                }
                            }

                            // 检索合成信息
                            switch (selectIndex.Count)
                            {
                                case 2:
                                    {
                                        for (int i = 0; i < compositeTable.Length; i++)
                                        {
                                            if (compositeTable[i].Split(' ').Length - 1 != selectIndex.Count)
                                                continue;

                                            if (compositeTable[i].Contains(item_Id[selectIndex[0]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[1]] + "-"))
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
                                            if (compositeTable[i].Split(' ').Length - 1 != selectIndex.Count)
                                                continue;

                                            if (compositeTable[i].Contains(item_Id[selectIndex[0]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[1]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[2]] + "-"))
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
                                            if (compositeTable[i].Split(' ').Length - 1 != selectIndex.Count)
                                                continue;

                                            if (compositeTable[i].Contains(item_Id[selectIndex[0]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[1]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[3]] + "-") && compositeTable[i].Contains(item_Id[selectIndex[4]] + "-"))
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

                                for (int j = 0; j < selectIndex.Count; j++)
                                {
                                    if (item_Id[selectIndex[j]] != itemId) continue;

                                    // 需求量不足
                                    if (item_StorageAmount[selectIndex[j]] < itemAmount)
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

                                for (int j = 0; j < selectIndex.Count; j++)
                                {
                                    if (item_Id[selectIndex[j]] != itemId) continue;
                                    ReflashItemAmount(selectIndex[j], 0, itemAmount);
                                    break;
                                }
                            }

                            // 生成合成物品(临时使用Key代替)
                            Debug.Log("合成成功 结果:" + amount[2].Split('=')[0] + " 数量: " + amount[2].Split('=')[1]);

                            for (int i = 0; i < int.Parse(amount[2].Split('=')[1]); i++)
                            {
                                GameObject item = ItemLoader.LoadItemToScene(int.Parse(amount[2].Split('=')[0]));
                                Room room = GameObject.Find("RoomLoader").GetComponent<RoomLoader>().GetPlayerRoom();
                                item.transform.parent = room.transform;
                                item.transform.position = this.gameObject.transform.position;

                                item.SendMessage("Interact", this.gameObject, SendMessageOptions.DontRequireReceiver);
                            }

                            ExitMultipleSelectMode();
                        }
                        else
                        {
                            // 延迟进度环
                            if (timer_ComposeItemUsageTime <= (COMPOSEITEMUSAGETIME))
                            {
                                ProgressRing.Use(((COMPOSEITEMUSAGETIME) - timer_ComposeItemUsageTime) / (COMPOSEITEMUSAGETIME));
                            }
                            inCompositeMode = true;
                            timer_ComposeItemUsageTime -= Time.deltaTime;
                        }
                    }
                }
            }
        }
       
    }

    //退出多选模式
    public void ExitMultipleSelectMode() {
        selectIndex.Clear();
        inSelected = false;

        ResetSelectedPrompt();
        CancelOperation();

        inCompositeMode = false;

        GameObject.Find("Canvas_UI").transform.Find("Button_Interact").gameObject.SetActive(true);
        GameObject.Find("Canvas_UI").transform.Find("Button_Compose").gameObject.SetActive(false);
    }

    // 重设定时器(取消操作)
    public void CancelOperation() {
        ProgressRing.Reset();
        timer_ComposeItemUsageTime = COMPOSEITEMUSAGETIME;

        inOperated = false;

        if (inUsed) {
            inUsed = false;
            item_Group[selectIndex[0]].SendMessage("Use_Reset", this.gameObject, SendMessageOptions.DontRequireReceiver);
        }

        if (inCompositeMode)
        {
            inCompositeMode = false;
        }
    }

    public bool GetInCompositeMode() { return inCompositeMode; }

    // 禁用背包
    public void DisableBackpack() {
        ExitMultipleSelectMode();

        GameObject.Find("Canvas_Backpack").transform.Find("Panel_Box").gameObject.SetActive(false);
    }

    // 启用背包
    public void EnableBackpack() {
        GameObject.Find("Canvas_Backpack").transform.Find("Panel_Box").gameObject.SetActive(true);
    }
}
