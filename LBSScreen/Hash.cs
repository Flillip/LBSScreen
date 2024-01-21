using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBSScreen
{
    internal static class Hash
    {
        // Super simple hashing algorithm taken from https://stackoverflow.com/a/9545731/16052290
        public static ulong CalculateHash(string input)
        {
            ulong hashedValue = 3074457345618258791ul;
            for (int i = 0; i < input.Length; i++)
            {
                hashedValue += input[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }

    }
}
