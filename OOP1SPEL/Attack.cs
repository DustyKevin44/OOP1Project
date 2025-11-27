using System;

namespace MonsterBattler
{
    public enum DamageTypes { Physical, Magical, Buff, None }

    public interface IAction
    {
        void Execute(Character se, Character? re = null);
        void PlayAnimation(Character se, Character re);
    }
    public interface IAttack
    {
        void ApplyDamage(Character se, Character re, int? total = null);
        string GetInfo(Character sender);
        (int min, int max) GetDamageBounds(Character sender);
    }
    public interface IBuff
    {
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

        public abstract void ApplyBuff(Character sender, Character? receiver = null);

        public virtual void Execute(Character sender, Character? receiver = null)
            => ApplyBuff(sender, receiver);

        public virtual void PlayAnimation(Character sender, Character receiver) { }

        public virtual string GetInfo(Character sender)
            => $"[Tier {Tier}] {Name}: {Desc}";
    }

    public class HealBuff : Buff
    {
        public HealBuff() : base("Healing Light", "Restores HP by INT * 2.", 1) { }

        public override void ApplyBuff(Character sender, Character? receiver = null)
        {
            sender.Health = Math.Min(sender.Health + sender.Intelligence * 2, sender.MaxHealth);
            Console.WriteLine($"{sender.Name} is healed for {sender.Intelligence * 2} HP!");
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
        public int Tier { get; protected set; } = 1;
        public DamageTypes DType = DamageTypes.None;

        public virtual void PlayAnimation(Character sender, Character receiver) { }

        public void ApplyDamage(Character se, Character re, int? total = null)
        {
            int dmg = total ?? CalculateDamageAmount(se, re);
      
            int remaining = dmg;
            if (remaining > 0)
                re.Health = Math.Max(0, re.Health - remaining);

            Console.WriteLine($"{se.Name} deals {dmg} damage!");
         
        }

        public virtual int CalculateDamageAmount(Character sender, Character receiver)
        {
            float stat = DType == DamageTypes.Magical ? sender.Intelligence : sender.Strength;
            float tierMult = Tier switch { 1 => 1.0f, 2 => 1.3f, 3 => 1.6f, _ => 1f };
            float baseDamage = Damage * stat * tierMult;
            float maxVar = (DType == DamageTypes.Magical) ? 3f : 2f;
            float low = Math.Max(1, stat * tierMult - 1);
            float high = baseDamage * maxVar;
            float dex = Math.Max(1, sender.Dexterity);
            float bias = 100f / (100f + dex);
            float r = (float)rng.NextDouble();
            float t = (float)Math.Pow(r, bias);
            return Math.Max(0, (int)Math.Round(low + (high - low) * t));
        }

        // ==================== CENTRALIZED EXECUTE ====================
        public virtual void Execute(Character sender, Character? receiver)
        {
            if (receiver == null) return;

            // ✅ Show action used animation for all attacks
            Animation.ActionUsedAnimation(sender, Name);

            // ✅ Play attack animation
            PlayAnimation(sender, receiver);

            // ✅ Calculate and show damage
            int dmg = CalculateDamageAmount(sender, receiver!);
            Animation.ShowDamage(receiver, dmg, DType == DamageTypes.Magical ? "✨" : "💥");

            // ✅ Apply damage
            ApplyDamage(sender, receiver, dmg);
        }

        public virtual (int min, int max) GetDamageBounds(Character sender)
        {
            float stat = (DType == DamageTypes.Magical) ? sender.Intelligence : sender.Strength;
            float tierMult = Tier switch { 1 => 1.0f, 2 => 1.3f, 3 => 1.6f, _ => 1f };
            float baseDamage = Damage * stat * tierMult;
            float maxVariance = (DType == DamageTypes.Magical) ? 3f : 2f;
            int minDamage = Math.Max(1, (int)Math.Round(stat * tierMult) - 1);
            int maxDamage = (int)Math.Round(baseDamage * maxVariance);
            return (minDamage, maxDamage);
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
            Name = "Ram"; Damage = 2; DType = DamageTypes.Physical; Tier = 1;
            Desc = "Charge the enemy with brute force.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
{
            if (sender is Player)
                Animation.Ram(sender, receiver);
            else
                Animation.ReceiveRam(sender, receiver);
        }    }

    public class FireBall : Attack
    {
        public FireBall()
        {
            Name = "FireBall"; Damage = 1; DType = DamageTypes.Magical; Tier = 1;
            Desc = "Hurl a fireball that burns the enemy.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
        {
            if (sender is Player)
                Animation.ShootAnimation(sender, receiver, "🔥");
            else
                Animation.ReceiveShootAnimation(sender, receiver, "🔥");
        }
    }

    public class BerserkStrike : Attack
    {
        public BerserkStrike()
        {
            Name = "Berserk Strike"; Damage = 2; DType = DamageTypes.Physical; Tier = 2;
            Desc = "Wild and unpredictable strike.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
            => Animation.BerserkStrike(sender, receiver);
    }

    public class RecoilShot : Attack
    {
        public RecoilShot()
        {
            Name = "Recoil Shot"; Damage = 2; DType = DamageTypes.Physical; Tier = 2;
            Desc = "Powerful shot that harms yourself slightly.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
        {
            if (sender is Player)
                Animation.ShootAnimation(sender, receiver, "➜");
            else
                Animation.ReceiveShootAnimation(sender, receiver, "➜");
        }

        public override void Execute(Character sender, Character? receiver)
        {
            // ✅ Insert extra behavior BEFORE base
            int dmg = CalculateDamageAmount(sender, receiver!);

            // Call centralized base Execute
            base.Execute(sender, receiver);

            // ✅ Apply recoil after base damage
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
            Name = "Gamble Bolt"; Damage = 1; DType = DamageTypes.Magical; Tier = 3;
            Desc = "50% chance to do triple damage.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
        {
            if (sender is Player)
                Animation.ShootAnimation(sender, receiver, "⚡");
            else
                Animation.ReceiveShootAnimation(sender, receiver, "⚡");
        }

        public override int CalculateDamageAmount(Character sender, Character receiver)
        {
            float stat = sender.Intelligence;
            int baseDamage = (int)(Damage * stat * 1.6f);
            return rng.Next(2) == 0 ? baseDamage * 3 : 0;
        }

        public override void Execute(Character sender, Character? receiver)
        {
            // Use centralized base Execute to handle animations and damage
            base.Execute(sender, receiver);
        }
    }

    public class BloodOffering : Attack
    {
        public BloodOffering()
        {
            Name = "Blood Offering"; Damage = 3; DType = DamageTypes.Magical; Tier = 3;
            Desc = "Sacrifice 20% of max HP to empower the attack.";
        }

        public override void PlayAnimation(Character sender, Character receiver)
            => Animation.BloodOffering(sender, receiver);

        public override void Execute(Character sender, Character? receiver)
        {
            // Extra step before base
            sender.Health -= (sender.Vitality * 10) / 5;
            Console.WriteLine($"{sender.Name} sacrifices 20% of health!");

            // If the sender died from the sacrifice, abort and let death handling run immediately
            if (!sender.IsAlive())
            {
                Console.WriteLine($"{sender.Name} died from the sacrifice!");
                return;
            }

            // Call centralized base Execute
            base.Execute(sender, receiver);
        }
    }


}
