using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("3.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PracticeController : ControllerBase
{
    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set => _amount = value;
    }

    [HttpGet("CheckPalindromeOrNot")]
    public string CheckPalindromeOrNot(string input)
    {
        string inputCloned = input.ToLowerInvariant();
        int start = 0;
        int end = inputCloned.Length - 1;
        while (start < end)
        {
            if (inputCloned[start] != inputCloned[end])
                return $"{input} is not a palindrome.";
            start++;
            end--;
        }
        return $"{input} is a palindrome.";
    }

    // ✅ Version 1 - basic prime check
    [HttpGet("CheckPrimeNumberOrNot")]
    [MapToApiVersion("1.0")]
    public string CheckPrimeNumberOrNotV1(int input)
    {
        if (input < 2)
            return $"{input} is not a prime.";

        for (int i = 2; i * i <= input; i++)
        {
            if (input % i == 0)
                return $"{input} is not a prime.";
        }
        return $"{input} is a prime.";
    }

    // ✅ Version 2 - enhanced with better explanation
    [HttpGet("CheckPrimeNumberOrNot")]
    [MapToApiVersion("3.0")]
    public object CheckPrimeNumberOrNotV3(int input)
    {
        if (input < 2)
            return new { Number = input, IsPrime = false, Message = "Numbers less than 2 are not prime." };

        for (int i = 2; i * i <= input; i++)
        {
            if (input % i == 0)
                return new { Number = input, IsPrime = false, Message = $"{input} is divisible by {i}, hence not a prime." };
        }

        return new { Number = input, IsPrime = true, Message = $"{input} is a prime number." };
    }

    [HttpPost("MergeArray")]
    public int[] MergeArray([FromBody] ArrayRequest request)
    {
        int n1 = request.Array1.Length;
        int n2 = request.Array2.Length;
        int[] mergeArray = new int[n1 + n2];

        int i = 0, j = 0, k = 0;

        while (n1 > i && n2 > j)
        {
            if (request.Array1[i] < request.Array2[j])
            {
                mergeArray[k++] = request.Array1[i++];
            }
            else
            {
                mergeArray[k++] = request.Array2[j++];
            }
        }

        while (n1 > i) mergeArray[k++] = request.Array1[i++];
        while (n2 > j) mergeArray[k++] = request.Array2[j++];

        return mergeArray;
    }

    [HttpGet("IsArmStrongNumber")]
    public bool IsArmStrongNumber(int num)
    {
        int original = num;
        int sum = 0;
        int digits = num.ToString().Length;
        while (num > 0)
        {
            int digit = num % 10;
            sum += (int)Math.Pow(digit, digits);
            num /= 10;
        }
        return original == sum;
    }

    [HttpGet("LengthOfLongestSubstring")]
    public int LengthOfLongestSubstring(string s)
    {
        HashSet<char> set = new();
        int maxLen = 0, start = 0;

        for (int end = 0; end < s.Length; end++)
        {
            while (set.Contains(s[end]))
            {
                set.Remove(s[start]);
                start++;
            }
            set.Add(s[end]);
            maxLen = Math.Max(maxLen, end - start + 1);
        }
        return maxLen;
    }
}

public class ArrayRequest
{
    public required int[] Array1 { get; set; }
    public required int[] Array2 { get; set; }
}
