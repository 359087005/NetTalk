/********************************************************************************
** UIManager
** 2017/8
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

class Serv
{
    //监听的套接字
    public Socket listenfd;
    //客户端连接
    public Conn[] conns;
    //最大连接数
    public int maxConn = 50;

    //获取连接索引池 返回负数表示获取失败
    public int NewIndex()
    {
        if (conns == null)
            return -1;
        for (int i = 0; i < conns.Length; i++)
        {
            if (conns[i] == null)
            {
                conns[i] = new Conn();
                return i;
            }
            else if (conns[i].isUse == false)
            {
                return i;
            }
        }
        return -1;
    }

    public void Start(string host, int port)
    {
        //连接池
        conns = new Conn[maxConn];
        for (int i = 0; i < conns.Length; i++)
        {
            conns[i] = new Conn();
        }
        //Socket 单机版流程  定义socket  bind IP端口 开启监听listen   开启accept 
        listenfd = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

        IPAddress ipAddress = IPAddress.Parse(host);
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress,port);
        //bind IP端口
        listenfd.Bind(ipEndPoint);
        //开启监听listen  
        listenfd.Listen(maxConn);
        //Accept
        listenfd.BeginAccept(AcceptCb,null);
        Console.WriteLine("[服务器]启动成功");
    }
    /// <summary>
    /// beginAccept的回调函数    
    /// 1，给新的连接分配conn
    /// 2，异步接受客户端数据
    /// 3，再次调用beginaccept方法实现循环。
    /// </summary>
    /// <param name="ar"></param>
    private void AcceptCb(IAsyncResult ar)
    {
        try
        {
            Socket socket = listenfd.EndAccept(ar);
            int index = NewIndex();//获取索引分配连接池
            if (index < 0)
            {
                socket.Close();
                Console.WriteLine("[Warn]连接已满");
            }
            else
            {
                Conn conn = conns[index];
                conn.Init(socket);
                string str = conn.GetAddress();
                Console.WriteLine("客户端连接[" + str + "] conn池ID:" + index);
                //异步接受客户端数据
                conn.socket.BeginReceive(conn.readBUFF,conn.buffCount,conn.BufferRemain(),SocketFlags.None,ReceiveCb,conn);
            }
            listenfd.BeginAccept(AcceptCb,null);
        }
        catch(Exception e)
        {
            Console.WriteLine("AcceptCb失败:" + e.Message);
        }
    }
    /// <summary>
    /// 服务端程序 接收回调
    /// 1,接受并处理消息，多人聊天，服务器接收到消息后 要转发给所有人
    /// 2,如果收到客户端连接关闭的信号 count==0 断开连接
    /// 3，继续调用beginreceive接受下一个数据
    /// </summary>
    private void ReceiveCb(IAsyncResult ar)
    {
        Conn conn = (Conn)ar.AsyncState; //获取beginreceive的conn对象
        try
        {
            int count = conn.socket.EndReceive(ar); //获取接收的字节数
            if (count < 0)
            {
                Console.WriteLine("收到[" + conn.GetAddress() + "]断开连接");
                conn.Close();
                return;
            }
            //连接成功  数据处理
            string str = System.Text.Encoding.UTF8.GetString(conn.readBUFF,0,count);
            Console.WriteLine("收到[" + conn.GetAddress() + "]数据" + str);
            str = conn.GetAddress() + ":" + str;
            byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
            //广播
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                    continue;
                if (!conns[i].isUse)
                    continue;
                Console.WriteLine("将消息转播给" + conns[i].GetAddress());
                conns[i].socket.Send(bytes);
            }
            conn.socket.BeginReceive(conn.readBUFF,conn.buffCount,conn.BufferRemain(),SocketFlags.None,ReceiveCb,conn);
        }
        catch (Exception e)
        {
            Console.WriteLine("收到[" + conn.GetAddress() + "]断开连接");
            conn.Close();
        }
    }
}

