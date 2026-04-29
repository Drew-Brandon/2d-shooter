using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class Checklist : Node
{
	private int _marked = 0;

	[Export]
	private Godot.Collections.Dictionary<StringName, bool> _list;

	[Signal]
	public delegate void OnCompletedEventHandler();
	
	public override void _Ready()
	{
		foreach (KeyValuePair<StringName, bool> pair in _list)
		{
			if (pair.Value)
			{
				_marked++;
			}
		}

		if (_marked == _list.Count)
		{
			EmitSignal(SignalName.OnCompleted);
		}
	}

	public void Unmark(StringName name)
	{
		if (_list[name])
		{
			_marked--;
		}

		_list[name] = false;
	}

	public void Mark(StringName name)
	{
		if (!_list[name])
		{
			_marked++;

			if (_marked == _list.Count)
			{
				EmitSignal(SignalName.OnCompleted);
			}
		}
		
		_list[name] = true;
	}
}
