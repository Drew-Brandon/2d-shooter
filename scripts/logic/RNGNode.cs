using Godot;
using System;

[GlobalClass]
public partial class RNGNode : Node
{
	[Signal]
	public delegate void OnRandIntEventHandler(int num);

	[Signal]
	public delegate void OnRandFloatEventHandler(float num);

	public void RandInt(int start, int end)
	{
		EmitSignal(SignalName.OnRandInt, GD.RandRange(start, end));
	}

	public void RandFloat(float start, float end)
	{
		EmitSignal(SignalName.OnRandFloat, RandUtils.RandRangef(start, end));
	}
}
