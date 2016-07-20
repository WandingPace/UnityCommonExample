public class EventBase
{
    public object eventValue;
    public string eventName;
    public EventBase()
    {
        eventName = this.GetType().FullName;
    }
    public EventBase(string _eventName, object ev)
    {
        eventValue = ev;
        eventName = _eventName;
    }
}
