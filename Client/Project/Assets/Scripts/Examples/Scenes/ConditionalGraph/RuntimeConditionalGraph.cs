using UnityEngine;
using GraphProcessor;
using NodeGraphProcessor.Examples;
using System.Collections;
using System.Collections.Generic;

public class RuntimeConditionalGraph : MonoBehaviour
{
	[Header("Graph to Run on Start")]
	public BaseGraph graph;

	private ConditionalProcessor processor;

	IEnumerator<int> text;

	private void Start()
	{
		if(graph != null)
			processor = new ConditionalProcessor(graph);

		processor.Run();

		text = GetEnumerator();
	}

    private void OnGUI()
    {
		Rect rect = new Rect(10, 10, 100, 100);
		if(GUI.Button(rect, "hhhhh"))
		{
			text.MoveNext();
		}

    }

    IEnumerator<int> GetEnumerator()
    {
		Debug.Log("11");
		yield return 1;
		Debug.Log("22");
		yield return 2;
		Debug.Log("33");
		yield return 3;
		Debug.Log("44");
	}
}