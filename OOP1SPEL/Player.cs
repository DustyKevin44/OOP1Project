namespace MonsterBattler
{
    public class Player : Character
    {
        public Player(string name, int armor, int strength,
            int vitality, int intelligence, int dexterity)
            : base(name, armor, strength, vitality, intelligence, dexterity) { }

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

            List<string> menuItems = new() { "Actions", "Give Up" };

            string[] header = BuildHeader(target, "Choose your action:");
            int choice = UI.NiceMenu(header, menuItems);

            if (choice == 0)
                AttackMenu(target);
            else
                GiveUp();
        }

        public void AttackMenu(Character target)
        {
            List<string> menuItems = Actions
                .Select(a => a?.GetType().Name ?? "UNKNOWN")
                .ToList();

            menuItems.Add("Back");

            string[] header = BuildHeader(target, "Choose attack:");
            int choice = UI.NiceMenu(header, menuItems);

            if (choice < Actions.Count)
                Actions[choice].Execute(this, target);
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

        // `NiceMenu` moved to `UI.NiceMenu` for reuse across the app.
        public void NewAbility()
        {
            ActionFactory factory = new ActionFactory();

            // All ability names registered inside ActionFactory
            List<string> allAbilities = factory.GetAllActionNames().ToList();

            // Player already owns these types
            HashSet<string> ownedNames = Actions
                .Select(a => a.GetType().Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Get abilities the player does NOT already own
            List<string> newAbilityNames = allAbilities
                .Where(a => !ownedNames.Contains(a))
                .ToList();

            // Menu header
            List<string> menuHeader = new()
    {
        $"{Name} Turn!",
        "Currently owned abilities:"
    };

            if (Actions.Count == 0)
                menuHeader.Add("  None");
            else
            {
                foreach (var action in Actions)
                {
                    if (action is Attack atk)
                        menuHeader.Add($"  {atk.Name} - {atk.GetInfo(this)}");
                    else if (action is Buff buf)
                        menuHeader.Add($"  {buf.Name} - {buf.GetInfo(this)}");
                    else
                        menuHeader.Add($"  {action.GetType().Name}");
                }
            }

            menuHeader.Add("");
            menuHeader.Add("Choose new abilities from below:");

            // Build menu items from NEW abilities
            List<string> menuItems = new();
            foreach (var abilityName in newAbilityNames)
            {
                var instance = factory.Create(abilityName)!;

                if (instance is Attack atk)
                    menuItems.Add($"{atk.Name} - {atk.GetInfo(this)}");
                else if (instance is Buff buf)
                    menuItems.Add($"{buf.Name} - {buf.GetInfo(this)}");
                else
                    menuItems.Add(abilityName);
            }

            menuItems.Add("No Attack");

            int choice = UI.NiceMenu(menuHeader.ToArray(), menuItems);

            if (choice >= 0 && choice < newAbilityNames.Count)
            {
                string chosenName = newAbilityNames[choice];
                var newAbility = factory.Create(chosenName)!;

                Actions.Add(newAbility);
                Console.WriteLine($"Added ability: {chosenName}");
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
