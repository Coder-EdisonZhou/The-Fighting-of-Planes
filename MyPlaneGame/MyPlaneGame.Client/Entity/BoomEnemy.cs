using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MyPlaneGame.Client.Properties;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象6：敌人飞机爆炸效果
    /// </summary>
    public class BoomEnemy : BoomBase
    {
        private Image[] imgsSmall = {
                                    Resources.enemy0_down11,
                                    Resources.enemy0_down2,
                                    Resources.enemy0_down3,
                                    Resources.enemy0_down4
                                };
        private Image[] imgsMiddle = { 
                                    Resources.enemy1_down11,
                                    Resources.enemy1_down2,
                                    Resources.enemy1_down3,
                                    Resources.enemy1_down4
                                };
        private Image[] imgsLarge = { 
                                    Resources.enemy2_down11,
                                    Resources.enemy2_down2,
                                    Resources.enemy2_down3,
                                    Resources.enemy2_down4,
                                    Resources.enemy2_down5,
                                    Resources.enemy2_down6
                                };

        // 敌人飞机类型
        public int Type
        {
            get;
            set;
        }

        public BoomEnemy(int x, int y, int type)
            : base(x, y)
        {
            this.Type = type;
        }

        // 重写Draw方法
        public override void Draw(Graphics g)
        {
            //根据当前飞机的类型绘制爆炸效果图片
            switch (this.Type)
            {
                case 0:
                    for (int i = 0; i < imgsSmall.Length; i++)
                    {
                        g.DrawImage(imgsSmall[i], this.X, this.Y);
                    }
                    break;
                case 1:
                    for (int i = 0; i < imgsMiddle.Length; i++)
                    {
                        g.DrawImage(imgsMiddle[i], this.X, this.Y);
                    }
                    break;
                case 2:
                    for (int i = 0; i < imgsLarge.Length; i++)
                    {
                        g.DrawImage(imgsLarge[i], this.X, this.Y);
                    }
                    break;
            }

            //爆炸图片播放完成后销毁该爆炸效果
            SingleObject.GetInstance().RemoveGameObject(this);
        }
    }
}
