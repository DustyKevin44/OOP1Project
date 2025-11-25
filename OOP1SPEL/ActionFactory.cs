
namespace MonsterBattler{

public class ActionFactory
{
    // ============ Inkapsling ======================= 
    // Jag har denna dictionary som private readonly då 
    private readonly Dictionary<string, Func<IAction>> _registry;

    public ActionFactory()
    {
        // Register abilities here.
        // Key = ability name used in menus
        // Value = function that returns a new instance
        _registry = new Dictionary<string, Func<IAction>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Ram", () => new Ram() },
            { "FireBall", () => new FireBall() },
            { "BerserkStrike", () => new BerserkStrike() },
            { "RecoilShot", () => new RecoilShot() },
            { "GambleBolt", () => new GambleBolt() },
            { "BloodOffering", () => new BloodOffering() },
            { "ArmorCrush", () => new ArmorCrush() },
            { "HealBuff", () => new HealBuff() },
            { "WeakenEnemy", () => new WeakenEnemy() }
        };
    }

    public IAction? Create(string actionName)
    {
        if (_registry.TryGetValue(actionName, out var constructor))
            return constructor();

        return null; // Unknown ability
    }

  public IEnumerable<string> GetAllActionNames() => _registry.Keys;
}}