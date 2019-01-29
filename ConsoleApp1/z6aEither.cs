using LaYumba.Functional;
using static System.Math;

namespace ConsoleApp1.Chapter6.A
{
    using static F;

    static class Thing
    {
        internal static void Run()
        {
            //Either.Right<int> a = Right(12);
            //Either.Left<string> b = Left("oops");

            var a = Calc(3, 0); // y cannot be negative
            var b = Calc(-3, 3); // x/y cannot be negative
            var c = Calc(8, 2); // 8/2==4, and Sqrt = 2
        }

        // result will be a string (error message) or a double (result of computation)
        static Either<string, double> Calc(double x, double y)
        {
            if (y == 0)
                return "y cannot be 0";

            if (x != 0 && Sign(x) != Sign(y))
                return "x / y cannot be negative";

            return Sqrt(x / y);
        }
    }
}
