using System;

namespace Pricer.Numerics;

public enum StrategyType
{
    Call,
    Put,
    CallSpread,
    Straddle,
    Butterfly
}

public static class GreeksVisualizer
{
    public static (
        double[] S,
        double[] Price,
        double[] Payoff,
        double[] Delta,
        double[] Gamma,
        double[] Theta,
        double[] Vega)
    Generate(
        StrategyType strategy,
        double r,
        double T,
        double sigma,
        double K,
        double Smin = 50,
        double Smax = 150,
        int points = 100)
    {
        var Svalues = new double[points];
        var payoff = new double[points];
        var price = new double[points];
        var delta = new double[points];
        var gamma = new double[points];
        var theta = new double[points];
        var vega = new double[points];

        const double h = 0.5;    // bump size for S finite-differences
        const double dt = 1e-4;   // bump size for theta
        const double dv = 1e-4;   // bump size for vega

        double step = (Smax - Smin) / (points - 1);

        for (int i = 0; i < points; i++)
        {
            double S = Smin + i * step;
            Svalues[i] = S;

            double V(double s, double t, double vol) => StrategyPrice(strategy, s, t, vol, r, K);

            payoff[i] = strategy switch
            {
                StrategyType.Call => Math.Max(S - K, 0),
                StrategyType.Put => Math.Max(K - S, 0),
                StrategyType.CallSpread => Math.Max(S - K, 0) - Math.Max(S - (K + 10), 0),
                StrategyType.Straddle => Math.Max(S - K, 0) + Math.Max(K - S, 0),
                StrategyType.Butterfly => Math.Max(S - (K - 10), 0) - 2 * Math.Max(S - K, 0) + Math.Max(S - (K + 10), 0),
                _ => 0
            };

            price[i] = V(S, T, sigma);
            delta[i] = (V(S + h, T, sigma) - V(S - h, T, sigma)) / (2 * h);
            gamma[i] = (V(S + h, T, sigma) - 2 * V(S, T, sigma) + V(S - h, T, sigma)) / (h * h);

            double tSafe = Math.Max(T, dt + 1e-6);
            theta[i] = -(V(S, tSafe + dt, sigma) - V(S, tSafe - dt, sigma)) / (2 * dt);
            vega[i] = (V(S, T, sigma + dv) - V(S, T, sigma - dv)) / (2 * dv);
        }

        return (Svalues, price, payoff, delta, gamma, theta, vega);
    }

    private static double StrategyPrice(
        StrategyType strategy,
        double S,
        double T,
        double sigma,
        double r,
        double K)
    {
        double Call(double k) => new BlackScholes(OptionType.Call, r, T, sigma, k, S).Price();
        double Put(double k) => new BlackScholes(OptionType.Put, r, T, sigma, k, S).Price();

        return strategy switch
        {
            StrategyType.Call => Call(K),
            StrategyType.Put => Put(K),
            StrategyType.CallSpread => Call(K) - Call(K + 10),
            StrategyType.Straddle => Call(K) + Put(K),
            StrategyType.Butterfly => Call(K - 10) - 2 * Call(K) + Call(K + 10),
            _ => 0.0
        };
    }
}