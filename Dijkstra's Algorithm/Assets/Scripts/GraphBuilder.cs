using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class GraphBuilder : MonoBehaviour
{
    public static GraphBuilder instance;

    public GameObject ButtonBeingMoved;
    public Transform positionOfDrag;
    public GraphButton buttonOne;
    public GraphButton buttonTwo;

    public GameObject TextHolder;

    public Sprite road;
    public bool isDragging;
    public bool isMovingButton;
    public GameObject panel;

    public GameObject buttonPrefab;

    public GameObject canvas;
    public Camera mainCamera;

    public LineBetweenObjects LinePrefab;
    public LineBetweenObjects LineBeingDrawn;


    public List<GraphNode> nodes = new List<GraphNode>();
    public List<GraphNode> visitedNodes = new List<GraphNode>();
    public List<GraphNode> unvisitedNodes = new List<GraphNode>();
    public int numberOfNodes = 65; //65 is the ASCII for A
    public GraphNode startNode; // root node
    public GraphNode currentNode;
    public GraphNode endNode;
    public List<LineBetweenObjects> LinesInGraph;

    public InputField speedInput;
    public int speedInt;

    GraphButton node1;
    GraphButton node2;


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
        speedInput.onEndEdit.AddListener(OnInputEndEdit);
        speedInput.gameObject.SetActive(false);
        panel.SetActive(false);

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

    void OnInputEndEdit(string value)
    {
        Debug.Log("CALLS THIS!");
        if (int.TryParse(value, out int result))
        {
            // Input is a valid integer
            Debug.Log("Input is an integer: " + result);

            // Perform actions with the integer as needed
            speedInt = result;
            LineBetweenObjects newLine = Instantiate(LinePrefab);
            newLine.Initialize();
            newLine.DrawLine(node1.transform.position, node2.transform.position, false, speedInt.ToString());
            newLine.transform.SetParent(canvas.transform);
            GraphNode butt1 = node1.GetComponent<GraphButton>().node;
            
            GraphNode butt2 = node2.GetComponent<GraphButton>().node;
            butt1.reachableNodes.Add(butt2, speedInt);
            butt2.reachableNodes.Add(butt1, speedInt);
            // Disable the input field
            speedInput.gameObject.SetActive(false);

            panel.SetActive(false);
        }
    }

    private void addNode()
    {
        GameObject ButtonGo = Instantiate(buttonPrefab, Input.mousePosition, Quaternion.identity);
        GraphButton button = ButtonGo.GetComponent<GraphButton>();
        button.Intitialize();
        Text buttonText = button.GetComponentInChildren<Text>();
        buttonText.text = button.node.nodeName;
        nodes.Add(button.node);
        visitedNodes.Add(button.node);
        ButtonGo.transform.SetParent(canvas.transform);
    }

    private void connectNodes(GraphButton objOne, GraphButton objTwo)
    {
        node1 = objOne;
        node2 = objTwo;
        panel.SetActive(true);
        speedInput.gameObject.SetActive(true);


        //int speed = Random.Range(1, 5) * 10;
        //nodeTwo.reachableNodes.Add(nodeOne, speed);
        //LineBetweenObjects newLine = Instantiate(LinePrefab);
        //newLine.Initialize();
        //newLine.DrawLine(objOne.transform.position, objTwo.transform.position, false, speed.ToString());
        //newLine.transform.SetParent(canvas.transform);

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
            connectNodes(buttonOne, buttonTwo);
            speedInput.enabled = true;
            buttonOne = null;
            buttonTwo = null;
            positionOfDrag = null;
        }


        if(Input.GetMouseButtonDown(1)) {
            Vector3 mousePosition = Input.mousePosition;
            float whereInScreen = mousePosition.x / Screen.width;

            if (whereInScreen > 0.15 && whereInScreen < 0.85)
            {
                addNode();
            }
        }

        if (isMovingButton)
        {
            ButtonBeingMoved.transform.position = Input.mousePosition;
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                isMovingButton = false;
            }
        }
    }

}
