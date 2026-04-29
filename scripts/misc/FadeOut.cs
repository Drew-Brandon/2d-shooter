using Godot;
using System;

public partial class FadeOut : Node2D
{
	private bool _isFading = false;

	[Export]
	private bool _autoStart = false;

	private float _fadeProgress = 0f;

	[Export]
	private float _fadeTime = 2f;

	public void SetFade(bool isFading)
	{
		_isFading = isFading;
	}

	public override void _Ready()
	{
		_isFading = _autoStart;
	}

	public override void _Process(double delta)
	{
		float deltaF = (float)delta;

		if (_isFading)
		{
			_fadeProgress += deltaF;
			Color curCol = Modulate;
			curCol.A = 1f - _fadeProgress / _fadeTime;
			Modulate = curCol;

			if (_fadeProgress >= _fadeTime)
			{
				QueueFree();
			}
		}
	}
}
