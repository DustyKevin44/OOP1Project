namespace MonsterBattler;

public class Enemy : Character
{
    private static readonly Random rng = new();

    // Constructor: everything defaults to 1
    public Enemy(string name)
        : base(name, armor: 0, strength: 1, vitality: 1, intelligence: 1, dexterity: 1)
    {
    }
    public Enemy(
    string name,
    int armor,
    int strength,
    int vitality,
    int intelligence,
    int dexterity)
    : base(name, armor, strength, vitality, intelligence, dexterity)
    {
    }

    /// <summary>
    /// Level up enemy with either random upgrades or specified ones.
    /// </summary>
    /// <param name="randomLevels">How many random attribute increases.
    /// If > 0, random mode is used and the specific attributes are ignored.</param>
    /// <param name="str">Strength increase</param>
    /// <param name="vit">Vitality increase</param>
    /// <param name="intel">Intelligence increase</param>
    /// <param name="dex">Dexterity increase</param>
    /// <param name="armor">Armor increase</param>
    public void LevelUp(
        int randomLevels = 1,
        int str = 0,
        int vit = 0,
        int intel = 0,
        int dex = 0,
        int armor = 0)
    {
        if (randomLevels > 0)
        {
            ApplyRandomLevelUps(randomLevels);
        }
        else
        {
            ApplySpecificLevelUps(str, vit, intel, dex, armor);
        }

        // Ensure health scales properly when vitality changes
        Health = MaxHealth;
    }

    private void ApplySpecificLevelUps(int str, int vit, int intel, int dex, int armor)
    {
        Strength += str;
        Vitality += vit;
        Intelligence += intel;
        Dexterity += dex;
        Armor += armor;
    }

    private void ApplyRandomLevelUps(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int roll = rng.Next(5); // 0–4 → 5 attributes

            switch (roll)
            {
                case 0: Strength++; break;
                case 1: Vitality++; break;
                case 2: Intelligence++; break;
                case 3: Dexterity++; break;
                case 4: Armor++; break;
            }
        }
    }

    /// <summary>
    /// Interactive menu for customizing enemy attributes
    /// </summary>
    public void LevelUpCustom()
    {
        bool levelingUp = true;

        while (levelingUp)
        {
            List<string> menuItems = new()
            {
                $"Strength   ({Strength})",
                $"Vitality   ({Vitality})",
                $"Intelligence ({Intelligence})",
                $"Dexterity   ({Dexterity})",
                $"Armor      ({Armor})",
                "Done Leveling Up"
            };

            string[] header = new string[]
            {
                "════════════════════════════",
                $"    LEVEL UP {Name.ToUpper()}",
                "════════════════════════════",
                "Choose an attribute to increase by +1:",
                ""
            };

            int choice = UI.NiceMenu(header, menuItems);

            switch (choice)
            {
                case 0:
                    Strength += 1;
                    Console.WriteLine($"{Name}'s Strength increased to {Strength}!");
                    System.Threading.Thread.Sleep(600);
                    break;

                case 1:
                    Vitality += 1;
                    Health += 10;
                    Console.WriteLine($"{Name}'s Vitality increased to {Vitality}!");
                    System.Threading.Thread.Sleep(600);
                    break;

                case 2:
                    Intelligence += 1;
                    Console.WriteLine($"{Name}'s Intelligence increased to {Intelligence}!");
                    System.Threading.Thread.Sleep(600);
                    break;

                case 3:
                    Dexterity += 1;
                    Console.WriteLine($"{Name}'s Dexterity increased to {Dexterity}!");
                    System.Threading.Thread.Sleep(600);
                    break;

                case 4:
                    Armor += 1;
                    Console.WriteLine($"{Name}'s Armor increased to {Armor}!");
                    System.Threading.Thread.Sleep(600);
                    break;

                case 5:
                    levelingUp = false;
                    break;
            }
        }

        // Update health to match new vitality
        Health = MaxHealth;
    }

    public override void TakeTurn(Character target)
    {
        if (Actions.Count == 0)
        {
            Console.WriteLine($"{Name} has no actions to perform!");
            return;
        }

        int choice = rng.Next(Actions.Count); // pick random index
        IAction selectedAction = Actions[choice];

        Console.WriteLine($"{Name} uses {selectedAction.GetType().Name}!");

        // Execute the chosen action
        selectedAction.Execute(this, target);
    }
}
