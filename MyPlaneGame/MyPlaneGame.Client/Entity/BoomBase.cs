using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyPlaneGame.Client
{
    /// <summary>
    /// 游戏对象：爆炸效果基类【抽象类】
    /// </summary>
    public abstract class BoomBase : GameObject
    {
        public BoomBase(int x, int y)
            : base(x, y)
        {

        }
    }
}
