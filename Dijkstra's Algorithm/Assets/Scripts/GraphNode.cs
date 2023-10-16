using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class GraphNode : MonoBehaviour
{
    string NodeName;
    public Dictionary<GraphNode, float> reachableNodes = new Dictionary<GraphNode, float>();
    public float minimumCost = Mathf.Infinity;

    void Start()
    {
        foreach (KeyValuePair<GraphNode, float> node in reachableNodes)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(gameObject.transform.position, node.Key.gameObject.transform.position);
            // Calculate the midpoint for annotation
            Vector3 midpoint = (gameObject.transform.position + node.Key.gameObject.transform.position) / 2;

            // Annotate the line with the float value
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            Handles.Label(midpoint, node.Value.ToString(), style);
        }
    }

}
