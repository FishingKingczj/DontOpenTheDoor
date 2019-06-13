using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BattleBackgroup : MonoBehaviour
{
    public float initialTime = 1.0f;
    public float changeTime = 0.7f;
    public float timer;

    public Image image;
    public Sprite image1;
    public Sprite image2;
    public int currentImage;

    public GameObject clickTip;

    // Start is called before the first frame update
    void Start()
    {
        timer = changeTime;

        currentImage = 1;
        image1 = Resources.Load<Sprite>("Image/UI/QTE/提示画面1");
        image2 = Resources.Load<Sprite>("Image/UI/QTE/提示画面2");

        clickTip = transform.Find("ClickTip").gameObject;
        image = clickTip.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (initialTime <= 0)
        {
            if (!clickTip.activeSelf) {
                clickTip.SetActive(true);
            }

            if (timer <= 0)
            {
                timer = changeTime;

                if (currentImage == 1)
                {
                    image.sprite = image2;
                    currentImage = 2;
                }
                else
                {
                    image.sprite = image1;
                    currentImage = 1;
                }
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
        else {
            initialTime -= Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        clickTip.SetActive(false);
        timer = changeTime;
        currentImage = 1;
        initialTime = 1.0f;
    }
}
