using System;
using System.Drawing;
using System.Numerics;

// Kevin 2025 8.2.1
namespace MonsterBattler
{

    public class Smain
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string pl1 = Console.ReadLine() ?? "Default 1";

            Player player1 = new Player(pl1, 0, 100, 10, 1, 1);
            Enemy zomb = new Enemy("Zombie");

            player1.NewAbility();
            zomb.Actions.Add(new FireBall());



            List<Character> characters = new() { player1, zomb };
            NewFight(characters);
            characters.Clear();
            characters.Add(player1);
            Enemy kajsa = new Enemy("Kajsa");
            characters.Add(kajsa);
            kajsa.LevelUp(100);
            kajsa.Actions.Add(new GambleBolt());
            
            

            NewFight(characters);
            Console.WriteLine("End Of Game");

            characters[0].Print();
            characters[1].Print();
        }
        static void NewFight(List<Character> chars)
        {
            Random rand = new();

            int current = rand.Next(0, 2);
            while (chars[0].IsAlive() && chars[1].IsAlive())
            {
                Console.Clear();
                // Determine current and target player
                Character active = chars[current];
                Character target = chars[1 - current];

                // Execute turn
                active.TakeTurn(target);

                // Switch to next player
                current = 1 - current;
            }



        }
    }
}