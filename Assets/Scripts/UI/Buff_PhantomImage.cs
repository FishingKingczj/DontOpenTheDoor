using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff_PhantomImage : MonoBehaviour
{
    public float duration = 0.3f;
    private float timer_Duration;
    private Image image;

    private void Start()
    {
        image = this.GetComponent<Image>();
        transform.position = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0);
        transform.SetParent(GameObject.Find("Canvas_UI").transform);

        string path = "Image/Buff/Phantom/Image_" + Random.Range(1, 5).ToString();// 此处Range最大值根据实际情况修改
        image.sprite = Resources.Load<Sprite>(path);

        timer_Duration = duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer_Duration <= 0)
        {
            Destroy(this.gameObject);
        }
        else {
            Color c = image.color;
            c.a = timer_Duration / duration;
            timer_Duration -= Time.deltaTime;
            image.color = c;
        }
    }
}
