using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLoader : MonoBehaviour
{
    // 加载物品到场景中
    public static GameObject LoadItemToScene(int _id)
    {
        switch (_id)
        {
            case 0:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/SceneItem_Food"));
                }
            case 1:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/SceneItem_Key"));
                }
            case 3:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/SceneItem_RemoteKey"));
                }
            case 4:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/SceneItem_Tranquilizer"));
                }
            case 5:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/SceneItem_Transporter"));
                }
            case 6:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/SceneItem_Candle"));
                }
            case 7:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/SceneItem_Invisibility"));
                }
            case 8:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Scene_Item/SceneItem_SpecialFood"));
                }
            default:
                {
                    return null;
                }
        }
    }

    // 加载物品到背包中
    public static GameObject LoadItemToBackpack(int _id) {
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
            case 3:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Backpack_Item/Item_RemoteKey"));
                }
            case 4:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Backpack_Item/Item_Tranquilizer"));
                }
            case 5:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Backpack_Item/Item_Transporter"));
                }
            case 6:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Backpack_Item/Item_Candle"));
                }
            case 7:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Backpack_Item/Item_Invisibility"));
                }
            case 8:
                {
                    return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Backpack_Item/Item_SpecialFood"));
                }
            default:
                {
                    return null;
                }
        }
    }
}
