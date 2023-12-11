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
    public List<GraphNode> nodes = new List<GraphNode>();
    public List<GraphNode> visitedNodes = new List<GraphNode>();
    public List<GraphNode> unvisitedNodes = new List<GraphNode>();
    public int numberOfNodes = 65; //65 is the ASCII for A
    public GraphNode startNode; // root node
    public GraphNode currentNode;
    public GraphNode endNode;
    public List<LineBetweenObjects> LinesInGraph;


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
        if (int.TryParse(value, out int result)) // if input is an integer
        {       
            speedInt = result;
            // drawn a line between the two nodes
            LineBetweenObjects newLine = Instantiate(LinePrefab);
            newLine.Initialize();
            newLine.DrawLine(node1.transform.position, node2.transform.position, false, speedInt.ToString());
            newLine.transform.SetParent(canvas.transform);
            // update the data of the two connected nodes
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

    }

    void Update()
    {

        LineBeingDrawn.DrawLine(Vector3.zero, Vector3.zero, false); // reset the line currently being drawn
        if (isDragging && buttonOne != null)
        {
            // draw a line between the button thats been clicked on and the mouse each frame
            Vector3 ButtonPos = buttonOne.transform.position;
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 MousePos = Input.mousePosition;
            Vector3 offsetMouse = MousePos - screenCenter;
            LineBeingDrawn.DrawLine(ButtonPos, MousePos, true);
        }
        if (buttonTwo != null) // when a second node has been clicked on
        {
            connectNodes(buttonOne, buttonTwo);
            speedInput.enabled = true;
            buttonOne = null;
            buttonTwo = null;
            positionOfDrag = null;
        }

        // if we right click, add another node
        if(Input.GetMouseButtonDown(1)) {
            Vector3 mousePosition = Input.mousePosition;
            float whereInScreen = mousePosition.x / Screen.width;

            if (whereInScreen > 0.15 && whereInScreen < 0.85)
            {
                addNode();
            }
        }

        // if we are moving a node
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
