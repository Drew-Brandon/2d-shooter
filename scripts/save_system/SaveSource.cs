using Godot;
using System;

public partial class SaveSource : Node
{
	private static readonly string SAVE_DIR = "user://";

	[Export]
	private string _autosaveFile = "";

	[Export]
	private StringName[] _saveGroups = null;

	private SaveResource _curSave = null;

	public void Save()
	{
		Save(_autosaveFile);
	}

	public void Save(string fileName)
	{
		string filePath = SAVE_DIR + fileName;
		_curSave = new SaveResource();
		_curSave.SaveScene(this, _saveGroups);
		ResourceSaver.Save(_curSave, filePath);
	}

	public void Load()
	{
		Load(_autosaveFile);
	}

	public void Load(string fileName)
	{
		string filePath = SAVE_DIR + fileName;

		if (ResourceLoader.Exists(filePath))
		{
			_curSave = ResourceLoader.Load(filePath, "", ResourceLoader.CacheMode.Ignore) as SaveResource;
			_curSave.Load(GetTree(), _saveGroups);
		}
	}

	public void LoadLastSave()
	{
		if (_curSave != null)
		{
			_curSave.Load(GetTree(), _saveGroups);
		}
	}
}
