using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
namespace zctestframe
{
    class Program
    {
        static void Main(string[] args)
        {
            string con = "server=woainixzh.e2.luyouxia.net;port=26459;user=root;password=QWElsl123;database=root;";
            MySqlConnection conn = new MySqlConnection(con);
            MySqlConnection conn1 = new MySqlConnection(con);
            try
            {
                Console.WriteLine("正在连接数据库...");
                conn.Open();
                conn1.Open();
                string text = "INSERT INTO login (name,pass) VALUES (@param1,@param2)";

                MySqlCommand cmd = new MySqlCommand(text, conn);
                Console.WriteLine("注册，用户名：");
                string name = Console.ReadLine();
                cmd.Parameters.AddWithValue("param1", name);
                Console.WriteLine("密码：");
                cmd.Parameters.AddWithValue("param2", Console.ReadLine());
                string sql = "select * from login where name=@para1;";
                MySqlCommand cmd1 = new MySqlCommand(sql, conn1);
                cmd1.Parameters.AddWithValue("para1", name);
                MySqlDataReader reader = cmd1.ExecuteReader();
                bool readresult = reader.Read();
                if (cmd.ExecuteNonQuery() != 0 && readresult == false)
                {
                    Console.WriteLine("注册成功");
                }

                else
                {
                    switch (readresult)
                    {
                        case true:
                            Console.WriteLine("用户名已占用！");
                            break;
                        case false:
                            Console.WriteLine("其他错误");
                            break;
                    }
                    Console.WriteLine("register failed");
                    Main(null);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                Main(null);
            }
        }
    }
}
