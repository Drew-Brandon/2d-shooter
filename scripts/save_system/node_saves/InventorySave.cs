using Godot;
using System;

public partial class InventorySave : NodeSave
{
	[Export]
	private BaseItemInstance[] _instances;
	
	public InventorySave()
	{ }

	public InventorySave(BaseItemInstance[] instances)
	{
		_instances = instances;
	}

	public int GetInstanceCount()
	{
		return _instances.Length;
	}

	public BaseItemInstance GetInstance(int index)
	{
		return _instances[index];
	}
}
