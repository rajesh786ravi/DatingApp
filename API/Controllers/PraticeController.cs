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
    public bool CheckPalindromeOrNot(string input)
    {
        input = input.ToLower();
        int start = 0;
        int end = input.Length - 1;
        while (start < end)
        {
            if (input[start] != input[end]) return false;
            start++;
            end--;
        }
        return true;
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


