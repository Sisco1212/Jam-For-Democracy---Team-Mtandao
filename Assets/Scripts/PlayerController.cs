using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Manages the player input
    public GraphManager graphManager;

    private void Start()
    {
        // Make sure the graphManager is set up correctly at the start
        // If the level has already instantiated the GraphManager, find it in the current level
        graphManager = FindObjectOfType<GraphManager>(); // Automatically finds the GraphManager in the scene
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); //get the first touch
            // Convert touch position to world space
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Debug.Log("Touch position: " + touchPos);

            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("Hit a collider: " + hit.collider.name);

                Node2D touchedNode = hit.collider.GetComponent<Node2D>();
                if (touchedNode != null)
                {
                    Debug.Log("Touched Node2D: " + touchedNode.name);
                    graphManager.MoveToNode(touchedNode);
                }
                else
                {
                    Debug.Log("Hit object is not a Node2D."); // Debug log if the hit object is not a Node2D
                }
            }
        }
#else
        // If the platform does not support touch (e.g., desktop)
        // Check if the left mouse button is clicked (0 is the left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Mouse position: " + mousePos);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if(hit.collider != null)
            { 
                Debug.Log("Hit a collider: " + hit.collider.name);

                Node2D clickedNode = hit.collider.GetComponent<Node2D>();
                if(clickedNode != null)
                {
                    Debug.Log("Clicked Node2D: " + clickedNode.name);
                    graphManager.MoveToNode(clickedNode);
                }
                else
                {
                   Debug.Log("Hit object is not a Node2D."); // Debug log if the hit object is not a Node2D
                }
            }
            else
                {
                    Debug.Log("No collider hit."); // Debug log if the raycast hit nothing
                }
        }
#endif
    }
}
