using Godot;
using Godot.Collections;

public partial class SaveResource : Resource
{
	[Export]
	private string _scenePath = "";

	[Export]
	private Dictionary<NodePath, NodeSave> _savedNodes = null;

	protected void SaveGroup(SceneTree tree, StringName group)
	{
		Array<Node> nodes = tree.GetNodesInGroup(group);

		for (int i = 0; i < nodes.Count; i++)
		{
			ISaveable saveable = nodes[i] as ISaveable;

			if (saveable != null)
			{
				_savedNodes.Add(nodes[i].GetPath(), saveable.GetSave());
			}
		}
	}

	protected void LoadGroup(SceneTree tree, StringName group)
	{
		Array<Node> nodes = tree.GetNodesInGroup(group);

		for (int i = 0; i < nodes.Count; i++)
		{
			ISaveable loadable = nodes[i] as ISaveable;
			NodePath nodePath = nodes[i].GetPath();
			GD.Print(nodes[i].Name + ": " + nodePath + "," + loadable);

			if (loadable != null && _savedNodes.ContainsKey(nodePath))
			{
				loadable.LoadSave(_savedNodes[nodePath]);
			}
		}
	}

	public void SaveScene(Node node, StringName[] groups)
	{
		_savedNodes = new Dictionary<NodePath, NodeSave>();

		SceneTree tree = node.GetTree();
		_scenePath = node.Owner.SceneFilePath;

		for (int i = 0; i < groups.Length; i++)
		{
			SaveGroup(tree, groups[i]);
		}
	}

	public async void Load(SceneTree tree, StringName[] groups)
	{
		tree.ChangeSceneToFile(_scenePath);
		await ToSignal(tree, SceneTree.SignalName.SceneChanged);
		
		for (int i = 0; i < groups.Length; i++)
		{
			LoadGroup(tree, groups[i]);
		}
	}
}
