using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public GameObject obj1;
    public GameObject obj2;

    // private CartOfBorders cart = new CartOfBorders();

    private void BuildObject(Node p0, Node p1, double crushingFactor)
    {
        double dX = p1.x - p0.x;
        double dZ = p1.y - p0.y;

        double unitMetric = (crushingFactor != 0) ? (1 / crushingFactor) : (1);
        double plustFactor = 0.05 * ((crushingFactor != 0) ? (crushingFactor) : (1));

        double x = (dX != 0) ? (p0.x + (dX / 2) + (Math.Abs(dX) / dX) * plustFactor) : (p0.x);
        double z = (dZ != 0) ? (p0.y + (dZ / 2) + (Math.Abs(dZ) / dZ) * plustFactor) : (p0.y);
        
        GameObject a = Instantiate(obj1, new Vector3((float) (x * unitMetric), 1, (float) (z * unitMetric)), Quaternion.Euler(0f, 0f, 0f));

        x = (dX != 0) ? (Math.Abs(dX) * unitMetric) : (0.1);
        z = (dZ != 0) ? (Math.Abs(dZ) * unitMetric) : (0.1);

        a.GetComponent<Transform>().localScale = new Vector3((float) x, (float) 3, (float) z);
    }

    private void BuildFigure(List<Node> circuit, int crushingFactor)
    {
        for (int i = 0; i < circuit.Count - 1; i++) 
        {
            BuildObject(circuit[i], circuit[i + 1], (double) crushingFactor);
        }
    }
    
    private void Start() 
    {
        int n = 5;
        int m = 5;
        int crushingFactor = 10;

        GameObject earth = Instantiate(obj2, new Vector3(0, 0, 0), Quaternion.Euler(0f, 0f, 0f));
        earth.GetComponent<Transform>().localScale = new Vector3(5, 1, 5);

        List<Node> circuit = (new Circuit()).CreateCircuit(n, m);
        // (new CartOfBorders()).GetChainPoints(circuit, n, m, );
        // BuildFigure(circuit, 0);

        BuildRoom bR = new BuildRoom();
        List<List<Node>> rooms = bR.CreateRooms(circuit, n, m, crushingFactor);
        BuildFigure(bR.GetChainCircuit(), crushingFactor);
        for (int i = 0; i < rooms.Count; i++)
        {
            BuildFigure(rooms[i], crushingFactor);
        }

    }

}
