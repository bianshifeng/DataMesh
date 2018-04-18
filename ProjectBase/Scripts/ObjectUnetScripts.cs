using DataMesh.AR;
using DataMesh.AR.Network;
using MEHoloClient.Core.Entities;
using MEHoloClient.Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectUnetScripts : MonoBehaviour, IMessageHandler
{


    [SerializeField]
    public string ShowId = "";
    [SerializeField]
    public bool IsUnetEnabled = true;


    [HideInInspector]
    public UnityEvent unetEventUnselect;
    [HideInInspector]
    public UnityEvent unetEventSelect;


    private bool m_isFocused = false;
    private CollaborationManager cm;
    private ShowObject m_showObject;
    private BehaviourType m_behaviourType = BehaviourType.FingerUnSelect;

    void Start()
    {
        StartCoroutine(WaitForInit());
    }
    private IEnumerator WaitForInit()
    {
        MEHoloEntrance entrance = MEHoloEntrance.Instance;
        while (!entrance.HasInit)
        {
            yield return null;
        }
        cm = CollaborationManager.Instance;
        cm.AddMessageHandler(this);
        initShowObject();
        cm.TurnOn();

    }

    private void initShowObject()
    {
        MsgEntry entry = new MsgEntry();
        entry.OpType = MsgEntry.Types.OP_TYPE.Upd;
        entry.ShowId = this.ShowId;
        entry.Vec.Add((long)m_behaviourType);
        m_showObject = new ShowObject(entry);

    }

    private void RequestToServer()
    {
        if (cm != null)
        {
            if (cm.enterRoomResult == EnterRoomResult.EnterRoomSuccess)
            {
                MsgEntry entry = new MsgEntry();
                entry.OpType = MsgEntry.Types.OP_TYPE.Upd;
                entry.ShowId = m_showObject.ShowId;
                entry.Vec.Add((long)m_behaviourType);

                if (m_behaviourType == BehaviourType.FingerDrag)
                {
                    GetTransformFloat(this.transform, entry);
                }

                SyncMsg msg = new SyncMsg();
                msg.MsgEntry.Add(entry);
                cm.SendMessage(msg);
            }
        }
 

    }

    private void ReponseFromServer(SyncProto proto)
    {
        Google.Protobuf.Collections.RepeatedField<MsgEntry> messages = proto.SyncMsg.MsgEntry;

        if (messages == null)
            return;

        for (int i = 0; i < messages.Count; i++)
        {
            MsgEntry msgEntry = messages[i];
            if (msgEntry.ShowId == m_showObject.ShowId)
            {
                BehaviourType t_type = (BehaviourType)((int)msgEntry.Vec[0]);
                if(t_type == BehaviourType.FingerDrag)
                {
                    this.transform.position = new Vector3(msgEntry.Pr[0], msgEntry.Pr[1], msgEntry.Pr[2]);
                    this.transform.eulerAngles = new Vector3(msgEntry.Pr[3], msgEntry.Pr[4], msgEntry.Pr[5]);
                    //Debug.LogFormat("The unet object id {0} which state is {1}.", m_showObject.ShowId,"draging");
                }
            }
        }

    }

    void IMessageHandler.DealMessage(SyncProto proto)
    {
        this.ReponseFromServer(proto);
    }


    public void TriggerToSelect()
    {
        if (!m_isFocused)
        {
            m_isFocused = true;
        }
    }

    public void TriggerToUnselect()
    {
        if (m_isFocused)
        {
            m_isFocused = false;
        }

    }

    public void TriggerToDraging()
    {
        if (m_isFocused)
        {
            m_behaviourType = BehaviourType.FingerDrag;
            RequestToServer();
        }

    }



    private void GetTransformFloat(Transform trans, MsgEntry entry)
    {
        entry.Pr.Clear();

        float[] rs = new float[6];
        entry.Pr.Add(trans.position.x);
        entry.Pr.Add(trans.position.y);
        entry.Pr.Add(trans.position.z);
        entry.Pr.Add(trans.eulerAngles.x);
        entry.Pr.Add(trans.eulerAngles.y);
        entry.Pr.Add(trans.eulerAngles.z);
    }


}
