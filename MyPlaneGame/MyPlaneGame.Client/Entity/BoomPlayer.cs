using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MyPlaneGame.Client.Properties;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象7：玩家爆炸效果
    /// </summary>
    public class BoomPlayer : BoomBase
    {
        private Image[] imgsBoom = { 
                                        Resources.hero_blowup_n1,
                                        Resources.hero_blowup_n2,
                                        Resources.hero_blowup_n3,
                                        Resources.hero_blowup_n4
                                    };
        public BoomPlayer(int x, int y)
            : base(x, y)
        { }

        // 重写Draw方法：绘制爆炸效果图片
        public override void Draw(Graphics g)
        {
            for (int i = 0; i < imgsBoom.Length; i++)
            {
                g.DrawImage(imgsBoom[i], this.X, this.Y);
            }
            //绘制完成后将爆炸的图片移除
            SingleObject.GetInstance().RemoveGameObject(this);
        }
    }
}
