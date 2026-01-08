namespace MonsterBattler{
    
public class Character
{
    
    public string Name { get; private set; }
    public int Strength { get; set; }
    public int Vitality { get; set; }
    public int Dexterity { get; set; }
    public int Intelligence { get; set; }
    
    // KRAV 3:
    // 1: Computed properties
    // 2: `MaxHealth` beräknas utifrån `Vitality` och `Health` hanteras via en property med validering.
    // 3: För att garantera korrekta invariants och automatiskt uppdatera beroende värden.
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

    // KRAV 4:
    // 1: Objektkomposition
    // 2: `Character` innehåller en lista `Actions` för att komponera beteenden från `IAction`-objekt.
    // 3: För att bygga komplexa objekt genom att kombinera enklare komponenter och främja återanvändning.
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

