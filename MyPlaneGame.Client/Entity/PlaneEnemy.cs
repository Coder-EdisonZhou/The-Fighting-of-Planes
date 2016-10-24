using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using MyPlaneGame.Client.Properties;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象5：敌人飞机
    /// </summary>
    public class PlaneEnemy : PlaneBase
    {
        private static Image imgEnemySmall = Resources.enemy0;
        private static Image imgEnemyMiddle = Resources.enemy1;
        private static Image imgEnemyLarge = Resources.enemy2;

        // 敌人飞机类型：Small or Middle or Large ? 
        public int EnemyType
        {
            get;
            set;
        }

        public PlaneEnemy(int x, int y, int type)
            : base(x, y, GetImage(type), GetSpeed(type), GetLife(type), Direction.Down)
        {
            this.EnemyType = type;
        }

        // 根据飞机类型获取飞机图片
        private static Image GetImage(int type)
        {
            //静态函数中只能访问静态成员
            switch (type)
            {
                case 0:
                    return imgEnemySmall;
                case 1:
                    return imgEnemyMiddle;
                case 2:
                    return imgEnemyLarge;
            }
            return null;
        }

        // 根据飞机类型获取飞机速度
        private static int GetSpeed(int type)
        {
            //静态函数中只能访问静态成员
            switch (type)
            {
                case 0:
                    return 5;
                case 1:
                    return 6;
                case 2:
                    return 7;
            }
            return 0;
        }

        // 根据飞机类型获取对应生命值
        public static int GetLife(int type)
        {
            switch (type)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
            }
            return 0;
        }

        // 重写Draw方法
        public override void Draw(Graphics g)
        {
            // 根据飞机类型绘制不同类型飞机图片
            switch (this.EnemyType)
            {
                case 0:
                    g.DrawImage(imgEnemySmall, this.X, this.Y);
                    break;
                case 1:
                    g.DrawImage(imgEnemyMiddle, this.X, this.Y);
                    break;
                case 2:
                    g.DrawImage(imgEnemyLarge, this.X, this.Y);
                    break;
            }
            // 移动飞机的坐标
            Move();
        }

        private static Random random = new Random();

        // 重写Move方法
        public override void Move()
        {
            //根据游戏对象的方向进行移动
            switch (this.Dir)
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
                this.Y = 870;
                // 移除敌人飞机对象
                SingleObject.GetInstance().RemoveGameObject(this);
            }

            // 当敌人飞机为小飞机时并且移动到某个坐标时不停改变其的横坐标
            if (this.EnemyType == 0 && this.Y >= 250)
            {
                if (X >= 0 && this.X <= 220)
                {
                    this.X += random.Next(0, 50);
                }
                else
                {
                    this.X -= random.Next(0, 50);
                }
            }
            else // 如果不是小飞机则加快移动速度
            {
                this.Speed += 1;
            }

            // 敌人飞机发射子弹
            if (random.Next(0, 100) > 90)
            {
                Fire();
            }
        }

        // 敌人飞机发射子弹
        public void Fire()
        {
            SingleObject.GetInstance().AddGameObject(new BulletEnemy(this, 20, 1));
        }

        // 重写IsOver方法：判断敌人是否阵亡
        public override void IsOver()
        {
            if (this.Life <= 0)
            {
                SingleObject singleObject = SingleObject.GetInstance();
                // 1.首先移除自身
                singleObject.RemoveGameObject(this);
                // 2.播放敌人爆炸图片
                singleObject.AddGameObject(new BoomEnemy(this.X, this.Y, this.EnemyType));
                // 3.播放敌人爆炸声音
                SoundPlayer sp = new SoundPlayer(Resources.enemy0_down1);
                sp.Play();
                // 4.根据不同的敌人类型添加不同的分数
                switch (this.EnemyType)
                {
                    case 0:
                        singleObject.Score += 100;
                        break;
                    case 1:
                        singleObject.Score += 200;
                        break;
                    case 2:
                        singleObject.Score += 300;
                        break;
                }
            }
        }
    }
}
