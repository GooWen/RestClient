/// <summary>
/// Credits @RastaMouse SharpC2
/// More details => https://restsharp.dev/getting-started/  RestAPI 客户端
/// </summary>

using System;
using System.Collections.Generic;
using System.Threading;
using _RestClient.Misc;
using _RestClient.Models;
using Newtonsoft.Json;
using static _RestClient.C2API;

using static _RestClient.Misc.Helps;
namespace _RestClient
{
    /// <summary>
    /// Fields
    /// </summary>
    public partial class Program
    {
        // 有信号状态
        public static ManualResetEvent hand = new ManualResetEvent(false);

        public static AutoResetEvent auto_ = new AutoResetEvent(false);     // false 无信号

        public static Mutex mutex = new Mutex();
        public static List<string> Events { get; set; } = new List<string>();
    }

    /// <summary>
    /// Support method
    /// </summary>
    public partial class Program
    {
        /// <summary>
        /// 初始化操作
        /// </summary>
        private static void Init()
        {
            if (CommandStore == null)
            {
                CommandStore = new List<Commands>()
                {
                    new Commands() { index = 0,Name ="Help",Description = "Print this menu..."},
                    new Commands() { index = 1,Name = "Conenct",Description = "Connect to TeamServer..."},
                    new Commands() { index = 2,Name = "Disconenct",Description = "Disconnect to TeamServer..."},
                    new Commands() { index = 3,Name = "GetUsers",Description = "Get Current Logon Users..."},
                    new Commands() { index = 4,Name = "GetServerEvents",Description = "Get TeamServer Events..."},


                    new Commands() { index = 5,Name = "Clear",Description = "Clear current console..."},

                    new Commands() { index = -1,Name = "Exit",Description = "Exit current console..."},
                };
            }

            Helps.Logo();
            Helps.info();
        }
        static void Login()
        {
            Write.WriteInfo1("ip:");
            string ip = Console.ReadLine();
            Write.WriteInfo1("port:");
            string port = Console.ReadLine();
            Write.WriteInfo1("nick:");
            string nick = Console.ReadLine();
            Write.WriteInfo1("pass:");
            string pass = PromptForPass();
            try
            {
                AuthResult ar = Users.ClientLogin(ip, port, nick, pass);
                if (ar?.Status != AuthResult.AuthStatus.LogonSuccess)
                {
                    Write.WritesError(ar.Status.ToString());
                }
            }
            catch (Exception)
            {
                Write.WritesError("Login Failed!");
            }

        }
        private static string PromptForPass()
        {
            var pass = string.Empty;
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            Console.WriteLine();
            Console.WriteLine();

            return pass;
        }

        private static void PrintUsers()
        {
            string ip = "";
            string port = "";
            if (!Users.flag)
            {
                Write.WriteInfo1("ip:");
                ip = Console.ReadLine();
                Write.WriteInfo1("port:");
                port = Console.ReadLine();
            }

            List<string> result = (List<string>)Users.GetAllUsers(ip, port);
            string user = "";
            if (result != null && result.Count > 0)
            {
                Write.WritesInfo2($"The current users number is {result?.Count.ToString()}");

                foreach (var item in result)
                {
                    user += item + ",";
                }
                Write.WritesInfo1("Exist users for:" + user.Substring(0, user.Length - 1) + "\n");
            }
            else
            {
                Write.WritesInfo2("Non user");
            }

        }

        private static async void PrintfEvents()
        {
            Events.Clear();  // 清空 保证每次都是从 TeamServer拉去的纯净数据 避免本地缓存干扰

            string ip = "";
            string port = "";
            if (!Users.eventsflag)
            {
                Write.WriteInfo1("ip:");
                ip = Console.ReadLine();
                Write.WriteInfo1("port:");
                port = Console.ReadLine();
            }

            var serverEvents = await Server.GetServerEvents(ip, port);
            if (serverEvents != null)
            {
                foreach (var ev in serverEvents)
                {
                    AddServerEvent(ev);
                }
            }
            Write.WritesInfo1("TeamServer Event\n\n");
            if (Events.Count > 0)
            {
                foreach (var item in Events)
                {
                    Write.WritesInfo2(item);
                }
            }
            Write.WritesInfo1($"Total count:\t{Events.Count.ToString()}\n");
            // mutex.ReleaseMutex();

            auto_.Set(); // 设置为有信号状态
            // auto_.Reset(); // 设置为无信号状态  WaitOne()将阻塞当前调用进程
        }

        static void AddServerEvent(ServerEvent ev)
        {
            switch (ev.Type)
            {
                case ServerEvent.EventType.UserLogon:

                    var status = Enum.Parse<AuthResult.AuthStatus>(ev.Data.ToString());

                    switch (status)
                    {
                        case AuthResult.AuthStatus.LogonSuccess:
                            Events.Insert(0, $"[{ev.Date}]     {ev.Nick} has joined. Say hi!");
                            break;
                        case AuthResult.AuthStatus.NickInUse:
                            Events.Insert(0, $"[{ev.Date}]     {ev.Nick} tried to join again.");
                            break;
                        case AuthResult.AuthStatus.BadPassword:
                            Events.Insert(0, $"[{ev.Date}]     {ev.Nick} got the password wrong. Duh!");
                            break;
                        default:
                            break;
                    }

                    break;

                case ServerEvent.EventType.UserLogoff:
                    Events.Insert(0, $"[{ev.Date}]     {ev.Nick} has left. Say goodbye!");
                    break;
                case ServerEvent.EventType.ListenerStarted:
                    var listener = JsonConvert.DeserializeObject<Listener>(ev.Data.ToString());
                    Events.Insert(0, $"[{ev.Date}]     {ev.Nick} has started listener {listener.Name}.");
                    break;
                case ServerEvent.EventType.ListenerStopped:
                    Events.Insert(0, $"[{ev.Date}]     {ev.Nick} has stopped listener {ev.Data}.");
                    break;
                case ServerEvent.EventType.ServerModuleRegistered:
                    Events.Insert(0, $"[{ev.Date}]     {ev.Data} module has started.");
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// EntryPoint function
    /// </summary>
    public partial class Program
    {
        static void Main(string[] args)
        {
            Init();

            while (true)
            {
                Write.WriteInfo1("RestClient>");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "Help" or "help" or "0" or "-h" or "?":
                        Helps.info();
                        break;
                    case "Connect" or "c" or "connect" or "1":
                        Login();
                        break;
                    case "Disconnect" or "disc" or "2":
                        Users.ClientLogooff();
                        break;
                    case "GetUsers" or "getuser" or "3":
                        PrintUsers();
                        break;
                    case "GetServerEvents" or "getevents" or "events" or "4":
                        PrintfEvents();
                        auto_.WaitOne();  // 等待直到有信号才执行
                        break;
                    case "Clear" or "cls"  or "5":
                        Console.Clear();
                        break;
                    case "Exit" or "exit" or "q" or "-1":
                        return;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
