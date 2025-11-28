namespace MonsterBattler
{
    public class Player : Character
    {
        public Player(string name, int strength,
            int vitality, int intelligence, int dexterity)
            : base(name, strength, vitality, intelligence, dexterity) { }

        public override void TakeTurn(Character target)
        {
            ActionMenu(target);
        }

        public void ActionMenu(Character target)
        {
            if (!IsAlive())
            {
                Console.WriteLine($"{Name} is dead, cannot act!");
                target.LevelUp();
                return;
            }

            // Keep showing the action menu until the player performs an action or gives up
            bool turnTaken = false;
            while (!turnTaken)
            {
                List<string> menuItems = new() { "Actions", "Give Up" };

                string[] header = BuildHeader(target, "Choose your action:");
                int choice = UI.NiceMenu(header, menuItems);

                if (choice == 0)
                {
                    // AttackMenu returns true when an action was executed (consumes the turn).
                    turnTaken = AttackMenu(target);
                }
                else
                {
                    GiveUp();
                    turnTaken = true;
                }
            }
        }

        // Returns true if an action was executed (turn consumed), false if player backed out
        public bool AttackMenu(Character target)
        {
            bool showDescriptions = false;

            while (true)
            {
                List<string> menuItems = new();

                // Build display entries depending on toggle
                for (int i = 0; i < Actions.Count; i++)
                {
                    var a = Actions[i];
                    if (a == null)
                    {
                        menuItems.Add("UNKNOWN");
                        continue;
                    }

                    if (!showDescriptions)
                    {
                        menuItems.Add(a.GetType().Name);
                    }
                    else
                    {
                        menuItems.Add($"{a.Name} - {a.GetInfo(this)}");
                      
                    }
                }

                // Add Toggle Description option just above Back
                menuItems.Add(showDescriptions ? "Hide Descriptions" : "Toggle Description");
                menuItems.Add("Back");

                string[] header = BuildHeader(target, "Choose attack:");
                int choice = UI.NiceMenu(header, menuItems);

                // If player selected one of the actions
                if (choice >= 0 && choice < Actions.Count)
                {
                    Actions[choice].Execute(this, target);
                    return true; // action executed, consume turn
                }

                int toggleIndex = Actions.Count;
                int backIndex = Actions.Count + 1;

                if (choice == toggleIndex)
                {
                    showDescriptions = !showDescriptions;
                    // continue loop to redraw menu
                    continue;
                }

                // Back selected -> do not consume turn, return to outer menu
                if (choice == backIndex)
                {
                    return false;
                }
            }
        }

        private string[] BuildHeader(Character target, string bottomText)
        {
            return new string[]
            {
            "__________________________________________________________________",
            $"                                                  {target.Health}/{target.MaxHealth}",
            $"                                                  {target.Name}",
            "",
            "",
            "",
            $"{Health}/{MaxHealth}",
            $"{Name}",
            "__________________________________________________________________",
            bottomText
            };
        }
        public void NewAbility(int maxTier)
        {
            ActionFactory factory = new ActionFactory();

            List<string> allAbilities = factory.GetAllActionNames().ToList();

            HashSet<string> ownedNames = Actions
                .Select(a => a.GetType().Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            List<string> unownedAbilities = allAbilities
                .Where(a => !ownedNames.Contains(a))
                .ToList();

           
            List<string> validAbilityNames = unownedAbilities
                .Where(a =>
                {
                    var instance = factory.Create(a)!;
                    int tier = instance switch
                    {
                        Attack atk => atk.Tier,
                        Buff buf => buf.Tier,
                        _ => 1
                    };
                    return tier <= maxTier;
                })
                .ToList();

            List<string> menuHeader = new()
    {
        $"{Name} – Choose ability (Tier allowed: {maxTier})",
        "Currently owned abilities:"
    };

            if (Actions.Count == 0)
                menuHeader.Add("  None");
            else
            {
                foreach (var action in Actions)
                {
                    if (action is Attack atk)
                        menuHeader.Add($"  {atk.Name} (Tier {atk.Tier}) - {atk.GetInfo(this)}");
                    else if (action is Buff buf)
                        menuHeader.Add($"  {buf.Name} (Tier {buf.Tier}) - {buf.GetInfo(this)}");
                }
            }

            menuHeader.Add("");
            menuHeader.Add("Choose new ability:");

            List<string> menuItems = new();
            foreach (var abilityName in validAbilityNames)
            {
                var instance = factory.Create(abilityName)!;

                if (instance is Attack atk)
                    menuItems.Add($"{atk.Name} (Tier {atk.Tier}) - {atk.GetInfo(this)}");
                else if (instance is Buff buf)
                    menuItems.Add($"{buf.Name} (Tier {buf.Tier}) - {buf.GetInfo(this)}");
                else
                    menuItems.Add($"{abilityName}");
            }

            menuItems.Add("No Ability");

            int choice = UI.NiceMenu(menuHeader.ToArray(), menuItems);

            if (choice >= 0 && choice < validAbilityNames.Count)
            {
                string chosen = validAbilityNames[choice];
                var newAbility = factory.Create(chosen)!;

                Actions.Add(newAbility);
                Console.WriteLine($"Added ability: {chosen}");
            }
            else
            {
                Console.WriteLine("No new ability added.");
            }
        }

        public override void LevelUp()
        {
            List<string> menuItems = new()
            {
                $"Strength   ({Strength})",
                $"Vitality   ({Vitality})",
                $"Intelligence ({Intelligence})",
                $"Dexterity   ({Dexterity})",
                "Back"
            };

            string[] header =
            {
                "================= LEVEL UP =================",
                "Choose an attribute to increase by +1:"
            };

            int levelupSelected = UI.NiceMenu(header, menuItems);

            switch (levelupSelected)
            {
                case 0:
                    Strength += 1;
                    Console.WriteLine($"{Name}'s Strength increased to {Strength}!");
                    break;

                case 1:
                    Vitality += 1;
                    Health += 10;
                    Console.WriteLine($"{Name}'s Vitality increased to {Vitality}!");
                    break;

                case 2:
                    Intelligence += 1;
                    Console.WriteLine($"{Name}'s Intelligence increased to {Intelligence}!");
                    break;

                case 3:
                    Dexterity += 1;
                    Console.WriteLine($"{Name}'s Dexterity increased to {Dexterity}!");
                    break;

                default:
                    Console.WriteLine("Level-up cancelled.");
                    break;
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }


    }

}
