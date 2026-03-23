using Godot;

[GlobalClass]
public partial class LabelPlus : Label
{
	[Export]
	private string _prefix = "";

	public void SetTextWithInt(int number)
	{
		Text = number.ToString();
	}

	public void SetTextWithFloat(float number)
	{
		Text = number.ToString();
	}

	public void SetTextWithPrefix(string text)
	{
		Text = _prefix + text;
	}

	public void SetTextWithPrefixWithInt(int number)
	{
		Text = _prefix + number.ToString();
	}

	public void SetTextWithPrefixWithFloat(float number)
	{
		Text = _prefix + number.ToString();
	}
}
