using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_draft : MonoBehaviour
{

    // Стартовые точки, от куда будет разростаться комната (или выступ, смотря от ситуации)
    private float x;
    private float y;
    private bool withVertexes;

    // Количество добавленных промежутков между основными точками.
    private int m;

    // Завершила ли комната своё расширение
    private bool full;

    // Тип создаваемой комнаты
    private string roomType;

    // При необходимости эти точки могут быть превращены в разрастающие пространства для заполнения оставщихся пробелов в контуре здания
    private List<Room_draft> vertexes;

    private float speedUpp;
    private float speedDown;
    private float speedLeft;
    private float speedRight;

    public Room_draft(float x, float y, bool withVertexes, int m, string type)
    {
        this.x = x;
        this.y = y;
        this.m = m;

        this.roomType = type;

        if (withVertexes)
        {
            addVertexes();
        }
    }

    // Разветвление опоной точки
    public void addVertexes()
    {
        for (int i = 0; i < 4; i++)
        {
            vertexes.Add(new Room_draft(x, y, false, m, roomType));
        }

        putSpeeds((new System.Random()).Next(1, (int) (m / 2)), (new System.Random()).Next(1, (int) (m / 2)), (new System.Random()).Next(1, (int) (m / 2)), (new System.Random()).Next(1, (int) (m / 2)));
        withVertexes = true;
    }

    private void putSpeeds(float upp, float down, float left, float right)
    {
        this.speedUpp = upp;
        this.speedDown = down;
        this.speedLeft = left;
        this.speedRight = right;
    }

    // Для передвижиения опорной точки
    public void moveX(float val)
    {
        x += val;
    }

    public void moveY(float val)
    {
        y += val;
    }

    // Для передвижения стенок по их напроавлениям
    // Здесь методы moveY() и moveX() уже работают в контексте классов созданных точек
    public void moveUpp()
    {
        if (withVertexes)
        {
            vertexes[2].moveY(speedUpp);
            vertexes[3].moveY(speedUpp);
        }
    }

    public void moveDown()
    {
        if (withVertexes)
        {
            vertexes[0].moveY(speedDown);
            vertexes[1].moveY(speedDown);
        }
    }

    public void moveLeft()
    {
        if (withVertexes)
        {
            vertexes[0].moveX(speedLeft);
            vertexes[3].moveX(speedLeft);
        }
    }

    public void moveRight()
    {
        if (withVertexes)
        {
            vertexes[1].moveX(speedRight);
            vertexes[2].moveX(speedRight);
        }
    }

    public bool isFull()
    {
        return full;
    }
}
