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

	public void SetTextWithCeilFloat(float number)
	{
		Text = Mathf.CeilToInt(number).ToString();
	}

	public void SetTextWithPrefix(string text)
	{
		Text = _prefix + text;
	}

	public void SetTextWithPrefixAndInt(int number)
	{
		Text = _prefix + number.ToString();
	}

	public void SetTextWithPrefixAndFloat(float number)
	{
		Text = _prefix + number.ToString();
	}

	public void SetTextWithPrefixAndCeilFloat(float number)
	{
		Text = _prefix + Mathf.CeilToInt(number).ToString();
	}
}
