using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Reference to the GraphManager for this level
    [SerializeField] private GraphManager graphManager;

    private void Start()
    {
        // Ensure the GraphManager is set when the level is active
        if (graphManager == null)
        {
            Debug.LogError("GraphManager is not assigned for this level.");
            return;
        }
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if (hit.collider != null)
            {
                Node2D touchedNode = hit.collider.GetComponent<Node2D>();
                if (touchedNode != null && graphManager != null)
                {
                    graphManager.MoveToNode(touchedNode);
                }
                else
                {
                    Debug.LogError("Touched Node2D or GraphManager is null.");
                }
            }
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                Node2D clickedNode = hit.collider.GetComponent<Node2D>();
                if (clickedNode != null && graphManager != null)
                {
                    graphManager.MoveToNode(clickedNode);
                }
                else
                {
                    Debug.LogError("Clicked Node2D or GraphManager is null.");
                }
            }
        }
#endif
    }
}
