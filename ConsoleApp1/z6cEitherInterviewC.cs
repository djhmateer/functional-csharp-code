using System;
using LaYumba.Functional;
using static System.Math;

namespace ConsoleApp1.Chapter6.C
{
    using static F;

    static class Thing
    {
        internal static void Run()
        {
            var dave = new Candidate { Name = "Dave" };
            Either<Rejection, Candidate> result = InterviewEither.FirstRound(dave);
            var output = GetMessage(result);

            string GetMessage(Either<Rejection, Candidate> res) =>
                res.Match(
                    Left: r => $"Rejected due to {r.reason}", // r is our custom type Rejection
                    Right: x => $"Success {x.Name}!"
                );

            var bob = new Candidate { Name = "Bob" };
            var result2 = InterviewEither.FirstRound(bob);
            var output2 = GetMessage(result2);
        }
    }
    
    static class InterviewEither
    {
        // we have a validation pipeline whereby
        // if it fails at any point it wont go deeper
        public static Either<Rejection, Candidate> FirstRound(Candidate c)
            => Right(c) // setting Right type to be candidate
                .Bind(CheckEligibility)
                .Bind(Interview);

        static Either<Rejection, Candidate> CheckEligibility(Candidate c)
        {
            // delegate variable ie just a pointer to a function
            Func<Candidate, bool> IsEligible = x => x.Name != "Alice";
            if (IsEligible(c)) return c;
            return new Rejection("Not eligible");
        }

        static Either<Rejection, Candidate> Interview(Candidate c)
        {
            if (c.Name == "Bob") return new Rejection("Bob always fails interviews");
            return c;
        }
    }

    class Candidate
    {
        public string Name { get; set; }
    }
    class Rejection
    {
        public string reason { get; set; }

        public Rejection(string reason)
        {
            this.reason = reason;
        }
    }
}
