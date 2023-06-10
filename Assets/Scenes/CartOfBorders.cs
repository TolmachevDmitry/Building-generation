using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartOfBorders
{
    // Количество точек по обеми осям
    private int countX;
    private int countY;
    
    // Для каждой координаты соответствующей оси храним список тех стен, которые ей перпендикулярны
    private List<List<Wall>> horizontal = new List<List<Wall>>();
    private List<List<Wall>> vertical = new List<List<Wall>>();

    // Конструктор (Получился большой)
    public CartOfBorders(List<Node> circuit, int n, int m, int crushingFactor)
    {
        this.countY = 2 * n * crushingFactor + 1;
        this.countX = 2 * m * crushingFactor + 1;

        // Инициализация списков отрезков по осям x и y
        for (int i = 0; i < Math.Max(countX, countY); i++)
        {
            if (i < countY)
            {
                horizontal.Add(new List<Wall>());
            }
            if (i < countX)
            {
                vertical.Add(new List<Wall>());
            }
        }

        int x1 = circuit[0].x;
        int y1 = circuit[0].y;

        bool sameX = (x1 == circuit[1].x);
        bool sameY = (y1 == circuit[1].y);

        // Чтобы "добить" последнюю стену
        circuit.Add(circuit[1]);
        for (int i = 2; i < circuit.Count; i++)
        {
            if (sameX != (circuit[i].x == x1) && sameY != (circuit[i].y == y1))
            {
                if (sameY)
                {
                    horizontal[indY(circuit[i - 1].y * crushingFactor)].Add(new Wall(indX(x1), indX(circuit[i - 1].x), circuit[i - 1].y * crushingFactor, "Circuit"));
                    sameX = true;
                    sameY = false;
                }
                if (sameX)
                {
                    vertical[indX(circuit[i - 1].x * crushingFactor)].Add(new Wall(indY(y1), indY(circuit[i - 1].y), circuit[i - 1].x, "Circuit"));
                    sameX = false;
                    sameY = true;
                }

                x1 = circuit[i - 1].x;
                y1 = circuit[i - 1].y;
            }
        }
    }

    // Преобразование координаты в индекс списка [0, 1, 2, 3, ...)
    private int indX(int x)
    {
        return x + (countX - 1) / 2;
    }

    private int indY(int y)
    {
        return y + (countY - 1) / 2;
    }

    // Два варианта для сравнения точекs
    // w0 линия из списка, wMoving - линия которую мы передвигаем
    // isIntersection - для обноружения препятствий на пути (стоит ли нам продолжать движение ?)
    private bool isIntersection(Wall w0, Wall wMoving)
    {
        return ((w0.getP1() < wMoving.getP1() && wMoving.getP1() < w0.getP2()) || (w0.getP1() < wMoving.getP2() && wMoving.getP2() < w0.getP2())) 
        || ((wMoving.getP1() < w0.getP1() && wMoving.getP1() < w0.getP2()) || (wMoving.getP1() < w0.getP2() && wMoving.getP2() < w0.getP2())) && notSame(w0, wMoving);
    }

    // inside - для обнаружения полного включения линии wMoving в wo (Нужно ли нам "выдавливать" стенку ?)
    private bool isInside(Wall w0, Wall wMoving)
    {
        return w0.getP1() <= wMoving.getP1() && wMoving.getP2() <= w0.getP2() && notSame(w0, wMoving);
    }

    // Типы разные (Чтобы не сравнивать с самим собой)
    private bool notSame(Wall w0, Wall wMoving)
    {
        return w0.getType() != wMoving.getType();
    }

    // Есть ли препятствия на пути - обобщённая функция
    private bool isWall(Wall candidate, List<Wall> axis, int typeCheck)
    {
        for (int i = 0; i < axis.Count; i++)
        {
            if (typeCheck == 1 && isIntersection(axis[i], candidate))
            {
                return true;
            }

            if (typeCheck == 2 && isInside(axis[i], candidate))
            {
                return true;
            }
        }

        return false;
    }

    // Есть ли препятствия на пути ?
    public bool isIntersectionVert(Wall candidate)
    {
        return isWall(candidate, vertical[indX(candidate.getInd())], 1);
    }

    public bool isIntersectionHoriz(Wall candidate)
    {
        return isWall(candidate, horizontal[indY(candidate.getInd())], 1);
    }

    // Полностью ли входит в какую-либо стену ?
    public bool isClosedVert(Wall candidate)
    {
        return isWall(candidate, vertical[indX(candidate.getInd())], 2);
    }

    public bool isClosedHoriz(Wall candidate)
    {
        return isWall(candidate, horizontal[indY(candidate.getInd())], 2);
    }

    // Добавить новую вертикальную стенку
    public void addVert(Wall w)
    {
        vertical[indX(w.getInd())].Add(w);
    }
    // Добавить новую горизонтальную стенку
    public void addHoriz(Wall w)
    {
        horizontal[indY(w.getInd())].Add(w);
    }

    // Поиск места элемента в списке соответствующей координаты оси
    private int searchElem(List<Wall> list, Wall old)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].isEquals(old))
            {
                return i;
            }
        }
        // По идее, такого не должно случиться (это гарантировано!)
        return -1;
    }

    // Переставляем нашу на другое значение оси (которое одинаково на орбразующих длинну точках)
    // Сначало удаление со старого места, а потом - запись на новое
    public void changeIndVert(Wall wOld, Wall wNew)
    {
        vertical[indX(wOld.getInd())].RemoveAt(searchElem(vertical[indX(wOld.getInd())], wNew));
        vertical[indX(wNew.getInd())].Add(wNew);
    }

    public void changeIndHoriz(Wall wOld, Wall wNew)
    {
        horizontal[indY(wOld.getInd())].RemoveAt(searchElem(horizontal[indY(wOld.getInd())], wNew));
        horizontal[indY(wNew.getInd())].Add(wNew);
    }

}
