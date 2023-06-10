using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point 
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class MainHouse : MonoBehaviour
{
    public GameObject obj;
    
    private void Start() 
    {
        // List<Node> circuit = (new Circuit()).createCircuit(5, 5);
        // for (int i = 0; i < circuit.Count - 1; i++) 
        // {
        //     Node p0 = circuit[i];
        //     Node p1 = circuit[i + 1];

        //     double[] vector = new double[2];
        //     vector[0] = (p0.x > p1.x) ? (-0.5) : ((p0.x == p1.x) ? (0) : (0.5));
        //     vector[1] = (p0.y > p1.y) ? (-0.5) : ((p0.y == p1.y) ? (0) : (0.5));

        //     for (double x = (double) p0.x, z = (double) p0.y; x != (double) p1.x || z != (double) p1.y; x += vector[0], z += vector[1])
        //     {
        //         Instantiate(obj, new Vector3((float) x, 0, (float) z), Quaternion.Euler(0f, 0f, 0f));
        //     }

        // }

        // // Debug.Log(isExtra(new Point(-5, -3), new Point(-5, -1), new Point(-5, -5)));

        // Debug.Log("Last proint: " + circuit[circuit.Count - 3].x +  " : " + circuit[circuit.Count - 3].y);
        // Debug.Log("Last proint: " + circuit[circuit.Count - 2].x +  " : " + circuit[circuit.Count - 2].y);
        // Debug.Log("Start point was: " + circuit[0].x + " : " + circuit[0].y);

        GameObject a =  Instantiate(obj, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f)) as GameObject;
        a.GetComponent<Transform>().localScale = new Vector3(10, 5, (float) 1.5);
        // a.GetComponent<Transform>().rotation = Quaternion.Euler(0, 45f, 0);
        
    }

}
