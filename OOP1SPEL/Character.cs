namespace MonsterBattler{
public class Character
{
    public string Name { get; private set; }
    public int Strength { get; set; }
    public int Vitality { get; set; }
    public int Dexterity { get; set; }
    public int Intelligence { get; set; }
    public int Armor { get; set; }
    public int Health { get; set; }
    public int MaxHealth => Vitality * 10;

    public List<IAction> Actions { get; } = new();
    protected Random rand = new();

    public Character(string name, int armor, int strength,
        int vitality, int intelligence, int dexterity)
    {
        Name = name;
        Armor = armor;
        Strength = strength;
        Vitality = vitality;
        Intelligence = intelligence;
        Dexterity = dexterity;
        Health = MaxHealth;
    }

    public bool IsAlive() => Health > 0;

    public virtual void GiveUp()
    {
        Health = 0;
    }

    public virtual void Revive()
    {
        if (Health <= 0)
            Health = MaxHealth / 2;
    }
     public void Print() { 
            Console.WriteLine($"{Name}: Health: {Health} MaxHealth: {MaxHealth} Armor: {Armor} Strength: {Strength} Intelligence: {Intelligence} Dexterity: {Dexterity}");
    }
    public virtual void TakeTurn(Character target){}
    public virtual void LevelUp(){}
}}

