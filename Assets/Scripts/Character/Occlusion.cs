using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occlusion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "背景墙" || collision.name == "左侧墙" || collision.name == "右侧墙") return;
        if (collision.name == "前景墙") {
            collision.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        }
        else if (collision.tag.Contains("NPC") || collision.tag.Contains("Item_Collision"))
        {
            if (collision.transform.position.y < this.transform.position.y)
            {
                collision.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            }
            else {
                if (collision.GetComponent<SpriteRenderer>().sortingLayerName.Equals("BeforePlayer"))
                    collision.GetComponent<SpriteRenderer>().sortingLayerName = "AAfterPlayer";
                else
                    collision.GetComponent<SpriteRenderer>().sortingLayerName = "AfterPlayer";
            }

            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "背景墙" || collision.name == "左侧墙" || collision.name == "右侧墙") return;
        if (collision.tag.Contains("NPC") || collision.tag.Contains("Item_Collision"))
        {
            collision.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            if (collision.GetComponent<SpriteRenderer>().sortingLayerName.Equals("AfterPlayer"))
                collision.GetComponent<SpriteRenderer>().sortingLayerName = "BBeforePlayer";
            else
                collision.GetComponent<SpriteRenderer>().sortingLayerName = "BeforePlayer";
        }
    }
}
