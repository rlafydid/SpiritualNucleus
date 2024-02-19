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
        public Action<SkillUnit> OnEndAbility;
        public string BPRes;
        public BaseGraph Skill;

        public bool IsAbilityEnded = false;

        public void EndAbility()
        {
            if (IsAbilityEnded)
                return;
            IsAbilityEnded = true;
            OnEndAbility?.Invoke(this);
        }
    }

    public class SkillProcess
    {

        public Action<SkillProcess> Finish { get; set; }

        public bool IsFinished { get; set; } = false;

        public long OwnerId { get => _skill.OwnerID; }

        private SkillUnit _skill;

        List<IAsyncNode> asyncNodes = new List<IAsyncNode>();

        int deltaTime {
            get
            {
                return (int)(Time.deltaTime * 1000);
            }
        }

        Stack<BaseNode> _nodeToExecute = new Stack<BaseNode>();

        public void Init(SkillUnit skill)
        {
            this._skill = skill;
        }

        public void Load()
        {
            BaseGraph baseGraph = Facade.Asset.Instantiate<BaseGraph>($"{_skill.BPRes}");
            
        }

        public void Run(BaseGraph baseGraph)
        {
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
                    (node as BaseSkillNode).Setup(this, _skill);
                }
            }


            if (startNode != null)
            {
                _nodeToExecute.Push(startNode);

                RunTheGraph(_nodeToExecute);
            }
        }

        public void Update()
        {
            bool isRun = false;

            for(int i = asyncNodes.Count - 1; i >= 0; i--)
            {
                var item = asyncNodes[i];
                item.Update(deltaTime);
                if(item.Finished && item is IExecuteNode node)
                {
                    var nodes = node.GetExecutedNodes();
                    if (nodes != null)
                    {
                        foreach(var n in nodes)
                        {
                            _nodeToExecute.Push(n);
                        }
                    }
                    isRun = true;
                    asyncNodes.RemoveAt(i);
                }
            }
 
            if(isRun)
            {
                RunTheGraph(_nodeToExecute);
            }

            if(asyncNodes.Count == 0 && _nodeToExecute.Count == 0)
            {
                if (!_skill.IsAbilityEnded)
                    _skill.EndAbility();
                Finish?.Invoke(this);
            }
        }

        public void Stop()
        {
            asyncNodes.Clear();
            _nodeToExecute.Clear();
        }


        public void ExecuteNode(BaseNode node)
        {
            _nodeToExecute.Push(node);
            RunTheGraph(_nodeToExecute);
        }

        void RunTheGraph(Stack<BaseNode> nodesToRun)
        {
            while(nodesToRun.Count > 0)
            {
                BaseNode node = nodesToRun.Pop();
                //Debug.Log("process" + node.GetType().Name);
                if (node is BaseSkillNode baseSkillNode)
                {
                    baseSkillNode.Reset();
                }
                node.OnProcess();
                switch (node)
                {
                    case IExecuteNode skillNode:
                        var nodes = skillNode.GetExecutedNodes();
                        if (nodes != null)
                        {
                            foreach (var n in nodes)
                            {
                                //Debug.Log("push" + n.GetType().Name);
                                nodesToRun.Push(n);
                            }
                        }
                        
                        break;
                }

                switch (node)
                {
                    case IAsyncNode asyncNode:
                        asyncNodes.Add(asyncNode);
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


