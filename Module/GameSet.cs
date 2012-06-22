using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ascension.Module
{
    class GameSet
    {
        const int CenterRowNum = 6;

        private ArrayList players;
        private ArrayList centerArea;
        private Area mysticArea;
        private Area infantryArea;
        private Area cultistArea;

        private Area centerDeckArea;
        private Area voidArea;

        public ArrayList Players 
        {
            get
            {
                return players;
            }
        }

        public ArrayList CenterArea
        {
            get
            {
                return centerArea;
            }
        }

        public Area MysticArea
        {
            get
            {
                return mysticArea;
            }
        }

        public Area InfantryArea
        {
            get
            {
                return infantryArea;
            }
        }

        public Area CultistArea
        {
            get
            {
                return cultistArea;
            }
        }

        public Area CenterDeckArea
        {
            get
            {
                return centerDeckArea;
            }
        }

        public Area VoidArea
        {
            get
            {
                return voidArea;
            }
        }

        public int PlayerNum
        {
            get
            {
                return players.Count;
            }
        }

        public GameSet()
        {
            players = new ArrayList();
            centerArea = new ArrayList();
            for (int i = 1; i <= CenterRowNum; i++)
            {
                centerArea.Add(new Area("中央牌区" + i.ToString()));
            }
            mysticArea = new Area("秘教士");
            infantryArea = new Area("重装步兵");
            cultistArea = new Area("邪教徒");
            centerDeckArea = new Area("中央牌库");
            voidArea = new Area("虚空区");
        }

        public void reset()
        {
            centerArea = new ArrayList();
            for (int i = 1; i <= CenterRowNum; i++)
            {
                centerArea.Add(new Area("中央牌区" + i.ToString()));
            }
            mysticArea = new Area("秘教士");
            infantryArea = new Area("重装步兵");
            cultistArea = new Area("邪教徒");
            centerDeckArea = new Area("中央牌库");
            voidArea = new Area("虚空区");
        }

        public void AddPlayer (Player player)
        {
            players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
        }
    }
}
