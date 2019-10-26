using System;

namespace Gymnasiearbete
{
    static class Menu
    {
        public static bool showOptionNumber = true;
        public static string cursor = ">";

        public static int Create(string[] alternatives)
        {
            var vissible = Console.CursorVisible;

            Console.CursorVisible = false;
            PrintMenu(alternatives);
            var result = Select(alternatives.Length);

            Console.CursorVisible = vissible;

            return result;
        }


        private static int Select(int length)
        {
            int menuPosition = Console.CursorTop - length;
            int currentIndex = 0;

            MoveCursor(menuPosition, currentIndex, currentIndex);

            bool numberPressed = false;
            var keyInfo = new ConsoleKey();
            do
            {
                int newIndex = currentIndex;
                if (UpAction(keyInfo))
                    newIndex--;
                else if (DownAction(keyInfo))
                    newIndex++;
                else if (NumberAcation(keyInfo, out int number) && number <= length && number > 0)
                {
                    newIndex = number - 1;
                    numberPressed = true;
                }

                // If new cursor position
                if (newIndex != currentIndex)
                {
                    // Loop around if newIndex is outside the menu
                    if (newIndex < 0)
                        newIndex = length + newIndex;
                    else if (newIndex > length - 1)
                        newIndex = newIndex - length;

                    // move cursor
                    MoveCursor(menuPosition, currentIndex, newIndex);
                    currentIndex = newIndex;
                }
            } while (!numberPressed && (keyInfo = Console.ReadKey(true).Key) != ConsoleKey.Enter);

            // reset console cursor
            Console.SetCursorPosition(0, menuPosition + length);

            return currentIndex;
        }

        private static bool UpAction(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                case ConsoleKey.PageUp:
                    return true;
                default:
                    return false;
            }
        }

        private static bool DownAction(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                case ConsoleKey.PageDown:
                    return true;
                default:
                    return false;
            }
        }

        private static bool NumberAcation(ConsoleKey key, out int number)
        {
            int value = -1;
            switch (key)
            {
                case ConsoleKey.NumPad0:
                case ConsoleKey.D0:
                    value = 0;
                    break;
                case ConsoleKey.NumPad1:
                case ConsoleKey.D1:
                    value = 1;
                    break;
                case ConsoleKey.NumPad2:
                case ConsoleKey.D2:
                    value = 2;
                    break;
                case ConsoleKey.NumPad3:
                case ConsoleKey.D3:
                    value = 3;
                    break;
                case ConsoleKey.NumPad4:
                case ConsoleKey.D4:
                    value = 4;
                    break;
                case ConsoleKey.NumPad5:
                case ConsoleKey.D5:
                    value = 5;
                    break;
                case ConsoleKey.NumPad6:
                case ConsoleKey.D6:
                    value = 6;
                    break;
                case ConsoleKey.NumPad7:
                case ConsoleKey.D7:
                    value = 7;
                    break;
                case ConsoleKey.NumPad8:
                case ConsoleKey.D8:
                    value = 8;
                    break;
                case ConsoleKey.NumPad9:
                case ConsoleKey.D9:
                    value = 9;
                    break;
            }
            number = value;
            return value != -1;
        }

        private static void MoveCursor(int orgin, int from, int to)
        {
            // remove old cursor
            Console.SetCursorPosition(0, orgin + from);
            Console.Write(new string(' ', cursor.Length));
            // display new cursor
            Console.SetCursorPosition(0, orgin + to);
            Console.Write(cursor);
        }

        private static void PrintMenu(string[] alternatives)
        {
            // distance from console edge to fist character
            int lineMargin = cursor.Length + 2;
            // distance form console edge to first text character
            int textMargin = alternatives.Length.ToString().Length + 2;

            // print all alternatives
            for (int i = 0; i < alternatives.Length; i++)
            {
                string num = (i + 1).ToString() + '.';
                Console.WriteLine(new string(' ', lineMargin) + num + new string(' ', textMargin - num.Length) + alternatives[i]);
            }
        }
    }
}
