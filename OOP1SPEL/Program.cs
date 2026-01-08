using System;
using System.Drawing;
using System.Numerics;

// Kevin och Kajsa 2025 8.2.1
namespace MonsterBattler
{

    public class Game
    {
        static void Main()
        {
            while (true)
            {
                try
                {
                    Init();
                }
                catch
                {

                }
            }

        }
        static void Init()
        {
            // KRAV 5:
            // 1: Beroendeinjektion
            // 2: Skapar en `ActionFactory` och injicerar den till `RoundInitiator` via konstruktorn.
            // 3: För att separera ansvar, öka återanvändbarhet och underlätta testning.
            ActionFactory factory = new ActionFactory();

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            RoundInitiator roundInitiator = new RoundInitiator(factory);
            roundInitiator.MainMenu();
            Console.WriteLine("Game close");
        }
    }
}