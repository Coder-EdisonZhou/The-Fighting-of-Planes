using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MyPlaneGame.Client.Properties;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象1：游戏背景
    /// </summary>
    public class GameBackground : GameObject
    {
        private static Image imgBackground = Resources.background;
        private Image imgTitle = Resources.name;

        public GameBackground(int x, int y, int speed)
            : base(x, y, imgBackground.Width, imgBackground.Height, speed, 0, Direction.Down)
        {
        }

        // 重写Draw方法
        public override void Draw(Graphics g)
        {
            // 背景图片不停向下移动
            this.Y += Speed;
            if (this.Y == 0)
            {
                this.Y = -850;
            }

            // 坐标改变后绘制图片到屏幕
            g.DrawImage(imgBackground, this.X, this.Y);
        }
    }
}
