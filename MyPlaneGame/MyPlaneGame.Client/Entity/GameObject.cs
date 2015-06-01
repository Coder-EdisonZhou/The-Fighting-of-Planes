using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 抽象类：游戏对象基类
    /// </summary>
    public abstract class GameObject
    {
        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public int Speed
        {
            get;
            set;
        }

        public int Life
        {
            get;
            set;
        }

        public Direction Dir
        {
            get;
            set;
        }

        public GameObject(int x, int y, int width, int height, int speed, int life, Direction dir)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.Speed = speed;
            this.Life = life;
            this.Dir = dir;
        }

        public GameObject(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        // 实例方法：返回所在矩形区域用于碰撞检测
        public Rectangle GetRectangle()
        {
            return new Rectangle(this.X, this.Y, this.Width, this.Height);
        }

        // 抽象方法：游戏对象的绘制各不相同
        public abstract void Draw(Graphics g);

        // 虚方法：游戏对象的移动各不相同
        public virtual void Move()
        {
            // 根据指定的移动方向进行移动
            switch (Dir)
            {
                case Direction.Up:
                    this.Y -= this.Speed;
                    break;
                case Direction.Down:
                    this.Y += this.Speed;
                    break;
                case Direction.Left:
                    this.X -= this.Speed;
                    break;
                case Direction.Right:
                    this.X += this.Speed;
                    break;
            }

            // 移动之后判断是否超出了边界
            if (this.X <= 0)
            {
                this.X = 0;
            }
            if (this.X >= 380)
            {
                this.X = 380;
            }
            if (this.Y <= 0)
            {
                this.Y = 0;
            }
            if (this.Y >= 670)
            {
                this.Y = 670;
            }
        }
    }
}
