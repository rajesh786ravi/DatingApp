using Microsoft.AspNetCore.Mvc;
// Using Events + Delegates
// Step Description
// 1	PaymentDelegate_V1 and PaymentCompletedEventHandler declared
// 2	Event declared in PaymentProcessor_V1 using event keyword
// 3	Event is raised (Invoke) after payment completes
// 4	Event is subscribed to in the controller constructor
// 5	When payment completes, OnPaymentCompleted is executed
namespace API.Controllers
{
    public delegate void PaymentDelegate_V1(decimal amount);
    // Step 1: Declare delegates
    public delegate void PaymentCompletedEventHandler(string message);

    [ApiController]
    [Route("api/[controller]")]
    public class PaymentEventController : ControllerBase
    {
        private readonly PaymentProcessor_V1 _paymentProcessor = new();
        private readonly PaymentMethods_V1 _paymentMethods = new();

        public PaymentEventController()
        {
            // Step 4: Subscribe to the event
            _paymentProcessor.PaymentCompleted += OnPaymentCompleted;
        }

        // Step 5: Event handler
        private void OnPaymentCompleted(string message)
        {
            Console.WriteLine("Event Received: " + message);
            // You can log to file, audit, or notify here
        }

        [HttpGet("credit")]
        public IActionResult PayByCredit(decimal amount)
        {
            PaymentDelegate_V1 creditCard = _paymentMethods.CreditCardPayment;
            _paymentProcessor.ProcessPayment(amount, creditCard, "Credit Card");
            return Ok("Credit card payment processed.");
        }

        [HttpGet("debit")]
        public IActionResult PayByDebit(decimal amount)
        {
            PaymentDelegate_V1 debitCard = _paymentMethods.DebitCardPayment;
            _paymentProcessor.ProcessPayment(amount, debitCard, "Debit Card");
            return Ok("Debit card payment processed.");
        }
    }

    public class PaymentProcessor_V1
    {
        // Step 2: Declare event
        public event PaymentCompletedEventHandler? PaymentCompleted;

        public void ProcessPayment(decimal amount, PaymentDelegate_V1 paymentMethod, string methodType)
        {
            Console.WriteLine("Processing payment...");
            paymentMethod(amount);

            // Step 3: Trigger event after payment
            OnPaymentCompleted($"{methodType} payment of ₹{amount} completed.");
        }

        protected virtual void OnPaymentCompleted(string message)
        {
            PaymentCompleted?.Invoke(message);
        }
    }

    public class PaymentMethods_V1
    {
        public void CreditCardPayment(decimal amount)
        {
            Console.WriteLine($"Credit card payment of ₹{amount} processed.");
        }

        public void DebitCardPayment(decimal amount)
        {
            Console.WriteLine($"Debit card payment of ₹{amount} processed.");
        }

        public void UpiPayment(decimal amount)
        {
            Console.WriteLine($"UPI payment of ₹{amount} processed.");
        }
    }
}

