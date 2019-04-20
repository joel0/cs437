using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    int i = 0;
    byte[] buffer2 = { 0 };
    Socket s = new Socket(SocketType.Dgram, ProtocolType.Udp);

    void netComplete(object o, SocketAsyncEventArgs args)
    {
        i += 20;
        s.ReceiveAsync(args);
    }

    // Start is called before the first frame update
    void Start()
    {
        s.Bind(new IPEndPoint(IPAddress.IPv6Loopback, 1337));
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        byte[] buffer = { 0 };
        args.SetBuffer(buffer, 0, 1);
        args.Completed += new System.EventHandler<SocketAsyncEventArgs>(netComplete);
        s.ReceiveAsync(args);

        EndPoint ep = new IPEndPoint(IPAddress.IPv6Loopback, 1337);
        s.SendTo(buffer2, ep);
        //this.transform.Rotate(0, 0, 45);
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody2D rb = (Rigidbody2D)GetComponents(typeof(Rigidbody2D))[0];
        //rb.AddForce(Vector2.right, ForceMode2D.Force);
        transform.Rotate(0, 0, i);
        i = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(2f * Time.deltaTime, 0, 0, Space.World);
            //rb.AddForce(Vector2.right);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-2f * Time.deltaTime, 0, 0, Space.World);
            //rb.AddForce(Vector2.left);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, 2f * Time.deltaTime, 0, Space.World);
            //rb.AddForce(Vector2.up);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(0, -2f * Time.deltaTime, 0, Space.World);
            //rb.AddForce(Vector2.down);
        }
    }
}
