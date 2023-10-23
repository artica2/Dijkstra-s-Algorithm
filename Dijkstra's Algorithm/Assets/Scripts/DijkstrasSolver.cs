using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstrasSolver : MonoBehaviour
{
    GameObject rowPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void BuildTable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GraphBuilder.instance.startNode != null)
        {
            Debug.Log("Start Node exists");
        }
    }
}
