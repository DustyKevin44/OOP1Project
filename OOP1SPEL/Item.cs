using System;
using System.Linq;

namespace MonsterBattler
{
    public abstract class Item : IAction
    {
        public string Name { get; protected set; } = "No name";
        public string Desc { get; protected set; } = "No description";
 
        public void Execute(Character se, Character? re = null)
        {
            Use(se, re);

            if (se.Actions != null)
            {
                var toRemove = se.Actions.FirstOrDefault(a => Object.ReferenceEquals(a, this) || a.GetType() == this.GetType());
                if (toRemove != null)
                {
                    se.Actions.Remove(toRemove);
                    Console.WriteLine($"{se.Name} used {Name} (consumed).");
                }
            }
        }

        protected abstract void Use(Character se, Character? re = null);

        public virtual void PlayAnimation(Character se, Character re) { }

        public virtual string GetInfo(Character sender)
        {
            return $"{Desc}";
        }
    
    }

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
        
            Animation.Heal(se);
        }
    }
    public class IntelligenceCrystal : Item
    {
        private readonly int _intelligenceGain = 1;
        public IntelligenceCrystal()
        {
            Name = "Intelligence Crystal";
            Desc = "Grants 1 intelligence permanently.";
        }

        protected override void Use(Character se, Character? re = null)
        {
            se.Strength += _intelligenceGain;
            Console.WriteLine($"{se.Name} consumes an Intelligence Crystal and gains {_intelligenceGain} Intelligence!");
            
        }
    }

   
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
