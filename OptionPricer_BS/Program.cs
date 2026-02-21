// Rune De Coninck s0220592
// 21/02/2026

// Write a function that computes the BS price of an option in C#.

// This coded calculates the option price of a vanilla call option using the Black-Scholes formula.
// The global input parameters refer to the EUR-USD market. This is to make sure that our result is correct,
// Indeed: we expect the price to be around 0.014.

using System;

namespace OptionPricer
{
    internal class Program
    {
        // Global input parameters (EUR-USD market).
        static double S0 = 1.18;        // Current spot
        static double K = 1.25;         // Strike
        static double r = 0.015;        // Risk-free rate (we can also put r = 0)
        static double sigma = 0.07;     // Volatility
        static double T = 1.0;          // Time to maturity (1 year)

        // Main method to execute the program
        static void Main(string[] args)
        {
            double callPrice = BlackScholesCall(S0, K, r, sigma, T);
            Console.WriteLine($"Call Option Price: {callPrice:F4}");
            Console.ReadLine();
        }

        // Function to calculate the Black-Scholes price of a call option
        static double BlackScholesCall(double S, double K, double r, double sigma, double T)
        {
            double d1 = (Math.Log(S / K) + (r + 0.5 * sigma * sigma) * T)
                        / (sigma * Math.Sqrt(T));

            double d2 = d1 - sigma * Math.Sqrt(T);

            return S * NormalCDF(d1)
                 - K * Math.Exp(-r * T) * NormalCDF(d2);
        }

        // Standard Normal CDF using Erf-approximation
        static double NormalCDF(double x)
        {
            return 0.5 * (1.0 + Erf(x / Math.Sqrt(2.0)));
        }

        // Implementation of the error function (Erf) using an approximation
        static double Erf(double x)
        {
            // Abramowitz and Stegun approximation
            double sign = Math.Sign(x);
            x = Math.Abs(x);

            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1)
                        * t * Math.Exp(-x * x);

            return sign * y;
        }
    }
}