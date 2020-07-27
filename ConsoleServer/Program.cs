using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace ConsoleServer
{
    class Program
    {
        static string msg;
        static int i = 0,clientscount=0;
        static IPEndPoint localip=new IPEndPoint(IPAddress.Parse("127.0.0.1"),25569);
        static Socket client11 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Dictionary<string, Socket> clients = new Dictionary<string, Socket>();
        static void Main(string[] args)
        {
            Console.WriteLine("ip:");
            localip.Address = IPAddress.Parse(Console.ReadLine());
            Console.WriteLine("port:");
            localip.Port = int.Parse(Console.ReadLine());
            Thread ini = new Thread(init);
            ini.Start();
            while (true)
            {
                msg = Console.ReadLine();
                foreach(Socket sendservermsg in clients.Values)
                {
                    sendservermsg.Send(Encoding.UTF8.GetBytes("[Server]" + msg));
                }
            }
        }
        static void init()
        {
            
            server.Bind(localip);
            Thread listenthread = new Thread(listen);
            listenthread.Start(server);
        }
        static void listen(object obj)
        {
            Socket s = (Socket)obj;
            s.Listen(100);
            while (true)
            {
                Socket c1 = s.Accept();

                clients.Add(c1.RemoteEndPoint.ToString(), c1);
                clientscount++;
                client11 = c1;
                Thread cli = new Thread(client);
                cli.Start(c1);
                Thread sen = new Thread(send);
                sen.Start(c1);
            }
        }
        /*receive and send.*/
        static void client(object obj)
        {
            Socket cli = (Socket)obj;
            byte[] buffer = new byte[1024 * 1024];
            while (true)
            {
                try
                {
                    int length = cli.Receive(buffer);
                    string msg = Encoding.UTF8.GetString(buffer, 0, length);//msg,do not send to screen
                    foreach (Socket tempsendsocket in clients.Values)
                    {
                        tempsendsocket.Send(Encoding.UTF8.GetBytes(msg));
                    }
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
                //Console.WriteLine(msg);

            }
            clients.Remove(cli.RemoteEndPoint.ToString());
            
        }
        //server send
        static void send(object obj)
        {

            Socket s = (Socket)obj;
            while (i == 1)
            {
                s.Send(Encoding.UTF8.GetBytes(msg));
                i = 0;
            }
        }
    }
}
