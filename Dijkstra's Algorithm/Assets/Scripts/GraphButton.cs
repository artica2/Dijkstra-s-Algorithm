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
            Debug.Log("START NODE GETS SET");
            GraphBuilder.instance.startNode = newNode;
            newNode.minimumCost = 0;
            newNode.prevNode = newNode;
        } else
        {
            newNode.minimumCost = 1000000;
        }
        GraphBuilder.instance.numberOfNodes++;
        
        node = newNode;
        
    }
}
