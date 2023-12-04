using System;
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

    [SerializeField]
    Sprite SLWhite;
    [SerializeField]
    Sprite SLRed;
    [SerializeField]
    Sprite SLGrey;
    [SerializeField]
    Sprite SLGreen;
    [SerializeField]
    Sprite SLBlue;

    private List<GraphNode> nodesUnderInspection = new List<GraphNode>();

    private List<GameObject> tableHolder = new List<GameObject>();

    private List<GameObject> clipBoard = new List<GameObject>();

    public Text endGameText;



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
        endGameText.text = " ";

    }

    public void BuildTable()
    {
        inSolvingPhase = true;

        dijkstrasRuleOne.text = "Work out the cost to get to all the unvisited nodes connected to our current node";
        dijkstrasRuleTwo.text = "Compare if any of these routes get us to our unvisited nodes in less time than we previously thought possible";
        dijkstrasRuleThree.text = "Mark our previous node as visited, and work out the shortest journey time to any of our unvisited nodes, and go there";

        Image endButtonImage = GraphBuilder.instance.endNode.nodeButton.GetComponent<Image>();
        Image startButtonImage = GraphBuilder.instance.startNode.nodeButton.GetComponent<Image>();
        // endButtonImage.color = Color.red;
        endButtonImage.sprite = SLRed;
        startButtonImage.sprite = SLGreen;

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
            if(buttonImage.sprite == SLBlue)
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
                textComponent.color = Color.cyan;
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
        currentButtonImage.sprite = SLGreen;

        foreach(GraphNode node in graph.nodes)
        {
            Image image = node.nodeButton.GetComponent<Image>();
            if (image.sprite == SLBlue)
            {
                image.sprite = SLWhite;
            }
        }

        foreach (KeyValuePair<GraphNode, float> reachableNode in graph.currentNode.reachableNodes)
        {
            if (!reachableNode.Key.hasBeenVisited)
            {
                Image buttonImage = reachableNode.Key.nodeButton.GetComponent<Image>();
                buttonImage.sprite = SLGreen;
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
                buttonImage.sprite = SLBlue;
                GameObject annotationObject = new GameObject("Annotation");
                annotationObject.transform.position = reachableNode.Key.nodeButton.transform.position + new Vector3(0f, 10f, 0f);
                annotationObject.transform.SetParent(reachableNode.Key.nodeButton.transform); // Make it a child of the same parent as your line object
                float distance = graph.currentNode.minimumCost + reachableNode.Value;
                if (distance < reachableNode.Key.minimumCost)
                {
                    reachableNode.Key.minimumCost = distance;
                    reachableNode.Key.prevNode = graph.currentNode;
                }
                RewriteTable();
            }
        }
    }

    // mark our previous node as visited, and work out the shortest journey time to any of our unvisited nodes.
    private void RuleThree()
    {
        // Reset rule two
        dijkstrasRuleTwo.color = Color.red;
        foreach (GameObject go in clipBoard)
        {
            Destroy(go);
        }
        clipBoard.Clear();
        // Excecute rule three
        dijkstrasRuleThree.color = Color.green;
        GraphBuilder graph = GraphBuilder.instance;
        GraphNode currentNode = graph.currentNode;
        Image buttonImage = currentNode.nodeButton.GetComponent<Image>();
        currentNode.hasBeenVisited = true;
        buttonImage.sprite = SLGrey;
        float trackerFloat = 100000000f;
        foreach(GraphNode node in graph.nodes)
        {
            if (!node.hasBeenVisited)
            {
                if (node.minimumCost < trackerFloat)
                {
                    graph.currentNode = node;
                    trackerFloat = node.minimumCost;
                }
            }
        }
        Image currentButtonImage = graph.currentNode.nodeButton.GetComponent<Image>();
        currentButtonImage.sprite = SLGreen;
        if(graph.currentNode == graph.endNode)
        {
            EndSim();
        }
    }

    void EndSim()
    {
        float minDistanceTracker = 0f;
        string journeyString = "";
        GraphBuilder Graph = GraphBuilder.instance;
        GraphNode examinedNode = Graph.endNode;
        GraphNode prevNode = Graph.currentNode;
        GraphNode endNode = Graph.endNode;

        while (examinedNode != Graph.startNode)
        {
            prevNode = examinedNode.prevNode;
            minDistanceTracker += prevNode.reachableNodes[examinedNode];
            journeyString += examinedNode.nodeName + " ";
            examinedNode = prevNode;
        }
        journeyString += "A";

        char[] charArray = journeyString.ToCharArray();
        Array.Reverse(charArray);
        string reversedJourney = new string(charArray);
        
        string printString = "You've made it to the final node! Your fastest journey is: " + reversedJourney + " and the minimum cost is: " + minDistanceTracker;
        endGameText.text = printString;
    }
}
