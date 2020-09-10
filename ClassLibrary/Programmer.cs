using System;

namespace ClassLibrary
{
    public class Programmer : Person, IDeepCopy
    {
        public double Exp { get; set; }
        public string Field { get; set; }

        public Programmer(string firstName = null, string secondName = null, DateTime birthdate = new DateTime(), double exp = 0, string field = null)
                : base(firstName, secondName, birthdate)
        {
            Exp = exp;
            Field = field;
        }

        public override object DeepCopy()
        {
            return new Programmer(FirstName, LastName, Birthdate, Exp, Field);
        }

        public override string ToString()
        {
            return $"This programmer is {FirstName} {LastName}, born {Birthdate}; has {Exp} years of experience in {Field}.";
        }
    }
}