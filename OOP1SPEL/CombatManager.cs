using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace MonsterBattler
{
    public static class CombatManager
    {
    public static Character?[]? Participants { get; private set; }
        public static bool FightEnded { get; private set; } = false;
        public static Character? LastDead { get; private set; }

        public static void StartFight(Character?[] participants)
        {
            Participants = participants;
            FightEnded = false;
            LastDead = null;
        }

        public static void OnCharacterDeath(Character c)
        {
            if (FightEnded) return;
            FightEnded = true;
            LastDead = c;
        }

        public static bool PromptEndFight()
        {
            if (Participants == null || Participants.Length < 2)
                return false;


            Character? p = Participants[0];
            Character? o = Participants[1];

            Character? dead = LastDead;
            Character? alive = LastDead;

            if (dead == null)
            {
                dead = !p.IsAlive() ? p : (!o.IsAlive() ? o : null);
                alive = p.IsAlive() ? p : (o.IsAlive() ? o : null);

            }

            if (dead == null || alive == null)
            {
                return false;
            }

            if (dead is Player)
            {
                Animation.YouDied(dead);

                string[] header = new string[]
                {
                    "_____________________",
                    "",
                    $"    {dead.Name} has fallen!",
                    "",
                    "Choose:",
                    ""
                };

                List<string> menu = new List<string> { "Retry", "Quit" };
                int choice = UI.NiceMenu(header, menu);

                if (choice == 0)
                {
                    foreach (var c in Participants)
                    {
                        c.Health = c.MaxHealth;
                    }
                    FightEnded = false;
                    LastDead = null;
                    return true;
                }
                return false;
            }
            else
            {
                Animation.TargetDied(dead);
                string[] header = new string[]
                {
                    "_____________________",
                    "",
                    $"    {dead.Name} was defeated!",
                    "",
                    "Press Enter to continue",
                    ""
                };
                List<string> menu = new List<string> { "Continue" };
                alive.LevelUp();
                UI.NiceMenu(header, menu);
                FightEnded = false;
                LastDead = null;
                return false;
            }
        }
    }
}
