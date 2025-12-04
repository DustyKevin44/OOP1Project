
namespace MonsterBattler{

public class ActionFactory
{
    // ============ Inkapsling ======================= 
    private readonly Dictionary<string, Func<IAction>> _registry;
    
    public ActionFactory()
    {
        _registry = new Dictionary<string, Func<IAction>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Ram", () => new Ram() },
            { "FireBall", () => new FireBall() },
            { "BerserkStrike", () => new BerserkStrike() },
            { "RecoilShot", () => new RecoilShot() },
            { "GambleBolt", () => new GambleBolt() },
            { "BloodOffering", () => new BloodOffering() },
            { "HealBuff", () => new HealBuff() },
            { "WeakenEnemy", () => new WeakenEnemy() },
            { "HealingPotion", () => new HealingPotion() },
            
            { "WebSnare", () => new WebSnare() },
            { "PoisonGas", () => new PoisonGas() },
            { "Summining", () => new Summoning() }

        };
    }

    public IAction? Create(string actionName)
    {
        if (_registry.TryGetValue(actionName, out var constructor))
            return constructor();

        return null; 
    }

  public IEnumerable<string> GetAllActionNames() => _registry.Keys;
}}