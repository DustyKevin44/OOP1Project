using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace MonsterBattler;

public class RoundInitiator
{
    private readonly ActionFactory _actionFactory;

    public RoundInitiator(ActionFactory actionFactory)
    {
        _actionFactory = actionFactory ?? throw new ArgumentNullException(nameof(actionFactory));
    }

    // Keep MainMenu static but create one RoundInitiator with the factory provided here.
    public void MainMenu()
    {
        // Create ONE shared ActionFactory and ONE shared RoundInitiator that will be reused.
        var sharedFactory = new ActionFactory();
        var roundSystem = new RoundInitiator(sharedFactory);

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
                "Story mode",
                "Quit"
            };

            int choice = UI.NiceMenu(header, menuItems);

            switch (choice)
            {
                case 0:
                    roundSystem.CreatePlayerAndBattle();
                    break;
                case 1:
                    roundSystem.RoundInitiater(null, 0);
                    break;
                case 2:
                    running = false;
                    Console.Clear();
                    Console.WriteLine("Thanks for playing!");
                    break;
            }
        }
    }

    // These helper methods do not require the factory and can remain static.
    static Player CreatePlayer()
    {
        Console.Clear();
        Console.WriteLine("════════════════════════════");
        Console.WriteLine("     CREATE YOUR PLAYER");
        Console.WriteLine("════════════════════════════");
        Console.WriteLine();
        Console.Write("Enter your player name: ");
        string playerName = Console.ReadLine() ?? "Hero";

        return new Player(playerName, 1, 1, 1, 1);
    }

    static Enemy StartRound(Character?[] chars, string enemyName)
    {
        Enemy e = new Enemy(enemyName);
        chars[1] = e;
        return e;
    }

    static void EndRound(Character?[] chars)
    {
        NewFight(chars);
        chars[1] = null;
        chars[0]!.LevelUp();
    }

    // Instance method — uses the injected _actionFactory for creating actions.
    public void RoundInitiater(Player? player, int round)
    {
        Character?[] chars = new Character?[2];

        switch (round)
        {
            case 0:
                {
                    Player p1 = CreatePlayer();
                    p1.NewAbility(1);
                    chars[0] = p1;
                    RoundInitiater(p1, 1);
                    break;
                }

            case 1:
                {
                    chars[0] = player;
                    player!.Actions.Add(new HealingPotion()!);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Animation.ShowText(new string[]
                    {
                        "You wake up in the heart of an enchanted forest",
                        "An eerie chorus of monsters cries is rapidly closing in."
                    });
                    Console.ResetColor();

                    Enemy e = StartRound(chars, "Zombie");
                    e.LevelUp(0, 2, 0, 0, 0);

                    var ram = _actionFactory.Create("Ram")!;
                    e.Actions.Add(ram);

                    EndRound(chars);
                    break;
                }
            case 2:
                {
                    chars[0] = player;

                    Animation.ShowText(new string[]
                    {
                        "The forest grows unnaturally silent...",
                        "A second zombie lurches from behind a twisted oak, its body twitching with violent rage.",
                        "Its hollow eyes lock onto you as it charges without hesitation."
                    });
                    Enemy e = StartRound(chars, "Enraged Zombie");
                    e.LevelUp(0, 2, 1, 0, 0);

                    var ram = _actionFactory.Create("Ram")!;
                    e.Actions.Add(ram);

                    EndRound(chars);
                    break;
                }

            case 3:
                {
                    chars[0] = player;
                    Animation.ShowText(new string[]
                    {
                        "Skeletons rise from the mist, bones clattering.",
                        "Their hollow eyes fix on you, empty and relentless."
                    });

                    Enemy e = StartRound(chars, "Skeleton");
                    e.LevelUp(0, 0, 2, 1, 1);

                    var fireBall = _actionFactory.Create("FireBall")!;
                    e.Actions.Add(fireBall);

                    var recoilShot = _actionFactory.Create("RecoilShot")!;
                    e.Actions.Add(recoilShot);

                    EndRound(chars);
                    break;
                }

            case 4:
                {
                    chars[0] = player;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Animation.ShowText(new string[]
                    {
                        "As you continue to wander through the forest you find your way towards a murky swamp",
                        "As there is no way around it, you have no choice but to enter"
                    });
                    Console.ResetColor();

                    Enemy e = StartRound(chars, "Daggelito🪱🪱🪱🪱🪱🪱🪱");
                    e.LevelUp(0, 2, 2);

                    var ram = _actionFactory.Create("Ram")!;
                    e.Actions.Add(ram);
                    var berserkStrike = _actionFactory.Create("BerserkStrike")!;
                    e.Actions.Add(berserkStrike);

                    EndRound(chars);
                    break;
                }

            case 5:
                {
                    chars[0] = player;
                    Animation.ShowText(new string[]
                    {
                        "A mutated serpent coils through the swamp reeds.",
                        "Its venomous eyes glint as it sense your presence."
                    });

                    Enemy e = StartRound(chars, "Mutated Swamp Serpent");
                    e.LevelUp(0, 1, 2, 1, 0);

                    var recoilShot = _actionFactory.Create("RecoilShot")!;
                    e.Actions.Add(recoilShot);

                    var weakenEnemy = _actionFactory.Create("WeakenEnemy")!;
                    e.Actions.Add(weakenEnemy);

                    EndRound(chars);
                    break;

                }
            case 6:
                {
                    chars[0] = player;
                    Animation.ShowText(new string[]
                    {
                        "A massive guardian emerges from the swamp's shadows.",
                        "Its hulking form blocks your path, ready to attack."
                    });


                    Enemy e = StartRound(chars, "Guardian of the swamp");
                    e.LevelUp(0, 3, 1, 2, 0);

                    var poisonGas = _actionFactory.Create("PoisonGas")!;
                    e.Actions.Add(poisonGas);
                    var ram = _actionFactory.Create("Ram")!;
                    e.Actions.Add(ram);

                    EndRound(chars);
                    break;
                }
            case 7:
                {
                    chars[0] = player;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Animation.ShowText(new string[]
                    {
                        "As you make your way out of the swamp you come across a mysterious cave",
                        "You choose to enter and explore"
                    });
                    Console.ResetColor();

                    Enemy e = StartRound(chars, "Fire Golem");
                    e.LevelUp(0, 3, 2, 2, 0);


                    var fireBall = _actionFactory.Create("FireBall")!;
                    e.Actions.Add(fireBall);
                    var berserkStrike = _actionFactory.Create("BerserkStrike")!;
                    e.Actions.Add(berserkStrike);

                    EndRound(chars);
                    break;
                }
            case 8:
                {
                    chars[0] = player;
                    Animation.ShowText(new string[]
                    {
                        "A gigantic spider descends from the ceiling, fangs dripping with venom.",
                        "Sticky webs litter the floor, threatening to trap you."
                    });

                    Enemy e = StartRound(chars, "Gigantic Spider");
                    e.LevelUp(0, 3, 2, 2, 0);

                    var webSnare = _actionFactory.Create("WebSnare")!;
                    e.Actions.Add(webSnare);
                    var weakenEnemy = _actionFactory.Create("WeakenEnemy")!;
                    e.Actions.Add(weakenEnemy);

                    EndRound(chars);
                    break;
                }
            case 9:
                {
                    chars[0] = player;

                    Animation.ShowText(new string[]
                    {
                        "A chill runs down your spine with each step,",
                        "Every move you make is being watched",
                        "Shadows shift as the EchoStalker circles unseen."
                    });

                    Enemy e = StartRound(chars, "EchoStalker");
                    e.LevelUp(0, 3, 2, 2, 0);

                    var recoilShot = _actionFactory.Create("RecoilShot")!;
                    e.Actions.Add(recoilShot);

                    var weakenEnemy = _actionFactory.Create("WeakenEnemy")!;
                    e.Actions.Add(weakenEnemy);

                    EndRound(chars);
                    break;
                }

            case 10:
                {  //en till attack? + textttt
                    chars[0] = player;
                    Animation.ShowText(new string[]
                    {
                        "As you find your way out of the cave a tall run-down mansion cathes your eye",
                        "You choose to enter",
                        "The air grows thick with dark energy.",
                        "A Satanic Priest steps forward, summoning power from the abyss."
                    });

                    Console.ForegroundColor = ConsoleColor.Red;
                    Animation.ShowText(new string[]
                    {

                    });
                    Console.ResetColor();

                    Enemy e = StartRound(chars, "Satanic Priest");
                    e.LevelUp(0, 0, 2, 3, 2);

                    var bloodOffering = _actionFactory.Create("BloodOffering")!;
                    e.Actions.Add(bloodOffering);

                    var summoning = _actionFactory.Create("Summoning")!;
                    e.Actions.Add(summoning);

                    EndRound(chars);
                    break;
                }
            // lägga till en mage m alla spells  och en mirror fiende som har samma stats och attacker som playern

            default:
                {
                    Console.WriteLine($"No logic implemented for round {round}.");
                    break;
                }
        }
    }

    // Now an instance method so it can use the injected factory for enemy creation/abilities.
    public void CreatePlayerAndBattle()
    {
        Player player = CreatePlayer();

        Console.WriteLine($"\nWelcome, {player.Name}!");
        System.Threading.Thread.Sleep(1000);

        string[] LevelHeader = new string[] { "You can now choose how many levels you want!" };
        List<string> LevelMenu = new List<string> { "" };
        for (int i = 1; i <= 10; i++)
        {
            LevelMenu.Add(i.ToString());
        }
        int playerLvl = UI.NiceMenu(LevelHeader, LevelMenu);
        for (int i = 0; i < playerLvl; i++) player.LevelUp();
        string[] abilityHeader = new string[] { "You can now choose your starting abilities!" };
        List<string> abilityMenu = new List<string> { "Continue" };
        UI.NiceMenu(abilityHeader, abilityMenu);

        player.NewAbility(1);
        player.NewAbility(3);

        bool continueBattle = true;
        while (continueBattle)
        {
            Enemy? enemy = CreateEnemyMenu();

            if (enemy == null)
            {
                continueBattle = false;
                break;
            }

            Character?[] combatants = new Character?[] { player, enemy };
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

    // Instance version so AddAbilitiesToEnemy can use the injected factory instance.
    public Enemy? CreateEnemyMenu()
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
            enemy.LevelUpCustom();
        }

        // Add abilities to enemy using the injected factory instance (no new factory).
        AddAbilitiesToEnemy(enemy);

        return enemy;
    }

    // Use the injected factory everywhere here (no new ActionFactory()).
    public void AddAbilitiesToEnemy(Enemy enemy)
    {
        string[] abilityHeader =
        {
            "════════════════════════════",
            $"    ADD ABILITIES TO {enemy.Name.ToUpper()}",
            "════════════════════════════",
            ""
        };

        // Get all abilities registered inside the injected ActionFactory
        List<string> allAbilities = _actionFactory.GetAllActionNames().ToList();
        allAbilities.Add("Done Adding Abilities");

        bool addingAbilities = true;

        while (addingAbilities)
        {
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
                // Use injected factory instance
                IAction? newAction = _actionFactory.Create(chosenName);

                if (newAction != null)
                {
                    enemy.Actions.Add(newAction);
                    Console.WriteLine($"Added {chosenName} to {enemy.Name}!");
                    System.Threading.Thread.Sleep(800);
                }
                else
                {
                    Console.WriteLine($"Could not create ability '{chosenName}'.");
                    System.Threading.Thread.Sleep(600);
                }
            }
        }
    }

    // NewFight doesn't need factory — keep static.
    static void NewFight(Character?[] chars)
    {
        Random rand = new();

        int current = rand.Next(0, 2);

        CombatManager.StartFight(chars);
        bool retryLoop;
        do
        {
            Animation.NewFightAnimation(chars[current]!);
            // Fight loop
            while (chars[0]!.IsAlive() && chars[1]!.IsAlive() && !CombatManager.FightEnded)
            {
                Console.Clear();
                Character active = chars[current]!;
                Character target = chars[1 - current]!;
                Animation.TurnAnimation(active);
                active.TakeTurn(target);

                if (CombatManager.FightEnded)
                    break;

                current = 1 - current;
            }

            retryLoop = CombatManager.PromptEndFight();

        } while (retryLoop);
    }
}
