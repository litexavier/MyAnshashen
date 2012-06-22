using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascension.Module
{
    class Player
    {
        private string name;

        private Area discardArea;
        private Area handArea;
        private Area deckArea;

        private Resource honor;

        public Player(string name)
        {
            this.name = name;
            discardArea = new Area(name+"的弃牌区");
            handArea = new Area(name+"的手牌区");
            deckArea = new Area(name+"的牌组区");
            honor = new Resource(ResourceType.Honor);
        }

        public void reset()
        {
            discardArea = new Area(name + "的弃牌区");
            handArea = new Area(name + "的手牌区");
            deckArea = new Area(name + "的牌组区");
            honor = new Resource(ResourceType.Honor);
        }

        public Resource Honor
        {
            get
            {
                return honor;
            }
            set
            {
                honor = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public Area DiscardArea
        {
            get
            {
                return discardArea;
            }
        }

        public Area HandArea
        {
            get
            {
                return handArea;
            }
        }

        public Area DeckArea
        {
            get
            {
                return deckArea;
            }
        }

        public override string ToString()
        {
            return name + "{" + deckArea + " " + handArea + " " + discardArea + "}";
        }
    }
}
