using Avalonia.Controls;
using Glash.Client;
using Microsoft.Win32;
using Quick.Localize;
using ReactiveUI;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace GlashClientDesktop.Core.ProxyTypes
{
    [JsonSerializable(typeof(Database))]
    internal partial class DatabaseSerializerContext : JsonSerializerContext { }

    public class Database : AbstractProxyType
    {
        protected override JsonTypeInfo ProxyTypeJsonTypeInfo => DatabaseSerializerContext.Default.Database;
        public override Control GetUI() => new Database_UI() { DataContext = this };
        public override string[] GetFormerIds() => ["Glash.Blazor.Client.ProxyTypes.Database"];
        public override object GetIcon() => Avalonia.Application.Current.FindResource("SemiIconGridSquare");
        public override string GetName() => Locale<Database>.GetString("Database");

        [JsonIgnore]
        public string Text_NetType => Locale<Database>.GetString("Network protocol type");
        [JsonIgnore]
        public string Text_Library => Locale<Database>.GetString("Library");
        
        [JsonIgnore]
        public string Text_User => Locale<Database>.GetString("User");
        [JsonIgnore]
        public string Text_Password => Locale<Database>.GetString("Password");

        [JsonIgnore]
        public Dictionary<string, string> NetTypes { get; set; } = new()
        {
            ["0"] = "MariaDB/MySQL",
            ["4"] = "Microsoft SQL Server",
            ["8"] = "PostgreSQL",
            ["12"] = "Interbase",
            ["14"] = "Firebird",
        };
        private string[] _Libraries;
        public string[] Libraries
        {
            get => _Libraries;
            set => this.RaiseAndSetIfChanged(ref _Libraries, value);
        }
        /*
Network protocol type:
0 = MariaDB/MySQL (TCP/IP)
1 = MariaDB/MySQL (named pipe)
2 = MariaDB/MySQL (SSH tunnel)
3 = MSSQL (named pipe)
4 = MSSQL (TCP/IP)
5 = MSSQL (SPX/IPX)
6 = MSSQL (Banyan VINES)
7 = MSSQL (Windows RPC)
8 = PostgreSQL (TCP/IP)
9 = PostgreSQL (SSH tunnel)
10 = SQLite
11 = ProxySQL Admin
12 = Interbase (TCP/IP)
13 = Interbase (local)
14 = Firebird (TCP/IP)
15 = Firebird (local)
         */

        private string _NetType;
        [Required]
        public string NetType
        {
            get=>_NetType;
            set
            {
                this.RaiseAndSetIfChanged(ref _NetType, value);
                if (value == null)
                    return;
                switch (value)
                {
                    case "0":
                        Libraries=["libmariadb.dll","libmysql.dll","libmysql-6.1.dll"];
                        break;
                    case "4":
                        Libraries=["SQLOLEDB"];
                        break;
                    case "8":
                        Libraries=["libpq.dll","libpq-12.dll"];
                        break;
                    case "12":
                        Libraries=["ibclient64-14.1.dll","gds32-14.1.dll"];
                        break;
                    case "14":
                        Libraries=["fbclient-4.0.dll"];
                        break;
                }
                Library = Libraries?.FirstOrDefault();
            }
        }
        /*
Library or provider (added in v11.1):
MySQL/MariaDB:
    libmariadb.dll
    libmysql.dll
    libmysql-6.1.dll
MS SQL:
    MSOLEDBSQL
    SQLOLEDB
PostgreSQL:
    libpq.dll
    libpq-12.dll
SQLite:
    sqlite3.dll
Interbase:
    ibclient64-14.1.dll
    gds32-14.1.dll
Firebird:
    fbclient-4.0.dll
         */

        private string _Library;
        [Required]
        public string Library
        {
            get => _Library;
            set => this.RaiseAndSetIfChanged(ref _Library, value);
        }
        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }

        [SupportedOSPlatform("windows")]
        public override ProxyTypeButton[] GetButtons(ProxyRuleContext t)
        {
            return
            [
                new ProxyTypeButton(
                    Locale<Database>.GetString("View"),
                    Avalonia.Application.Current.FindResource("SemiIconGridSquare"),
                    ()=>
                    {
                        #pragma warning disable CA1416 // 验证平台兼容性
                    //从注册表中读取NSIS的安装目录
                    var regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\HeidiSQL_is1", false);
                    if (regKey == null)
                    {
                        throw new IOException(Locale<Database>.GetString("Can't found {0},please install {0} first.","HeidiSQL"));
                    }
                    var installLocation = regKey.GetValue("InstallLocation").ToString();
                    var exeFile = Path.Combine(installLocation, "heidisql.exe");
#pragma warning restore CA1416 // 验证平台兼容性

                        var process = Process.Start(exeFile,$"--nettype={NetType} --library={Library} --host={GetLocalIPAddress(t.Config.LocalIPAddress)} --port={t.LocalPort} --user={User} --password={Password}");
                        WaitForProcessMainWindow(process);
                    }
                )
            ];
        }
    }
}
