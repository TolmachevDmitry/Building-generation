using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public class Rectangle
    {
        // Cтенки, которыми мы будем двигать (притом, только в одну сторону)
        // Вертикальные стены
        private Wall left;
        private Wall right;
        // Горизонтальные стены
        private Wall upp;
        private Wall down;
        // Тип комнаты, к которой принадлежит этот квадрат в т.ч. его потомки
        private string type;
        // Потомки, могут быть - null, либо непустой экземпляр (хотя бы с одним элементом)
        private List<Rectangle> childLeft = null;
        private List<Rectangle> childRight = null;
        private List<Rectangle> childUpp = null;
        private List<Rectangle> childDown = null;
        // Флажки, для разрешения/неразрешения разветвления стенки
        private bool stopedLeft = false;
        private bool stopedRigth = false;
        private bool stopedUpp = false;
        private bool stopedDown = false;

        // Наш узел - Rectangle
        // --> x, y, x + 1, y - 1
        public Rectangle(int x1, int y1, int x2, int y2, string type)
        {
            this.left = new Wall(y1, y2, x1, type);
            this.right = new Wall(y1, y2, x2, type);

            this.upp = new Wall(x1, x2, y2, type);
            this.down = new Wall(x1, x2, y1, type);

            this.type = type;
        }

        // Передвигаем стенка (не забываем поддерживать связность с другими)
        // Вертикальные
        public List<Delta> MoveLeft(int speed)
        {
            Delta old1 = left.ChangeInd(-speed);
            Delta old2 = upp.ChangeLength(-speed, 0);
            Delta old3 = down.ChangeLength(-speed, 0);

            return new List<Delta>(){old1, old2, old3};
        }

        public List<Delta> MoveRight(int speed)
        {
            Delta old1 = right.ChangeInd(speed);
            Delta old2 = upp.ChangeLength(0, speed);
            Delta old3 = down.ChangeLength(0, speed);

            return new List<Delta>(){old1, old2, old3};;
        }

        // Горизонтальные
        public List<Delta> MoveUpp(int speed)
        {
            Delta old1 = upp.ChangeInd(speed);
            Delta old2 = left.ChangeLength(0, speed);
            Delta old3 = right.ChangeLength(0, speed);

            return new List<Delta>(){old1, old2, old3};;
        }

        public List<Delta> MoveDown(int speed)
        {
            Delta old1 = down.ChangeInd(-speed);
            Delta old2 = left.ChangeLength(-speed, 0);
            Delta old3 = right.ChangeLength(-speed, 0);

            return new List<Delta>(){old1, old2, old3};;
        }

        // Получаем стенки - экземпляры класса Wall
        public Wall GetLeft()
        {
            return left;
        }

        public Wall GetRight()
        {
            return right;
        }

        public Wall GetUpp()
        {
            return upp;
        }

        public Wall GetDown()
        {
            return down;
        }

        // Добавление потомков к разным стенам. Дубликаты созданы, чтобы инициализировать потомки, но оставлять пустым
        public void AddLeftChild(int x1, int y1, int x2, int y2)
        {
            childLeft = (childLeft != null) ? (childLeft) : (new List<Rectangle>());
            childLeft.Add(new Rectangle(x1, y1, x2, y2, type));
            childLeft[childLeft.Count - 1].TurnStopedRigth(true);
        }

        public void OpenLeftChild()
        {
            childLeft = new List<Rectangle>();
        }

        public void AddRightChild(int x1, int y1, int x2, int y2)
        {
            childRight = (childRight != null) ? (childRight) : (new List<Rectangle>());
            childRight.Add(new Rectangle(x1, y1, x2, y2, type));
            childRight[childRight.Count - 1].TurnStopedLeft(true);
        }

        public void OpenRightChild()
        {
            childRight = new List<Rectangle>();
        }

        public void AddUppChild(int x1, int y1, int x2, int y2)
        {
            childUpp = (childUpp != null) ? (childUpp) : (new List<Rectangle>());
            childUpp.Add(new Rectangle(x1, y1, x2, y2, type));
            childUpp[childUpp.Count - 1].TurnStopedDown(true);
        }

        public void OpenUppChild()
        {
            childUpp = new List<Rectangle>();
        }

        public void AddDownChild(int x1, int y1, int x2, int y2)
        {
            childDown = (childDown != null) ? (childDown) : (new List<Rectangle>());
            childDown.Add(new Rectangle(x1, y1, x2, y2, type));
            childDown[childDown.Count - 1].TurnStopedUpp(true);
        }

        public void OpenDownChild()
        {
            childDown = new List<Rectangle>();
        }

        // Получение потомков
        public List<Rectangle> GetLeftChilds()
        {
            return childLeft;
        }

        public List<Rectangle> GetRightChilds()
        {
            return childRight;
        }

        public List<Rectangle> GetUppChilds()
        {
            return childUpp;
        }

        public List<Rectangle> GetDownChilds()
        {
            return childDown;
        }

        // Имеет ли стена хотя бы одного потомка
        public bool HaveLeftChilds()
        {
            return childLeft != null && childLeft.Count > 0;
        }

        public bool HaveRightChilds()
        {
            return childRight != null && childRight.Count > 0;
        }

        public bool HaveUppChilds()
        {
            return childUpp != null && childUpp.Count > 0;
        }

        public bool HaveDownChilds()
        {
            return childDown != null && childDown.Count > 0;
        }

        // Завершено ли разветвление ? Завершено только если стена не имеет потомков либо у её потомков не может быть больше потомков 
        public bool IsCompleted()
        {
            return stopedLeft && stopedRigth && stopedUpp && stopedDown;
        }

        // Получаем состояние флажков
        public bool IsLeftStoped()
        {
            return stopedLeft;
        }

        public bool IsRightStoped()
        {
            return stopedRigth;
        }

        public bool IsUppStoped()
        {
            return stopedUpp;
        }
        
        public bool IsDownStoped()
        {
            return stopedDown;
        }

        // Изменение значения флажков
        public void TurnStopedLeft(bool value)
        {
            this.stopedLeft = value;
        }

        public void TurnStopedRigth(bool value)
        {
            this.stopedRigth = value;
        }
        
        public void TurnStopedUpp(bool value)
        {
            this.stopedUpp = value;
        }

        public void TurnStopedDown(bool value)
        {
            this.stopedDown = value;
        }
    }

    private string type;
    private Rectangle root = null;

    public Room(string type, int startX, int startY)
    {
        this.type = type;
        this.root = new Rectangle(startX, startY, startX + 1, startY + 1, type);
    }

    public Rectangle GetRoot()
    {
        return root;
    }
}
