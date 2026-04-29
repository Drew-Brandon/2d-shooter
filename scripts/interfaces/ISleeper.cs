/// <summary>
/// Adds the ability to make an object sleep and be awoken.
/// This is similar to disabling and enabling something.
/// </summary>
public partial interface ISleeper
{
	/// <summary>
	/// Puts the object to sleep.
	/// </summary>
	public void Sleep();

	/// <summary>
	/// Awakes the object.
	/// </summary>
	public void Awake();
}
