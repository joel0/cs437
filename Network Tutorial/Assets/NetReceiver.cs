using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetReceiver : MonoBehaviour
{
    Socket mUdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    public int ListenPort = 1337;
    private float mTargetX = 0;
    private float mTargetY = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
        mUdpSocket.Bind(new IPEndPoint(IPAddress.Any, ListenPort));
        socketArgs.SetBuffer(new byte[8], 0, 8);
        socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(PacketReceived);
        mUdpSocket.ReceiveAsync(socketArgs);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(mTargetX, mTargetY);
    }

    void PacketReceived(object o, SocketAsyncEventArgs args)
    {
        Socket socket = (Socket)o;
        mTargetX = BitConverter.ToSingle(args.Buffer, 0);
        mTargetY = BitConverter.ToSingle(args.Buffer, 4);
        socket.ReceiveAsync(args);
    }
}
