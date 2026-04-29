using Godot;
using System;

[GlobalClass]
public partial class InventoryUIStatus : Resource
{
	private bool _isDragging = false;
	public bool IsDragging { get => _isDragging; set => _isDragging = value; }
}
