using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MyPlaneGame.Client.Properties;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象2：游戏标题
    /// </summary>
    public class GameTitle : GameObject
    {
        private static Image imgTitle = Resources.name;

        public GameTitle(int x, int y, int speed)
            : base(x, y, imgTitle.Width, imgTitle.Height, speed, 0, Direction.Down)
        {
        }

        // 重写Draw方法
        public override void Draw(Graphics g)
        {
            // 标题图片不停向下移动
            this.Y += Speed;
            if (this.Y >= 670)
            {
                this.Y = 800;
                // 移除游戏标题对象
                SingleObject.GetInstance().RemoveGameObject(this);
            }

            // 坐标改变后绘制图片到屏幕
            g.DrawImage(imgTitle, this.X, this.Y);
        }
    }
}
