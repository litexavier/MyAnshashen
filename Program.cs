using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ascension
{
    using Module;

    class Program
    {
        static void Main(string[] args)
        {
            Resource res = new Resource(ResourceType.Honor, 1);
            Resource res2 = new Resource(ResourceType.Power);
            res2.Num += 5;
            System.Console.WriteLine(res.ToString());
            Cards cards = new Cards();
            cards.Add(new MonsterCard("邪教徒"));
            cards.Add(new MonsterCard("邪教徒"));
            cards.Add(new ConstructCard("妖刀村正",FactionType.Mechana, 3));
            cards.Add(new HeroCard("密教徒",FactionType.None));
            System.Console.WriteLine(cards.ToString());
            cards.Shuffle();
            System.Console.WriteLine(cards.ToString());
            System.Console.WriteLine(res2.ToString());
            System.Console.Read();
        }
    }
}
