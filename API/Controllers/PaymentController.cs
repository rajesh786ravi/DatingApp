using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // ✅ Step 1: Declare the delegate type
    // This allows any method that takes decimal and returns void to be plugged in
    public delegate void PaymentDelegate(decimal amount);
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController(PaymentProcessor processor, PaymentMethods methods) : ControllerBase
    {
        // ✅ Step 2: Create processor and method service instances
        // These simulate business logic layers
        private readonly PaymentProcessor _paymentProcessor = processor;
        private readonly PaymentMethods _paymentMethods = methods;

        // ✅ Step 3: Endpoint using CreditCard payment via delegate
        // Usage: GET /api/payment/credit?amount=1000
        [HttpGet("credit")]
        public IActionResult PayByCredit(decimal amount)
        {
            // Bind the delegate to the method
            PaymentDelegate creditCard = _paymentMethods.CreditCardPayment;

            // Pass the delegate to the processor — closed for modification
            _paymentProcessor.ProcessPayment(amount, creditCard);

            return Ok("Credit card payment processed.");
        }

        // ✅ Step 4: Endpoint using DebitCard payment via delegate
        // Usage: GET /api/payment/debit?amount=1500
        [HttpGet("debit")]
        public IActionResult PayByDebit(decimal amount)
        {
            PaymentDelegate debitCard = _paymentMethods.DebitCardPayment;
            _paymentProcessor.ProcessPayment(amount, debitCard);

            return Ok("Debit card payment processed.");
        }
    }

    // ✅ Processor class — generic logic to execute any payment method
    // Follows Open/Closed Principle: doesn't care how the payment is done
    public class PaymentProcessor
    {
        public void ProcessPayment(decimal amount, PaymentDelegate paymentMethod)
        {
            Console.WriteLine("Started processing the payment...");

            // 🔁 Executes the assigned method via delegate
            paymentMethod(amount); // Delegate call here

            Console.WriteLine("Finished processing.");
        }
    }

    // ✅ Different payment implementations
    // Easy to extend by just adding a new method — open for extension
    public class PaymentMethods
    {
        public void CreditCardPayment(decimal amount)
        {
            Console.WriteLine($"Credit card payment processed for ₹{amount}");
        }

        public void DebitCardPayment(decimal amount)
        {
            Console.WriteLine($"Debit card payment processed for ₹{amount}");
        }

        // 🚀 New methods can be added here without touching the processor
        public void UpiPayment(decimal amount)
        {
            Console.WriteLine($"UPI payment processed for ₹{amount}");
        }
    }
}
