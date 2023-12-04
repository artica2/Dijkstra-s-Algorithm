using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.AnnotationUtility;

public class LineBetweenObjects : MonoBehaviour
{
    private Image image;
    private RectTransform rectTransform;
    Sprite roadSprite;
    
    private float length = 1f;

    // Start is called before the first frame update
    public void Initialize()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        GraphBuilder Graph = GraphBuilder.instance;
        roadSprite = Graph.road;
        // SpriteRenderer spriteRend = image.GetComponent<SpriteRenderer>();
        // spriteRend.sprite = roadSprite;
        // spriteRend.size = new Vector2(1, length);
        image.sprite = roadSprite;
        
    }

    public void DrawLine(Vector3 pos1, Vector3 pos2, bool needsToRecalibrate, string annotationText = null) // some UI components go from bottom left, some go from middle
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 pos1Copy = pos1;
        Vector3 pos2Copy = pos2;
        Vector3 dif = pos1Copy - pos2Copy;
        Vector3 rectPos = new Vector3(0f,0f,0f);
        if (needsToRecalibrate)
        {
            rectPos = ((pos1Copy - screenCenter) + (pos2Copy - screenCenter)) / 2;
        }
        else
        {
            rectPos = (pos1Copy + pos2Copy) / 2;
        }
        rectTransform.sizeDelta = new Vector3(dif.magnitude - 120f, 100
);
        rectTransform.localPosition = rectPos;        
        
        if(dif.x != 0)
        {
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }

        if (annotationText != null)
        {
            GameObject annotationObject = new GameObject("Annotation");
            annotationObject.transform.position = rectTransform.position;
            annotationObject.transform.position = rectPos; // Make it a child of the same parent as your line object
            annotationObject.transform.SetParent(GraphBuilder.instance.canvas.transform);
            // Create a Text component
            Text text = annotationObject.AddComponent<Text>();
            // Set the text content
            text.text = annotationText;
            text.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            text.fontSize = 30;
            text.color = new Color(1, 0, 1, 1);
        }
    }

        // Update is called once per frame
        void Update()
    {
        
    }
}
