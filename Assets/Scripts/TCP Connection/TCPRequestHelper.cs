using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

[Obsolete]
[RequireComponent(typeof(TCPController))]
public class TCPRequestHelper: MonoBehaviour
{
    private int LastDelta = 0;

    private TCPController controller;

    void Start()
    {
        this.controller = gameObject.GetComponent<TCPController>();
    }

    [Obsolete]
    public void AddGetRandomCommentRequest(Action<CommentInfo> handler)
    {

        Action<BinaryReader> reading = (r) =>
        {
            var id = r.ReadInt32();
            if (id == 0)
            {
                AddGetRandomCommentRequest(handler);
                return;
            }
            var len = r.ReadUInt16();
            var text = new string(r.ReadChars(len));
            var x = r.ReadSingle();
            var y = r.ReadSingle();
            var z = r.ReadSingle();
            //handler(new CommentInfo(id, text, new Vector3(x, y, z)));
        };

        controller.AddRequest(0x05, (w) => { }, reading);
    }

    public void AddCommentPostRequest(string text, List<int> buildings, float x, float y, float z)
    {
        Action<BinaryWriter> writing = (w) =>
        {
            w.Write((short)text.Length);
            w.Write(text.ToCharArray());
            w.Write(x);
            w.Write(y);
            w.Write(z);
            w.Write((ushort)buildings.Count);
            foreach (var b in buildings)
                w.Write((ushort)b);
        };

        controller.AddRequest(0x03, writing, (r) => { r.ReadChars(2); });
    }



    public void AddStatusRequest(Action<bool> handler)
    {
        controller.AddRequest(0x02, (w) => { }, (r) => { handler(r.ReadByte() == 1); });
    }

    public void AddUpdateRequest(Action<GridInfo> handler)
    {
        Action<BinaryReader> reading = (reader) =>
        {
            var gridInfo = new GridInfo(16,16);

            var newDelta = reader.ReadInt32();
            var nObjects = reader.ReadInt16();
            //Debug.Log(string.Format("delta {0}; nObjects: {1}", newDelta, nObjects));

            //Debug.LogFormat("nObjects = {0}", nObjects);

            if(nObjects == 0)
            {
                AddUpdateRequest(handler);
                return;
            }
            for (int i = 0; i < nObjects; i++)
            {
                
                var id = reader.ReadInt16();
                //if(i == 257)
                    //Debug.LogFormat("id={0}", id); 
                var oLen = reader.ReadInt16();
                //if (i == 257)
                    //Debug.LogFormat("oLen={0}", oLen);
                if (id >= 0 && id < 256)
                {
                    var x = reader.ReadByte();
                    var y = reader.ReadByte();
                    var type = reader.ReadSByte();
                    var rot = reader.ReadInt16();
                    var heat = reader.ReadInt16();
                    gridInfo.UpdateCell(new GridInfo.CellData(x, y, type, rot, heat));
                }
                else if (id == 1000)
                {
                    //Debug.LogFormat("Posb {0}", reader.BaseStream.Position);
                    var list = new List<int>();
                    for (int obj = 0; obj < oLen; obj++)
                        list.Add(reader.ReadSByte());
                    gridInfo.BuildingDensity = list;
                    //Debug.LogFormat("Received density of n({0}, i={2}): {1}", list.Count, string.Join(" ", list.ConvertAll(elem => elem.ToString()).ToArray()), i);

                    //Debug.LogFormat("Posa {0}", reader.BaseStream.Position);
                }
                else if (id == 1001)
                {
                    var list = new List<int>();
                    //Debug.LogFormat("olen = {0}", oLen);
                    for (int obj = 0; obj < oLen / 4; obj++)
                        list.Add(reader.ReadInt32());
                    gridInfo.PopulationInfo = list;
                    //Debug.LogFormat("Received population of n({0}, i={2}): {1}", oLen, string.Join(" ", list.ConvertAll(elem => elem.ToString()).ToArray()),i);
                }
                    //else Debug.LogErrorFormat("Unknown id {0}", id);
                // Debug.Log(string.Format("id: {0}; oLen: {1}; x,y = ({2},{3}); type = {4}; rot = {5}", id, oLen, x, y, type, rot));
                //ReceivedObjects[id] = string.Format("{0}\t{1}\t{2}\t{3}", type, x, y, rot);
            }

            LastDelta = newDelta;

            handler(gridInfo);
            
        };
        controller.AddRequest(0x01, (w) => w.Write(LastDelta), reading);
    }

}
