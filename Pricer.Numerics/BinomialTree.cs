using System;

namespace Pricer.Numerics;

// This file calculates the price of a European option using the Binomial Tree method.
// The method is based on the Cox-Ross-Rubinstein model, which is a discrete-time model for option pricing.
public static class BinomialTree
{
    public static double ComputeEuropeanOptionPrice(
        OptionType optionType,
        double S,
        double K,
        double T,
        double r,
        double sigma,
        int n)
    {
        if (n <= 0) return 0;

        // 1. Calculate time step and Cox-Ross-Rubinstein parameters
        double dt = T / n;
        double u = Math.Exp(sigma * Math.Sqrt(dt));
        double d = Math.Exp(-sigma * Math.Sqrt(dt));
        double q = (Math.Exp(r * dt) - d) / (u - d);
        double discount = Math.Exp(-r * dt);

        // 2. Array to store option values at the nodes
        // We only need an array of size n+1 to hold the values at the current time step
        double[] values = new double[n + 1];

        // 3. Initialize terminal payoffs at maturity (t = T)
        for (int i = 0; i <= n; i++)
        {
            // The price after (n-i) up moves and i down moves
            double ST = S * Math.Pow(u, n - i) * Math.Pow(d, i);

            values[i] = optionType == OptionType.Call
                ? Math.Max(ST - K, 0)
                : Math.Max(K - ST, 0);
        }

        // 4. Step backward through the tree
        for (int step = n - 1; step >= 0; step--)
        {
            for (int i = 0; i <= step; i++)
            {
                // Discounted expected value in the risk-neutral world
                values[i] = discount * (q * values[i] + (1 - q) * values[i + 1]);
            }
        }

        // The value at the root node (t = 0)
        return values[0];
    }
}