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
            List<Type> allAbilities = new List<Type>()
            {
                typeof(Ram),
                typeof(FireBall),
                typeof(HealBuff),
                typeof(BerserkStrike),
                typeof(RecoilShot),
                typeof(GambleBolt),
                typeof(BloodOffering),
                typeof(ArmorCrush),
                typeof(WeakenEnemy) // HealBuff removed
            };

            HashSet<Type> ownedTypes = new HashSet<Type>(Actions.Select(a => a.GetType()));
            List<Type> newAbilities = allAbilities.Where(a => !ownedTypes.Contains(a)).ToList();

            // Prepare display strings for menu
            List<string> menuPrints = new List<string>();

            // Show currently owned abilities
            menuPrints.Add(Name + " Turn!");
            menuPrints.Add("Currently owned abilities:");
            if (Actions.Count == 0)
                menuPrints.Add("  None");
            else
            {
                foreach (var action in Actions)
                {
                    if (action is Attack atk)
                        menuPrints.Add($"  {atk.Name} - {atk.GetInfo(this)}");
                    else if (action is Buff buf)
                    {
                        menuPrints.Add($"  {buf.Name} - {buf.GetInfo(this)}");
                    }
                    else{menuPrints.Add($"  {action.GetType().GetProperty("Name")?.GetValue(action) ?? action.GetType().Name}");}
                }
            }

            menuPrints.Add(""); // Spacer
            menuPrints.Add("Choose new abilities from below:");

            // Show new abilities with info
            List<string> menuItems = new List<string>();
            foreach (Type ability in newAbilities)
            {
                IAction instance = (IAction)Activator.CreateInstance(ability)!;
                if (instance is Attack atk)
                        menuItems.Add($"  {atk.Name} - {atk.GetInfo(this)}");
                    else if (instance is Buff buf)
                    {
                        menuItems.Add($"  {buf.Name} - {buf.GetInfo(this)}");
                    }
                    else{menuItems.Add($"  {instance.GetType().GetProperty("Name")?.GetValue(instance) ?? instance.GetType().Name}");}
            }

            menuItems.Add("No Attack");

            int choice = UI.NiceMenu(menuPrints.ToArray(), menuItems);

            // Determine which ability was chosen
            if (choice >= 0 && choice < newAbilities.Count)
            {
                IAction chosenAbility = (IAction)Activator.CreateInstance(newAbilities[choice])!;
                Actions.Add(chosenAbility);
                Console.WriteLine($"Added ability: {menuItems[choice]}");
            }
            else
            {
                // "No Attack" selected, do nothing
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
