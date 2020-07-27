using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MySql.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ConsoleClient
{
    class Program
    {
        static string username;
        static Socket client_truesocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            ///验证

            login();

            ///验证
            Console.WriteLine("Version:1.0\nIP:");
            string str = Console.ReadLine();
            string[] s;
            try
            {
                s = str.Split(':');
                IPEndPoint ip = new IPEndPoint(IPAddress.Parse(s[0]), int.Parse(s[1]));
                Connect_Async(ip);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message+"   [提示]请同时输入ip和端口！中间用半角:隔开");
                Main(null);
            }

        }
        /// <summary>
        /// 登录并验证密码
        /// </summary>
        static void login()
        {
            
            string constr = "server=woainixzh.e2.luyouxia.net;port=26459;user=root;password=QWElsl123; database=root;";
            MySqlConnection conn = new MySqlConnection(constr);
            try
            {
                Console.WriteLine("正在连接数据库...");
                conn.Open();
                Console.WriteLine("Connected to Database!");
                string sql = "select * from login where name =@para1 and pass=@para2";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                Console.WriteLine("Username:");
                string name = Console.ReadLine();
                cmd.Parameters.AddWithValue("para1",name);
                Console.WriteLine("passwd:");
                string pass = Console.ReadLine();
                cmd.Parameters.AddWithValue("para2",pass);

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine("Login Successful!");
                    username = name;
                }
                else {
                    Console.WriteLine("Login failed! 检查账号密码！");
                    login();
                }
            }catch(MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to login server!");
                        break;
                    case 1045:
                        Console.WriteLine("错误的账号或密码");
                        break;
                }
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 异步执行连接步骤防止线程阻塞
        /// </summary>
        static void Connect_Async(IPEndPoint ip)
        {
            Thread ConnectThread = new Thread(Connect);
            ConnectThread.Start(ip);
        }
        /// <summary>
        /// 异步连接的调用
        /// </summary>
        static void Connect(object obj)
        {
            try
            {
                client_truesocket.Connect((IPEndPoint)obj);
                //Console.WriteLine("Input UserName：");
                //username=Console.ReadLine();
                Thread thread1 = new Thread(send);
                Thread thread2 = new Thread(recieve);
                thread1.Start();thread2.Start();
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
                Main(null);
            }

        }
        static void send()
        {
            while (true)
            {
                client_truesocket.Send(Encoding.UTF8.GetBytes(username+":"+Console.ReadLine()));
            }
        }
        static void recieve()
        {
            Socket recsocket = client_truesocket;
            byte[] cache = new byte[1024 * 1024];
            while (true) {
                int length = recsocket.Receive(cache);
                string msg = Encoding.UTF8.GetString(cache, 0, length);
                Console.WriteLine(msg);
                    }
        }
    }
}
