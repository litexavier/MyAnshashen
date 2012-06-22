using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ascension.Module
{
    class Area
    {
        ArrayList cards;
        string name;

        public Area(string name)
        {
            this.name = name;
            this.cards = new ArrayList();
        }

        public void Shuffle()
        {
            cards = (ArrayList)MLib.Shuffle(cards.ToArray());
        }

        public ArrayList Cards
        {
            get
            {
                return cards;
            }
        }

        public int Num
        {
            get
            {
                return cards.Count;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public override string  ToString()
        {
            string ret = name;
            foreach (Card card in cards)
            {
                ret += card;
            }
            return ret;
        }

        public void MoveFromTopToTop ( Area desArea )
        {
            Card card = (Card)cards[0];
            this.RemoveAt(0);
            desArea.InsertToTop(card);
        }

        public void MoveFromTopToBottom(Area desArea)
        {
            Card card = (Card)cards[0];
            this.RemoveAt(0);
            desArea.InsertToBottom(card);
        }

        public void MoveAllToBottom(Area desArea)
        {
            while(cards.Count>0)
            {
                this.MoveFromTopToBottom(desArea);
            }
        }

        public void MoveAnyToTop (Card card, Area desArea)
        {
            this.Remove(card);
            desArea.InsertToTop(card);
        }

        public void MoveAnyToBottom(Card card, Area desArea)
        {
            this.Remove(card);
            desArea.InsertToBottom(card);
        }

        public void Remove( Card card )
        {
            cards.Remove(card);
            card.Belong = null;
        }

        public void RemoveAt(int index)
        {
            ((Card)cards[index]).Belong = null;
            cards.RemoveAt(index);
        }

        public void InsertToTop(Card card)
        {
            cards.Insert(0, card);
            card.Belong = this;
        }

        public void InsertToBottom(Card card)
        {
            cards.Add(card);
            card.Belong = this;
        }

        public void Add(Card card)
        {
            cards.Add(card);
            card.Belong = this;
        }
    }
}
