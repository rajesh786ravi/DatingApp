using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PracticeController() : ControllerBase
{
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
        int n1 = request.Array1.Length;
        int n2 = request.Array2.Length;
        int[] mergeArray = new int[n1 + n2];
        int i = 0;
        int j = 0;
        int k = 0;
        while (n1 > i && n2 > j)
        {
            if (request.Array1[i] < request.Array2[j])
            {
                mergeArray[k] = request.Array1[i];
                i++;
            }
            else
            {
                mergeArray[k] = request.Array2[j];
                j++;
            }
            k++;
        }
        while (n1 > i)
        {
            mergeArray[k++] = request.Array1[i++];
        }
        while (n2 > j)
        {
            mergeArray[k++] = request.Array2[j++];
        }
        return mergeArray;
    }
}

public class ArrayRequest
{
    public required int[] Array1 { get; set; }
    public required int[] Array2 { get; set; }
}


