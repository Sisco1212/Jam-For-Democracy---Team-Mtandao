using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Manages the player input
    public GraphManager graphManager;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if(hit.collider != null)
            {
                Node2D clickedNode = hit.collider.GetComponent<Node2D>();
                if(clickedNode != null)
                {
                    graphManager.MoveToNode(clickedNode);
                }
            }
        }
    }
}
