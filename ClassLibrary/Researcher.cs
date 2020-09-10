using System;
using System.Windows.Data;
using System.Text;

namespace ClassLibrary
{
    public class Researcher : Person//, IComparable<Researcher>
    {
        public string SciField { get; set; }
        public int PubNumber { get; set; }

        public Researcher() : this(null, null, new DateTime(), null, 0) { }

        public Researcher(string firstName = null, string secondName = null, DateTime birthdate = new DateTime(), string sciField = null, int pubNumber = 0)
                : base(firstName, secondName, birthdate)
        {
            SciField = sciField;
            PubNumber = pubNumber;
        }

        public override string ToString()
        {
            return $"This researcher is {FirstName} {LastName}, born {Birthdate}; field of scientific interest is {SciField}, with {PubNumber} publications.";
        }

        public override object DeepCopy()
        {
            return new Researcher(FirstName, LastName, Birthdate, SciField, PubNumber);
        }

        //public int CompareTo(Researcher other)
        //{
        //    if (other != null)
        //    {
        //        return PubNumber - other.PubNumber;
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Comparing a Researcher object with null object");
        //    }
        //}
    }

    [ValueConversion(typeof(Researcher), typeof(string))]
    public class ResearcherConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Researcher person = value as Researcher;
            return $"{person.LastName} {person.FirstName}.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
