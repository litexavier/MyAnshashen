using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Collections;

namespace AscensionModule
{
    class Cards : IEnumerable
    {
        private ArrayList card_list;

        public Cards()
        {
            card_list = new ArrayList();
        }

        public int Num
        {
            get
            {
                return card_list.Count;
            }
        }

        public Card this[int index]
        {
            get
            {
                return (Card)card_list[index];
            }
        }

        public void Add( Card card )
        {
            card_list.Add(card);
        }

        public void RemoveAt( int index )
        {
            card_list.RemoveAt(index);
        }

        public void AddToTop(Card card)
        {
            card_list.Insert(0, card);
        }

        public void Shuffle()
        {
            if (Num == 0) // do not need action
            {
                return;
            }
            card_list = (ArrayList)MLib.Shuffle(card_list.ToArray());
        }

        public override string ToString()
        {
            string ret = "";
            foreach (Card i in card_list)
            {
                ret += i.ToString();
            }
            return ret;
        }

        public IEnumerator GetEnumerator()
        {
            return new CardEnumerator(this);
        }

        private class CardEnumerator : IEnumerator
        {
            private int position = -1;
            private Cards cards;

            public CardEnumerator(Cards cards)
            {
                this.cards = cards;
            }

            public bool MoveNext()
            {
                position++;
                return position < cards.Num;
            }

            public void Reset()
            {
                position = -1;
            }

            public object Current
            {
                get
                {
                    return this.cards[position];
                }
            }

            public void Dispose()
            {
                this.cards = null;
            }
        }
    }
}
