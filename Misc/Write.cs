/// <summary>
/// Credits @RastaMouse SharpC2
/// More details => https://restsharp.dev/getting-started/  RestAPI 客户端
/// </summary>

using System;

namespace _RestClient.Misc
{
    public static class Write
    {
        public static void WritesInfo1(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void WritesInfo2(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void WritesWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void WritesError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteInfo1(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(message);
            Console.ResetColor();
        }
        public static void WriteInfo2(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);
            Console.ResetColor();
        }
        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);
            Console.ResetColor();
        }
        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            Console.ResetColor();
        }
    }
}
