using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PaymentBridgeController : ControllerBase
{
    // 🔌 Endpoint using Credit Card
    // GET /api/PaymentBridge/credit?amount=1000
    [HttpGet("credit")]
    public IActionResult PayByCredit(decimal amount)
    {
        Console.WriteLine($"💡 Controller hit with amount = {amount}");
        IPaymentMethod creditCard = new CreditCardPayment();
        Payment payment = new OnlinePayment(creditCard);
        payment.MakePayment(amount);

        return Ok("Credit card payment processed.");
    }

    // 🔌 Endpoint using Debit Card
    // GET /api/PaymentBridge/debit?amount=1500
    [HttpGet("debit")]
    public IActionResult PayByDebit(decimal amount)
    {
        IPaymentMethod debitCard = new DebitCardPayment();
        Payment payment = new OnlinePayment(debitCard);
        payment.MakePayment(amount);

        return Ok("Debit card payment processed.");
    }

    // 🔌 Endpoint using UPI
    // GET /api/PaymentBridge/upi?amount=700
    [HttpGet("upi")]
    public IActionResult PayByUpi(decimal amount)
    {
        IPaymentMethod upi = new UpiPayment();
        Payment payment = new OnlinePayment(upi);
        payment.MakePayment(amount);

        return Ok("UPI payment processed.");
    }

    // 🔌 Endpoint using cash
    // GET /api/PaymentBridge/cash?amount=700
    [HttpGet("cash")]
    public IActionResult PayByCash(decimal amount)
    {
        IPaymentMethod upi = new CashPayment();
        Payment payment = new OnlinePayment(upi);
        payment.MakePayment(amount);

        return Ok("Cash payment processed.");
    }

    [HttpGet("test")]
    public IActionResult Test() => Ok("Bridge controller is reachable");
}

// ✅ Bridge Interface (Implementor)
public interface IPaymentMethod
{
    void Pay(decimal amount);
}

// ✅ Concrete Implementors
public class CreditCardPayment : IPaymentMethod
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Credit card payment processed for ₹{amount}");
    }
}

public class DebitCardPayment : IPaymentMethod
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Debit card payment processed for ₹{amount}");
    }
}

public class UpiPayment : IPaymentMethod
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"UPI payment processed for ₹{amount}");
    }
}

public class CashPayment : IPaymentMethod
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Cash payment processed for ₹{amount}");
    }
}

// ✅ Abstraction
public abstract class Payment
{
    protected readonly IPaymentMethod _paymentMethod;

    protected Payment(IPaymentMethod paymentMethod)
    {
        _paymentMethod = paymentMethod;
    }

    public abstract void MakePayment(decimal amount);
}

// ✅ Refined Abstraction
public class OnlinePayment : Payment
{
    public OnlinePayment(IPaymentMethod paymentMethod) : base(paymentMethod) { }

    public override void MakePayment(decimal amount)
    {
        Console.WriteLine("Started processing online payment...");
        _paymentMethod.Pay(amount);
        Console.WriteLine("Finished processing online payment.");
    }
}

public class OfflinePayment : Payment
{
    public OfflinePayment(IPaymentMethod paymentMethod) : base(paymentMethod) { }

    public override void MakePayment(decimal amount)
    {
        Console.WriteLine("Started processing offline payment...");
        _paymentMethod.Pay(amount);
        Console.WriteLine("Finished processing offline payment.");
    }
}
