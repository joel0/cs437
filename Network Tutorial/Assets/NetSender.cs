using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetSender : MonoBehaviour
{
    Socket mUdpSend = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    public string remoteHost = "127.0.0.1";
    public int remotePort = 1337;
    IPEndPoint mRemoteEndPoint;

    // Start is called before the first frame update
    void Start()
    {
        mRemoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteHost), remotePort);
    }

    // Update is called once per frame
    void Update()
    {
        SendLocation(transform.position.x,
            transform.position.y);
    }

    void SendLocation(float x, float y)
    {
        byte[] buffer = new byte[8];
        BitConverter.GetBytes(x).CopyTo(buffer, 0);
        BitConverter.GetBytes(y).CopyTo(buffer, 4);
        mUdpSend.SendTo(buffer, mRemoteEndPoint);
    }
}
