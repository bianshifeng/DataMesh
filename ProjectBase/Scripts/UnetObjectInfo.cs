using MEHoloClient.Core.Entities;
using MEHoloClient.Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviourType
{
    FingerUnSelect = 0,
    GazeExit = 1,
    GazeEnter = 2,
    FingerSelect = 3,
    FingerDrag = 4
}

public class UnetObjectInfo
{

    [SerializeField]
    public string objectName;

    [SerializeField]
    public string objectShowId;
    private string objectType = "BehaviourType";
    public BehaviourType objectBehavior = BehaviourType.GazeExit;


    private ShowObject m_showObject;

    public MsgEntry objectEntry;


    public  UnetObjectInfo()
    {

        ObjectInfo info = new ObjectInfo
        {
            ObjType = this.objectType
        };

        objectEntry = new MsgEntry
        {
            OpType = MsgEntry.Types.OP_TYPE.Upd,
            ShowId = objectShowId
        };
        objectEntry.Vec.Add((long)objectBehavior);

    }
	// Use this for initialization

     public void CreateObjectEntryData(Transform objectTrans, MsgEntry entry)
    {
        entry.ShowId = objectShowId;
        entry.Pr.Clear();
        float[] rs = new float[6];
        entry.Pr.Add(objectTrans.position.x);
        entry.Pr.Add(objectTrans.position.y);
        entry.Pr.Add(objectTrans.position.z);
        entry.Pr.Add(objectTrans.eulerAngles.x);
        entry.Pr.Add(objectTrans.eulerAngles.y);
        entry.Pr.Add(objectTrans.eulerAngles.z);
    }

    public void RefreshObjectEntryData( Transform  trans, SyncProto proto)
    {
        Google.Protobuf.Collections.RepeatedField<MsgEntry> messages = proto.SyncMsg.MsgEntry;
        if (messages == null) return;

        for(int i = 0; i < messages.Count; i++)
        {
            MsgEntry msg = messages[i];
            if(msg.ShowId == this.objectShowId)
            {
                trans.position = new Vector3(msg.Pr[0], msg.Pr[1], msg.Pr[2]);
                trans.eulerAngles = new Vector3(msg.Pr[3], msg.Pr[4], msg.Pr[5]);
            }
        }

    }
	

}
