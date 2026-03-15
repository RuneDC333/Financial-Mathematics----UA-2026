using MathNet.Numerics.Distributions;

namespace Pricer.Numerics;

public static class DiffusiveScalingGenerator
{

    /*
    -------------------------------------------------------
    DIFFUSIVE SCALING

    X_i = Normal(0,1) * (dt)^α
    where

    dt = 1/N
    α = scaling exponent controlled by slider
    -------------------------------------------------------
    */

    public static double[] GeneratePath(
        int N,                          // total number of steps
        double alpha,
        int seed,
        double visualScale = 90)
    {
        double dt = 1.0 / N;            // time increment

        var rand = new Random(seed);

        double[] path = new double[N];

        for (int i = 1; i < N; i++)
        {
            // Gaussian increment scaled by dt^α and visual amplification factor
            double increment =
                visualScale *
                Normal.Sample(rand, 0, 1) *
                Math.Pow(dt, alpha);

            // cumulative sum to form path
            path[i] = path[i - 1] + increment;
        }

        return path;
    }
}