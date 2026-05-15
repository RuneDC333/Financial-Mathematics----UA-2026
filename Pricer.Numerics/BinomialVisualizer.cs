using System;
using System.Collections.Generic;

namespace Pricer.Numerics;

// This class generates data to visualize the convergence of
// the Binomial Tree method to the Black-Scholes price as the number of steps increases.

public static class BinomialVisualizer
{
    public static (List<double> steps, List<double> absoluteErrors) GenerateErrorConvergenceData(
        OptionType optionType,
        double S,
        double K,
        double T,
        double r,
        double sigma,
        int maxSteps)
    {
        // 1. Calculate the analytical Black-Scholes price
        var bs = new BlackScholes(optionType, r, T, sigma, K, S);
        double bsPrice = bs.Price();

        var stepsList = new List<double>();
        var absoluteErrors = new List<double>();

        // 2. Calculate the Binomial price and the absolute error for every n
        for (int n = 1; n <= maxSteps; n++)
        {
            stepsList.Add(n);
            double binPrice = BinomialTree.ComputeEuropeanOptionPrice(optionType, S, K, T, r, sigma, n);

            // Calculate the absolute difference between the discrete and continuous models
            double error = Math.Abs(binPrice - bsPrice);
            absoluteErrors.Add(error);
        }

        return (stepsList, absoluteErrors);
    }
}