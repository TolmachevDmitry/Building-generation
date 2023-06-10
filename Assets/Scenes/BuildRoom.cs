using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildRoom : MonoBehaviour
{

    private CartOfBorders cartOfBorders = null;
    private Speeds speeds = null;

    // Вычисляем потомков - стенки, которые мы будем потом "выдавливать" из данной
    private List<Wall> getChilds(Wall w, int ax)
    {
        bool passEmpty = false;
        int p0 = w.getP1();
        List<Wall> newChilds = new List<Wall>();
        for (int p = p0 + 1; p <= w.getP2(); p++)
        {
            Wall piece = new Wall(p - 1, p, w.getInd(), w.getType());
            bool itEmpty = !((ax == 1) ? (cartOfBorders.isClosedVert(w)) : (cartOfBorders.isClosedHoriz(w)));
            if (itEmpty && !passEmpty)
            {
                p0 = p - 1;
                passEmpty = true;
            }
            if (!itEmpty && passEmpty)
            {
                newChilds.Add(new Wall(p0, p - 1, w.getInd(), w.getType()));
                passEmpty = false;
            }
        }

        return newChilds;
    }  

    // Вычисляем стенки, которые мы будем потом "выдавливать" из данной стенки
    private List<Wall> getVertChilds(Wall w)
    {
        return getChilds(w, 1);
    }

    private List<Wall> getHorizChilds(Wall w)
    {
        return getChilds(w, 2);
    }

    // Поиск препятствий на пути к расширению вершины (Возвращаем максимальную координату, к которой мы вообще можем передвинуться)
    // Если на мы можем передвинуть на координату, но там есть стена, то возвращаем dInt + 1 - как знак, что это последнее передвижение
    // Возвращаем преращение
    private int validPath(Wall w, int dInd, int ax)
    {
        int p1 = w.getP1();
        int p2 = w.getP2();
        string type = w.getType();

        int sign = dInd / Math.Abs(dInd); 
        int indEnd = w.getInd() + dInd;
        for (int ind = w.getInd() + 1; ind <= indEnd; ind += sign)
        {
            if ((ax == 1) ? (cartOfBorders.isIntersectionVert(new Wall(p1, p2, ind, type))) : (cartOfBorders.isIntersectionHoriz(new Wall(p1, p2, ind, type))))
            {
                int answ = Math.Abs(w.getInd() - ind) * sign;
                return (ind == indEnd) ? (answ + 1) : (answ);
            }
        }

        return dInd;
    }

    // Вычисление и вставка потомков, если их не нашлось, то эту стену помечаем флагом true - чтобы потом её не трогать больше;
    private void setVerChilds(Room.Rectangle rec, int side)
    {
        List<Wall> childs = getVertChilds((side == 1) ? (rec.getUpp()) : (rec.getDown()));
        if (childs.Count == 0)
        {
            if (side == 1) 
            {
                rec.turnStopedUpp(true);
            } 
            else 
            {
                rec.turnStopedDown(true);
            }
        }

        for (int i = 0; i < childs.Count; i++)
        {
            Wall c = childs[i];
            if (side == 1)
            {
                rec.addUppChild(c.getP1(), c.getInd(), c.getP2(), c.getInd());
            }
            else 
            {
                rec.addDownChild(c.getP1(), c.getInd(), c.getP2(), c.getInd());
            }
        }
    }

    private void setHorizChilds(Room.Rectangle rec, int side)
    {
        List<Wall> childs = getHorizChilds((side == 1) ? (rec.getLeft()) : (rec.getRight()));
        if (childs.Count == 0)
        {
            if (side == 1) 
            {
                rec.turnStopedLeft(true);
            } 
            else 
            {
                rec.turnStopedRigth(true);
            }
        }

        for (int i = 0; i < childs.Count; i++)
        {
            Wall c = childs[i];
            if (side == 1)
            {
                rec.addLeftChild(c.getP1(), c.getInd(), c.getP2(), c.getInd());
            }
            else 
            { 
                rec.addRightChild(c.getP1(), c.getInd(), c.getP2(), c.getInd());
            }
        }
    }

    public bool obstacleEncountered(int dActual, int expectedInd)
    {
        return dActual < expectedInd || dActual == expectedInd + 1;
    }

    // Двигаем наши стены
    private bool moveLeft(Room.Rectangle rec, int dInd)
    {
        int dActual = validPath(rec.getLeft(), -dInd, 1);
        rec.moveLeft((dActual == dInd + 1) ? (dActual - 1) : (dActual));
        return obstacleEncountered(dActual, dInd);
    }

    private bool moveRight(Room.Rectangle rec, int dInd)
    {
        int dActual = validPath(rec.getRight(), dInd, 1);
        rec.moveRight((dActual == dInd + 1) ? (dActual - 1) : (dActual));
        return obstacleEncountered(dActual, dInd);
    }

    private bool moveUpp(Room.Rectangle rec, int dInd)
    {
        int dActual = validPath(rec.getUpp(), dInd, 2);
        rec.moveUpp((dActual == dInd + 1) ? (dActual - 1) : (dActual));
        return obstacleEncountered(dActual, dInd);
    }

    private bool moveDown(Room.Rectangle rec, int dInd)
    {
        int dActual = validPath(rec.getDown(), -dInd, 2);
        rec.moveDown((dActual == dInd + 1) ? (dActual - 1) : (dActual));
        return obstacleEncountered(dActual, dInd);
    }

    // Главный метод
    public void createRooms(List<Node> circuit, int n, int m, int crushingFactor)
    {
        // Типы комнат
        string[] roomTypes = new string[] {"Bedroom", "Kitchen", "Living room", "Bathroom"};

        cartOfBorders = new CartOfBorders(circuit, n, m, crushingFactor);
        speeds = new Speeds((int) crushingFactor / 2);

        // Инициализация комнат
        List<Room> rooms = new List<Room>();
        for (int i = 0; i < roomTypes.Length; i++)
        {
            Room r = new Room(roomTypes[i], 1, 1, 10);
            rooms.Add(r);
            cartOfBorders.addVert(r.getRoot().getLeft());
            cartOfBorders.addVert(r.getRoot().getRight());
            cartOfBorders.addHoriz(r.getRoot().getUpp());
            cartOfBorders.addHoriz(r.getRoot().getDown());
        }

        // Расширение корней наших комнат во все стороны одновременно
        bool stop = false;;
        while(!stop)
        {
            stop = true;
            for (int i = 0; i < rooms.Count; i++)
            {
                Room.Rectangle iRoom = rooms[i].getRoot();

                if (!iRoom.isLeftStoped() || !iRoom.haveLeftChild())
                {
                    stop &= moveLeft(iRoom, speeds.getSpeedLeft());
                }

                if (!iRoom.isRightStoped() || !iRoom.haveLeftChild())
                {
                    stop &= moveRight(iRoom, speeds.getSpeedRight());
                }

                if (!iRoom.isUppStoped() || !iRoom.haveLeftChild())
                {
                    stop &= moveUpp(iRoom, speeds.getSpeedUpp());
                }

                if (!iRoom.isDownStoped() || !iRoom.haveLeftChild())
                {
                    stop &= moveDown(iRoom, speeds.getSpeedDown());
                }
            }
        }

        // "Выдавливание" стенок для заполнения пробелов в конутре здания
        bool stop1 = false;
        stop = false;
        while(!false)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                
            }
        }
    }
}
