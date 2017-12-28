using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class Net : MonoBehaviour {

    public InputField hostInput;
    public InputField portInput;
    public Text recvText; //显示客户端的消息
    public string recvStr; 
    public Text clientText;//显示客户端的IP端口
    public InputField textInput;//聊天输入框

    Socket socket;
    const int BUFFER_SIZE = 1024;
    public byte[] readBuff = new byte[BUFFER_SIZE];

	void Update ()
    {
        recvText.text = recvStr;
	}
    //当"连接"按钮按下 改方法执行
    public void Connetion()
    {
        recvText.text = "";
         socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        string host = hostInput.text;
        int port = int.Parse(portInput.text);
        socket.Connect(host,port);
        clientText.text = "客户端地址" + socket.LocalEndPoint.ToString();
        //当收到服务端的消息异步接收回调receiveCb被调用  
        //1，它解析服务端的消息，显示在recvStr中。
        //，2继续开启异步接收
        socket.BeginReceive(readBuff,0,BUFFER_SIZE,SocketFlags.None,ReceiveCb,null);
    }

    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            //count 接收数据的大小
            int count = socket.EndReceive(ar);
            string str = System.Text.Encoding.UTF8.GetString(readBuff,0,count);
            if (recvStr.Length > 300) recvStr = "";
            recvStr += str + "\n";
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch (Exception e)
        {
            recvText.text += "连接已断开";
            socket.Close();
        }
    }

    //当点击“发送” 按钮  方法执行
    public void Send()
    {
        string str = textInput.text;
        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        try
        {
            socket.Send(bytes);
        }
        catch { }
    }
}
