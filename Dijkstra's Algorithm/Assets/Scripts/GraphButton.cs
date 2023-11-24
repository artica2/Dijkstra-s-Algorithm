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


        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!GraphBuilder.instance.isMovingButton)
            {
                GraphBuilder.instance.isMovingButton = true;
                GraphBuilder.instance.ButtonBeingMoved = gameObject;
            } else
            {
                GraphBuilder.instance.isMovingButton = false;
                return;
            }
        } 
        else if (!GraphBuilder.instance.isMovingButton)
        {
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
        }
    }

    public void Intitialize()
    {
        GraphNode newNode = new GraphNode();
        newNode.nodeName = Convert.ToChar(GraphBuilder.instance.numberOfNodes).ToString();
        newNode.hasBeenVisited = false;
        if(GraphBuilder.instance.numberOfNodes == 65) // this is the first node
        {
            Debug.Log("START NODE GETS SET");
            GraphBuilder.instance.startNode = newNode;
            GraphBuilder.instance.currentNode = newNode;
            newNode.minimumCost = 0;
            newNode.prevNode = newNode;
        } else
        {
            newNode.minimumCost = 1000000;
        }
        GraphBuilder.instance.numberOfNodes++;    
        GraphBuilder.instance.endNode = newNode;
        node = newNode;
        node.nodeButton = gameObject;
    }
}
