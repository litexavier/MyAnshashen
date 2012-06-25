using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ascension.Module
{
    class GameSet
    {
        public const int CenterRowNum = 6;

        private Dictionary<string, Area> areas;
        
        private Resource honor;
        private ArrayList players;
        private ArrayList centerArea;

        public GameSet( ArrayList players, Resource honor )
        {
            areas = new Dictionary<string, Area>();
            centerArea = new ArrayList();
            this.honor = honor;
            this.players = players;
        }

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

        public Dictionary<string, Area> Areas
        {
            get
            {
                return areas;
            }
        }

        public int PlayerNum
        {
            get
            {
                return players.Count;
            }
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
        }
    }
}
