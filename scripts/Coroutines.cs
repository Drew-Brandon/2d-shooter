using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static partial class Coroutines
{
	private class CoroutineGroup
	{
		public int Count;
		public CancellationTokenSource CTS;

		public CoroutineGroup(int count, CancellationTokenSource cts)
		{
			Count = count;
			CTS = cts;
		}
	}

	private static Dictionary<StringName, CoroutineGroup> _tokens = new();

	private static async void CallAndCleanCoroutine(StringName groupName, CoroutineGroup group, Func<CancellationTokenSource, Task> routine)
	{
		await routine.Invoke(group.CTS);
		group.Count--;

		if (group.Count == 0)
		{
			_tokens.Remove(groupName);
		}
	}

	public static void StartCoroutine(Func<CancellationTokenSource, Task> routine, StringName groupName)
	{
		CoroutineGroup group;

		if (_tokens.ContainsKey(groupName))
		{
			group = _tokens[groupName];
		}
		else
		{
			group = new CoroutineGroup(1, new CancellationTokenSource());
			_tokens.Add(groupName, group);
		}

		CallAndCleanCoroutine(groupName, group, routine);
	}

	public static bool HasCoroutineGroup(StringName groupName)
	{
		return _tokens.ContainsKey(groupName);
	}

	public static void StopCoroutineGroup(StringName groupName)
	{
		_tokens[groupName].CTS.Cancel();
		_tokens.Remove(groupName);
	}

	public static Task Wait(Node node, double seconds)
	{
		if (GodotObject.IsInstanceValid(node) && node.IsInsideTree())
		{
			SceneTree tree = node.GetTree();
			return Task.Run(async () =>
			{
				await tree.ToSignal(tree.CreateTimer(seconds), SceneTreeTimer.SignalName.Timeout);
			});
		}

		return Task.Run(() => { });
	}
}
