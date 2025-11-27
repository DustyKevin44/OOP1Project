using System.Security.Cryptography.X509Certificates;

namespace MonsterBattler;
static class RoundInitiator
{
    public static void MainMenu()
        {
            bool running = true;

            while (running)
            {
                string[] header = new string[]
                {
                        "════════════════════════════",
                        "     MONSTER BATTLER ARENA",
                        "════════════════════════════",
                        ""
                };

                List<string> menuItems = new List<string>
                    {
                        "Create Player & Battle",
                        "Quit"
                    };

                int choice = UI.NiceMenu(header, menuItems);

                switch (choice)
                {
                    case 0:
                        CreatePlayerAndBattle();
                        break;
                    case 1:
                        running = false;
                        Console.Clear();
                        Console.WriteLine("Thanks for playing!");
                        break;
                }
            }
        }
        static Player CreatePlayer()
        {
            Console.Clear();
            Console.WriteLine("════════════════════════════");
            Console.WriteLine("     CREATE YOUR PLAYER");
            Console.WriteLine("════════════════════════════");
            Console.WriteLine();
            Console.Write("Enter your player name: ");
            string playerName = Console.ReadLine() ?? "Hero";

            return new Player(playerName, 0, 1, 1, 1, 1);
        }
    
        static void RoundInitiater(Player player, int round)
        {
            if(round == 0)
            {
                    // Get player name
                CreatePlayer();
            
            }else if(round == 1)
            {
                List<Character> chars = new List<Character>{player, new Enemy("Zombie")};
                NewFight(chars);
            }
        }

        static void CreatePlayerAndBattle()
        {
            // Get player name
            Player player = CreatePlayer();

            // Let player choose abilities
            Console.WriteLine($"\nWelcome, {player.Name}!");
            System.Threading.Thread.Sleep(1000);

            string[] LevelHeader = new string[] { "You can now choose how many levels you want!" };
            List<string> LevelMenu = new List<string> { "" };
            for (int i = 1; i <= 10; i++)
            {
                LevelMenu.Add(i.ToString());
            }
            int playerLvl = UI.NiceMenu(LevelHeader, LevelMenu);
            for(int i = 0; i < playerLvl; i++) player.LevelUp();
            string[] abilityHeader = new string[] { "You can now choose your starting abilities!" };
            List<string> abilityMenu = new List<string> { "Continue" };
            UI.NiceMenu(abilityHeader, abilityMenu);

            player.NewAbility();
            player.NewAbility();

            // Enter combat loop
            bool continueBattle = true;
            while (continueBattle)
            {
                Enemy? enemy = CreateEnemyMenu();

                if (enemy == null)
                {
                    continueBattle = false;
                    break;
                }

                List<Character> combatants = new() { player, enemy };
                NewFight(combatants);

                // Ask if player wants to continue
                if (player.IsAlive())
                {
                    string[] continueHeader = new string[]
                    {
                            "━━━━━━━━━━━━━━━━━━━━",
                            "Battle Complete!",
                            ""
                    };
                    
                    List<string> continueMenu = new List<string> { "Challenge Another Enemy", "Return to Main Menu" };
                    int continueChoice = UI.NiceMenu(continueHeader, continueMenu);

                    if (continueChoice == 1)
                        continueBattle = false;
                }
                else
                {
                    continueBattle = false;
                }
            }
        }

        static Enemy? CreateEnemyMenu()
        {
            string[] nameHeader = new string[]
            {
                    "════════════════════════════",
                    "    CREATE ENEMY",
                    "════════════════════════════",
                    ""
            };
            Thread.Sleep(200);


            Console.Clear();
            Console.WriteLine("════════════════════════════");
            Console.WriteLine("    ENTER ENEMY NAME");
            Console.WriteLine("════════════════════════════");
            Console.WriteLine();
            Console.Write("Enemy name: ");
            string? enemyInput = Console.ReadLine();
            string enemyName = string.IsNullOrWhiteSpace(enemyInput) ? "Enemy" : enemyInput;

            Enemy enemy = new Enemy(enemyName);

            // Level up menu
            string[] levelHeader = new string[]
            {
                    "════════════════════════════",
                    "    LEVEL UP ENEMY",
                    "════════════════════════════",
                    ""
            };

            List<string> levelMenu = new List<string> { "Random Levels", "Custom Levels" };
            int levelChoice = UI.NiceMenu(levelHeader, levelMenu);

            if (levelChoice == 0)
            {
                // Random levels
                string[] randomHeader = new string[]
                {
                        "════════════════════════════",
                        "    HOW MANY RANDOM LEVELS?",
                        "════════════════════════════",
                        ""
                };

                List<string> randomMenu = new List<string>();
                for (int i = 1; i <= 10; i++)
                {
                    randomMenu.Add(i.ToString());
                }

                int randomChoice = UI.NiceMenu(randomHeader, randomMenu);
                int levels = randomChoice + 1;

                for (int i = 0; i < levels; i++)
                {
                    enemy.LevelUp();
                }
            }
            else
            {
                // Custom levels
                enemy.LevelUpCustom();
            }

            // Add abilities to enemy
            AddAbilitiesToEnemy(enemy);

            return enemy;
        }
        static void AddAbilitiesToEnemy(Enemy enemy)
        {
            ActionFactory factory = new ActionFactory();

            string[] abilityHeader =
            {
        "════════════════════════════",
        $"    ADD ABILITIES TO {enemy.Name.ToUpper()}",
        "════════════════════════════",
        ""
        };

            // Get all abilities registered inside ActionFactory
            List<string> allAbilities = factory.GetAllActionNames().ToList();
            allAbilities.Add("Done Adding Abilities");

            bool addingAbilities = true;

            while (addingAbilities)
            {
                // Filter out already-learned abilities
                var available = allAbilities
                    .Where(a =>
                        a == "Done Adding Abilities" ||
                        !enemy.Actions.Any(x => x.GetType().Name.Equals(a, StringComparison.OrdinalIgnoreCase))
                    )
                    .ToList();

                int choice = UI.NiceMenu(abilityHeader, available);

                string chosenName = available[choice];

                if (chosenName == "Done Adding Abilities")
                {
                    addingAbilities = false;
                }
                else
                {
                    IAction? newAction = factory.Create(chosenName);

                    if (newAction != null)
                    {
                        enemy.Actions.Add(newAction);
                        Console.WriteLine($"Added {chosenName} to {enemy.Name}!");
                        System.Threading.Thread.Sleep(800);
                    }
                }
            }
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
