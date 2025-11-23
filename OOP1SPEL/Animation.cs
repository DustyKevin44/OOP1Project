using System;
using System.Threading;

namespace MonsterBattler
{
    public class Animation
    {
        private static void DrawFrame(string top, string mid1, string mid2, string mid3, string mid4, string mid5, string bottom)
        {
            Console.Clear();
            Console.WriteLine("__________________________________________________________________");
            Console.WriteLine(top);
            Console.WriteLine(mid1);
            Console.WriteLine(mid2);
            Console.WriteLine(mid3);
            Console.WriteLine(mid4);
            Console.WriteLine(mid5);
            Console.WriteLine(bottom);
            Console.WriteLine("__________________________________________________________________");
        }
        public static void ShootAnimation(Character sender, Character receiver, string attackSymbol)
        {
            string r = $"{receiver.Name} ({receiver.Health})";
            string s = $"{sender.Name} ({sender.Health})";

            for (int i = 0; i < 7; i++) // same number of steps as Fireball
            {
                DrawFrame(
                    $"                                                    {r}",           // Receiver on top
                    (i == 5 ? $"                                              {attackSymbol}" : " "),
                    (i == 4 ? $"                                        {attackSymbol}" : " "),
                    (i == 3 ? $"                                  {attackSymbol}" : " "),
                    (i == 2 ? $"                            {attackSymbol}" : " "),
                    (i == 1 ? $"                      {attackSymbol}" : " "),
                    (i == 0 ? $" {s}  " : $" {s}  ")                           // Sender on bottom
                );
                Thread.Sleep(150);
            }
        }

        // Receiving variant for visual effect only, keeping sender intact
        public static void ReceiveShootAnimation(Character sender, Character receiver, string attackSymbol)
        {
            string r = $"{receiver.Name} ({receiver.Health})";
            string s = $"{sender.Name} ({sender.Health})";

            for (int i = 0; i < 6; i++)
            {
                DrawFrame(
                    $"                                                  {r}",
                    (i == 4 ? $"                                        {attackSymbol}" : " "),
                    (i == 3 ? $"                                    {attackSymbol}" : " "),
                    (i == 2 ? $"                                {attackSymbol}" : " "),
                    (i == 1 ? $"                        {attackSymbol}" : " "),
                    $" {s}",
                    ""
                );
                Thread.Sleep(150);
            }
        }

        public static void Ram(Character sender, Character receiver)
        {
            for (int i = 0; i < 8; i++)
            {
                string s = sender.Name;
                string r = receiver.Name + " (" + receiver.Health + ")";
                DrawFrame(
                    $"                                                        {r}",
                    (i == 6 ? $"                                                      {s}" : " "),
                    (i == 5 ? $"                                                 {s}" : " "),
                    (i == 3 ? $"                                            {s}" : " "),
                    (i == 1 ? $"                                       {s}" : " "),
                    (i == 0 ? $"                                {s}" : " "),
                    ""
                );
                Thread.Sleep(120);
            }
        }

        // Receiving Ram (sender stays in place visually)
        public static void ReceiveRam(Character sender, Character receiver)
        {
            string s = sender.Name;
            string r = receiver.Name + " (" + receiver.Health + ")";
            for (int i = 0; i < 8; i++)
            {
                DrawFrame(
                    $"                                                        ",
                    (i == 6 ? $"                                      {r}" : " "),
                    (i == 5 ? $"                                 {r}" : " "),
                    (i == 3 ? $"                           {r}" : " "),
                    (i == 1 ? $"                    {r}" : " "),
                    $" {s}",
                    ""
                );
                Thread.Sleep(120);
            }
        }


        public static void Heal(Character sender)
        {
            string s = sender.Name + " (" + sender.Health + ")";
            for (int i = 0; i < 6; i++)
            {
                DrawFrame(
                    "",
                    (i % 2 == 0 ? "                              ✨✨✨✨✨" : "                                 ✨✨✨"),
                    (i % 2 == 1 ? "                              ✨✨✨✨✨" : "                                 ✨✨✨"),
                    $"                                  {s}",
                    "",
                    "",
                    ""
                );
                Thread.Sleep(200);
            }
        }

        public static void Weaken(Character sender, Character receiver)
        {
            string r = receiver.Name + " (" + receiver.Strength + " STR)";
            for (int i = 0; i < 6; i++)
            {
                DrawFrame(
                    $"                                                 {r}",
                    (i % 2 == 0 ? "                            👊 → -2 STR" : "                              👊 → -2 STR"),
                    "",
                    "",
                    "",
                    $" {sender.Name} (casts weaken)",
                    ""
                );
                Thread.Sleep(200);
            }
        }

        public static void BerserkStrike(Character sender, Character receiver)
        {
            for (int i = 0; i < 6; i++)
            {
                DrawFrame(
                    $"                                                    {receiver.Name} ({receiver.Health})",
                    (i == 4 ? "                                             💢" : " "),
                    (i == 3 ? "                                     💢💢" : " "),
                    (i == 2 ? "                             💢💢💢" : " "),
                    (i == 1 ? "                     💢💢💢💢" : " "),
                    (i == 0 ? $" {sender.Name} (BERSERK!)" : " "),
                    ""
                );
                Thread.Sleep(140);
            }
        }



        public static void BloodOffering(Character sender, Character receiver)
        {
            for (int i = 0; i < 6; i++)
            {
                DrawFrame(
                    "",
                    (i % 2 == 0 ? "                                 ❤️" : "                                  💔"),
                    $"                        {sender.Name} (sacrificing)",
                    "",
                    "",
                    $"                                  → {receiver.Name}",
                    ""
                );
                Thread.Sleep(180);
            }
        }

        public static void ArmorCrush(Character sender, Character receiver)
        {
            for (int i = 0; i < 5; i++)
            {
                DrawFrame(
                    $"                                                  {receiver.Name} ({receiver.Armor} ARM)",
                    (i == 3 ? "                                    🛡️" : " "),
                    (i == 2 ? "                                💥🛡️💥" : " "),
                    (i == 1 ? "                                    🛡️" : " "),
                    $" {sender.Name} (Crushing Armor)",
                    "",
                    ""
                );
                Thread.Sleep(170);
            }
        }
       public static void ShowDamage(Character target, int damage, string symbol = "💥")
{
    string original = $"{target.Name} ({target.Health})";
    int newHealth = target.Health - damage;

    // Frames: original, damage shown, health updated
    string[] frames = new string[]
    {
        $"                        {original}                        ",
        $"                        {original} - {damage}                        ",
        $"                        {target.Name} ({newHealth})                        "
    };

    foreach (var frame in frames)
    {
        DrawFrame(
            frame,
            "", "", "", "", "", ""
        );
        Thread.Sleep(300);
    }

    // Apply the damage after animation
  
}

    }
}
