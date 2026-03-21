// This file has the numerics engine to visualize the Black-Scholes price curve against the underlying asset price S.

using System;
using System.Collections.Generic;

namespace Pricer.Numerics;

public static class BlackScholesVisualizer
{
    /*
    -------------------------------------------------------
    GENERATE PRICE CURVE VS UNDERLYING S
    -------------------------------------------------------
    */

    public static (double[] S, double[] Price, double[] Payoff)
        GeneratePriceCurve(
            OptionType optionType,
            double r,
            double T,
            double sigma,
            double K,
            double Smin = 50,
            double Smax = 150,
            int points = 100)
    {
        double[] Svalues = new double[points];
        double[] prices = new double[points];
        double[] payoff = new double[points];

        double step = (Smax - Smin) / (points - 1);

        for (int i = 0; i < points; i++)
        {
            double S = Smin + i * step;

            var bs = new BlackScholes(
                optionType,
                r,
                T,
                sigma,
                K,
                S);

            Svalues[i] = S;
            prices[i] = bs.Price();

            payoff[i] = optionType == OptionType.Call
                ? Math.Max(S - K, 0)
                : Math.Max(K - S, 0);
        }

        return (Svalues, prices, payoff);
    }
}