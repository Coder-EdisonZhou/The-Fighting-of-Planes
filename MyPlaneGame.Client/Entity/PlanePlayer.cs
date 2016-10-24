using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MyPlaneGame.Client.Properties;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象3：玩家飞机
    /// </summary>
    public class PlanePlayer : PlaneBase
    {
        private static Image imgPlayer = Resources.hero1;

        public PlanePlayer(int x, int y, int speed, int life, Direction dir)
            : base(x, y, imgPlayer, speed, life, dir)
        {
        }

        // 重写Draw方法绘制玩家飞机图片至屏幕
        public override void Draw(Graphics g)
        {
            g.DrawImage(imgPlayer, this.X, this.Y, this.Width / 2, this.Height / 2);
        }

        // 随着鼠标移动而移动
        public void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            this.X = e.X;
            this.Y = e.Y;
        }

        // 开炮发射子弹
        public void Fire()
        {
            SingleObject.GetInstance().AddGameObject(new BulletPlayer(this, 15, 1));
        }

        // 重写IsOver方法：判断玩家是否阵亡
        public override void IsOver()
        {
            SingleObject singleObject = SingleObject.GetInstance();
            singleObject.AddGameObject(new BoomPlayer(this.X, this.Y));
            if (singleObject.Score >= 100)
            {
                singleObject.Score -= 50;
            }
        }
    }
}
