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

            System.Console.WriteLine("------------------Area Add Test----------------------");
            Area areaA = new Area("手牌区");
            areaA.Add(new MonsterCard("邪教徒"));
            areaA.Add(new MonsterCard("邪教徒"));
            areaA.Add(new ConstructCard("妖刀村正",FactionType.Mechana, 3));
            areaA.Add(new HeroCard("密教徒",FactionType.None));
            System.Console.WriteLine(areaA.ToString());
            System.Console.WriteLine("------------------Area Shuffle Test----------------------");
            areaA.Shuffle();
            System.Console.WriteLine(areaA.ToString());

            System.Console.WriteLine("------------------Area Move Test----------------------");
            Area areaB = new Area("弃牌区");
            areaA.MoveAllToBottom(areaB);
            System.Console.WriteLine(areaA.ToString());
            System.Console.WriteLine(areaB.ToString());
            System.Console.WriteLine("------------------Area Move Test 2----------------------");
            areaB.MoveFromTopToTop(areaA);
            areaB.MoveFromTopToTop(areaA);
            System.Console.WriteLine(areaA.ToString());
            System.Console.WriteLine(areaB.ToString());

            System.Console.WriteLine("------------------New Card Test----------------------");
            System.Console.WriteLine(new MonsterCard("死神"));

            System.Console.WriteLine(res2.ToString());
            System.Console.Read();
        }
    }
}
