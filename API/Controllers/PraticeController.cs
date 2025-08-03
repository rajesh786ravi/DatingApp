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

    [HttpGet("IsArmStrongNumber")]
    public bool IsArmStrongNumber(int num)
    {
        int original = num;
        int sum = 0;
        int digits = (int)num.ToString().Length;
        while (num > 0)
        {
            int digit = num % 10;
            sum += (int)Math.Pow(digit, digits);
            num /= 10;
        }
        return original == sum;
    }
}

public class ArrayRequest
{
    public required int[] Array1 { get; set; }
    public required int[] Array2 { get; set; }
}