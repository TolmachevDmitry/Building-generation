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
        this.speedLeft = (new System.Random()).Next(1, maxValue);
        this.speedRight = (new System.Random()).Next(1, maxValue);
        this.speedUpp = (new System.Random()).Next(1, maxValue);
        this.speedDown = (new System.Random()).Next(1, maxValue);
    }

    public int getSpeedLeft()
    {
        return speedLeft;
    }

    public int getSpeedRight()
    {
        return speedRight;
    }

    public int getSpeedUpp()
    {
        return speedUpp;
    }

    public int getSpeedDown()
    {
        return speedDown;
    }
}
