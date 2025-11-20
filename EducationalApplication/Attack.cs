using System;
using System.Configuration.Assemblies;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;

namespace test
{
    public enum DamageTypes { Physical, Magical, Buff, None }

    public interface IAction { 
        void Execute(Character se, Character? re = null);
        void PlayAnimation(Character se, Character re);
        }
    public interface IAttack
    {
        void ApplyDamage(Character se, Character re);
        string GetInfo(Character sender);
        (int min, int max) GetDamageBounds(Character sender);
    }
    public interface IBuff { 
        void ApplyBuff(Character se, Character? re = null); 
        string GetInfo(Character sender);
        }

    // ==================== BUFFS ====================
 public abstract class Buff : IAction, IBuff
    {
        public string Name { get; protected set; }
        public string Desc { get; protected set; }
        public int Tier { get; protected set; }

        protected Buff(string name, string desc, int tier)
        {
            Name = name;
            Desc = desc;
            Tier = tier;
        }

        // Default behaviour — can be overridden
        public abstract void ApplyBuff(Character sender, Character? receiver = null);

        public virtual void Execute(Character sender, Character? receiver = null)
            => ApplyBuff(sender, receiver);

        public virtual void PlayAnimation(Character sender, Character receiver)
        {
            // Optional override
        }

        public virtual string GetInfo(Character sender)
            => $"[Tier {Tier}] {Name}: {Desc}";
    }

   
    public class HealBuff : Buff
    {
        public HealBuff() : base("Healing Light", "Restores HP by INT.", 1) { }

        public override void ApplyBuff(Character sender, Character? receiver = null)
        {
            sender.Health += sender.Intelligence;
            Console.WriteLine($"{sender.Name} is healed for {sender.Intelligence} HP!");
            Animation.Heal(sender);
        }
    }

    public class WeakenEnemy : Buff
    {
        public WeakenEnemy() : base("Weaken", "Reduces enemy Strength by 2.", 1) { }

        public override void ApplyBuff(Character sender, Character? receiver)
        {
            if (receiver == null) return;

            receiver.Strength = Math.Max(0, receiver.Strength - 2);
            Console.WriteLine($"{receiver.Name}'s Strength is reduced by 2!");
            Animation.Weaken(sender, receiver);
        }
    }

    // ==================== BASE ATTACK ====================

    public class Attack : IAttack, IAction
    {
        private static readonly Random rng = new Random();

        public string Name { get; protected set; } = "No Name";
        public int Damage { get; protected set; } = 0;
        public string Desc { get; protected set; } = "No description";
        public int Tier { get; protected set; } = 1;   // ★ ADDED TIER
        public DamageTypes DType = DamageTypes.None;

        public virtual void PlayAnimation(Character sender, Character receiver) { }

        // --- Damage application ---
        public void ApplyDamage(Character se, Character re)
        {
            int total = CalculateDamageAmount(se, re);

            int armorAbsorb = Math.Min(total, re.Armor);
            re.Armor -= armorAbsorb;

            int remaining = total - armorAbsorb;
            if (remaining > 0)
                re.Health = Math.Max(0, re.Health - remaining);

            Console.WriteLine($"{se.Name} deals {total} damage!");
            if (re.Health == 0) {Console.WriteLine($"{re.Name} is DEAD!"); se.LevelUp();}
        }

        // --- Universal formula WITH tier scaling ---
        public virtual int CalculateDamageAmount(Character sender, Character receiver)
        {
            float stat = DType == DamageTypes.Magical ? sender.Intelligence : sender.Strength;

            // ★ Tier scaling multiplier
            float tierMult = Tier switch
            {
                1 => 1.00f,
                2 => 1.30f,
                3 => 1.60f,
                _ => 1f
            };

            float baseDamage = Damage * stat * tierMult;

            // Magical = 300% variance, Physical = 200%
            float maxVar = (DType == DamageTypes.Magical) ? 3f : 2f;

            float low = 0f;
            float high = baseDamage * maxVar;

            float dex = Math.Max(1, sender.Dexterity);
            float bias = 100f / (100f + dex); // Higher DEX = weighted toward high end

            float r = (float)rng.NextDouble();
            float t = (float)Math.Pow(r, bias);

            return Math.Max(0, (int)Math.Round(low + (high - low) * t));
        }

