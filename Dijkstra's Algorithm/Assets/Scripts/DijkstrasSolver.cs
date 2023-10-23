using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DijkstrasSolver : MonoBehaviour
{
    public GameObject rowPrefab;
    public GameObject spawner;
    public float rowSpacing;



    private string leftString;
    private string middleString;
    private string rightString;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void BuildTable()
    {
        // make the top row
        Vector3 startPos = spawner.transform.position;
        GameObject topRow = Instantiate(rowPrefab, spawner.transform.position, Quaternion.identity, spawner.transform);
        leftString = "Node Name";
        middleString = "Minimum distance";
        rightString = "Previous Node";
        SetTextValues(topRow, leftString, middleString, rightString);

        for (int i = 0; i < GraphBuilder.instance.nodes.Count; i++)
        {
            GraphNode node = GraphBuilder.instance.nodes[i];
            leftString = node.nodeName;
            if (node.minimumCost < 100)
            {
                middleString = node.minimumCost.ToString();
            } else 
            { 
                middleString = "infinity";
            }
            if (node.prevNode == null)
            {
                rightString = "No path";
            } else
            {
                rightString = node.prevNode.nodeName;
            }
            Vector3 position = new Vector3(0, -(i + 1) * rowSpacing, 0) + spawner.transform.position;
            GameObject uiElement = Instantiate(rowPrefab, position, Quaternion.identity, spawner.transform);
            SetTextValues(uiElement, leftString, middleString, rightString);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GraphBuilder.instance.startNode != null)
        {
            Debug.Log("Start Node exists");
        }
    }

    void SetTextValues(GameObject uiElement, string leftText, string middleText, string rightText)
    {
        Text[] textComponents = uiElement.GetComponentsInChildren<Text>();
        foreach (Text textComponent in textComponents)
        {
            if (textComponent.name == "Left text")
            {
                textComponent.text = leftText;
            }
            else if (textComponent.name == "Middle text"){
                textComponent.text = middleText;
            } 
            else if (textComponent.name == "Right text")
            {
                textComponent.text = rightText;
            }

        }

    }
}
