using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphButton : MonoBehaviour
{
    public GraphNode node;
    public void OnClick()
    {
        if (GraphBuilder.instance.isDragging == false) // if we're not currently dragging we want to begin dragging
        {
            GraphBuilder.instance.isDragging = true;
            GraphBuilder.instance.positionOfDrag = gameObject.transform;
            GraphBuilder.instance.buttonOne = this;
        } else
        {
            GraphBuilder.instance.isDragging = false;
            GraphBuilder.instance.buttonTwo = this;
        }
    }

    public void Intitialize()
    {
        GraphNode newNode = new GraphNode();
        newNode.nodeName = Convert.ToChar(GraphBuilder.instance.numberOfNodes).ToString();
        if(GraphBuilder.instance.numberOfNodes == 65) // this is the first node
        {
            GraphBuilder.instance.startNode = newNode;
        }
        GraphBuilder.instance.numberOfNodes++;
        newNode.minimumCost = 400;
        node = newNode;
        
    }
}
