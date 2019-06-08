using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public static GameObject dialog;
    public static Text text;

    public static bool isTrigger = false;

    private void Awake()
    {
        dialog = GameObject.Find("Canvas_UI").transform.Find("Dialog").gameObject;
        text = dialog.transform.GetComponentInChildren<Text>();
        CloseDialog();
    }

    public static void ShowDialog(string _text) {
        if (!isTrigger)
        {
            dialog.SetActive(true);
            isTrigger = true;
        }

        text.text = _text;
    }

    public static void CloseDialog() {
        dialog.SetActive(false);
        text.text = null;
        isTrigger = false;
    }
}
