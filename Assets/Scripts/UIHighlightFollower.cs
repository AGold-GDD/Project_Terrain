using UnityEngine;

public class UIHighlightFollower : MonoBehaviour
{

    public Transform target3DObject;  // The terrain editing location
    public RectTransform uiElement;   // The UI Image RectTransform

    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target3DObject.position);

        // Optional: check if target is in front of camera
        if (screenPos.z > 0)
        {
            uiElement.position = screenPos;
            uiElement.gameObject.SetActive(true);
        }
        else
        {
            uiElement.gameObject.SetActive(false); // Hide if behind camera
        }
    }
}
