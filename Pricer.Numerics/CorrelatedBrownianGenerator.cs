using MathNet.Numerics.Distributions;

namespace Pricer.Numerics;

public static class CorrelatedBrownianGenerator
{

    /*
    -------------------------------------------------------
    GENERATE TWO CORRELATED BROWNIAN MOTIONS

    W3 = ρ W1 + sqrt(1-ρ²) W2
    where W1 and W2 are independent Brownian motions.
    Now W1 and W3 are correlated with ρ as the correlation coefficient.
    -------------------------------------------------------
    */

    public static (double[], double[]) Generate(
        int N,
        double rho,
        int seed,
        double visualScale = 5)
    {
        var rand = new Random(seed);

        double dt = 1.0 / N;

        double[] W1 = new double[N];
        double[] W3 = new double[N];

        for (int i = 1; i < N; i++)
        {
            double dW1 = visualScale * Normal.Sample(rand, 0, Math.Sqrt(dt));
            double dW2 = visualScale * Normal.Sample(rand, 0, Math.Sqrt(dt));

            double dW3 = rho * dW1 + Math.Sqrt(1 - rho * rho) * dW2;

            W1[i] = W1[i - 1] + dW1;
            W3[i] = W3[i - 1] + dW3;
        }

        return (W1, W3);
    }
}