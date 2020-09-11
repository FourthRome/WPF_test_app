using System;
using System.Text;
using System.Windows.Data;

namespace ClassLibrary
{
    [Serializable]
    public class Person : IDeepCopy
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }

        public Person(string firstName = null, string secondName = null, DateTime birthdate = new DateTime())
        {
            FirstName = firstName;
            LastName = secondName;
            Birthdate = birthdate;
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj as Person != null)
        //    {
        //        Person other = (Person)obj;
        //        return (FirstName == other.FirstName) && (LastName == other.LastName) && (Birthdate == other.Birthdate);
        //    }
        //    return false;
        //}

        //public override int GetHashCode()
        //{
        //    return (FirstName + LastName + Birthdate.ToString()).GetHashCode();
        //}

        public override string ToString()
        {
            return $"This person is {FirstName} {LastName}, born {Birthdate}.";
        }

        public virtual object DeepCopy()
        {
            return new Person(FirstName, LastName, Birthdate);
        }

        public bool IsProgrammer()
        {
            if (this as Programmer != null) return true;
            else return false;
        }

        public bool IsResearcher()
        {
            if (this as Researcher != null) return true;
            else return false;
        }

        //public static bool operator ==(Person a, Person b)
        //{
        //    if (a is null || b is null)
        //    {
        //        if (a is null && b is null)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    return a.Equals(b);
        //}

        //public static bool operator !=(Person a, Person b)
        //{
        //    return !(a == b);
        //}
    }

    [ValueConversion(typeof(Person), typeof(string))]
    public class PersonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Person person = value as Person;
            StringBuilder result = new StringBuilder($"{person.FirstName} {person.LastName}");
            if (person.IsResearcher()) result.Append(", researcher");
            if (person.IsProgrammer()) result.Append(", programmer");
            result.Append(", person.");
            return result.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

}

