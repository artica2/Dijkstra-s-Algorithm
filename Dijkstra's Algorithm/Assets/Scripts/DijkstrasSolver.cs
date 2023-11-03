using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DijkstrasSolver : MonoBehaviour
{
    public GameObject rowPrefab;
    public GameObject spawner;
    public float rowSpacing;

    public Text dijkstrasRuleOne;
    public Text dijkstrasRuleTwo;
    public Text dijkstrasRuleThree;

    public int dijkstrasRuleCounter;
    public bool inSolvingPhase;

    private string leftString;
    private string middleString;
    private string rightString;

    // Start is called before the first frame update
    void Start()
    {
        dijkstrasRuleOne.text = " ";
        dijkstrasRuleTwo.text = " ";
        dijkstrasRuleThree.text = " ";
        dijkstrasRuleOne.color = Color.red;
        dijkstrasRuleTwo.color = Color.red;
        dijkstrasRuleThree.color = Color.red;
        inSolvingPhase = false;
        dijkstrasRuleCounter = 3;
    }

    public void BuildTable()
    {
        inSolvingPhase = true;

        dijkstrasRuleOne.text = "Work out the cost to get to all the unvisited nodes connected to our current node";
        dijkstrasRuleTwo.text = "Compare if any of these routes get us to our unvisited nodes in less time than we previously thought possible";
        dijkstrasRuleThree.text = "Mark our previous node as visited, and work out the shortest journey time to any of our unvisited nodes, and go there";
        
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
        if (inSolvingPhase)
        {
            DoDijkstras();
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
    
    // a manager function for the dijkstra's algorithm
    private void DoDijkstras()
    {
        // need a better key then h
        if (Input.GetKeyDown(KeyCode.H))
        {
            dijkstrasRuleCounter++;
            if(dijkstrasRuleCounter == 4)
            {
                dijkstrasRuleCounter = 1;
                RuleOne();
            }
            if(dijkstrasRuleCounter == 2)
            {
                RuleTwo();
            }
            if(dijkstrasRuleCounter == 3)
            {
                RuleThree();
            }
        }
    }

    private void RuleOne()
    {
        // Reset rule three
        dijkstrasRuleThree.color = Color.red;
        // Excecute rule one
        dijkstrasRuleOne.color = Color.green;

    }

    private void RuleTwo()
    {
        // Reset rule one
        dijkstrasRuleOne.color = Color.red;
        // Excecute rule two
        dijkstrasRuleTwo.color = Color.green;
    }

    private void RuleThree()
    {
        // Reset rule two
        dijkstrasRuleTwo.color = Color.red;
        // Excecute rule three
        dijkstrasRuleThree.color = Color.green;
    }
}
