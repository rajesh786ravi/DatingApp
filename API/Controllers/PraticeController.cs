using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PracticeController() : ControllerBase
{
    private decimal _amount;
    public decimal Amount
    {
        get
        {
            return _amount;
        }
        set
        {
            _amount = value;

        }
    }
    [HttpGet("CheckPalindromeOrNot")]
    public string CheckPalindromeOrNot(string input)
    {
        string inputCloned = input;
        inputCloned = inputCloned.ToLower();
        int start = 0;
        int end = input.Length - 1;
        while (start < end)
        {
            if (inputCloned[start] != inputCloned[end]) return $"{input} is not a palindrome.";
            start++;
            end--;
        }
        return $"{input} is a palindrome.";
    }

    [HttpGet("CheckPrimeNumberOrNot")]
    public string CheckPrimeNumberOrNot(int input)
    {
        if (input < 2)
        {
            return $"{input} is not a prime.";
        }
        for (int i = 2; i * i <= input; i++)
        {
            if (input % i == 0)
            {
                return $"{input} is not a prime.";
            }
        }
        return $"{input} is a prime.";
    }

    [HttpPost("MergeArray")]
    public int[] MergeArray([FromBody] ArrayRequest request)
    {
        // Step 1: Get the lengths of both input arrays
        int n1 = request.Array1.Length;
        int n2 = request.Array2.Length;

        // Step 2: Create a new array to store the merged result
        int[] mergeArray = new int[n1 + n2];

        // Step 3: Initialize pointers for array1 (i), array2 (j), and merged array (k)
        int i = 0;
        int j = 0;
        int k = 0;

        // Step 4: Traverse both arrays and compare elements
        while (n1 > i && n2 > j)
        {
            // If current element of array1 is smaller, add it to mergeArray
            if (request.Array1[i] < request.Array2[j])
            {
                mergeArray[k] = request.Array1[i];
                i++; // Move pointer in array1
            }
            else
            {
                // Otherwise, add current element of array2 to mergeArray
                mergeArray[k] = request.Array2[j];
                j++; // Move pointer in array2
            }
            k++; // Move pointer in merged array
        }
        // Array1 = [1, 3, 5]
        // Array2 = [2, 4, 6]
        // All comparisons happen in the main loop:
        // 1 vs 2 → take 1
        // 3 vs 2 → take 2
        // 3 vs 4 → take 3
        // 5 vs 4 → take 4
        // 5 vs 6 → take 5
        // -- now i == 3
        // j == 2 → still 6 remaining

        // Step 5: If any elements remain in array1, add them
        while (n1 > i)
        {
            mergeArray[k++] = request.Array1[i++];
        }

        // Step 6: If any elements remain in array2, add them
        while (n2 > j)
        {
            mergeArray[k++] = request.Array2[j++];
        }

        // Step 7: Return the fully merged and sorted array
        return mergeArray;
    }
    public string ReverseAString(string input)
    {
        string result = string.Empty;
        for (int i = input.Length - 1; i >= 0; i--)
        {
            result += input[i];
        }
        return result;
    }
    public static void FindSecondLargestNumber(int[] numbers)
    {
        // Check if array has at least two elements
        if (numbers.Length < 2)
        {
            Console.WriteLine("Array must have at least 2 numbers.");
            return;
        }

        // Initialize both largest and secondLargest with smallest possible value
        int largest = int.MinValue;
        int secondLargest = int.MinValue;

        // Loop through each number in the array
        foreach (int num in numbers)
        {
            // 🔹 Case 1: If current number is bigger than current largest
            if (num > largest)
            {
                // Before updating largest, store it as secondLargest
                secondLargest = largest;

                // Now update the largest
                largest = num;
            }
            // 🔹 Case 2: If current number is not the largest,
            // but still greater than current secondLargest
            else if (num > secondLargest && num != largest)
            {
                // Then update secondLargest
                secondLargest = num;
            }
            // 🔸 Other cases: Ignore if num is duplicate of largest or too small
        }

        // Check if we found a valid secondLargest
        if (secondLargest == int.MinValue)
        {
            Console.WriteLine("No second largest number found (all numbers might be the same).");
        }
        else
        {
            Console.WriteLine("Second largest number is: " + secondLargest);
        }
    }

    [TypeFilter(typeof(CustomResourceFilter))]
    [HttpGet("FibonacciSeries")]
    public string FibonacciSeries(int input)
    {
        StringBuilder sb = new();
        int first = 0;
        int second = 1;
        for (int i = 0; i < input; i++)
        {
            sb.Append(first);
            if (i < input - 1) sb.Append(',');
            int next = first + second;
            first = second;
            second = next;
        }
        return sb.ToString();
    }

    [HttpGet("FactorialOfNumber")]
    public int FactorialOfNumber(int input)
    {
        if (input == 0 || input == 1) return 1;
        else return input * FactorialOfNumber(input - 1);
    }

    #region IsArmstrong

    // Main method to check if a number is Armstrong or not
    [HttpGet("IsArmstrong")]
    public bool IsArmstrong(int number)
    {
        // Step 1: Count how many digits are in the number (e.g., 153 → 3 digits)
        int digits = CountDigits(number);

        // Step 2: Compute the sum of each digit raised to the power of 'digits'
        int sum = SumOfPowers(number, digits);

        // Step 3: If the sum of powered digits equals the original number → Armstrong!
        return sum == number;
    }
    // Recursively counts the number of digits in a number
    private static int CountDigits(int number)
    {
        // Base case: when number becomes 0, stop recursion
        if (number == 0)
            return 0;

        // Recursive case: divide by 10 to remove one digit,
        // and add 1 to count this digit
        return 1 + CountDigits(number / 10);
    }
    // Recursively calculates the sum of each digit raised to the power of 'power'
    private static int SumOfPowers(int number, int power)
    {
        // Base case: if number is 0, return 0 (no more digits)
        if (number == 0)
            return 0;

        // Get last digit of the number
        int digit = number % 10;

        // Raise the digit to the given power using recursive Power() method
        // and add the result of the remaining digits
        return Power(digit, power) + SumOfPowers(number / 10, power);
    }
    // Recursively computes baseNum raised to the power of exp
    private static int Power(int baseNum, int exp)
    {
        // Base case: any number raised to power 0 is 1
        if (exp == 0)
            return 1;

        // Recursive case: baseNum^exp = baseNum * baseNum^(exp - 1)
        return baseNum * Power(baseNum, exp - 1);
    }
    #endregion IsArmstrong
}

public class ArrayRequest
{
    public required int[] Array1 { get; set; }
    public required int[] Array2 { get; set; }
}


