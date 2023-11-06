using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

    private List<GraphNode> nodesUnderInspection = new List<GraphNode>();

    private List<GameObject> tableHolder = new List<GameObject>();

    private List<GameObject> clipBoard = new List<GameObject>();

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

        // tableHolder.Add(topRow.name, topRow);
        leftString = "Node Name";
        middleString = "Minimum distance";
        rightString = "Previous Node";
        SetTextValues(topRow, leftString, middleString, rightString, false);

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
            uiElement.name = "Row " + i.ToString();
            tableHolder.Add(uiElement);
            SetTextValues(uiElement, leftString, middleString, rightString, false);
        }
    }

    private void RewriteTable()
    {
        foreach(GameObject row in tableHolder)
        {
            Destroy(row);
        }
        tableHolder.Clear();

        for(int i = spawner.transform.childCount -1; i >= 0; i--)
        {
            GameObject childObject = spawner.transform.GetChild(i).gameObject;
            Destroy(childObject);
        }

        // make the top row
        Vector3 startPos = spawner.transform.position;
        GameObject topRow = Instantiate(rowPrefab, spawner.transform.position, Quaternion.identity, spawner.transform);

        // tableHolder.Add(topRow.name, topRow);
        leftString = "Node Name";
        middleString = "Minimum distance";
        rightString = "Previous Node";
        SetTextValues(topRow, leftString, middleString, rightString, false);
        bool isUnderInspection = false;
        for (int i = 0; i < GraphBuilder.instance.nodes.Count; i++)
        {
            GraphNode node = GraphBuilder.instance.nodes[i];
            GameObject button = node.nodeButton;
            Image buttonImage = button.GetComponent<Image>();
            if(buttonImage.color == Color.cyan)
            {
                isUnderInspection = true;
            } else
            {
                isUnderInspection = false;
            }

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
            SetTextValues(uiElement, leftString, middleString, rightString, isUnderInspection);
            

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

    void SetTextValues(GameObject uiElement, string leftText, string middleText, string rightText, bool isBlue)
    {
        Text[] textComponents = uiElement.GetComponentsInChildren<Text>();
        foreach (Text textComponent in textComponents)
        {
            if (isBlue)
            {
                textComponent.color = Color.blue;
            }
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

    // Work out the cost to get to all the unvisited nodes connected to our current node
    private void RuleOne()
    {
        // Reset rule three
        dijkstrasRuleThree.color = Color.red;
        // Excecute rule one
        dijkstrasRuleOne.color = Color.green;
        GraphBuilder graph = GraphBuilder.instance;
        Image currentButtonImage = graph.currentNode.nodeButton.GetComponent<Image>();
        currentButtonImage.color = Color.green;

        foreach (KeyValuePair<GraphNode, float> reachableNode in graph.currentNode.reachableNodes)
        {
            if (!reachableNode.Key.hasBeenVisited)
            {
                Image buttonImage = reachableNode.Key.nodeButton.GetComponent<Image>();
                buttonImage.color = Color.cyan;
                GameObject annotationObject = new GameObject("Annotation");
                annotationObject.transform.position = reachableNode.Key.nodeButton.transform.position + new Vector3(0f, 10f, 0f);
                annotationObject.transform.SetParent(reachableNode.Key.nodeButton.transform); // Make it a child of the same parent as your line object
                float distance = graph.currentNode.minimumCost + reachableNode.Value;
                clipBoard.Add(annotationObject);

                nodesUnderInspection.Add(reachableNode.Key);
                // Create a Text component
                Text text = annotationObject.AddComponent<Text>();
                // Set the text content
                text.text = distance.ToString();
                text.color = Color.cyan;
                text.font = Font.CreateDynamicFontFromOSFont("Arial", 16);
            }
        }

    }

    // compare if any of these routes get us to our unvisited nodes in less time than we previously thought possible
    private void RuleTwo()
    {
        // Reset rule one
        dijkstrasRuleOne.color = Color.red;
        // Excecute rule two
        GraphBuilder graph = GraphBuilder.instance;
        dijkstrasRuleTwo.color = Color.green;
        foreach (KeyValuePair<GraphNode, float> reachableNode in graph.currentNode.reachableNodes)
        {
            if (!reachableNode.Key.hasBeenVisited)
            {
                Image buttonImage = reachableNode.Key.nodeButton.GetComponent<Image>();
                buttonImage.color = Color.cyan;
                GameObject annotationObject = new GameObject("Annotation");
                annotationObject.transform.position = reachableNode.Key.nodeButton.transform.position + new Vector3(0f, 10f, 0f);
                annotationObject.transform.SetParent(reachableNode.Key.nodeButton.transform); // Make it a child of the same parent as your line object
                float distance = graph.currentNode.minimumCost + reachableNode.Value;
                if (distance < reachableNode.Key.minimumCost)
                {
                    reachableNode.Key.minimumCost = distance;
                }
                RewriteTable();
            }
        }
    }

    private void RuleThree()
    {
        // Reset rule two
        dijkstrasRuleTwo.color = Color.red;
        // Excecute rule three
        dijkstrasRuleThree.color = Color.green;
        GraphBuilder graph = GraphBuilder.instance;
        GraphNode currentNode = graph.currentNode;
        Image buttonImage = currentNode.nodeButton.GetComponent<Image>();
        currentNode.hasBeenVisited = true;
        buttonImage.color = Color.grey;
        float trackerFloat = 100000000f;
        foreach(KeyValuePair<GraphNode, float> node in currentNode.reachableNodes)
        {
            if(node.Key.minimumCost < trackerFloat)
            {
                graph.currentNode = node.Key;
            }
        }
        Image currentButtonImage = graph.currentNode.nodeButton.GetComponent<Image>();
        currentButtonImage.color = Color.green;

    }
}
