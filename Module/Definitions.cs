using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ascension.Module
{
    public enum CardType
    {
        Monsters = 0,
        Heros = 1,
        Constructs = 2,
    }

    public enum FactionType
    {
        None = 0,
        Lifebound = 1,
        Mechana = 2,
        Enlightened = 3,
        Void = 4,
    }

    public enum ResourceType
    {
        Runes = 0,
        Power = 1,
        Honor = 2,
    }

    static class MLib
    {
        private static Random rng = new Random();

        public static int GetRandomNumber( int maxValue )
        {
            return rng.Next( maxValue );
        }

        public static ICollection Shuffle(ICollection c)
        {
            object[] a = new object[c.Count];
            c.CopyTo(a, 0);
            byte[] b = new byte[a.Length];
            rng.NextBytes(b);   
            Array.Sort(b, a);
            return new ArrayList(a);
        }

    }
}
