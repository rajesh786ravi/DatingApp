public class Publisher
{
    public delegate void MyNewspaperPublish(string message);
    public event MyNewspaperPublish? OnPublish;
    public void Publish(string message)
    {
        OnPublish?.Invoke(message);
    }
}