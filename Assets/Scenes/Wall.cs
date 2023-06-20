using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delta
{
    private Wall oldValue;
    private Wall newValue;

    public Delta(Wall wall0, Wall wall1)
    {
        this.oldValue = wall0;
        this.newValue = wall1;
    }

    public Wall GetOld()
    {
        return oldValue;
    }

    public Wall GetNew()
    {
        return newValue;
    }
}

public class Wall
{
    // Образующая длину отрезка (стенки)
    // Не важно, горизонтальная или вертикальная будет стенка, это решается на уровне использования этого класса
    private int p1;
    private int p2;
    // Распололжение по другой оси (одинаковые значения точек в этой оси)
    private int ind;

    // К кому принадлежит эта стенка
    private string type;

    public Wall(int p1, int p2, int ind, string type)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.ind = ind;
        this.type = type;
    }

    // Изменение точек, образующих длину
    // На вход подаются величина преращения
    public Delta ChangeLength(int dP1, int dP2)
    {
        Wall wallOld = new Wall(p1, p2, ind, type);

        this.p1 += dP1;
        this.p2 += dP2;

        return new Delta(wallOld, this);
    }

    // Изменение расположения на оси, по которой обе точки равны друг-другу
    public Delta ChangeInd(int dInd)
    {
        Wall wallOld = new Wall(p1, p2, ind, type);
        ind += dInd;

        return new Delta(wallOld, this);
    }

    // Мы сравниваем с элементами из индекса, который мы получили из этой стены
    public bool IsEquals(Wall wallOther)
    {
        return this.p1 == wallOther.GetP1() && this.p2 == wallOther.GetP2() && this.ind == wallOther.GetInd() 
        && this.type == wallOther.GetTypeRoom();
    }

    // Получение всех параметров
    public int GetP1()
    {
        return p1;
    }

    public int GetP2()
    {
        return p2;
    }

    public int GetInd()
    {
        return ind;
    }

    public string GetTypeRoom()
    {
        return type;
    }
    
}
