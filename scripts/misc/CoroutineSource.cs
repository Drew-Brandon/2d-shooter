using System.Collections;
using System.Threading.Tasks;

/// <summary>
/// Represents a source for coroutines to run on.
/// </summary>
public class CoroutineSource
{
	private bool _isCancelled = false;
	
	/// <summary>
	/// Whether or not the current coroutines should be cancelled.
	/// If no coroutines are running, then this value will always be false.
	/// </summary>
	public bool IsCancelled { get => _isCancelled; set => _isCancelled = value; }

	private int _routineCount = 0;

	/// <summary>
	/// The number of coroutines currently running on the source.
	/// </summary>
	public int RoutineCount { get => _routineCount; }

	/// <summary>
	/// Whether or not any coroutines are running on this source.
	/// </summary>
	public bool IsRunning { get => _routineCount > 0; }

	/// <summary>
	/// Starts the specified coroutine.
	/// </summary>
	/// <param name="routine">
	/// The coroutine to run. Arguments to the function must be included.
	/// </param>
	public async void StartCoroutine(IEnumerable routine)
	{
		// If this source is blocked, do not run the coroutine.
		if (_isCancelled)
		{
			return;
		}

		_routineCount++;

		foreach (int ms in routine)
		{
			await Task.Delay(ms);

			// Check if this coroutine should still be running.
			if (_isCancelled)
			{
				break;
			}
		}

		_routineCount--;
	}

	/// <summary>
	/// Starts the specified coroutine.
	/// This coroutine is not affiliated with a source, and thus has a lot less to it.
	/// </summary>
	/// <param name="routine">
	/// The coroutine to run. Arguments to the function must be included.
	/// </param>
	public async static void StartSovereignCoroutine(IEnumerable routine)
	{
		foreach (int ms in routine)
		{
			await Task.Delay(ms);
		}
	}
}
