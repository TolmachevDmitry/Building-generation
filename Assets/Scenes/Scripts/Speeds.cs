using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Приращения наших стенок
public class Speeds
{
    private int speedLeft;
    private int speedRight;
    private int speedUpp;
    private int speedDown;

    public Speeds(int maxValue)
    {
        this.speedLeft = (new System.Random()).Next(1, maxValue / 2 + 1);
        this.speedRight = (new System.Random()).Next(1, maxValue / 2 + 1);
        this.speedUpp = (new System.Random()).Next(1, maxValue / 2 + 1);
        this.speedDown = (new System.Random()).Next(1, maxValue / 2 + 1);
    }

    public int GetSpeedLeft()
    {
        return speedLeft;
    }
    public int GetSpeedRight()
    {
        return speedRight;
    }
    public int GetSpeedUpp()
    {
        return speedUpp;
    }
    public int GetSpeedDown()
    {
        return speedDown;
    }
}
