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
    public List<List<Wall>> horizontal = new List<List<Wall>>();
    public List<List<Wall>> vertical = new List<List<Wall>>();   

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

        // Добавляем в конец второую точку чтобы "добить" последнюю стену
        circuit.Add(circuit[1]);
        for (int i = 2; i < circuit.Count; i++)
        {
            if (sameX != (circuit[i].x == x1) || sameY != (circuit[i].y == y1))
            {
                x1 *= crushingFactor;
                y1 *= crushingFactor;

                if (sameY)
                {
                    int x2 = circuit[i - 1].x * crushingFactor;
                    int ind = circuit[i - 1].y * crushingFactor;
                    horizontal[IndY(ind)].Add(new Wall(Math.Min(x1, x2), Math.Max(x1, x2), ind, "Circuit"));
                }
                if (sameX)
                {
                    int y2 = circuit[i - 1].y * crushingFactor;
                    int ind = circuit[i - 1].x * crushingFactor;
                    vertical[IndX(ind)].Add(new Wall(Math.Min(y1, y2), Math.Max(y1, y2), ind, "Circuit"));
                }

                sameX = !sameX;
                sameY = !sameY;

                x1 = circuit[i - 1].x;
                y1 = circuit[i - 1].y;
            }
        }
    }

    // Преобразование координаты в индекс списка [0, 1, 2, 3, ...)
    public int IndX(int x)
    {
        return x + (countX - 1) / 2;
    }

    private int IndY(int y)
    {
        return y + (countY - 1) / 2;
    }

    // Два варианта для сравнения точек
    // wall0 линия из списка, wallMoving - линия которую мы передвигаем
    // Для обноружения препятствий на пути (стоит ли нам продолжать движение/ выдавливать стенку ?)
    public bool IsIntersection(Wall wall0, Wall wallMoving, bool sifter)
    {
        int aP1 = wall0.GetP1();
        int aP2 = wall0.GetP2();
        int aInd = wall0.GetInd();

        int bP1 = wallMoving.GetP1();
        int bP2 = wallMoving.GetP2();
        int bInd = wallMoving.GetInd();

        bool part1 = (aP1 <= bP1 && bP1 < aP2) || (aP1 < bP2 && bP2 <= aP2);
        bool part2 = (bP1 <= aP1 && aP1 < bP2) || (bP1 < aP2 && aP2 <= bP2);

        return (part1 || part2) && (sifter);
    }
    // Сравнивам стенки из разных комнат (чтобы не учитывать эту стенку, из которой будет иходить данная)
    public bool IsIntersection1(Wall wall0, Wall wallMoving)
    {
        return IsIntersection(wall0, wallMoving, !IsSame(wall0, wallMoving));
    }

    // Проверяем, нет ли пересечений с другими стенками из ТОЙ ЖЕ комнаты
    private bool IsIntersection2(Wall wall0, Wall wallMoving)
    {
        return IsIntersection(wall0, wallMoving, IsSame(wall0, wallMoving));
    }

    // Типы разные (Чтобы не сравнивать с самим собой)
    private bool IsSame(Wall wall0, Wall wallMoving)
    {
        return wall0.GetTypeRoom() == wallMoving.GetTypeRoom();
    }

    // Есть ли препятствия на пути - обобщённая функция
    private bool IsWall(Wall candidate, List<Wall> axis, int typeCheck)
    {
        for (int i = 0; i < axis.Count; i++)
        {
            if (typeCheck == 1 && IsIntersection1(axis[i], candidate))
            {
                return true;
            }

            if (typeCheck == 2 && IsIntersection2(axis[i], candidate))
            {
                return true;
            }
        }

        return false;
    }

    // Перекрывает ли эта стенка другую стену другой команты
    public bool IsIntersectionVert1(Wall candidate)
    {
        return IsWall(candidate, vertical[IndX(candidate.GetInd())], 1);
    }
    public bool IsIntersectionHoriz1(Wall candidate)
    {
        return IsWall(candidate, horizontal[IndY(candidate.GetInd())], 1);
    }

    // Перекрывает ли эта стенка другую стену той же комнаты (для выдавливания потомков)
    public bool IsIntersectionVert2(Wall candidate)
    {
        return IsWall(candidate, vertical[IndX(candidate.GetInd())], 2);
    }

    public bool IsIntersectionHoriz2(Wall candidate)
    {
        return IsWall(candidate, horizontal[IndY(candidate.GetInd())], 2);
    }

    // Добавить новую вертикальную стенку
    public void AddVert(Wall wall)
    {
        vertical[IndX(wall.GetInd())].Add(wall);
    }
    // Добавить новую горизонтальную стенку
    public void AddHoriz(Wall wall)
    {
        horizontal[IndY(wall.GetInd())].Add(wall);
    }
    // Поиск места элемента в списке соответствующей координате оси
    private int SearchElem(List<Wall> list, Wall wallNew)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].IsEquals(wallNew))
            {
                return i;
            }
        }
        return -1;
    }
    // Переставляем нашу на другое значение оси (которое одинаково на орбразующих длинну точках)
    // Сначало удаление со старого места, а потом - запись на новое
    public void ChangeIndVert(Wall wallOld, Wall wallNew)
    {
        vertical[IndX(wallOld.GetInd())].RemoveAt(SearchElem(vertical[IndX(wallOld.GetInd())], wallNew));
        vertical[IndX(wallNew.GetInd())].Add(wallNew);
    }

    public void ChangeIndHoriz(Wall wallOld, Wall wallNew)
    {
        horizontal[IndY(wallOld.GetInd())].RemoveAt(SearchElem(horizontal[IndY(wallOld.GetInd())], wallNew));
        horizontal[IndY(wallNew.GetInd())].Add(wallNew);
    }

    // Получение последовательной цепочки точек кого-либо контура чего-нибудь (сдания или комнат)
    public List<Node> GetChainPoints(string type)
    {
        List<Node> border = new List<Node>();

        Wall wall = null;
        Wall wallPrev = null;
        Wall firstWall = null;
        for (int i = 0; i < horizontal.Count; i++)
        {
            bool isFound = false;
            for (int j = 0; j < horizontal[i].Count; j++)
            {
                if (horizontal[i][j].GetTypeRoom() == type)
                {
                    Wall w = horizontal[i][j];
                    wall = new Wall(w.GetP1(), w.GetP2(), w.GetInd(), w.GetTypeRoom());
                    wallPrev = new Wall(w.GetP1(), w.GetP2(), w.GetInd(), w.GetTypeRoom());
                    firstWall = new Wall(w.GetP1(), w.GetP2(), w.GetInd(), w.GetTypeRoom());
                    
                    isFound = true;
                    break;
                }
            }

            if (isFound)
            {
                break;
            }
        }

        border.Add(new Node(wall.GetP1(), wall.GetInd()));
        for (int i = 0; (!wall.IsEquals(firstWall) || i % 2 != 0) || border.Count == 1; i++)
        {
            // i % 2 == 0 - горизонтальная, иначе - вертикальная
            Wall wall1 = (i % 2 == 0) ? (GetWall(vertical[IndX(wall.GetP2())], wall.GetInd(), type)) 
            : (GetWall(horizontal[IndY(wall.GetP2())], wall.GetInd(), type));

            Wall wall2 = (i % 2 == 0) ? (GetWall(vertical[IndX(wall.GetP1())], wall.GetInd(), type)) 
            : (GetWall(horizontal[IndY(wall.GetP1())], wall.GetInd(), type));

            if (!wallPrev.IsEquals(wall1))
            {
                border.Add((i % 2 == 0) ? (new Node(wall.GetP2(), wall.GetInd())) : (new Node(wall.GetInd(), wall.GetP2())));

                wallPrev = new Wall(wall.GetP1(), wall.GetP2(), wall.GetInd(), wall.GetTypeRoom());
                wall = new Wall(wall1.GetP1(), wall1.GetP2(), wall1.GetInd(), wall1.GetTypeRoom());
                
                continue;
            }
            
            if (!wallPrev.IsEquals(wall2))
            {
                border.Add((i % 2 == 0) ? (new Node(wall.GetP1(), wall.GetInd())) : (new Node(wall.GetInd(), wall.GetP1())));

                wallPrev = new Wall(wall.GetP1(), wall.GetP2(), wall.GetInd(), wall.GetTypeRoom());
                wall = new Wall(wall2.GetP1(), wall2.GetP2(), wall2.GetInd(), wall2.GetTypeRoom());
            }
        }

        return border;
    }

    // Получение точки из списка соответствующего ind'а, в которой хотя-бы одна точка совпадает с 
    private Wall GetWall(List<Wall> wallList, int point, string type)
    {
        for (int i = 0; i < wallList.Count; i++)
        {
            Wall wall = wallList[i];
            
            if ((wall.GetP1() == point || wall.GetP2() == point) && type == wall.GetTypeRoom())
            {
                return new Wall(wall.GetP1(), wall.GetP2(), wall.GetInd(), wall.GetTypeRoom());
            }
        }
        // Такого не будет
        return null;
    }

}
