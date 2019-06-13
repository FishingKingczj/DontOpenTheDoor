using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTE_Feedback : MonoBehaviour
{
    public float destoryTime;
    private float timer_DestoryTime;
    public Image image;

    // Start is called before the first frame update
    void Awake()
    {
        destoryTime = 0.7f;
        timer_DestoryTime = destoryTime;
        image = this.GetComponent<Image>();

        Vector2 v2 = this.GetComponent<RectTransform>().sizeDelta;
        v2.x = Camera.main.pixelWidth;
        v2.y = Camera.main.pixelHeight;
        this.GetComponent<RectTransform>().sizeDelta = v2;

        this.transform.SetParent(GameObject.Find("Canvas_UI").GetComponent<Transform>());

        this.transform.position = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (timer_DestoryTime <= 0)
        {
            Destroy(this.gameObject);
        }
        else {
            Color c = image.color;
            c.a = timer_DestoryTime / destoryTime;
            image.color = c;

            timer_DestoryTime -= Time.deltaTime;
        }
    }

    /*
     * 0-翻滚
     * 1-普通格挡
     * 2-受击
     * 3-格挡受击
     * 4-完美格挡
     * 
     * 方向默认向左
     */
     
    public void SetSprite(int _id,int _dir = 0) {
        switch (_id) {
            case 0: {
                    if(_dir == 0)
                        image.sprite = Resources.Load<Sprite>("Image/UI/QTE/左翻滚");
                    else
                        image.sprite = Resources.Load<Sprite>("Image/UI/QTE/右翻滚");
                    break;
                }
            case 1:
                {
                    image.sprite = Resources.Load<Sprite>("Image/UI/QTE/普通格挡");
                    break;
                }
            case 2:
                {
                    image.sprite = Resources.Load<Sprite>("Image/UI/QTE/受击");
                    break;
                }
            case 3:
                {
                    image.sprite = Resources.Load<Sprite>("Image/UI/QTE/格挡受击");
                    break;
                }
            case 4:
                {
                    image.sprite = Resources.Load<Sprite>("Image/UI/QTE/完美格挡");
                    break;
                }
            default: {
                    break;
                }
        }
    }
}
