/// <summary>
/// Credits @RastaMouse SharpC2
/// More details => https://restsharp.dev/getting-started/  RestAPI 客户端
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;

using _RestClient.Models;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using _RestClient.Misc;
using Newtonsoft.Json;

namespace _RestClient
{
    /// <summary>
    /// Global Fileds
    /// </summary>
    public partial class C2API
    {
        public static RestClient Client { get; set; } = new RestClient();

        private static byte[] AcceptedHash;
    }

    /// <summary>
    /// Support functions
    /// </summary>
    public partial class C2API
    {
        public static bool ValidateServerCertficate(object sender, X509Certificate Cert, X509Chain Chain, SslPolicyErrors SslPolicyErrors)
        {
            if (SslPolicyErrors == SslPolicyErrors.None) { return true; }
            byte[] thumbPrint = Cert.GetCertHash();
            if (AcceptedHash != null && AcceptedHash.SequenceEqual(thumbPrint)) { return true; }
            CertThumbprint.CertHash = BitConverter.ToString(thumbPrint);
            Write.WritesInfo1($"TeamServer fingerprint:\t {CertThumbprint.CertHash}");
            AcceptedHash = thumbPrint;
            return true;
        }
    }

    /// <summary>
    /// Credits @RastaMouse SharpC2
    /// More details => https://restsharp.dev/getting-started/
    /// </summary>
    public partial class C2API
    {
        public class Users
        {
            public static Dictionary<string, string> WhoesToken = new Dictionary<string, string>();
            public static bool flag = false;
            public static bool eventsflag = false;
            public static void Init(string host, string port)
            {
                if (!flag)
                {
                    Client.BaseUrl = new Uri($"https://{host}:{port}");
                    Client.AddDefaultHeader("Content-Type", "application/json");
                    // set web proxy
                    // Client.Proxy = new System.Net.WebProxy("https://127.0.0.1", 8080);
                    // 利用Callback 回调验证 服务器证书
                    ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertficate;
                    flag = true;
                }
            }
            /// <summary>
            /// RestRequest(url,POST);
            /// </summary>
            /// <param name="host"></param>
            /// <param name="port"></param>
            /// <param name="Nick"></param>
            /// <param name="Pass"></param>
            /// <returns></returns>
            public static AuthResult ClientLogin(string host, string port, string Nick, string Pass)
            {
                // 初始化
                Init(host,port);

                var apiRequest = new RestRequest("/api/users", Method.POST);
                apiRequest.AddParameter("application/json", JsonConvert.SerializeObject(new AuthRequest { Nick = Nick, Password = Pass }), ParameterType.RequestBody);

                var apiResponse = Client.Execute(apiRequest);
                // 反序列化结果
                var result = JsonConvert.DeserializeObject<AuthResult>(apiResponse.Content);
                if (result != null && result.Status == AuthResult.AuthStatus.LogonSuccess)
                {
                    Client.AddDefaultHeader("Authorization",$"Bearer {result.Token}");
                    /*
                    每次登录成功设置全局 token以便后面使用
                    */

                    WhoesToken.Add(Nick,result.Token);

                    Write.WritesInfo1($"Logon Status:\t{result.Status.ToString()}");
                    Write.WritesInfo1($"Authenticate token:\t {result.Token}");
                }
                return result;
            }

            public static void ClientLogooff()
            {
                
                var apiRequest = new RestRequest($"api/users", Method.DELETE);
                Client.Execute(apiRequest);
            }
            public static IEnumerable<string> GetAllUsers(string host, string port)
            {
                Init(host, port);
                var apiRequest = new RestRequest($"api/users", Method.GET);
                if (WhoesToken != null && WhoesToken.Count > 0)
                {
                    // 默认取第一个token
                    string token = WhoesToken.Where(t => t.Key == WhoesToken.Keys.ToArray()[0]).Select( p => p.Value).FirstOrDefault();
                    // Client.AddDefaultHeader("Authorization", $"Bearer {token}");
                }
                var apiResponse = Client.Execute(apiRequest);
                if (apiResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Write.WritesError($"{apiResponse.StatusCode.ToString()}");
                    flag = false;
                }
                // 反序列化结果
                var result = JsonConvert.DeserializeObject<IEnumerable<string>>(apiResponse.Content);
                return result;
            }

        }

        /// <summary>
        /// Async await RestClient.ExecuteAsync() 异步执行
        /// </summary>
        public class Server
        {
            public static async Task<List<ServerEvent>> GetServerEvents(string host,string port)
            {
                Users.Init(host,port);
                var apiRequest = new RestRequest("/api/server/events", Method.GET);
                if (Users.WhoesToken != null && Users.WhoesToken.Count > 0)
                {
                    // 默认取第一个token
                    string token = Users.WhoesToken.Where(t => t.Key == Users.WhoesToken.Keys.ToArray()[0]).Select(p => p.Value).FirstOrDefault();
                    // Client.AddDefaultHeader("Authorization", $"Bearer {token}");
                }
                var apiResponse = await Client?.ExecuteAsync(apiRequest);
                if (apiResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Write.WritesError($"{apiResponse.StatusCode.ToString()}");
                    Users.eventsflag = false;
                }
                else
                {
                    Users.eventsflag = true;
                }
                return JsonConvert.DeserializeObject<List<ServerEvent>>(apiResponse.Content);
            }
        }
    }
}
