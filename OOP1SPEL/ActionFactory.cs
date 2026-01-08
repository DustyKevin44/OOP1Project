
namespace MonsterBattler{

public class ActionFactory
{
    // KRAV 1:
    // 1: Inkapsling
    // 2: Privat `_registry` döljer implementationen; publika metoder exponerar endast skapande av `IAction`.
    // 3: För att skydda intern representation och begränsa åtkomst, vilket förenklar underhåll.
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