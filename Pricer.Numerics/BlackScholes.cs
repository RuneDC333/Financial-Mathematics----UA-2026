// Rune De Coninck 24-02-2024

using MathNet.Numerics.Distributions;


namespace Pricer.Numerics;


public enum OptionType
{
    Call,

    Put
}

public class BlackScholes
{
    private OptionType option;
    private double r; // Risk-free interest rate
    private double T; // Time to maturity
    private double sigma; // Volatility
    private double K; // Strike price
    private double S; // Underlying asset price
    private double q; // Dividend yield

    public BlackScholes(
        OptionType optionType,
        double riskFreeRate,
        double timeToMaturity,
        double volatility,
        double strike,
        double underlyingPrice,
        double dividendYield = 0.0)
    {
        option = optionType;
        r = riskFreeRate;
        T = timeToMaturity;
        sigma = volatility;
        K = strike;
        S = underlyingPrice;
        q = dividendYield;
    }

    // Cumulative normal distribution
    private static double N(double x)
        => Normal.CDF(0.0, 1.0, x);

    // Normal probability density function
    private static double n(double x)
        => Normal.PDF(0.0, 1.0, x);


    // Black–Scholes price
    public double Price()
    {
        if (T <= 0)
        {
            return option == OptionType.Call
                ? Math.Max(S - K, 0)    // If true: Call payoff.
                : Math.Max(K - S, 0);   // If false: Put payoff.
        }

        if (sigma <= 0)
            return 0.0;     // No volatility means no risk, so option is worthless.

        // Define d1 and d2.
        double d1 = (Math.Log(S / K) + 
            (r - q + 0.5 * sigma * sigma) * T) / (sigma * Math.Sqrt(T));

        double d2 = d1 - sigma * Math.Sqrt(T);

        // Calculate option price based on type.
        if (option == OptionType.Call)
        {
            return S * Math.Exp(-q * T) * N(d1)
                   - K * Math.Exp(-r * T) * N(d2);
        }
        else
        {
            return K * Math.Exp(-r * T) * N(-d2)
                   - S * Math.Exp(-q * T) * N(-d1);
        }
    }

    // Greek Vega - sensitivity of option price to volatility.
    public double Vega()
    {
        if (T <= 0 || sigma <= 0)
            return 0.0;

        double d1 = (Math.Log(S / K) + (r - q + 0.5 * sigma * sigma) * T)
                    / (sigma * Math.Sqrt(T));

        return S * Math.Exp(-q * T) * Math.Sqrt(T) * n(d1);
    }

}

// Implied volatility of Black-Scholes and Bachelier models.
public static class ImpliedVolatilityCalculator
{
    // 1. Black-Scholes implied volatility.
    public static double Compute(
        OptionType optionType,
        double marketPrice,
        double interestRate,
        double timeToMaturity,
        double strike,
        double underlyingPrice,
        double initialGuess = 0.2,
        double tolerance = 1e-8,
        int maxIterations = 100)
    {
        // We calculate the implied volatility using the Newton-Raphson method.
        
        // Initial guess.
        double sigma = initialGuess;

        for (int i = 0; i < maxIterations; i++)
        {
            // Calculate the option price and Vega for the current guess of volatility.
            var bs = new BlackScholes(
                optionType,
                interestRate,
                timeToMaturity,
                sigma,
                strike,
                underlyingPrice);

            double price = bs.Price();
            double vega = bs.Vega();

            double diff = price - marketPrice;

            if (Math.Abs(diff) < tolerance)
                return sigma;

            if (vega < 1e-10)
                break;

            // Update the guess using Newton-Raphson step.
            sigma -= diff / vega;
        }

        return sigma;
    }

    // 2. Bachelier implied volatility at-the-money (ATM).
    public static double BachelierImpliedVolATM(
        double optionPrice,
        double underlyingPrice,
        double timeToMaturity,
        double interestRate)
    {
        return optionPrice / underlyingPrice
                * Math.Sqrt(2 * Math.PI / timeToMaturity);
    }
}