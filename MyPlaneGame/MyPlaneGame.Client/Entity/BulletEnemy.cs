using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MyPlaneGame.Client.Properties;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象6：敌人飞机子弹
    /// </summary>
    public class BulletEnemy : BulletBase
    {
        private static Image imgBulletEnemy = Resources.bullet11;

        public BulletEnemy(PlaneBase planeBase, int speed, int power)
            : base(planeBase, imgBulletEnemy, speed, power)
        {
        }

        public override void Draw(Graphics g)
        {
            base.Move();    // 移动坐标
            g.DrawImage(imgBulletEnemy, this.X + 30, this.Y + 10, this.Width / 2, this.Height / 2); // 绘制图片
        }
    }
}
