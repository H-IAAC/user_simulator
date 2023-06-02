public interface EventListener
{
    public void OnEventRaised(string eventName);

    public void OnEventRaised<T>(string eventName, T arg);
}