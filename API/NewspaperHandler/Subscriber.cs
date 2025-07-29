namespace API.Services;
public class Subscriber
{
    public void Subscriber1(string message)
    {
        System.Console.WriteLine("Subscriber 1: " + message);
    }
    public void Subscriber2(string message)
    {
        System.Console.WriteLine("Subscriber 2: " + message);
    }
}