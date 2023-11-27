using System;
using System.Numerics;

class DiffieHellman
{
    static Random random = new Random();

    static BigInteger GeneratePrime(int bitLength)
    {
        while (true)
        {
            BigInteger potentialPrime = GenerateRandomBigInteger(bitLength);

            if (IsProbablyPrime(potentialPrime, 5))
                return potentialPrime;
        }
    }

    static BigInteger GenerateRandomBigInteger(int bitLength)
    {
        byte[] bytes = new byte[bitLength / 8];
        random.NextBytes(bytes);

        return new BigInteger(bytes);
    }

    static bool IsProbablyPrime(BigInteger n, int k)
    {
        if (n <= 1 || n == 4)
            return false;
        if (n <= 3)
            return true;

        BigInteger d = n - 1;
        while (d % 2 == 0)
            d /= 2;

        for (int i = 0; i < k; i++)
        {
            if (!WitnessTest(GenerateRandomBigInteger(n.ToByteArray().Length - 1), n, d))
                return false;
        }

        return true;
    }

    static bool WitnessTest(BigInteger a, BigInteger n, BigInteger d)
    {
        BigInteger x = BigInteger.ModPow(a, d, n);
        if (x == 1 || x == n - 1)
            return true;

        while (d != n - 1)
        {
            x = BigInteger.ModPow(x, 2, n);
            d *= 2;

            if (x == 1)
                return false;
            if (x == n - 1)
                return true;
        }

        return false;
    }

    static BigInteger GenerateGenerator(BigInteger p)
    {
        BigInteger g;
        do
        {
            g = GenerateRandomBigInteger(p.ToByteArray().Length - 1);
        } while (g < 2 || g >= p - 1 || BigInteger.ModPow(g, 2, p) == 1 || BigInteger.ModPow(g, (p - 1) / 2, p) == 1);

        return g;
    }

    static void Main()
    {
        int bitLength = 256;
        BigInteger p = GeneratePrime(bitLength);
        Console.WriteLine("Generated prime p: " + p);

        BigInteger g = GenerateGenerator(p);
        Console.WriteLine("Generated generator g: " + g);

        BigInteger secretKeyA = GenerateRandomBigInteger(bitLength);
        BigInteger secretKeyB = GenerateRandomBigInteger(bitLength);

        if (g < 0 || secretKeyA < 0 || p <= 0)
        {
            Console.WriteLine("Invalid values for g, secretKeyA, or p.");
            return;
        }

        BigInteger publicKeyA = BigInteger.ModPow(g, secretKeyA, p);
        BigInteger publicKeyB = BigInteger.ModPow(g, secretKeyB, p);

        BigInteger secretKeySharedA = BigInteger.ModPow(publicKeyB, secretKeyA, p);
        BigInteger secretKeySharedB = BigInteger.ModPow(publicKeyA, secretKeyB, p);

        Console.WriteLine("As public key: " + secretKeyA);
        Console.WriteLine("B public key: " + secretKeyB);
        Console.WriteLine("Shared secret key (A): " + secretKeySharedA);
        Console.WriteLine("Shared secret key (B): " + secretKeySharedB);
    }
}
