using Godot;

[GlobalClass]
public partial class BTPlayer : Node
{
	[Export]
	private bool _autoStart;

	[Export]
	private BTParentNode _root;

	[Export]
	private AIBody2D _ai;
	public AIBody2D AI { get => _ai; }

	[Export]
	private AIBlackboard _bb;
	public AIBlackboard BB { get => _bb; }

	private async void Loop()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		Play();
	}

	public override void _Ready()
	{
		_root.OnNodeSuccess += Loop;
		_root.OnNodeFailure += Loop;
		_root.SetPlayer(this);

		if (_autoStart)
		{
			Play();
		}
	}

	public void Play()
	{
		_root.Enter();
	}

	public void Stop()
	{
		_root.Stop();
	}

	public void Restart()
	{
		_root.Stop();
		_root.Enter();
	}
}