        public virtual void Execute(Character sender, Character? receiver)
        {
            if (receiver == null) return;
            PlayAnimation(sender, receiver);
            ApplyDamage(sender, receiver);
        }

        public virtual (int min, int max) GetDamageBounds(Character sender)
        {
            int a = CalculateDamageAmount(sender, null!);
            int b = CalculateDamageAmount(sender, null!);
            return (Math.Min(a, b), Math.Max(a, b));
        }

        public virtual string GetInfo(Character sender)
        {
            var bounds = GetDamageBounds(sender);
            return $"[Tier {Tier}] Damage: {bounds.min}-{bounds.max} | {Desc}";
        }
    }

    // ==================== ATTACKS WITH TIERS ====================

    public class Ram : Attack
    {
        public Ram()
        {
            Name = "Ram";
            Damage = 2;
            DType = DamageTypes.Physical;
            Tier = 1;     // ★ TIER 1
            Desc = "Charge the enemy with brute force.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
            => Animation.Ram(sender, receiver);
    }

    public class FireBall : Attack
    {
        public FireBall()
        {
            Name = "FireBall";
            Damage = 1;
            DType = DamageTypes.Magical;
            Tier = 1;    // ★ TIER 1
            Desc = "Hurl a fireball that burns the enemy.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
            => Animation.Fireball(sender, receiver);
    }

    public class BerserkStrike : Attack
    {
        public BerserkStrike()
        {
            Name = "Berserk Strike";
            Damage = 2;
            DType = DamageTypes.Physical;
            Tier = 2;   // ★ TIER 2
            Desc = "Wild and unpredictable strike.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
            => Animation.BerserkStrike(sender, receiver);
    }

    public class RecoilShot : Attack
    {
        public RecoilShot()
        {
            Name = "Recoil Shot";
            Damage = 2;
            DType = DamageTypes.Physical;
            Tier = 2;  // ★ TIER 2
            Desc = "Powerful shot that harms yourself slightly.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
            => Animation.RecoilShot(sender, receiver);

        public override void Execute(Character sender, Character receiver)
        {
            PlayAnimation(sender, receiver);

            int dmg = CalculateDamageAmount(sender, receiver);
            ApplyDamage(sender, receiver);

            int recoil = (int)Math.Ceiling(dmg * 0.2f);
            sender.Health -= recoil;

            Console.WriteLine($"{sender.Name} takes {recoil} recoil damage!");
        }
    }

    public class GambleBolt : Attack
    {
        private static readonly Random rng = new Random();

        public GambleBolt()
        {
            Name = "Gamble Bolt";
            Damage = 1;
            DType = DamageTypes.Magical;
            Tier = 3;    // ★ TIER 3
            Desc = "50% chance to do triple damage.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
            => Animation.GambleBolt(sender, receiver);

        public override int CalculateDamageAmount(Character sender, Character receiver)
        {
            float stat = sender.Intelligence;
            int baseDamage = (int)(Damage * stat * 1.6f); // Tier 3 boost

            return rng.Next(2) == 0 ? baseDamage * 3 : 0;
        }
    }

    public class BloodOffering : Attack
    {
        public BloodOffering()
        {
            Name = "Blood Offering";
            Damage = 3;
            DType = DamageTypes.Magical;
            Tier = 3;  // ★ TIER 3
            Desc = "Sacrifice 20% of max HP to empower the attack.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
            => Animation.BloodOffering(sender, receiver);

        public override void Execute(Character sender, Character receiver)
        {
            sender.Health -= sender.Vitality / 5;
            Console.WriteLine($"{sender.Name} sacrifices 20% of health!");

            PlayAnimation(sender, receiver);
            ApplyDamage(sender, receiver);
        }
    }

    public class ArmorCrush : Attack
    {
        public ArmorCrush()
        {
            Name = "Armor Crush";
            Damage = 1;
            DType = DamageTypes.Physical;
            Tier = 3;    // ★ TIER 3
            Desc = "Shatters all enemy armor before hitting.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
            => Animation.ArmorCrush(sender, receiver);

        public override void Execute(Character sender, Character receiver)
        {
            receiver.Armor = 0;
            Console.WriteLine($"{receiver.Name}'s armor is shattered!");
            PlayAnimation(sender, receiver);
            ApplyDamage(sender, receiver);
        }
    }
}
