using System;
using System.Collections.Generic;

namespace MonsterBattler
{
    public static class UI
    {
        public static int NiceMenu<T>(string[] header, List<T> items)
        {
            ConsoleKey key;
            int selected = 0;
            string head = string.Join("\n", header);

            do
            {
                Console.Clear();
                Console.WriteLine(head);
                Console.WriteLine();

                for (int i = 0; i < items.Count; i++)
                {
                    Console.Write(i == selected ? "> " : "  ");
                    Console.WriteLine(items[i]);
                }

                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.DownArrow || key == ConsoleKey.RightArrow) selected = (selected + 1) % items.Count;
                if (key == ConsoleKey.UpArrow || key == ConsoleKey.LeftArrow) selected = (selected - 1 + items.Count) % items.Count;

            } while (key != ConsoleKey.Enter);

            return selected;
        }
    }
}
