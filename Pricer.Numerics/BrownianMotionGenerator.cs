using System;

namespace Pricer.Numerics;

public static class BrownianMotionGenerator
{
    /*
    -------------------------------------------------------
    SCALED RANDOM WALK GENERATOR

    W^(n)(t) = (1/sqrt(n)) * sum_{j=1}^{floor(nt)} X_j

    X_j = ±1 with probability 1/2
    -------------------------------------------------------
    */

    public static double[] GenerateScaledRandomWalk(int n, int seed, double visualScale = 5.0)
    {
        var rand = new Random(seed);

        double[] path = new double[n + 1];

        for (int j = 1; j <= n; j++)
        {
            // Variable: +1 or -1 with probability 1/2.
            int Xj = rand.NextDouble() < 0.5 ? -1 : 1;

            // scaled increment (we also multiply by a constant factor to make the path more visible in the chart).
            double increment = visualScale * Xj / Math.Sqrt(n);

            path[j] = path[j - 1] + increment;
        }

        return path;
    }
}