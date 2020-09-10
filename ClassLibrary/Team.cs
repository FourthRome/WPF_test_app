using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    class Team : IDeepCopy
    {
        public string TeamName { get; set; }
        public List<Person> Members { get; set; }

        public Team(string teamName = null)
        {
            TeamName = teamName;
            Members = new List<Person>();
        }

        public void AddPerson(params Person[] persons)
        {
            foreach (var pers in persons)
            {
                bool newMember = true;
                foreach (var member in Members)
                {
                    if ((member as Person) == (pers as Person))
                    {
                        newMember = false;
                        break;
                    }
                }

                if (newMember)
                {
                    Members.Add((Person)pers.DeepCopy());
                }
            }
        }

        public void AddDefaults()
        {
            this.AddPerson(
                new Person("John", "Doe"),
                new Programmer("Edsger", "Dijkstra", exp: 51, field: "Computer Science"),
                new Researcher("Richard", "Feynman", sciField: "Quantum Physics", pubNumber: 42)
            );
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder($"Team {TeamName} has following members:\n");
            foreach (var member in Members)
            {
                result.Append($"{member.ToString()}\n");
            }
            return result.ToString();
        }

        public virtual object DeepCopy()
        {
            Team result = new Team(TeamName);
            result.AddPerson(Members.ToArray());
            return result;
        }

        public static bool IsProgrammer(Person ps)
        {
            return ps is Programmer;
        }

        public IEnumerable<Person> Subset(Predicate<Person> Filter)
        {
            foreach (var member in Members)
            {
                if (Filter(member))
                {
                    yield return member;
                }
            }
        }
    }
}
