using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象：飞机子弹的基类
    /// </summary>
    public class BulletBase : GameObject
    {
        private Image imgBullet; // 存储玩家飞机和敌人飞机的子弹的图片

        // 记录子弹的威力
        public int Power
        {
            get;
            set;
        }

        public BulletBase(PlaneBase planeBase, Image img, int speed, int power)
            : base(planeBase.X + planeBase.Width / 2 - 30, planeBase.Y + planeBase.Height / 2 - 50, img.Width, img.Height, speed, 0, planeBase.Dir)
        {
            this.imgBullet = img;
            this.Power = power;
        }

        // 重写Draw方法
        public override void Draw(Graphics g)
        {
            Move();    // 移动坐标
            g.DrawImage(imgBullet, this.X, this.Y, this.Width / 2, this.Height / 2); // 绘制图片
        }

        // 重写Move方法
        public override void Move()
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
            }

            // 子弹发射之后控制子弹纵坐标
            if (this.Y <= 0)
            {
                this.Y = -100;
                // 在游戏中移除子弹对象
                SingleObject.GetInstance().RemoveGameObject(this);
            }
            if (this.Y >= 670)
            {
                this.Y = 870;
                // 在游戏中移除子弹对象
                SingleObject.GetInstance().RemoveGameObject(this);
            }
        }
    }
}
