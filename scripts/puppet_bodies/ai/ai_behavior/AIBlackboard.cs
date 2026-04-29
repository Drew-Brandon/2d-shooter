using Godot;
using Godot.Collections;

/// <summary>
/// Stores various values associated with key names for an AI behavior tree.
/// </summary>
[GlobalClass]
public partial class AIBlackboard : Node
{
	private StringName _curState;
	public StringName CurState { get => _curState; }

	[Export]
	private StringName[] _states;

	/// <summary>
	/// The set of states that the blackboard is making use of.
	/// This is a set as it allows for fast access/checking of states. 
	/// </summary>
	private System.Collections.Generic.HashSet<StringName> _stateSet = new();

	[Export]
	private Dictionary<StringName, Variant> _vals;

	[Export]
	private Dictionary<StringName, NodePath> _nodePaths;

	/// <summary>
	/// The dictionary to use for accessing nodes.
	/// This uses the data from the _nodePaths dictionary.
	/// </summary>
	private Dictionary<StringName, Node> _nodes = new();

	public override void _Ready()
	{
		if (_states != null && _states.Length > 0)
		{
			_curState = _states[0];

			for (int i = 0; i < _states.Length; i++)
			{
				_stateSet.Add(_states[i]);
			}
		}

		foreach (System.Collections.Generic.KeyValuePair<StringName, NodePath> pair in _nodePaths)
		{
			if (pair.Value.IsEmpty)
			{
				_nodes.Add(pair.Key, null);
			}
			else
			{
				_nodes.Add(pair.Key, GetNode(pair.Value));
			}
		}
	}

	/// <summary>
	/// Gets the variant value of the specified name.
	/// </summary>
	/// <param name="valName">
	/// The name of the variant/value to get.
	/// </param>
	/// <returns>
	/// The variant containing the value.
	/// </returns>
	public Variant GetVal(StringName valName)
	{
		return _vals[valName];
	}

	/// <summary>
	/// Determines whether the blackboard has the specified value name.
	/// </summary>
	/// <param name="valName">
	/// The value name to check for.
	/// </param>
	/// <returns>
	/// Whether or not the value name is on the blackboard.
	/// </returns>
	public bool HasVal(StringName valName)
	{
		return _vals.ContainsKey(valName);
	}

	/// <summary>
	/// Sets the variant value to a new variant value. 
	/// </summary>
	/// <param name="valName">
	/// The name of the value to set.
	/// </param>
	/// <param name="newVal">
	/// The new value to store.
	/// </param>
	public void SetVal(StringName valName, Variant newVal)
	{
		_vals[valName] = newVal;
	}

	/// <summary>
	/// Sets the variant value to a new variant value. 
	/// </summary>
	/// <param name="newVal">
	/// The new value to store.
	/// </param>
	/// <param name="valName">
	/// The name of the value to set.
	/// </param>
	public void SetValAlt(Variant newVal, StringName valName)
	{
		_vals[valName] = newVal;
	}

	/// <summary>
	/// Gets the node of the specified name.
	/// </summary>
	/// <param name="nodeName">
	/// The name of the node to get.
	/// </param>
	/// <returns>
	/// The accessed node.
	/// </returns>
	public Node GetNode(StringName nodeName)
	{
		return _nodes[nodeName];
	}

	/// <summary>
	/// Determines whether the blackboard has the specified node name.
	/// </summary>
	/// <param name="nodeName">
	/// The name of the node to set.
	/// </param>
	/// <returns>
	/// Whether or not the node name is on the blackboard.
	/// </returns>
	public bool HasNode(StringName nodeName)
	{
		return _nodes.ContainsKey(nodeName);
	}

	/// <summary>
	/// Sets the node to a new value. 
	/// </summary>
	/// <param name="nodeName">
	/// The name of the node to set.
	/// </param>
	/// <param name="newNode">
	/// The new node to store.
	/// </param>
	public void SetNode(StringName nodeName, Node newNode)
	{
		_nodes[nodeName] = newNode;
	}

	/// <summary>
	/// Sets the node to a new value. 
	/// </summary>
	/// <param name="newNode">
	/// The new node to store.
	/// </param>
	/// <param name="nodeName">
	/// The name of the node to set.
	/// </param>
	public void SetNodeAlt(Node newNode, StringName nodeName)
	{
		_nodes[nodeName] = newNode;
	}

	/// <summary>
	/// Determines whether or not the blackboard is in the specified state.
	/// </summary>
	/// <param name="state">
	/// The state to check for.
	/// </param>
	/// <returns>
	/// Whether or not the current state is the specified state.
	/// </returns>
	public bool InState(StringName state)
	{
		return _curState == state;
	}

	/// <summary>
	/// Determines whether or not the specified state is within the blackboard.
	/// </summary>
	/// <param name="state">
	/// The state to check for.
	/// </param>
	/// <returns>
	/// Whether or not the state is on the blackboard.
	/// </returns>
	public bool HasState(StringName state)
	{
		return _stateSet.Contains(state);
	}

	/// <summary>
	/// Moves the blackboard to the specified state.
	/// </summary>
	/// <param name="state">
	/// The state to change to.
	/// </param>
	/// <exception cref="System.Collections.Generic.KeyNotFoundException">
	/// Thrown if the specified state is not in the blackboard.
	/// </exception>
	public void SetState(StringName state)
	{
		if (_stateSet.Contains(state))
		{
			_curState = state;
		}
		else
		{
			throw new System.Collections.Generic.KeyNotFoundException("\"" + state + "\" is not an existing state on the blackboard.");
		}
	}
}
