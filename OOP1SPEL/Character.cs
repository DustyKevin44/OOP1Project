namespace MonsterBattler{
    
public class Character
{
    
    public string Name { get; private set; }
    public int Strength { get; set; }
    public int Vitality { get; set; }
    public int Dexterity { get; set; }
    public int Intelligence { get; set; }
    
    // ============== Computed properties ==================
    private int _health;
    public int Health
    {
        get => _health;
        set
        {
            _health = Math.Max(0, value);
            if (_health == 0) OnDeath();
        }
    }

    public int MaxHealth => Vitality * 10;

    // Objektkomposition
    public List<IAction> Actions { get; } = new();
    protected Random rand = new();

    private void OnDeath(){
        CombatManager.OnCharacterDeath(this);
    }
    public Character(string name,  int strength,
        int vitality, int intelligence, int dexterity)
    {
        Name = name;
     
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
            Console.WriteLine($"{Name}: Health: {Health} MaxHealth: {MaxHealth} Strength: {Strength} Intelligence: {Intelligence} Dexterity: {Dexterity}");
    }
    public virtual void TakeTurn(Character target){}
    public virtual void LevelUp(){}
}}

