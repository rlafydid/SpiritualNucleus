using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;
using System;
using Battle;

[System.Serializable]
public class ActEventData
{
    public int triggerTime;
    public string displayName;
    public string id;
}

[System.Serializable, NodeMenuItem("表现类效果/播放ACT")]
public class PlayActNode : BaseAsyncNode
{
    public override string name => "播放Act";

    [HideInInspector]//[SerializeField, Input(name = "资源ID")]
    public string resId;

    [SerializeField, Input(name = "生命周期")]
    public int lifeTime;

    [Input(name = "目标集合")]
    public List<long> targets;

    [SerializeField, Input(name = "角色攻速影响")]
    public bool attackSpeedScale;

    public List<ActEventData> actEvents = new List<ActEventData>();

    float timer;
    Queue<ActEventData> waitTriggerEvents = new Queue<ActEventData>();
    Dictionary<string, List<BaseSkillNode>> eventOutputsMapping = new Dictionary<string, List<BaseSkillNode>>();

    ActEventsController actEventsCtrl;

    #region 技能蓝图流程
    //ICustomProcessHandler handler;
    #endregion

    #region 自定义Port
    [Output]
    public ExecuteLink eventOutputs;

    [CustomPortBehavior(nameof(eventOutputs))]
    List<PortData> GetPortsForOutput(List<SerializableEdge> edges)
    {
        List<PortData> ports = new List<PortData>();

        if (actEvents == null)
            return ports;

        actEvents = actEvents.OrderBy(d => d.triggerTime).ToList();
        
        foreach (var eventData in actEvents)
        {
            var portData = new PortData { displayName = $"{eventData.displayName} ({eventData.triggerTime * 0.001f}s)", displayType = typeof(ExecuteLink), identifier = eventData.id };
            ports.Add(portData);
        }
        return ports;
    }
    #endregion

    float m_speed;

    protected override void Process()
    {
        base.Process();
        InitEventOutputs();
        //Debug.Log($"ACT  Process {resId}");

        List<long> tTarget = targets != null ? new List<long>(targets) : null;

        SkillUnit skill = skilUnit as SkillUnit;

        float speed = 1;

        //Debug.Log($"ACT Speed {speed} attackSpeedScale {attackSpeedScale} val {entity.GetComponent<AttrCom>().GetValue(AttrType.AttackSpeed)}");

        //CMDUtility.SendMsg(skill.ownerEntityID, CommandType.PlayAct, new MSG_PlayAct { targetList = tTarget, actName = resId, speed = m_speed.AsFloat() });

        actEventsCtrl = new ActEventsController()
        {
            actEvents = actEvents,
            speed = m_speed,
            lifeTime = lifeTime,
            resId = resId,
            finished = OnFinished,
            triggerEvent = TriggerEvent
        };
        actEventsCtrl.Process();

        Facade.Command.PlayAct(skilUnit.OwnerID, resId);
        
    }

    void InitEventOutputs()
    {
        if (eventOutputsMapping.Count > 0)
            return;

        foreach (NodePort port in outputPorts)
        {
            if (port.fieldName == "eventOutputs")
            {
                if (!eventOutputsMapping.TryGetValue(port.portData.identifier, out var list))
                {
                    list = port.GetEdges().Select(d => d.inputNode as BaseSkillNode).ToList();
                    if (list.Count > 0)
                        eventOutputsMapping.Add(port.portData.identifier, list);
                }
            }
        }
    }

    public override void Update(int deltaTime)
    {
        if (actEventsCtrl == null)
            return;

        actEventsCtrl.OnTick(deltaTime);
    }

    void TriggerEvent(ActEventData data)
    {
        //Debug.Log($"ACT 触发弹道事件 {data.displayName} {resId}");
        if (eventOutputsMapping.TryGetValue(data.id, out var nodes) && nodes.Count > 0)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                //Debug.Log($"ACT 触发弹道事件成功 push {nodes[i].GetType().Name} 名称 {data.displayName} {resId}");
                // processList.Push(nodes[i]);
                //handler.PushCustomProcessNodeAsync(this, nodes[i]);
                this.ExecuteNode(nodes[i]);
            }
        }
    }

    public void SetData(Act.ActAsset actAsset)
    {
        if (actAsset == null)
        {
            actEvents.Clear();
            lifeTime = 0;
            return;
        }
        actEvents = actAsset.ToBPEvents();
        actEvents.Add(new ActEventData(){triggerTime = 0, displayName = "Start", id = "Start"});
        lifeTime = TologicLifeTime(actAsset.LifeTime);
    }

    public bool Equals(Act.ActAsset actAsset)
    {
        if (actAsset == null)
            return true;

        if (lifeTime != TologicLifeTime(actAsset.LifeTime))
            return false;
        if (actEvents.Count != actAsset.Events.Count)
            return false;

        HashSet<string> eventHashA = new HashSet<string>(actEvents.Select(d => d.triggerTime + "_" + d.id));
        HashSet<string> eventHashB = new HashSet<string>(actAsset.Events.Select(d => (Mathf.RoundToInt(d.TriggerTime * 1000) + "_" + d.GUID)));
        eventHashA.ExceptWith(eventHashB);
        return eventHashA.Count == 0;
    }

    int TologicLifeTime(float lifeTime)
    {
        int deltaTime = 20;
        int time = deltaTime * Mathf.CeilToInt(lifeTime * 1000 / deltaTime);
        return time;
    }

}

public class ActEventsController
{
    public string resId;

    public int lifeTime;

    public List<ActEventData> actEvents;

    Queue<ActEventData> waitTriggerEvents = new Queue<ActEventData>();
    Dictionary<string, List<BaseNode>> eventOutputsMapping = new Dictionary<string, List<BaseNode>>();
    long timer;
    public float speed;

    public Action finished;
    public Action<ActEventData> triggerEvent;

    public void Setup()
    {

    }

    public void Process()
    {
        timer = 0;

        waitTriggerEvents = new Queue<ActEventData>(actEvents);
        OnTick(0);
    }

    public void OnTick(int deltaTimeMill)
    {
        while (waitTriggerEvents.Count > 0)
        {
            var actEvent = waitTriggerEvents.Peek();
            if (timer >= actEvent.triggerTime)
            {
                // try
                // {
                    TriggerEvent(actEvent);
                    waitTriggerEvents.Dequeue();
                // }
                // catch (Exception e)
                // {
                //     break;
                // }
            }
            else
                break;
        }
        if (timer >= lifeTime)
        {
            finished();
        }
        timer += deltaTimeMill;
    }

    void TriggerEvent(ActEventData data)
    {
        //Debug.Log($"ACT 触发弹道事件 {data.displayName} {resId} {timer}");
        triggerEvent?.Invoke(data);
    }
}

public static class BPActExtensions
{
    public static List<ActEventData> ToBPEvents(this Act.ActAsset actAsset)
    {
        List<ActEventData> actEvents = new List<ActEventData>();
        int id = 0;
        foreach (var actEvent in actAsset.Events)
        {
            ActEventData eventData = new ActEventData()
            {
                displayName = string.IsNullOrEmpty(actEvent.EventName) ? actEvent.GetType().Name : actEvent.EventName,
                triggerTime = Mathf.RoundToInt(actEvent.TriggerTime * 1000),
                id = actEvent.GUID
            };
            actEvents.Add(eventData);
        }
        return actEvents.OrderBy(d => d.triggerTime).ToList();
    }
}
