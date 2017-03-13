using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;

[Obsolete]
public class TCPController : MonoBehaviour {

    //variables
    private class PacketInfo
    {
        public Action<BinaryWriter> composer;
        public Action<BinaryReader> handler;
        public byte opcode;

        public PacketInfo(byte opcode, Action<BinaryWriter> composer, Action<BinaryReader> handler)
        {
            this.opcode = opcode;
            this.composer = composer;
            this.handler = handler;
        }
    }

    private TCPConnection myTCP;

    private string serverMsg;

    public string msgToServer;


    public Event ObjectChange;

    public string Host = "localhost";
    public int Port = 9999;

    //public static 

    public bool UpdateOn = true;

    private Thread receiveThread;
    private object inputLock = new object();
    private object outputLock = new object();

    private List<PacketInfo> pendingRequests;
    private List<PacketInfo> expectedRequests;

    //private byte[] QueryBuffer;

    void Start()
    {
        pendingRequests = new List<PacketInfo>();
        expectedRequests = new List<PacketInfo>();

        myTCP = new TCPConnection(Host, Port);

        receiveThread = new Thread(SocketThread);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void OnDestroy()
    {
        receiveThread.Abort();
        myTCP.CloseSocket();
    }

    void SocketThread()
    {
        SetupConnection();

        while (true)
        {
            Thread.Sleep(500);
            if (!myTCP.IsReady)
            {
                Debug.Log("Trying to re-connect");
                SetupConnection();
            }
            try
            {
                lock(inputLock)
                {
                    lock (outputLock)
                    {
                        if (SocketResponse())
                            socketQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    void SetupConnection()
    {
        //try to connect
        //Debug.Log("Attempting to connect..");
        myTCP.SetupSocket();
        if (myTCP.IsReady)
        {
            //Debug.Log("Attempting to login");
            var buffer = new byte[1024];
            using (var writer = new BinaryWriter(new MemoryStream(buffer), System.Text.Encoding.ASCII))
            {
                writer.Write("CIOMIT".ToCharArray());
                writer.Write((byte)0x04);
                writer.Write("tab1");
            }
            myTCP.WriteSocket(buffer);
        }
    }

    private void socketQuery()
    {    
        byte[] buffer;

        expectedRequests.Clear();

        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream, Encoding.ASCII))
            {
                for(var i = 0; i < pendingRequests.Count; i++)
                {
                    if (i > 0)
                        writer.Write((byte)0x00);
                    var r = pendingRequests[i];
                    writer.Write(r.opcode);
                    r.composer(writer);

                    expectedRequests.Add(r);
                }
                
            }
            buffer = memoryStream.ToArray();
            //Debug.Log(string.Format("SQuery: Buffer is {0}", BitConverter.ToString(buffer)));
            myTCP.WriteSocket(buffer);
            ResetResquests();
        }
    }

    private bool SocketResponse()
    {
        byte[] msg;
        var len = myTCP.ReadSocket(out msg);
        if (len > 0)
        {
            //Debug.Log(string.Format("Len: {0}, MSG: {1}", serverSays, BitConverter.ToString(msg)));
            using (var memoryStream = new MemoryStream(msg, 0, len))
            using (var reader = new BinaryReader(memoryStream))
            {
                //Debug.Log(BitConverter.ToString(msg, 0, len));

                int opcode_num = 0;
                while (reader.PeekChar() != -1)
                {
                    
                    var opcode = reader.ReadByte();
                    //Debug.Log(string.Format("Response for OpCode {0}:", opcode));
                    if (opcode == 0)
                    {
                        var message = new string(reader.ReadChars(2));
                        if (message != "OK")
                            Debug.Log(string.Format("Opcode {0} produced following message: {1}.", opcode, message));
                    }
                    else
                    {
                        if (expectedRequests[opcode_num].opcode != opcode)
                            Debug.LogError("Error: packet inconsistency.");
                        
                        expectedRequests[opcode_num].handler(reader);
                    }


                    opcode_num++;

                    if (reader.PeekChar() != -1 && reader.ReadByte() != 0)
                        Debug.LogError("Bad response format.");
                }
            }

            expectedRequests.Clear();
            return true;
        }
        return expectedRequests.Count == 0;
    }

    

    


    private void ResetResquests()
    {
        lock (inputLock)
        {
            pendingRequests.Clear();
        }
    }

    public void AddRequest(byte opcode, Action<BinaryWriter> composer, Action<BinaryReader> handler)
    {
        lock(inputLock)
        {
            pendingRequests.Add(new PacketInfo(opcode, composer, handler));
        }
    }

    
    
}