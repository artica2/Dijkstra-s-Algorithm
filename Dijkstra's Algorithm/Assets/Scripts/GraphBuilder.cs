using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GraphBuilder : MonoBehaviour
{
    public static GraphBuilder instance;
    public bool isDragging;
    public Transform positionOfDrag;
    public GraphButton buttonOne;
    public GraphButton buttonTwo;

    public GameObject buttonPrefab;

    public GameObject canvas;
    public Camera mainCamera;

    public LineBetweenObjects LinePrefab;
    public LineBetweenObjects LineBeingDrawn;
    public List<LineBetweenObjects> LinesInGraph;


    public List<GraphNode> nodes;
    public List<GraphNode> visitedNodes;
    public List<GraphNode> unvisitedNodes;
    public int numberOfNodes = 65; //65 is the ASCII for A

    public GraphNode currentNode;

    public GraphNode startNode; // root node

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (instance != null && instance != this)
        {
            Destroy(this);
        } else
        {
            instance = this;
        }

        Initialize();

    }

    private void Initialize()
    {
        buttonOne = null;
        buttonTwo = null;

        LineBeingDrawn = Instantiate(LinePrefab);
        LineBeingDrawn.Initialize();
        LineBeingDrawn.DrawLine(Vector3.zero, Vector3.zero, false);
        LineBeingDrawn.transform.SetParent(canvas.transform);
    }

    private void addNode()
    {
        GameObject ButtonGo = Instantiate(buttonPrefab, Input.mousePosition, Quaternion.identity);
        GraphButton button = ButtonGo.GetComponent<GraphButton>();
        button.Intitialize();
        Text buttonText = button.GetComponentInChildren<Text>();
        buttonText.text = button.node.nodeName;
        ButtonGo.transform.SetParent(canvas.transform);
    }

    private void connectNodes(GraphButton objOne, GraphButton objTwo)
    {
        GraphNode nodeOne = objOne.node;
        GraphNode nodeTwo = objTwo.node;
        int speed = Random.Range(1, 5) * 10;
        nodeOne.reachableNodes.Add(nodeTwo, speed);
        nodeTwo.reachableNodes.Add(nodeOne, speed);
        LineBetweenObjects newLine = Instantiate(LinePrefab);
        newLine.Initialize();
        newLine.DrawLine(objOne.transform.position, objTwo.transform.position, false, speed.ToString());
        newLine.transform.SetParent(canvas.transform);

    }

    private void deleteGraph()
    {

    }

    void Update()
    {

        LineBeingDrawn.DrawLine(Vector3.zero, Vector3.zero, false);
        if (isDragging && buttonOne != null)
        {            
            Vector3 ButtonPos = buttonOne.transform.position;
            // RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 MousePos = Input.mousePosition;
            Vector3 offsetMouse = MousePos - screenCenter;
            LineBeingDrawn.DrawLine(ButtonPos, MousePos, true);
        }
        if (buttonTwo != null)
        {
            connectNodes(buttonOne, buttonTwo); // why does it do this twice?????
            buttonOne = null;
            buttonTwo = null;
            positionOfDrag = null;
        }

        if(Input.GetMouseButtonDown(1)) {
            addNode();
        }
    }
}
