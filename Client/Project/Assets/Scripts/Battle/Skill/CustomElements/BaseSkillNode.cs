using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using NodeGraphProcessor.Examples;
using Battle;
using System.Reflection;

public struct ExecuteLink {}

public interface IAsyncNode
{
	bool AllowStop { get; }
	bool Finished { get;}
	void Update(int deltaTime);
}

interface IExecuteNode
{
	IEnumerable<BaseSkillNode> GetExecutedNodes();

	FieldInfo[] GetNodeFields(); // Provide a custom order for fields (so conditional links are always at the top of the node)
}

public class BaseSkillNode : BaseNode
{
	protected SkillProcess process {get;set;}

	protected SkillUnit skilUnit {get; set;}

	protected SceneActorController owner {get => Facade.Battle.GetActor(skilUnit.OwnerID); }

	public override string name => "BaseSkillNode";

	public void Setup(SkillProcess process, SkillUnit unit)
    {
		this.process = process;
		this.skilUnit = unit;
	}

	public virtual void OnInit() { }

	public virtual void Reset()
	{
	}

	protected override void Process()
    {
        base.Process();
    }

	protected void ExecuteNode(BaseSkillNode node)
	{
		process.ExecuteNode(node);
	}
}


public class UniversalNode : BaseSkillNode, IExecuteNode
{
	[Input(name = "In", allowMultiple = true)]
	public ExecuteLink executed;

	[Output(name = "Out")]
	public ExecuteLink executes;

	public IEnumerable<BaseSkillNode> GetExecutedNodes()
	{
		// Return all the nodes connected to the executes port
		return outputPorts.FirstOrDefault(n => n.fieldName == nameof(executes))
			.GetEdges().Select(e => e.inputNode as BaseSkillNode);
	}

	protected override void Process()
	{
		base.Process();
	}
}

public class BaseAsyncNode : UniversalNode, IAsyncNode
{
	[Input(name = "In")]
	bool allowStop;

    public bool AllowStop { get => allowStop;}

	bool _finish = false;
    public bool Finished { get => _finish; }

    public virtual void Update(int deltaTime)
    {
    }

    public override void Reset()
    {
	    base.Reset();
	    _finish = false;
    }
    
    protected override void Process()
    {
	    base.Process();
    }

    protected void ExecuteNode(BaseNode node)
	{
		process.ExecuteNode(node);
    }

	protected void OnFinished()
    {
		_finish = true;
	}
}



