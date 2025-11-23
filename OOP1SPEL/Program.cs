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

            Player player1 = new Player("Kevin", 0, 10, 1, 1, 1);

            Enemy zomb = new Enemy("Zombie");
            player1.NewAbility();
            player1.NewAbility();



            List<Character> characters = new() { player1 };

            
            Enemy kajsa = new Enemy("Kajsa");

        
         
            for(int i = 0; i < 10; i++)
            {
                Console.WriteLine("Round" + i);
                List<Character> newFig = new() { player1 };
                Enemy skele = new Enemy("skeleton" + i);
                for(int j = 0; j < i; j++) skele.LevelUp();
                skele.Actions.Add(new FireBall());
                skele.Actions.Add(new HealBuff());
                newFig.Add(skele);
                NewFight(newFig);

                newFig.Clear();
                if(i % 3 == 0){player1.NewAbility();}
            }
            

            NewFight(characters);
            Console.WriteLine("End Of Game");

            characters[0].Print();
            characters[1].Print();
        }
        static void NewFight(List<Character> chars)
        {
            Random rand = new();

            int current = rand.Next(0, 2);

            // Start combat and allow retry loops
            CombatManager.StartFight(chars);
            bool retryLoop;
            do
            {
                Animation.NewFightAnimation(chars[current]);
                // Fight loop
                while (chars[0].IsAlive() && chars[1].IsAlive() && !CombatManager.FightEnded)
                {
                    Console.Clear();
                    Character active = chars[current];
                    Character target = chars[1 - current];
                    Animation.TurnAnimation(active);
                    active.TakeTurn(target);

                    if (CombatManager.FightEnded)
                        break;

                    current = 1 - current;
                }

                // Ask combat manager to show appropriate end screen and decide retry
                retryLoop = CombatManager.PromptEndFight();

            } while (retryLoop);



        }
    }
}