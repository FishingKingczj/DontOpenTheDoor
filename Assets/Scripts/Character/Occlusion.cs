using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Occlusion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("NPC") || collision.tag.Contains("Item_Collision"))
        {
            if (collision.transform.position.y < this.transform.position.y)
            {
                collision.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            }
            else {
                /*if (collision.GetComponent<SpriteRenderer>().sortingLayerName.Equals("BBeforePlayer"))
                    collision.GetComponent<SpriteRenderer>().sortingLayerName = "AfterPlayer";
                else
                    collision.GetComponent<SpriteRenderer>().sortingLayerName = "AAfterPlayer";*/
            }

            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Contains("NPC") || collision.tag.Contains("Item_Collision"))
        {
            /*if (collision.GetComponent<SpriteRenderer>().sortingLayerName.Equals("AfterPlayer"))
                collision.GetComponent<SpriteRenderer>().sortingLayerName = "BBeforePlayer";
            else
                collision.GetComponent<SpriteRenderer>().sortingLayerName = "BeforePlayer";*/
        }
    }
}
