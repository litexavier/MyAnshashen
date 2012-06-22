using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionModule
{
    abstract class Card
    {
        protected CardType type;
        protected FactionType faction;
        protected Resource honor;
        protected string name;

        public CardType Type
        {
            get
            {
                return type;
            }
        }

        public FactionType Faction
        {
            get
            {
                return faction;
            }
        }

        public override string ToString()
        {
            return "(" + name + "," + type + "," + faction + "," + honor + ")";
        }
    }

    class MonsterCard : Card
    {
        public MonsterCard( string name )
        {
            this.type = CardType.Monsters;
            this.faction = FactionType.None;
            this.honor = new Resource(ResourceType.Honor, 0);
            this.name = name;
        }
    }

    class HeroCard : Card
    {
        public HeroCard( string name, FactionType faction)
        {
            this.type = CardType.Heros;
            this.faction = faction;
            this.honor = new Resource(ResourceType.Honor, 0);
            this.name = name;
        }

        public HeroCard( string name, FactionType faction, uint honorp)
        {
            this.type = CardType.Heros;
            this.faction = faction;
            this.honor = new Resource(ResourceType.Honor, honorp);
            this.name = name;
        }
    }

    class ConstructCard : Card
    {
        public ConstructCard( string name, FactionType faction, uint honorp)
        {
            this.type = CardType.Constructs;
            this.faction = faction;
            this.honor = new Resource(ResourceType.Honor, honorp);
            this.name = name;
        }
    }
}
