namespace UITests.Core;

/// <summary>
/// Explicit, condition-based polling waits for UI test code. This replaces
/// fixed <c>Thread.Sleep</c>/<c>Task.Delay</c> waits (see
/// spec/UITestArchitecture.md §15) with a wait that returns as soon as the
/// condition is satisfied and times out predictably otherwise.
/// </summary>
public static class WaitHelper
{
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromMilliseconds(250);

    /// <summary>
    /// Polls <paramref name="condition"/> until it returns <see langword="true"/>
    /// or <paramref name="timeout"/> elapses. Exceptions thrown by
    /// <paramref name="condition"/> (e.g. a transient "element not found" while
    /// the UI is still animating) are treated as "not yet satisfied" rather
    /// than failing the wait immediately.
    /// </summary>
    /// <returns><see langword="true"/> if the condition was satisfied before the timeout.</returns>
    public static bool WaitUntil(Func<bool> condition, TimeSpan? timeout = null, TimeSpan? pollingInterval = null)
    {
        var effectiveTimeout = timeout ?? DefaultTimeout;
        var effectiveInterval = pollingInterval ?? DefaultPollingInterval;
        var deadline = DateTime.UtcNow + effectiveTimeout;

        while (true)
        {
            if (TryEvaluate(condition))
                return true;

            if (DateTime.UtcNow >= deadline)
                return false;

            // The poll interval below is the sanctioned use of a fixed delay in
            // this codebase: it paces the polling loop itself, it is not a
            // substitute for an explicit wait condition.
            Thread.Sleep(effectiveInterval);
        }
    }

    static bool TryEvaluate(Func<bool> condition)
    {
        try
        {
            return condition();
        }
        catch
        {
            return false;
        }
    }
}
