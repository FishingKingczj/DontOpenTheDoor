using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_SecretLockManager : MonoBehaviour
{
    public GameObject itemPosition;

    public List<Item_SecretLock> secretLock;
    public List<int> password;

    public int currentPasswordIndex = 0;

    public float pressureIncrementIfFailed = 20.0f;
    public int itemId = -1;

    public void Start()
    {
        if (secretLock.Count != password.Count) { Debug.LogError("子对象与密码本不一致"); }
        if (itemPosition == null) { Debug.LogError("无物品生成位置"); }
    }

    // 重设
    public void Reset()
    {
        Dialog.ShowDialog("Error,nothing happen");
        currentPasswordIndex = 0;

        for (int i = 0; i < secretLock.Count; i++)
        {
            secretLock[i].Reset();
        }
    }

    // 配对当前Key值
    public bool Pair(int _key,GameObject _user) {
        if (_key != password[currentPasswordIndex])
        {
            Reset();
            _user.GetComponent<Player_BuffManager>().AddPressurePoint(pressureIncrementIfFailed);
            return false;
        }
        else {
            currentPasswordIndex++;


            if (currentPasswordIndex == password.Count)
            {
                Unlock();
            }
            return true;
        }
    }

    // 解锁
    public void Unlock() {
        GameObject go = ItemLoader.LoadItemToScene(itemId);
        go.transform.SetParent(this.gameObject.transform.parent);
        go.transform.position = itemPosition.transform.position;
    }
}
