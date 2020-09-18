using System;
using System.Windows.Data;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;

namespace ClassLibrary
{
    [Serializable]
    public class Researcher : Person, IDataErrorInfo//, IComparable<Researcher>
    {
        public string SciField { get; set; }
        public int PubNumber { get; set; }

        public string Error { get { throw new NotImplementedException(); } }

        public string this[string propertyName]
        {
            get
            {
                string msg = null;
                switch(propertyName)
                {
                    case "FirstName":
                        if (FirstName == null || FirstName == "") { msg = "FirstName must be non-empty."; }
                        break;
                    case "LastName":
                        if (LastName == null || LastName == "") { msg = "LastName must be non-empty."; }
                        break;
                    case "Birthdate":
                        if (Birthdate.Year < 1930 || Birthdate.Year > 2000) { msg = "Birthdate is not correct."; }
                        break;
                    case "SciField":
                        if (SciField == null || SciField == "") { msg = "SciField must be non-empty."; }
                        break;
                    case "PubNumber":
                        if (PubNumber < 0) { msg = "Pubnumber can't be negative."; }
                        break;
                    default:
                        break;
                }

                return msg;
            }
        }

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
            return $"{person.LastName} {person.FirstName[0]}.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
