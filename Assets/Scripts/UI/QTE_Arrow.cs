using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTE_Arrow : MonoBehaviour
{
    public float time_BeforeAttack;
    public float time_Attack;

    public float timer_BeforeAttack;
    public float timer_Attack;

    public Image image;

    public bool inStart = false;

    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<Image>();
        image.fillAmount = 0;
        image.fillOrigin = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!inStart) return;

        if (timer_BeforeAttack <= 0) {
            image.fillOrigin = 1;

            if (timer_Attack <= 0)
            {
                Destroy(this.gameObject);
            }
            else {
                timer_Attack -= Time.deltaTime;
                image.fillAmount -= (Time.deltaTime / time_Attack);
            }
        }
        else{
            Color color = image.color;
            timer_BeforeAttack -= (Time.deltaTime);
            color.g -= (Time.deltaTime / time_BeforeAttack);
            color.b -= (Time.deltaTime / time_BeforeAttack);
            image.color = color;

            image.fillAmount += (Time.deltaTime / time_BeforeAttack);
        }
    }


    //dir 0->左攻击 1->右攻击 2->全屏
    public void Initial(float _time_BeforeAttack, float _time_Attack,int _dir)
    {
        
        inStart = true;

        time_BeforeAttack = _time_BeforeAttack;
        time_Attack = _time_Attack;

        timer_BeforeAttack = time_BeforeAttack;
        timer_Attack = time_Attack;

        if (_dir == 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (_dir == 2) {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }
}
