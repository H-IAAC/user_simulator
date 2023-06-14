/// <summary>
/// Interface for listeners of <see cref="GameEvent"> GameEvents </see>.
/// </summary>
public interface EventListener
{
    /// <summary>
    /// Called when the event is raised without arguments.
    /// </summary>
    /// <param name="eventName">Name of the event</param>
    public void OnEventRaised(string eventName);

    /// <summary>
    /// Called when the event is raised with argument
    /// </summary>
    /// <typeparam name="T">Type of the argument</typeparam>
    /// <param name="eventName">Name of the event</param>
    /// <param name="arg">Argument</param>
    public void OnEventRaised<T>(string eventName, T arg);
}