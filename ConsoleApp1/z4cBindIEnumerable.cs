using System.Security.Cryptography.X509Certificates;
using LaYumba.Functional;

namespace ConsoleApp1.Chapter4.C
{
    using System.Collections.Generic;

    class PetsInNeighbourhood
    {
        // flattening nested lists with Bind
        // internal modifier is assembly scope (as opposed to private which is class scope)
        internal static void Run()
        {
            var neighbours = new[]
            {
                new {Name = "John", Pets = new Pet[] {"Fluffy", "Thor"}},
                new {Name = "Tim", Pets = new Pet[] {}},
                //new {Name = "Carl", Pets = new Pet[] {"Sybil"}},
                new {Name = "Carl", Pets = new[] {new Pet("Sybil")}},
            };

            IEnumerable<IEnumerable<Pet>> nested = neighbours.Map(n => n.Pets);
            IEnumerable<Pet> flat = neighbours.Bind(n => n.Pets);
        }

        class Neighbour
        {
            public string Name { get; set; }
            public IEnumerable<Pet> Pets { get; set; } = new Pet[] { };
        }
    }

    // this implementation is confusing (implicit operator style)
    internal class Pet
    {
        // private readonly string name;
        // generally considered better to wrap in a property
        private string Name { get; }

        //private Pet(string name) => this.Name = name;
        public Pet(string name) => this.Name = name;

        // when we pass a string to Pet
        // it knows to return a new Pet with that name
        public static implicit operator Pet(string name)
            => new Pet(name);
    }
}
