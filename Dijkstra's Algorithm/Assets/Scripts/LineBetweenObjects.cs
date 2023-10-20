using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineBetweenObjects : MonoBehaviour
{
    private Image image;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    public void Initialize()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void DrawLine(Vector3 pos1, Vector3 pos2)
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        rectTransform.localPosition = ((pos1 - screenCenter) + (pos2 - screenCenter))/2;
        Vector3 dif = pos1 - pos2;
        rectTransform.sizeDelta = new Vector3(dif.magnitude - 1f, 5);
        if(dif.x != 0)
        {
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
