using System;
using System.Net;
using System.Net.Sockets;

namespace NetFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            //Socket socket1 = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            //IPAddress address = IPAddress.Parse("192.168.0.173");
            //IPEndPoint endPoint = new IPEndPoint(address,8888);
            //socket1.Bind(endPoint);

            //socket1.Listen(0);

            //Console.WriteLine("[服务器启动成功]");

            //while (true)
            //{
            //   Socket conneted =  socket1.Accept();
            //    Console.WriteLine("服务器Accept");
            //    byte[] readBuffer = new byte[1024];
            //    int count =  conneted.Receive(readBuffer);
            //    string str =  System.Text.Encoding.UTF8.GetString(readBuffer,0,count);
            //    Console.WriteLine("服务器接受:" + str);
            //    byte[] bytes = System.Text.Encoding.Default.GetBytes("Please Receive" + str);
            //    conneted.Send(bytes);
            //}

            Console.WriteLine("hello world");
            Serv serv = new Serv();
            serv.Start("192.168.1.173",8888); //本机IP地址  如果输入127.0.0.1  那么端口只对本机自己服务
            while (true)
            {
                string str = Console.ReadLine();
                switch (str)
                {
                    case "quit":
                        return;
                }
            }
        }
    }
}
