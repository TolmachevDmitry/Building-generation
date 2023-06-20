using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Node 
{
    public int x;
    public int y;

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class Circuit
{
    // Получить расстояние межу точками
    static double GetRay(Node p1, Node p2)
    {
        return Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
    }

    // Получить угол точкек относительно самой первой (для сортироки)
    static double GetCorner(Node p1, Node p2)
    {
        double dX = p1.x - p2.x;
        double dY = p1.y - p2.y;

        double r = GetRay(p1, p2);

        double corner = Math.Asin(dY / r) * 180 / Math.PI;

        if (dY < 0 && dX >= 0) {
            return 360 - corner;
        }

        if (dX < 0 && dY <= 0) 
        {
            return 180 + corner;
        }

        if (dX < 0 && dY > 0) 
        {
            return 180 - corner;
        }

        return corner;
        
    }

    // Сравнение точек для сортироки
    static double ComparePoints(Node p1, Node p2, Node p0)
    {
        double r1 = GetRay(p1, p0);
        double r2 = GetRay(p2, p0);

        double corner1 = GetCorner(p1, p0);
        double corner2 = GetCorner(p2, p0);

        if (corner1 - corner2 != 0) 
        {
            return corner1 - corner2;
        }
        else 
        {
            return r1 - r2;
        }
        
    }

    // Вспомогательный метод для сортироки
    static List<Node> SiftDown(List<Node> list, int k, int n, Node p0)
    {
        Node point = list[k];

        while (true) 
        {  
            int childIndex = 2 * k + 1;
            
            if (childIndex >= n) 
            {
                break;
            }
            
            if (childIndex + 1 < n && ComparePoints(list[childIndex + 1], list[childIndex], p0) > 0) 
            {
                childIndex++;
            }
            
            if (ComparePoints(point, list[childIndex], p0) > 0) 
            {
                break;
            }
        
            list[k] = list[childIndex];  
            k = childIndex;
        }
        
        list[k] = point;

        return list;
    }

    // Сортировка точек по полярным координатам (углу и расстоянию) относительно p0
    static List<Node> SortForCircuit(List<Node> list, int indexP0)
    {
        Node pStart = list[indexP0];
        list.RemoveAt(indexP0);

        int heapSize = list.Count;
    
        for (int i = heapSize / 2; i >= 0; i--) 
        {
            list = SiftDown(list, i, heapSize, pStart);
        }

        while (heapSize > 1) 
        {
            Node tmp = list[heapSize - 1];
            list[heapSize - 1] = list[0];
            list[0] = tmp;
            
            heapSize--;
            
            list = SiftDown(list, 0, heapSize, pStart);
        }

        return list;
    }

    // Проверяем, нужная ли это для нас точка (образует ли прямой или развёрнутый углы)
    static bool IsRightPoint(Node p0, Node p1, Node candidate)
    {
        if ((p0.x == p1.x && p1.x == candidate.x) || (p0.y == p1.y && p1.y == candidate.y))
        {
            return true;
        }

        double a = GetRay(p0, p1);
        double b = GetRay(p1, candidate);
        double c = GetRay(p0, candidate);

        return Math.Sqrt(a * a + b * b) == c;

    }

    // Лишняя точка ("торчащая линия")
    static bool IsExtra(Node p0, Node p1, Node newP)
    {
        bool a = p0.y == p1.y && newP.y == p0.y && ((p1.x > p0.x && newP.x < p1.x) || (p1.x < p0.x && newP.x > p1.x));
        bool b = p0.x == p1.x && newP.x == p0.x && ((p1.y > p0.y && newP.y < p1.y) || (p1.y < p0.y && newP.y > p1.y));

        return a || b;
    }

    // Определяем, по какой оси нам двигаться - по x или по ys
    static int[] GetVector(Node pStart, Node pGoal)
    {
        int[] vector = new int[2];

        vector[0] = (pStart.x > pGoal.x) ? (-1) : ((pStart.x == pGoal.x) ? (0) : (1));
        vector[1] = (pStart.y > pGoal.y) ? (-1) : ((pStart.y == pGoal.y) ? (0) : (1));

        return vector;
    }

    // Проверка, не будут ли образововаться "ножницы" (пепесечения отрезков) при переходе в такую точку
    static bool Within(int [,] matrix, Node pStart, Node penultimate,  Node pGoal)
    {
        int[] vector = GetVector(pStart, pGoal);

        int compY = (matrix.GetLength(0) - 1) / 2;
        int compX = (matrix.GetLength(1) - 1) / 2;

        int x = pStart.x;
        int y = pStart.y;
        while (x != pGoal.x || y != pGoal.y)
        {
            x += vector[0];
            y += vector[1];

            if (matrix[y + compY, x + compX] == 1 && !(x == penultimate.x && y == penultimate.y))
            {
                return false;
            }

        }

        return true;
    }

    // Заполнение границ контура (для проверки)
    static void BorderSetting(int [,] matrix, bool f, Node pStart, Node pGoal)
    {
        // Если f = true - заполнеяем отрезок 1-ами, в противном случае - 0-ми
        int value = (f) ? (1) : (0);

        int[] vector = GetVector(pStart, pGoal);
        int[,] newMatrix = matrix;

        int compY = (matrix.GetLength(0) - 1) / 2;
        int compX = (matrix.GetLength(1) - 1) / 2;

        int x = pStart.x;
        int y = pStart.y;
        while (x != pGoal.x || y != pGoal.y)
        {
            x += vector[0];
            y += vector[1];

            if (x != pStart.x || y != pStart.y)
            {   
                matrix[y + compY, x + compX] = value;
            }
        }
    }

    // Поиск контура (Основной метод для этой задачи)
    static List<Node> BuildingOutLine(List<Node> list, int indexP0, int n, int m) 
    {
        // Отсортированные точки
        Node firstPoint = list[indexP0];
        List<Node> nodes = SortForCircuit(list, indexP0);
        nodes.Insert(0, firstPoint);

        if(firstPoint.y != nodes[1].y)
        {
            Node pT = new Node(firstPoint.x + (new System.Random()).Next(1, n), firstPoint.y);
            nodes.Insert(1, pT);
        }

        // circuit - контур, circInt - порядковые номера точек в хронологической поледовательности добавления
        List<Node> circuit = new List<Node>();
        List<int> circInt = new List<int>();

        // Для проверки, занята (записана) ли i-ая точка или нет
        bool[] exitEn = new bool[nodes.Count];
        exitEn[0] = true;
        exitEn[1] = true;

        // Наш контур
        circuit.Add(nodes[0]);
        circuit.Add(nodes[1]);
        circInt.Add(0);
        circInt.Add(1);

        // Матрица, покаывающая заполненность всего контура (включая все точки между точками из circuit)
        int[,] matrixComp = new int[n + 1, m + 1];

        // Резервные точки
        List<Node> missingVer = new List<Node>();
        List<int> missingInt = new List<int>();

        // Таблица плохих элементов, путь в которые из конкретной точки ведёт в плохому исходу  (+ 1 для каждой оси - чтобы учитывать нули)
        int[,] enemies = new int[nodes.Count, nodes.Count];

        // Собственно алгоритм построения контура здания
        int i = 1;
        while (true)
        {
            bool pointFromMissing = false;
            bool pointFound = false;
            // Возвращаемся к заранее отложенным на время вершинам
            int k = 0;
            while (k < missingVer.Count)
            {
                // Подходит ли нам эта точка ?
                if (IsRightPoint(circuit[circuit.Count - 2], circuit[circuit.Count - 1], missingVer[k]) && Within(matrixComp, circuit[circuit.Count - 1], circuit[circuit.Count - 2], missingVer[k]) && enemies[circInt[circInt.Count - 1], missingInt[k]] != 1 && !exitEn[missingInt[k]])
                {
                    // Чтобы не образовывались "торчащие" линии
                    if (IsExtra(circuit[circuit.Count - 2], circuit[circuit.Count - 1], missingVer[k]))
                    {
                        BorderSetting(matrixComp, false, circuit[circuit.Count - 2], circuit[circuit.Count - 1]);
                        circuit.RemoveAt(circuit.Count - 1);
                    }

                    BorderSetting(matrixComp, true, circuit[circuit.Count - 1], missingVer[k]);

                    // Добавляем эту точку
                    circuit.Add(missingVer[k]);
                    circInt.Add(missingInt[missingInt.Count - 1]);
                    exitEn[missingInt[k]] = true;

                    missingVer.RemoveAt(k);
                    missingInt.RemoveAt(k);

                    // Фиксируем, что последняя добавленная точка перед вторым циклом была из резервного списка
                    pointFromMissing = true;
                }
                else 
                {
                    k += 1;
                }
            }

            int fixI = i;
            // Идём прямо по отсортированным точкам
            while (i + 1 < nodes.Count)
            {
                i += 1;

                // Подходит ли эта точка ?
                if (IsRightPoint(circuit[circuit.Count - 2], circuit[circuit.Count - 1], nodes[i]) && Within(matrixComp, circuit[circuit.Count - 1], circuit[circuit.Count - 2], nodes[i]) && enemies[circInt[circInt.Count - 1], i] != 1 && !exitEn[i])
                {
                    // Чтобы не образовывались "торчащие" линии
                    if (IsExtra(circuit[circuit.Count - 2], circuit[circuit.Count - 1], nodes[i]))
                    {
                        BorderSetting(matrixComp, false, circuit[circuit.Count - 2], circuit[circuit.Count - 1]);
                        circuit.RemoveAt(circuit.Count - 1);
                    }

                    // Добавляем точку
                    BorderSetting(matrixComp, true, circuit[circuit.Count - 1], nodes[i]);
                    circuit.Add(nodes[i]);
                    circInt.Add(i);
                    exitEn[i] = true;

                    pointFound = true;
                    break;
                }
                else if (pointFromMissing)
                {
                    missingVer.Add(nodes[i]);
                    missingInt.Add(i);
                }
            }

            // Чтобы не переполнять раньше времени список резервных точек
            if (pointFromMissing)
            {
                i = fixI;
            }

            // В случае, если из поседней точки мы так никуда не попадём и не смыкаем контур
            if (!pointFound && !IsRightPoint(circuit[circuit.Count - 2], circuit[circuit.Count - 1], circuit[0]))
            {
                int badPoint = circInt[circInt.Count - 1];
                int indOfBad = circInt[circInt.Count - 1];
                
                BorderSetting(matrixComp, false, circuit[circuit.Count - 2], circuit[circuit.Count - 1]);

                circuit.RemoveAt(circuit.Count - 1);
                circInt.RemoveAt(circInt.Count - 1);
                exitEn[indOfBad] = false;
                i = badPoint - 1;

                enemies[circInt[circInt.Count - 1], badPoint] = 1;
            }

            // Не пора ли нам останавливать алгоритм
            if (!pointFound && IsRightPoint(circuit[circuit.Count - 2], circuit[circuit.Count - 1], circuit[0]))
            {
                if (IsExtra(circuit[circuit.Count - 2], circuit[circuit.Count - 1], nodes[0]))
                {
                    BorderSetting(matrixComp, false, circuit[circuit.Count - 2], circuit[circuit.Count - 1]);
                    circuit.RemoveAt(circuit.Count - 1);
                }
 
                circuit.Add(nodes[0]);
                circInt.Add(0);

                break;
            }

        }

        return circuit;
    }

    public List<Node> CreateCircuit(int n, int m)
    {
        List<Node> nodes = new List<Node>();
        List<string> againstRepeat = new List<string>();

        int countPoints = 50;
        int minY = n;
        int minX = m;
        int index = 0;

        for (int i = 0; i < countPoints; i++)
        {
            int x, y;
            while (true)
            {
                x = (new System.Random()).Next(-m, m);
                y = (new System.Random()).Next(-n, n);
                string str = "" + x + ":" + y;
                
                if (!againstRepeat.Contains(str))
                {
                    againstRepeat.Add(str);
                    break;
                }
            }
            
            nodes.Add(new Node(x, y));

            if (minY > y || (minY == y && minX > x))
            {
                minY = y;
                minX = x;
                index = i;
            }
        }
        // Вызов основоного метода генерации контура
        List<Node> circuit = BuildingOutLine(nodes, index, 2 * n, 2 * m);
        return circuit;

    }
}
