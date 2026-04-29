using Godot;
using Godot.Collections;
using System.Xml.Linq;

public partial class NodeReferences : Node
{
	[Export]
	private Dictionary<StringName, Node> nodes = new();

	public Node GetNode(StringName name)
	{
		if (nodes.ContainsKey(name))
		{
			return nodes[name];
		}

		return null;
	}

	public T GetNode<T>(StringName name) where T : Node
	{
		return GetNode(name) as T;
	}
}
