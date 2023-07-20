using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;

namespace Battle
{
    public class SkillUnit
    {
        public long OwnerID;
        public long TargetID;
        public int SkillID;
        public SkillConfig Config;
        public Action<SkillUnit> Finish;
        public string BPRes;
    }

    public class SkillProcess
    {

        public Action<SkillProcess> Finish { get; set; }

        public bool IsFinished { get; set; } = false;

        public long OwnerId { get => skill.OwnerID; }

        protected SkillUnit skill;

        List<IAsyncNode> asyncNodes = new List<IAsyncNode>();

        int deltaTime {
            get
            {
                return (int)(Time.deltaTime * 1000);
            }
        }

        Stack<BaseNode> nodeToExecute = new Stack<BaseNode>();

        public void Init(SkillUnit skill)
        {
            this.skill = skill;
        }

        public void Load()
        {
            BaseGraph baseGraph = Facade.Asset.Instantiate<BaseGraph>($"{skill.BPRes}");
            StartNode startNode = null;
            foreach (var node in baseGraph.nodes)
            {
                switch(node)
                {
                    case StartNode tStart:
                        startNode = tStart;
                        break;
                }
                if (node is StartNode tStartNode)
                {
                    startNode = tStartNode;
                }
                else
                {
                    (node as BaseSkillNode).Setup(this, skill);
                }
            }


            if (startNode != null)
            {
                nodeToExecute.Push(startNode);

                RunTheGraph(nodeToExecute);
            }
        }

        public void Update()
        {
            bool isRun = false;

            for(int i = asyncNodes.Count - 1; i >= 0; i--)
            {
                var item = asyncNodes[i];
                item.Update(deltaTime);
                if(item.Finished)
                {
                    foreach(var n in (item as IExecuteNode).GetExecutedNodes())
                    {
                        nodeToExecute.Push(n);
                    }
                    isRun = true;
                    asyncNodes.RemoveAt(i);
                }
            }
 
            if(isRun)
            {
                RunTheGraph(nodeToExecute);
            }

            if(asyncNodes.Count == 0 && nodeToExecute.Count == 0)
            {
                skill.Finish?.Invoke(skill);
                Finish?.Invoke(this);
            }
        }

        public void Stop()
        {
            asyncNodes.Clear();
            nodeToExecute.Clear();
        }


        public void ExecuteNode(BaseNode node)
        {
            nodeToExecute.Push(node);
            RunTheGraph(nodeToExecute);
        }

        void RunTheGraph(Stack<BaseNode> nodesToRun)
        {
            while(nodesToRun.Count > 0)
            {
                BaseNode node = nodesToRun.Pop();
                //Debug.Log("process" + node.GetType().Name);
                node.OnProcess();
                switch (node)
                {
                    case IAsyncNode asyncNode:
                        asyncNodes.Add(asyncNode);
                        break;
                    case IExecuteNode skillNode:
                        foreach (var n in skillNode.GetExecutedNodes())
                        {
                            //Debug.Log("push" + n.GetType().Name);
                            nodesToRun.Push(n);
                        }
                        break;
                }
            }
        }

    }

    public enum EAttackRangeType
    {
        Sector,
        Line
    }
}


