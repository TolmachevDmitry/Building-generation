using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public class Rectangle
    {
        // Значения (в данном случае - это стенки, которыми мы будем двигать)
        // Вертикальные стены
        private Wall left;
        private Wall right;
        // Горизонтальные стены
        private Wall upp;
        private Wall down;
        // Тип комнаты, к которой принадлежит этот квадрат в т.ч. его потомки
        private string type;
        // Потомки
        private List<Rectangle> childLeft;
        private List<Rectangle> childRight;
        private List<Rectangle> childUpp;
        private List<Rectangle> childDown;
        // Флажки, для разрешения/неразрешения разветвления стенки
        private bool stopedLeft = false;
        private bool stopedRigth = false;
        private bool stopedUpp = false;
        private bool stopedDown = false;

        // Наш узел - Rectangle
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
        public List<Delta> moveLeft(int speed)
        {
            Delta old1 = left.changeInd(-speed);
            Delta old2 = upp.changeLength(-speed, 0);
            Delta old3 = down.changeLength(-speed, 0);

            return new List<Delta>(){old1, old2, old3};
        }

        public List<Delta> moveRight(int speed)
        {
            Delta old1 = right.changeInd(speed);
            Delta old2 = upp.changeLength(0, speed);
            Delta old3 = down.changeLength(0, speed);

            return new List<Delta>(){old1, old2, old3};;
        }

        // Горизонтальные
        public List<Delta> moveUpp(int speed)
        {
            Delta old1 = upp.changeInd(speed);
            Delta old2 = left.changeLength(0, speed);
            Delta old3 = right.changeLength(0, speed);

            return new List<Delta>(){old1, old2, old3};;
        }

        public List<Delta> moveDown(int speed)
        {
            Delta old1 = down.changeInd(-speed);
            Delta old2 = left.changeLength(-speed, 0);
            Delta old3 = right.changeLength(-speed, 0);

            return new List<Delta>(){old1, old2, old3};;
        }

        // Получаем стенки - экземпляры класса Wall
        public Wall getLeft()
        {
            return left;
        }

        public Wall getRight()
        {
            return right;
        }

        public Wall getUpp()
        {
            return upp;
        }

        public Wall getDown()
        {
            return down;
        }

        // Изменение значения флажков
        public void turnStopedLeft(bool value)
        {
            this.stopedLeft = value;
        }

        public void turnStopedRigth(bool value)
        {
            this.stopedRigth = value;
        }
        
        public void turnStopedUpp(bool value)
        {
            this.stopedUpp = value;
        }

        public void turnStopedDown(bool value)
        {
            this.stopedDown = value;
        }

        // Есть ли потомки ?
        public bool haveLeftChild()
        {
            return childLeft.Count != 0;
        }

        public bool haveRightChild()
        {
            return childRight.Count != 0;
        }

        public bool haveUppChild()
        {
            return childUpp.Count != 0;
        }

        public bool haveDownChild()
        {
            return childDown.Count != 0;
        }

        // Добавление потомков к разным стенам
        public void addLeftChild(int x1, int y1, int x2, int y2)
        {
            Rectangle rec = new Rectangle(x1, y1, x2, y2, type);
            rec.turnStopedLeft(true);
            childLeft.Add(rec);
        }

        public void addRightChild(int x1, int y1, int x2, int y2)
        {
            Rectangle rec = new Rectangle(x1, y1, x2, y2, type);
            rec.turnStopedRigth(true);
            childRight.Add(new Rectangle(x1, y1, x2, y2, type));
        }

        public void addUppChild(int x1, int y1, int x2, int y2)
        {
            Rectangle rec = new Rectangle(x1, y1, x2, y2, type);
            rec.turnStopedUpp(true);
            childUpp.Add(new Rectangle(x1, y1, x2, y2, type));
        }

        public void addDownChild(int x1, int y1, int x2, int y2)
        {
            Rectangle rec = new Rectangle(x1, y1, x2, y2, type);
            rec.turnStopedDown(true);
            childDown.Add(new Rectangle(x1, y1, x2, y2, type));
        }

        // Получение потомков
        public List<Rectangle> getLeftChilds()
        {
            return childLeft;
        }

        public List<Rectangle> getRightChilds()
        {
            return childRight;
        }

        public List<Rectangle> getUppChilds()
        {
            return childUpp;
        }

        public List<Rectangle> getDownChilds()
        {
            return childDown;
        }

        // Завершено ли разветвление ?
        public bool isCompleted()
        {
            return stopedLeft && stopedRigth && stopedUpp && stopedDown;
        }

        // Получаем состояние флажков
        public bool isLeftStoped()
        {
            return stopedLeft;
        }

        public bool isRightStoped()
        {
            return stopedRigth;
        }

        public bool isUppStoped()
        {
            return stopedUpp;
        }
        
        public bool isDownStoped()
        {
            return stopedDown;
        }

    }

    private string type;
    private Rectangle root = null;

    public Room(string type, int startX, int startY, int crushingFactor)
    {
        this.type = type;
        this.root = new Rectangle(startX, startX + 1, startY - 1, startY, type);
    }

    public Rectangle getRoot()
    {
        return root;
    }

    // Обход нашей комнаты по конутру - ПОДУМАТЬ!
    // public List<Wall> search()
    // {
    //     return searchMain(root, new List<Wall>() {left, down, right, upp}, new List<List<Rectangle>>() {childLeft, childDown, childRight, childUpp});
    // }

    // public List<Wall> searchMain(Rectangle rec, List<Wall> walls, List<List<Rectangle>> childs)
    // {
    //     for (int i = 0; i < walls.Count; i++)
    //     {
    //         for (int j = 0; j < childs[i]; j++)
    //         {
                
    //         }
    //     }
    // }

}
