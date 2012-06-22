using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascension.Module
{
    class Player
    {
        string name;

        private Area discardArea;
        private Area handArea;
        private Area deckArea;

        public Player()
        {
            discardArea = new Area("弃牌区");
            handArea = new Area("手牌区");
            deckArea = new Area("牌组区");
        }

        
    }
}
