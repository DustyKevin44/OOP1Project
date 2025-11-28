using System;
using System.Linq;

namespace MonsterBattler
{
    // Base consumable item implementing IAction
    public abstract class Item : IAction
    {
        public string Name { get; protected set; } = "No name";
        public string Desc { get; protected set; } = "No description";
 
        // Execute should perform the item's effect and then remove itself from the user's Actions list
        public void Execute(Character se, Character? re = null)
        {
            // Perform the specific item effect
            Use(se, re);

            // After use, remove this item from the sender's action list (consumable)
            if (se.Actions != null)
            {
                // Find the first action instance that is the same runtime type as this item instance
                var toRemove = se.Actions.FirstOrDefault(a => Object.ReferenceEquals(a, this) || a.GetType() == this.GetType());
                if (toRemove != null)
                {
                    se.Actions.Remove(toRemove);
                    Console.WriteLine($"{se.Name} used {Name} (consumed).");
                }
            }
        }

        // Each item implements its own effect here
        protected abstract void Use(Character se, Character? re = null);

        // Default no-op animation; items can override if they want
        public virtual void PlayAnimation(Character se, Character re) { }

        public virtual string GetInfo(Character sender)
        {
            return $"{Desc}";
        }
    
    }

    // Healing Potion: restores health to the user
    public class HealingPotion : Item
    {
        private readonly int _healAmount = 5;
        public HealingPotion()
        {
            Name = "Healing Potion";
            Desc =  $"Restores {_healAmount} HP.";
        }

        protected override void Use(Character se, Character? re = null)
        {
            int healed = Math.Min(_healAmount, se.MaxHealth - se.Health);
            se.Health = Math.Min(se.MaxHealth, se.Health + _healAmount);
            Console.WriteLine($"{se.Name} drinks a Healing Potion and recovers {healed} HP!");
            Animation.Heal(se);
        }
    }

    // Strength Potion: increases sender's Strength (temporary persistence depends on design; we'll make it permanent for simplicity)
    public class StrengthPotion : Item
    {
        private readonly int _strengthGain = 1;
        public StrengthPotion()
        {
            Name = "Strength Potion";
            Desc = "Grants 1 strength permanently.";
        }

        protected override void Use(Character se, Character? re = null)
        {
            se.Strength += _strengthGain;
            Console.WriteLine($"{se.Name} drinks a Strength Potion and gains {_strengthGain} Strength!");
            // Optionally show a buff animation
            Animation.Heal(se);
        }
    }

    // Web Trap: reduces target's Dexterity
    public class WebTrap : Item
    {
        private readonly int _dexReduction = 1;
        public WebTrap()
        {
            Name = "Web trap";
            Desc = $"Reduces target's Dexterity by {_dexReduction}.";
        }

        protected override void Use(Character se, Character? re = null)
        {
            if (re == null)
            {
                Console.WriteLine("No target to use the Web Trap on.");
                return;
            }

            re.Dexterity = Math.Max(0, re.Dexterity - _dexReduction);
            Console.WriteLine($"{se.Name} uses a Web Trap on {re.Name}, reducing their Dexterity by {_dexReduction}!");
            Animation.Weaken(se, re);
        }
    }
}
