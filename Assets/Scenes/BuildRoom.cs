using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildRoom : MonoBehaviour
{
    private CartOfBorders cart = null;
    private Speeds speeds = null;

    private string[] roomTypes = new string[] {"Bedroom", "Kitchen", "Living room 1", "Bathroom", "Living room 2", "Livig room 3"};

    // Поиск препятствий на пути к расширению вершины (Возвращаем максимальную координату, к которой мы вообще можем передвинуться)
    // Если мы можем передвинуть на координату, но там есть стена, то возвращаем dInt + 1 - как знак, что это последнее передвижение
    public int ValidPath(Wall wall, int dInd, int axis)
    {
        int p1 = wall.GetP1();
        int p2 = wall.GetP2();
        string type = wall.GetTypeRoom();

        int sign = dInd / Math.Abs(dInd); 
        int indEnd = wall.GetInd() + dInd;
        for (int ind = wall.GetInd(); (dInd > 0) ? (ind <= indEnd) : (ind >= indEnd); ind += sign)
        {
            if ((axis == 1) ? (cart.IsIntersectionVert1(new Wall(p1, p2, ind, type))) : (cart.IsIntersectionHoriz1(new Wall(p1, p2, ind, type))))
            {
                int answ = Math.Abs(wall.GetInd() - ind);
                return Math.Abs((ind == indEnd) ? (answ + 1) : (answ));
            }
        }

        return Math.Abs(dInd);
    }

    // Вычисляем потомков - стенки, которые мы будем потом "выдавливать" из данной. Изначально - это линии
    public List<Wall> GetChilds(Wall wall, int axis, int p1, int p2)
    {
        bool passEmpty = false;
        int a = p1;
        int b;
        int step = -(p1 - p2) / Math.Abs(p1 - p2);

        List<Wall> newChilds = new List<Wall>();
        for (int p = p1 + step; (p1 < p2) ? (p - 1 < p2) : (p + 1 > p2) ; p += step)
        {
            Wall piece = new Wall(Math.Min(p - step, p), Math.Max(p - step, p), wall.GetInd(), wall.GetTypeRoom());
            bool itEmpty = !((axis == 1) ? (cart.IsIntersectionVert1(piece)) : (cart.IsIntersectionHoriz1(piece)));
            if (itEmpty && !passEmpty)
            {
                a = p - step;
                passEmpty = true;
            }

            if ((!itEmpty || p == p2) && passEmpty)
            {
                b = (!itEmpty) ? (p - step) : (p);
                newChilds.Add(new Wall(Math.Min(a, b), Math.Max(a, b), wall.GetInd(), wall.GetTypeRoom()));
                passEmpty = false;
            }
        }

        return newChilds;
    }
    // Вычисляем стенки, которые мы будем потом "выдавливать" из данной стенки
    // Для обхода комнаты очень важно, напраавление, по которому будем вычислять потомков (сверху вниз или наоборот)
    private List<Wall> GetVertChilds(Wall wall, int side)
    {
        List<Node> both = BothPointWall(wall, 1, side);
        return GetChilds(wall, 1, both[0].y, both[1].y);
    }
    private List<Wall> GetHorizChilds(Wall wall, int side)
    {
        List<Node> both = BothPointWall(wall, 2, side);
        return GetChilds(wall, 2, both[0].x, both[1].x);
    }

    // Вычисление и вставка потомков. Возращаем новых добавленных потомков
    private List<Room.Rectangle> SetHorizChilds(Room.Rectangle rectangle, int side)
    {
        List<Wall> childs = GetHorizChilds((side == 1) ? (rectangle.GetUpp()) : (rectangle.GetDown()), side);
        List<Room.Rectangle> childsNew = new List<Room.Rectangle>();

        for (int i = 0; i < childs.Count; i++)
        {
            Wall c = childs[i];
            Wall t1 = new Wall(c.GetInd() + ((side == 2) ? (-1) : (0)), c.GetInd() + ((side == 1) ? (1) : (0)), c.GetP1(), c.GetTypeRoom());
            Wall t2 = new Wall(c.GetInd() + ((side == 2) ? (-1) : (0)), c.GetInd() + ((side == 1) ? (1) : (0)), c.GetP2(), c.GetTypeRoom());

            if (cart.IsIntersectionVert2(t1) || cart.IsIntersectionVert2(t2))
            {
                // Не добавляем потомки, если их стены совпадают с другими из той же комнаты - для непересечения 
                continue;
            }
            if (side == 1)
            {
                rectangle.AddUppChild(c.GetP1(), c.GetInd(), c.GetP2(), c.GetInd());
            }
            if (side == 2)
            {
                rectangle.AddDownChild(c.GetP1(), c.GetInd(), c.GetP2(), c.GetInd());
            }

            List<Room.Rectangle> currChilds = (side == 1) ? (rectangle.GetUppChilds()) : (rectangle.GetDownChilds());
            Room.Rectangle newChild = currChilds[currChilds.Count - 1];
            cart.AddVert(newChild.GetLeft());
            cart.AddVert(newChild.GetRight());
            cart.AddHoriz(newChild.GetUpp());
            cart.AddHoriz(newChild.GetDown());

            childsNew.Add(newChild);
        }

        return childsNew;
    }

    private List<Room.Rectangle> SetVertChilds(Room.Rectangle rectangle, int side)
    {
        List<Wall> childs = GetVertChilds((side == 1) ? (rectangle.GetLeft()) : (rectangle.GetRight()), side);
        List<Room.Rectangle> childsNew = new List<Room.Rectangle>();

        for (int i = 0; i < childs.Count; i++)
        {
            Wall c = childs[i];
            Wall t1 = new Wall(c.GetInd() + ((side == 1) ? (-1) : (0)), c.GetInd() + ((side == 2) ? (1) : (0)), c.GetP1(), c.GetTypeRoom());
            Wall t2 = new Wall(c.GetInd() + ((side == 1) ? (-1) : (0)), c.GetInd() + ((side == 2) ? (1) : (0)), c.GetP2(), c.GetTypeRoom());

            if (cart.IsIntersectionHoriz2(t1) || cart.IsIntersectionHoriz2(t2))
            {
                // Не добавляем потомки, если их стены совпадают с другими из той же комнаты - для непересечения
                continue;
            }
            if (side == 1)
            {
                rectangle.AddLeftChild(c.GetInd(), c.GetP1(), c.GetInd(), c.GetP2());
            }
            if (side == 2)
            { 
                rectangle.AddRightChild(c.GetInd(), c.GetP1(), c.GetInd(), c.GetP2());
            }

            List<Room.Rectangle> currChilds = (side == 1) ? (rectangle.GetLeftChilds()) : (rectangle.GetRightChilds());
            Room.Rectangle newChild = currChilds[currChilds.Count - 1];
            cart.AddVert(newChild.GetLeft());
            cart.AddVert(newChild.GetRight());
            cart.AddHoriz(newChild.GetUpp());
            cart.AddHoriz(newChild.GetDown());

            childsNew.Add(newChild);
        }

        return childsNew;
    }
    // Выявляем, было ли встречено препятствие на пути, сравнивая ожидаемый шаг и фактический
    public bool ObstacleEncountered(int dActual, int expectedInd)
    {
        return dActual < expectedInd || dActual == expectedInd + 1;
    }

    // Двигаем наши стены
    private bool MoveLeft(Room.Rectangle rectangle, int dInd)
    {
        // Debug.Log("Move Left");
        int dActual = ValidPath(rectangle.GetLeft(), -dInd, 1);
        Delta delt = rectangle.MoveLeft((dActual == dInd + 1) ? (dActual - 1) : (dActual))[0];
        cart.ChangeIndVert(delt.GetOld(), delt.GetNew());
        bool conclusion = ObstacleEncountered(dActual, dInd);
        if (conclusion)
        {
            rectangle.OpenLeftChild();
        }

        return conclusion;
    }
    
    private bool MoveRight(Room.Rectangle rectangle, int dInd)
    {
        int dActual = ValidPath(rectangle.GetRight(), dInd, 1);
        Delta delt = rectangle.MoveRight((dActual == dInd + 1) ? (dActual - 1) : (dActual))[0];
        cart.ChangeIndVert(delt.GetOld(), delt.GetNew());
        bool conclusion = ObstacleEncountered(dActual, dInd);
        if (conclusion)
        {
            rectangle.OpenRightChild();
        }

        return conclusion;
    }

    private bool MoveUpp(Room.Rectangle rectangle, int dInd)
    {
        int dActual = ValidPath(rectangle.GetUpp(), dInd, 2);
        Delta delt = rectangle.MoveUpp((dActual == dInd + 1) ? (dActual - 1) : (dActual))[0];
        cart.ChangeIndHoriz(delt.GetOld(), delt.GetNew());
        bool conclusion = ObstacleEncountered(dActual, dInd);
        if (conclusion)
        {
            rectangle.OpenUppChild();
        }

        return conclusion;
    }

    private bool MoveDown(Room.Rectangle rectangle, int dInd)
    {
        int dActual = ValidPath(rectangle.GetDown(), -dInd, 2);
        Delta delt = rectangle.MoveDown((dActual == dInd + 1) ? (dActual - 1) : (dActual))[0];
        cart.ChangeIndHoriz(delt.GetOld(), delt.GetNew());
        bool conclusion = ObstacleEncountered(dActual, dInd);
        if (conclusion)
        {
            rectangle.OpenDownChild();
        }

        return conclusion;
    }

    // Создаём точки, с которых будет "расти" комната
    private Node GetStartPoint(int n, int m, int crushingFactor)
    {
        bool isFound = false;
        // Максимальные по модулю
        int xMax = m * crushingFactor;
        int yMax = n * crushingFactor;

        int y1 = 0;
        int x1 = 0;
        int a = 0;
        int b = 0;
        // 1 Этап: Находим точку y, прямая которой проходит скозь стены здания и вычисляем дипазон [a; b] для x
        while(!isFound)
        {
            y1 = (new System.Random()).Next(-n + 1, n - 1) * crushingFactor;
            for (int x = -xMax; x <= xMax; x += crushingFactor)
            {
                if (cart.IsIntersectionVert1(new Wall(y1, y1 + 1, x, "don't care")))
                {
                    if (!isFound)
                    {
                        a = x;
                        isFound = true;
                    }
                    else
                    {
                        b = x;
                    }
                }
            }

            Wall checkWall1 = new Wall(a, a + 1, y1 + 1, "don't care");
            Wall checkWall2 = new Wall(a, a + 1, y1, "don't care");
            if (cart.IsIntersectionHoriz1(checkWall1) || cart.IsIntersectionHoriz1(checkWall2))
            {
                isFound = false;
            }
        }

        // 2 Этап: Находим точку x, такую, чтобы не пресекалась с другими начальнымим квадратами
        isFound = false;
        while(!isFound)
        {
            x1 = (new System.Random()).Next(a + 1, b - 2);
            
            if (IsFreePlace(new Node(x1, y1)))
            {
                isFound = true;
            }
        }

        return new Node(x1, y1);
    }

    // Не сталкивается ли с другими уже созадаными начальными квадратами или с контуром здания ?
    private bool IsFreePlace(Node p)
    {
        // Вертикальные
        Wall wall1 = new Wall(p.y, p.y + 1, p.x, "don't care");
        Wall wall2 = new Wall(p.y, p.y + 1, p.x + 1, "don't care");
        // Горизонтальные
        Wall wall3 = new Wall(p.x, p.x + 1, p.y + 1, "don't care");
        Wall wall4 = new Wall(p.x, p.x + 1, p.y, "don't care");

        bool vertIntersection = !cart.IsIntersectionVert1(wall1) && !cart.IsIntersectionVert1(wall2);
        bool horizIntersection = !cart.IsIntersectionHoriz1(wall3) && !cart.IsIntersectionHoriz1(wall4);

        return vertIntersection && horizIntersection;
    }

    // Главный метод
    public List<List<Node>> CreateRooms(List<Node> circuit, int n, int m, int crushingFactor)
    {
        // Этап 1: Инициализация данных
        cart = new CartOfBorders(circuit, n, m, crushingFactor);
        speeds = new Speeds((int) crushingFactor / 2);

        // Инициализация комнат в карте границ
        List<Room> rooms = new List<Room>();
        for (int i = 0; i < roomTypes.Length; i++)
        {
            Node startP = GetStartPoint(n, m, crushingFactor);

            Room room = new Room(roomTypes[i], startP.x, startP.y);
            rooms.Add(room);
            // Добавляем экземпляры стен прямо из класс, чтобы изменения "на лету" отражались в карте
            cart.AddVert(room.GetRoot().GetLeft());
            cart.AddVert(room.GetRoot().GetRight());
            cart.AddHoriz(room.GetRoot().GetUpp());
            cart.AddHoriz(room.GetRoot().GetDown());
        }

        // Этап 2: Инициализация списка незавершённых квадратов (частей комнат)
        List<List<Room.Rectangle>> activeRectangles = new List<List<Room.Rectangle>>(rooms.Count);
        for (int i = 0; i < rooms.Count; i++)
        {
            activeRectangles.Add(new List<Room.Rectangle>());
            activeRectangles[i].Add(rooms[i].GetRoot());
        }

        // Этап 3: Расширение комнат во все стороны
        bool stop = false;
        int itter = 0;
        while (!stop && itter < 50)
        {
            stop = true;
            for (int i = 0; i < rooms.Count; i++)
            {
                ExpansionOfUnfinishedRectangles(activeRectangles[i]);
                stop &= activeRectangles[i].Count == 0;
            }
            itter += 1;
        }
        
        // Этап 4: Получение контуров комнат с помощью обхода в глубину
        List<List<Node>> roomsCircuits = new List<List<Node>>();
        for (int i = 0; i < rooms.Count; i++)
        {
            roomsCircuits.Add(WalkAroundRoom(rooms[i].GetRoot()));
        }

        return roomsCircuits;
    }

    private void OutCart(List<List<Wall>> list, int f)
    {
        string str = (f == 1) ? ("vertical") : ("horizontal");
        int k = 0;
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[i].Count; j++)
            {
                Wall w = list[i][j];
                if (w.GetTypeRoom() == "Bedroom")
                {
                    Debug.Log("[" + k + "] -> " + str + "; p1: " + w.GetP1() + "; p2: " + w.GetP2() + "; ind: " + w.GetInd());
                }
            }
        }
    }

    // Единажды расширяем все квадраты конкретной комнаты (изначально квадратов всего один - корень)
    private void ExpansionOfUnfinishedRectangles(List<Room.Rectangle> activeRectangles)
    {
        int increment;
        // int itter = 0;
        for (int i = 0; i < activeRectangles.Count; i += increment)
        {
            int oldLength = activeRectangles.Count;
            bool isActive = RectangleExpansion(activeRectangles, i);
            increment = (activeRectangles.Count != oldLength) ? (activeRectangles.Count - oldLength + 1) : (1);

            if (!isActive)
            {
                activeRectangles.RemoveAt(i);
                increment -= 1;
            }
        }

    }
    // Расширяем конкретный квадрат, добавляем новых потоков, удаляем квадрат в случае его завершённости
    private bool RectangleExpansion(List<Room.Rectangle> activeRectangles, int index)
    {
        Room.Rectangle rectangle = activeRectangles[index];
        
        List<Room.Rectangle> newRectangles = new List<Room.Rectangle>();
        bool stayedActive = false;
        // Left
        if (!rectangle.IsLeftStoped() && rectangle.GetLeftChilds() == null)
        {
            MoveLeft(rectangle, speeds.GetSpeedLeft());
            stayedActive = true;
        }
        else if (rectangle.GetLeftChilds().Count == 0)
        {
            List<Room.Rectangle> childsNew = SetVertChilds(rectangle, 1);
            SqueezeChilds(childsNew);
            newRectangles = newRectangles.Concat(childsNew).ToList();
        }
        // Right
        if (!rectangle.IsRightStoped() && rectangle.GetRightChilds() == null)
        {
            MoveRight(rectangle, speeds.GetSpeedRight());
            stayedActive = true;
        }
        else if (rectangle.GetRightChilds().Count == 0)
        {
            List<Room.Rectangle> childsNew = SetVertChilds(rectangle, 2);
            SqueezeChilds(childsNew);
            newRectangles = newRectangles.Concat(childsNew).ToList();
        }
        // Upp
        if (!rectangle.IsUppStoped() && rectangle.GetUppChilds() == null)
        {
            MoveUpp(rectangle, speeds.GetSpeedUpp());
            stayedActive = true;
        }
        else if (rectangle.GetUppChilds().Count == 0)
        {
            List<Room.Rectangle> childsNew = SetHorizChilds(rectangle, 1);
            SqueezeChilds(childsNew);
            newRectangles = newRectangles.Concat(childsNew).ToList();
        }
        // Down
        if (!rectangle.IsDownStoped() && rectangle.GetDownChilds() == null)
        {
            MoveDown(rectangle, speeds.GetSpeedDown());
            stayedActive = true;
        }
        else if (rectangle.GetDownChilds().Count == 0)
        {
            List<Room.Rectangle> childsNew = SetHorizChilds(rectangle, 2);
            SqueezeChilds(childsNew);
            newRectangles = newRectangles.Concat(childsNew).ToList();
        }

        activeRectangles = newRectangles.Concat(activeRectangles).ToList();
        
        return stayedActive;
    }

    // Чтобы "выдавить" новоявленных потомков и включать их "в дело"
    // (они априори будут передвинуты хотя бы на одну клетку)
    // Нам нужно передвинуть только одну стену. Такая стена - противоположная той, которая граничит с квадратом-родителем
    private void SqueezeChilds(List<Room.Rectangle> childs)
    {
        for (int i = 0; i < childs.Count; i++)
        {
            Room.Rectangle child = childs[i];
            if (childs[i].IsRightStoped())
            {
                MoveLeft(childs[i], speeds.GetSpeedLeft());
            }
            if (childs[i].IsLeftStoped())
            {
                MoveRight(childs[i], speeds.GetSpeedRight());
            }
            if (childs[i].IsDownStoped())
            {
                MoveUpp(childs[i], speeds.GetSpeedUpp());
            }
            if (childs[i].IsUppStoped())
            {
                MoveLeft(childs[i], speeds.GetSpeedDown());
            }
        }
    }

    // Для упрощения будет принимать лишь корень - основе его будем вычислять начальные параметры для обхода
    // Корневой квадрат - единственный, у которого мы будем обходить все четыре стены
    public List<Node> WalkAroundRoom(Room.Rectangle root)
    {
        List<Wall> listWall = new List<Wall>() {root.GetLeft(), root.GetDown(), root.GetRight(), root.GetUpp()};
        List<List<Room.Rectangle>> listChildren = new List<List<Room.Rectangle>>() {root.GetLeftChilds(), root.GetDownChilds(), root.GetRightChilds(), root.GetUppChilds()};
        List<int> numbersWalls = new List<int>() {1, 2, 3, 4};

        return WalkAroundRoom(listWall, listChildren, numbersWalls);
    }

    // Алгоритм обхода команаты. Его цель - получение контура комнаты.
    // Принимает список стен и их потоков соответственно, а также индесы стен: 1 - left; 2 - down; 3 - right; 4 - upp
    private List<Node> WalkAroundRoom(List<Wall> walls, List<List<Room.Rectangle>> children, List<int> numbersWalls)
    {
        List<Node> roomCircuit = new List<Node>();
        // Обходим квадрат по всем сторонам, которые были переданы
        for (int i = 0; i < walls.Count; i++)
        {
            List<Node> bothPointWall = BothPointWall(walls[i], numbersWalls[i]);
            roomCircuit.Add(bothPointWall[0]);

            // Углубляемся в потомков
            List<Node> childrenPoint = new List<Node>();
            for (int j = 0; children[i] != null && j < children[i].Count; j++)
            {
                Wall w = walls[i];
                Room.Rectangle ch = children[i][j];
                int n = numbersWalls[i];
                List<Node> branch = WalkAroundRoom(ListWalls(ch, n), ListChildren(ch, n), ListNumberWall(n));

                childrenPoint = childrenPoint.Concat(branch).ToList();
            }

            if (childrenPoint.Count > 0)
            {
                DeleteExtraPoints(childrenPoint, bothPointWall);
                roomCircuit = roomCircuit.Concat(childrenPoint).ToList();
            }

            if (i == walls.Count - 1)
            {
                roomCircuit.Add(bothPointWall[1]);
            }
        }

        return roomCircuit;
    }

    // Удаление точек, совпадающих с точками потомков
    private void DeleteExtraPoints(List<Node> childrenPoint, List<Node> bothPointWall)
    { 
        Node childFirst = childrenPoint[0];
        Node childLast = childrenPoint[childrenPoint.Count - 1];

        if (childFirst.x == bothPointWall[0].x && childFirst.y == bothPointWall[0].y)
        {
            childrenPoint.RemoveAt(0);
        }
        if (childLast.x == bothPointWall[1].x && childLast.y == bothPointWall[1].y)
        {
            childrenPoint.RemoveAt(childrenPoint.Count - 1);
        }
        
    }

    // Функции для вычисления значений, которые необходимо передать в рекурсивный вызов функции WalkAroundRoom()
    // Вычисление стенок и порядка их обхода
    private List<Wall> ListWalls(Room.Rectangle rectangle, int numberWall)
    {
        List<Wall> listWalls = new List<Wall>();
        // Left
        if (numberWall == 1)
        {
            listWalls = new List<Wall>() {rectangle.GetUpp(), rectangle.GetLeft(), rectangle.GetDown()};
        }
        // Down
        if (numberWall == 2)
        {
            listWalls = new List<Wall>() {rectangle.GetLeft(), rectangle.GetDown(), rectangle.GetRight()};
        }
        // Right
        if (numberWall == 3)
        {
            listWalls = new List<Wall>() {rectangle.GetDown(), rectangle.GetRight(), rectangle.GetUpp()};
        }
        // Upp
        if (numberWall == 4)
        {
            listWalls = new List<Wall>() {rectangle.GetRight(), rectangle.GetUpp(), rectangle.GetLeft()};
        }

        return listWalls;
    }

    // Вычисление потомков
    private List<List<Room.Rectangle>> ListChildren(Room.Rectangle rectangle, int numberWall)
    {
        List<List<Room.Rectangle>> listChildren = new List<List<Room.Rectangle>>();
        // Left
        if (numberWall == 1)
        {
            listChildren = new List<List<Room.Rectangle>>() {rectangle.GetUppChilds(), rectangle.GetLeftChilds(), rectangle.GetDownChilds()};
        }
        // Down
        if (numberWall == 2)
        {
            listChildren = new List<List<Room.Rectangle>>() {rectangle.GetLeftChilds(), rectangle.GetDownChilds(), rectangle.GetRightChilds()};
        }
        // Right
        if (numberWall == 3)
        {
            listChildren = new List<List<Room.Rectangle>>() {rectangle.GetLeftChilds(), rectangle.GetDownChilds(), rectangle.GetRightChilds()};
        }
        // Upp
        if (numberWall == 4)
        {
            listChildren = new List<List<Room.Rectangle>>() {rectangle.GetRightChilds(), rectangle.GetUppChilds(), rectangle.GetLeftChilds()};
        }

        return listChildren;
    }

    // Вычисление индексов нужных стен в порядке их последовательного обхода
    private List<int> ListNumberWall(int numberWall)
    {
        List<int> listNumberWall = new List<int>();
        // Left
        if (numberWall == 1)
        {
            listNumberWall = new List<int>() {4, 1, 2};
        }
        // Down
        if (numberWall == 2)
        {
            listNumberWall = new List<int>() {1, 2, 3};
        }
        // Right
        if (numberWall == 3)
        {
            listNumberWall = new List<int>() {2, 3, 4};
        }
        // Upp
        if (numberWall == 4)
        {
            listNumberWall = new List<int>() {3, 4, 1};
        }

        return listNumberWall;
    }

    // Отличается от BothPointWall тем что, мы имеем на входе лишь numberWall. Эта фукнция конкретизирует задачу
    private List<Node> BothPointWall(Wall wall, int numberWall)
    {
        List<Node> bothPointsWall = new List<Node>();
        // Left
        if (numberWall == 1)
        {
            bothPointsWall = BothPointWall(wall, 1, 1);
        }
        // Down
        if (numberWall == 2)
        {
            bothPointsWall = BothPointWall(wall, 2, 2);
        }
        // Right
        if (numberWall == 3)
        {
            bothPointsWall = BothPointWall(wall, 1, 2);
        }
        // Upp
        if (numberWall == 4)
        {
            bothPointsWall = BothPointWall(wall, 2, 1);
        }

        return bothPointsWall;
    }

    // Определяет сверху вниз или снизу вверх мы идём по стене, возвращает точки стенки в порядке обхода (их всегда две)
    private List<Node> BothPointWall(Wall wall, int axis, int side)
    {
        List<Node> bothPointWall = new List<Node>();
        int ind = wall.GetInd();
        int p1 = wall.GetP1();
        int p2 = wall.GetP2();

        // Вертикальные ind = x
        if (axis == 1)
        {
            if (side == 1)
            {
                bothPointWall = new List<Node>() {new Node(ind, p2), new Node(ind, p1)};
            }
            if (side == 2)
            {
                bothPointWall = new List<Node>() {new Node(ind, p1), new Node(ind, p2)};
            }
        }
        // Горизонтальные ind = y
        if (axis == 2)
        {
            if (side == 1)
            {
                bothPointWall = new List<Node>() {new Node(p2, ind), new Node(p1, ind)};
            }
            if (side == 2)
            {
                bothPointWall = new List<Node>() {new Node(p1, ind), new Node(p2, ind)};
            }
        }

        return bothPointWall;
    }


    public List<Node> GetChainCircuit()
    {
        return cart.GetChainPoints("Circuit");
    }
}