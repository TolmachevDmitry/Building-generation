using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delta
{
    private Wall oldValue;
    private Wall newValue;

    public Delta(Wall w0, Wall w1)
    {
        this.oldValue = w0;
        this.newValue = w1;
    }

    public Wall getOld()
    {
        return oldValue;
    }

    public Wall getNew()
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
    public Delta changeLength(int dP1, int dP2)
    {
        Wall old = new Wall(p1, p2, ind, type);

        this.p1 += dP1;
        this.p2 += dP2;

        return new Delta(old, this);
    }

    // Изменение расположения на оси, по которой обе точки равны друг-другу
    public Delta changeInd(int dInd)
    {
        Wall old = new Wall(p1, p2, ind, type);
        ind += dInd;

        return new Delta(old, this);
    }

    // Мы сравниваем с элементами из индекса, который мы получили из этой стены
    public bool isEquals(Wall wOther)
    {
        return this.p1 == wOther.getP1() && this.p2 == wOther.getP2() && this.ind == wOther.getInd() && this.type == wOther.getType();
    }

    // Получение всех параметров
    public int getP1()
    {
        return p1;
    }

    public int getP2()
    {
        return p2;
    }

    public int getInd()
    {
        return ind;
    }

    public string getType()
    {
        return type;
    }
    
}
