namespace MonsterBattler;

public class Enemy : Character
{
    private static readonly Random rng = new();

    public Enemy(string name)
        : base(name, strength: 1, vitality: 1, intelligence: 1, dexterity: 1)
    {
    }
    // Overloading av konstruktorer
    public Enemy(
    string name,
    int strength,
    int vitality,
    int intelligence,
    int dexterity)
    : base(name , strength, vitality, intelligence, dexterity)
    {
    }

    public void LevelUp(
        int randomLevels = 1,
        int str = 0,
        int vit = 0,
        int intel = 0,
        int dex = 0)
    {
        if (randomLevels > 0)
        {
            ApplyRandomLevelUps(randomLevels);
        }
        else
        {
            ApplySpecificLevelUps(str, vit, intel, dex);
        }

        Health = MaxHealth;
    }

    private void ApplySpecificLevelUps(int str, int vit, int intel, int dex)
    {
        Strength += str;
        Vitality += vit;
        Intelligence += intel;
        Dexterity += dex;
    }

    private void ApplyRandomLevelUps(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int roll = rng.Next(5); 

            switch (roll)
            {
                case 0: Strength++; break;
                case 1: Vitality++; break;
                case 2: Intelligence++; break;
                case 3: Dexterity++; break;
            }
        }
    }

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

           

                case 5:
                    levelingUp = false;
                    break;
            }
        }
        Health = MaxHealth;
    }

    public override void TakeTurn(Character target)
    {
        if (Actions.Count == 0)
        {
            Console.WriteLine($"{Name} has no actions to perform!");
            return;
        }

        int choice = rng.Next(Actions.Count); 
        IAction selectedAction = Actions[choice];

        Console.WriteLine($"{Name} uses {selectedAction.GetType().Name}!");

        selectedAction.Execute(this, target);
    }
}
