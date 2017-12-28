/********************************************************************************
** UIManager
** 2017/8
*********************************************************************************/
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;


class Conn
{
    //常量 缓冲区大小
    public const int BUFFER_SIZE = 1024;
    //与客户端连接的套接字
    public Socket socket;
    //是否使用 在serv类中定义一个固定长度的conn形成对想吃，如果有客户端连接，找到可用的。断开，设置为未使用。找不到，拒绝连接。
    public bool isUse;
    //每次读的缓冲区的大小
    public byte[] readBUFF = new byte[BUFFER_SIZE];
    //当前读缓冲区的长度
    public int buffCount = 0;

    public Conn()
    {
        readBUFF = new byte[BUFFER_SIZE];
    }

    public void Init(Socket socket)
    {
        this.socket = socket;
        isUse = true;
        buffCount = 0;
    }
    //缓冲区剩余的字节数
    public int BufferRemain()
    {
        return BUFFER_SIZE - buffCount;
    }
    //获取客户端地址  
    public string GetAddress()
    {
        if (!isUse)
        {
            return "无法获取地址";
        }
        return socket.RemoteEndPoint.ToString(); //这个方法是获取客户端的IP地址和端口
    }
    //调用socket的close方法关闭这条连接
    public void Close()
    {
        if (!isUse)
            return;
        Console.WriteLine("[断开连接]"+GetAddress());
        socket.Close();
        isUse = false;
    }

}

