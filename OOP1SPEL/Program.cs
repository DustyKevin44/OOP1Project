using System;
using System.Drawing;
using System.Numerics;

// Kevin och Kajsa 2025 8.2.1
namespace MonsterBattler
{

    public class Smain
    {
        static void Main()
        {
            ActionFactory factory = new ActionFactory();

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            RoundInitiator roundInitiator = new RoundInitiator(factory);
            roundInitiator.MainMenu();
            Console.WriteLine("Game close");
        }
    }
}