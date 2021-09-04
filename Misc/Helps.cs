/// <summary>
/// Credits @RastaMouse SharpC2
/// More details => https://restsharp.dev/getting-started/  RestAPI 客户端
/// </summary>

using _RestClient.Models;
using System;
using System.Collections.Generic;

namespace _RestClient.Misc
{
    class Helps
    {
        public static void Logo()
        {
            Write.WriteInfo1("====> DotNet Client for API Access TeamServer\n");
            Write.WriteInfo1("====\n");
            Write.WriteInfo1("====\n");
            Write.WriteInfo1("                                          v1.0\n\n");
        }
        public static List<Commands> CommandStore;
        public static void info()
        {
            foreach (var item in CommandStore)
            {
                
                Write.WriteInfo1("\t" + item.index.ToString() + "\t" + item.Name.ToString() + "\t"+ item.Description.ToString());
                Console.WriteLine("\n");
            }
        }
    }
}
