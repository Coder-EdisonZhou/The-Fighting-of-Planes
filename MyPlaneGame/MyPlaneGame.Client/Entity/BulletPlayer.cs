using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MyPlaneGame.Client.Properties;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象4：玩家飞机子弹
    /// </summary>
    public class BulletPlayer : BulletBase
    {
        private static Image imgBulletPlayer = Resources.bullet1;

        public BulletPlayer(PlaneBase planeBase, int speed, int power)
            : base(planeBase, imgBulletPlayer, speed, power)
        {
        }
    }
}
