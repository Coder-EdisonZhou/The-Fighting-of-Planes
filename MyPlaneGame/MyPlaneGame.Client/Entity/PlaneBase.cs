using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象：飞机对象的基类【抽象类】
    /// </summary>
    public abstract class PlaneBase : GameObject
    {
        private Image imgPlane; // 存储玩家飞机或敌人飞机的图片

        public PlaneBase(int x, int y, Image img, int speed, int life, Direction dir)
            : base(x, y, img.Width, img.Height, speed, life, dir)
        {
            this.imgPlane = img;
        }

        // 抽象方法：判断飞机是否阵亡
        public abstract void IsOver();
    }
}
