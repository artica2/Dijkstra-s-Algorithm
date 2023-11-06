using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;



public class GraphNode
{
    public string nodeName;
    public Dictionary<GraphNode, float> reachableNodes = new Dictionary<GraphNode, float>(); // dictionary with the reachable nodes and how long it takes to add them
    public float minimumCost = Mathf.Infinity; // min cost to get to the node
    public GraphNode prevNode = null;
    public GameObject nodeButton;
    public bool hasBeenVisited;
}
