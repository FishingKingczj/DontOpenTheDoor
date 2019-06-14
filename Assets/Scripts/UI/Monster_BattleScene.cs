using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_BattleScene : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    public void SetEnable(bool _value) {
        if (_value)
        {
            Color c = this.GetComponent<Image>().color;
            c.a = 1;
            this.GetComponent<Image>().color = c;

            ResetAnimation();
        }
        else {
            Color c = this.GetComponent<Image>().color;
            c.a = 0;
            this.GetComponent<Image>().color = c;

            ResetAnimation();
        }
    }

    public void PlayAnimation(int _attackType) {
        animator.SetInteger("AttackType", _attackType);
    }

    public void ResetAnimation() {
        animator.SetInteger("AttackType", -1);
    }
}
