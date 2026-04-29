using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class BTVariable : Resource
{
	public enum BTVariableType
	{
		None,
		Value,
		State,
		Node
	}

	[Export]
	private BTVariableType _bbVarType;

	[Export]
	private StringName _bbVarName;

	[Export]
	private Variant _val;
	public Variant Val { get => _val; }

	public Variant GetVariable(AIBlackboard bb)
	{
		switch (_bbVarType)
		{
			default:
				return _val;

			case BTVariableType.Value:
				return bb.GetVal(_bbVarName);

			case BTVariableType.State:
				if (!bb.HasState(_bbVarName))
				{
					throw new KeyNotFoundException("State \"" + _bbVarName + "\" not found in blackboard.");
				}

				return _bbVarName;

			case BTVariableType.Node:
				return bb.GetNode(_bbVarName);
		}
	}

	public void SetVariable(AIBlackboard bb, Variant newVal)
	{
		switch (_bbVarType)
		{
			case BTVariableType.Value:
				bb.SetVal(_bbVarName, newVal);
				break;

			case BTVariableType.State:
				StringName newState = newVal.AsStringName();

				if (!bb.HasState(newState))
				{
					throw new KeyNotFoundException("State \"" + newState + "\" not found in blackboard.");
				}

				bb.SetState(newState);
				break;

			case BTVariableType.Node:
				bb.SetNode(_bbVarName, newVal.As<Node>());
				break;
		}
	}

	public bool VariableMatch(AIBlackboard bb)
	{
		switch (_bbVarType)
		{
			default:
				return true;

			case BTVariableType.Value:
				return bb.GetVal(_bbVarName).Obj == _val.Obj;

			case BTVariableType.State:
				if (!bb.HasState(_bbVarName))
				{
					throw new KeyNotFoundException("State \"" + _bbVarName + "\" not found in blackboard.");
				}

				return bb.InState(_bbVarName);

			case BTVariableType.Node:
				return bb.GetNode(_bbVarName) == _val.As<Node>();
		}
	}
}
