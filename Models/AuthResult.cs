/// <summary>
/// Credits @RastaMouse SharpC2
/// More details => https://restsharp.dev/getting-started/  RestAPI 客户端
/// </summary>

using System;

namespace _RestClient.Models
{
    public class AuthResult
    {
        public AuthStatus Status { get; set; }
        public string Token { get; set; }
        public enum AuthStatus
        { 
            LogonSuccess,
            NickInUse,
            BadPassword
        }
    }
    /// <summary>
    /// TeamServer fingerprint
    /// </summary>
    public class CertThumbprint
    { 
        public static string CertHash { get; set; }
    }

    public class AuthRequest
    { 
        public string Nick { get; set; }

        public string Password { get; set; }
    }
    public class ServerEvent
    {
        public DateTime Date { get; set; }
        public EventType Type { get; set; }
        public object Data { get; set; }
        public string Nick { get; set; }

        public ServerEvent(EventType Type, object Data, string Nick = "")
        {
            this.Type = Type;
            this.Data = Data;
            this.Nick = Nick;

            Date = DateTime.UtcNow;
        }

        public enum EventType
        {
            UserLogon,
            UserLogoff,
            ListenerStarted,
            ListenerStopped,
            ServerModuleRegistered,
            WebLog
        }
    }

    public class Listener
    {
        public string Name { get; set; }
        public ListenerType Type { get; set; }
        public DateTime KillDate { get; set; }

        public enum ListenerType
        {
            HTTP,
            TCP,
            SMB
        }
    }

    /// <summary>
    /// Beacon 元数据
    /// </summary>
    public class AgentMetadata
    {
        public string AgentID { get; set; }
        public string ParentAgentID { get; set; }
        public string IPAddress { get; set; }
        public string Hostname { get; set; }
        public string Identity { get; set; }
        public string Process { get; set; }
        public int PID { get; set; }
        public Platform Arch { get; set; }
        public Integrity Elevation { get; set; }
        public DateTime LastSeen { get; set; }

        public enum Platform
        { 
            x64,
            x86
        }
        public enum Integrity
        {
            Unknown,
            Medium,
            High,
            SYSTEM
        }
    }
}
