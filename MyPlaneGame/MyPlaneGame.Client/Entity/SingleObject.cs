using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MyPlaneGame.Client
{
    /// <summary>
    ///  单例模式类
    /// </summary>
    public class SingleObject
    {
        private SingleObject()
        {
        }

        private static SingleObject singleInstance = null;

        public static SingleObject GetInstance()
        {
            if (singleInstance == null)
            {
                singleInstance = new SingleObject();
            }

            return singleInstance;
        }

        #region 单一实例对象列表

        // 1.游戏背景单一实例
        public GameBackground Background
        {
            get;
            set;
        }

        // 2.游戏标题单一实例
        public GameTitle Title
        {
            get;
            set;
        }

        // 3.玩家飞机单一实例
        public PlanePlayer Player
        {
            get;
            set;
        }

        // 4.玩家飞机子弹集合单一实例
        public List<BulletPlayer> PlayerBulletList= new List<BulletPlayer>();

        // 5.敌人飞机集合单一实例
        public List<PlaneEnemy> EnemyList = new List<PlaneEnemy>();

        // 6.敌人飞机子弹集合单一实例
        public List<BulletEnemy> EnemyBulletList = new List<BulletEnemy>();

        // 7.玩家飞机爆炸效果单一实例
        public List<BoomPlayer> PlayerBoomList = new List<BoomPlayer>();

        // 8.敌人飞机爆炸效果单一实例
        public List<BoomEnemy> EnemyBoomList = new List<BoomEnemy>();

        #endregion

        // 为游戏屏幕增加一个游戏对象
        public void AddGameObject(GameObject go)
        {
            if (go is GameBackground)
            {
                this.Background = go as GameBackground;
            }
            if (go is GameTitle)
            {
                this.Title = go as GameTitle;
            }
            if (go is PlanePlayer)
            {
                this.Player = go as PlanePlayer;
            }
            if (go is BulletPlayer)
            {
                this.PlayerBulletList.Add(go as BulletPlayer);
            }
            if (go is PlaneEnemy)
            {
                this.EnemyList.Add(go as PlaneEnemy);
            }
            if(go is BulletEnemy)
            {
                this.EnemyBulletList.Add(go as BulletEnemy);
            }
            if (go is BoomPlayer)
            {
                this.PlayerBoomList.Add(go as BoomPlayer);
            }
            if (go is BoomEnemy)
            {
                this.EnemyBoomList.Add(go as BoomEnemy);
            }
        }

        // 移除指定的游戏对象
        public void RemoveGameObject(GameObject go)
        {
            if (go is GameTitle)
            {
                this.Title = null;
            }
            if (go is BulletPlayer)
            {
                this.PlayerBulletList.Remove(go as BulletPlayer);
            }
            if (go is PlaneEnemy)
            {
                this.EnemyList.Remove(go as PlaneEnemy);
            }
            if (go is BulletEnemy)
            {
                this.EnemyBulletList.Remove(go as BulletEnemy);
            }
            if (go is BoomPlayer)
            {
                this.PlayerBoomList.Remove(go as BoomPlayer);
            }
            if (go is BoomEnemy)
            {
                this.EnemyBoomList.Remove(go as BoomEnemy);
            }
        }

        // 为游戏屏幕绘制游戏背景对象
        public void DrawFirstBackground(Graphics g)
        {
            if (Background != null)
            {
                Background.Draw(g);
            }
            if (Title != null)
            {
                Title.Draw(g);
            }
            if (Player != null)
            {
                Player.Draw(g);
            }
        }

        // 为游戏屏幕绘制所有游戏对象
        public void DrawGameObjects(Graphics g)
        {
            if (Background != null)
            {
                Background.Draw(g);
            }
            if (Player != null)
            {
                Player.Draw(g);
            }
            if (PlayerBulletList != null)
            {
                for (int i = 0; i < PlayerBulletList.Count; i++)
                {
                    PlayerBulletList[i].Draw(g);
                }
            }
            if (EnemyList != null)
            {
                for (int i = 0; i < EnemyList.Count; i++)
                {
                    EnemyList[i].Draw(g);
                }
            }
            if(EnemyBulletList != null)
            {
                for (int i = 0; i < EnemyBulletList.Count; i++)
                {
                    EnemyBulletList[i].Draw(g);
                }
            }
            if (PlayerBoomList != null)
            {
                for (int i = 0; i < PlayerBoomList.Count; i++)
                {
                    PlayerBoomList[i].Draw(g);
                }
            }
            if (EnemyBoomList != null)
            {
                for (int i = 0; i < EnemyBoomList.Count; i++)
                {
                    EnemyBoomList[i].Draw(g);
                }
            }
        }

        // 玩家得分
        public int Score
        {
            get;
            set;
        }

        // 碰撞检测方法
        public void CollisionDetect()
        {
            #region 1.判断玩家的子弹是否打到了敌人飞机身上
            for (int i = 0; i < PlayerBulletList.Count; i++)
            {
                for (int j = 0; j < EnemyList.Count; j++)
                {
                    if(PlayerBulletList[i].GetRectangle().IntersectsWith(EnemyList[j].GetRectangle()))
                    {
                        // 1.敌人的生命值减少
                        EnemyList[j].Life -= PlayerBulletList[i].Power;
                        // 2.生命值减少后判断敌人是否死亡
                        EnemyList[j].IsOver();
                        // 3.玩家子弹打到了敌人身上后将玩家子弹销毁
                        PlayerBulletList.Remove(PlayerBulletList[i]);
                        break;
                    }
                }
            }
            #endregion

            #region 2.判断敌人的子弹是否打到了玩家飞机身上
            for (int i = 0; i < EnemyBulletList.Count; i++)
            {
                if(EnemyBulletList[i].GetRectangle().IntersectsWith(Player.GetRectangle()))
                {
                    // 使玩家发生一次爆炸但不阵亡
                    Player.IsOver();
                    break;
                }
            }
            #endregion

            #region 3.判断敌人飞机是否和玩家飞机相撞
            for (int i = 0; i < EnemyList.Count; i++)
            {
                if (EnemyList[i].GetRectangle().IntersectsWith(Player.GetRectangle()))
                {
                    EnemyList[i].Life = 0;
                    EnemyList[i].IsOver();
                    break;
                }
            } 
            #endregion
        }
    }
}
