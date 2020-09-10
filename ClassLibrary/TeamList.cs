using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary
{
    class TeamList
    {
        public List<Team> Contents { get; set; }

        public int MaxPubNumber
        {
            get
            {
                var researcher = (from team in Contents
                                  from person in team.Members
                                  where person is Researcher
                                  select person as Researcher).Max<Researcher>();
                              
                if (researcher != null)
                {
                    return researcher.PubNumber;
                } else
                {
                    return -1;
                }
            }
        }

        public Researcher GreatestResearcher
        {
            get
            {
                return (from team in Contents
                        from person in team.Members
                        where person is Researcher
                        select person as Researcher).Max<Researcher>();             
            }
        }

        public IEnumerable<Programmer> ProgrammersByExperience
        {
            get
            {
                return from team in Contents
                       from person in team.Members
                       where person is Programmer
                       orderby (person as Programmer).Exp ascending
                       select person as Programmer;
            }
        }

        public IEnumerable<IGrouping<double, Programmer>> ProgrammersExperienceCategories
        {
            get
            {
                return from team in Contents
                       from person in team.Members
                       where person is Programmer
                       group (person as Programmer) by (person as Programmer).Exp;
            }
        }

        public IEnumerable<Person> UniqueMultiTeammates
        {
            get
            {
                var groups = from team in Contents
                         from person in team.Members
                         group person by new
                         {
                             key1 = person.FirstName,
                             key2 = person.LastName,
                             key3 = person.Birthdate
                         } into personGroup
                       where personGroup.Count() > 1
                       select personGroup;

                return from gr in groups
                       select gr.First();

            }
        }

        public IEnumerable<string> CommonTopics
        {
            get
            {
                var topics_1 = from team in Contents
                               from person in team.Members
                               where person is Researcher
                               select (person as Researcher).SciField;

                var topics_2 = from team in Contents
                               from person in team.Members
                               where person is Programmer
                               select (person as Programmer).Field;

                return topics_1.Distinct().Intersect(topics_2.Distinct());
            }
        }

        public TeamList()
        {
            Contents = new List<Team>();
        }

        public void AddDefaults()
        {
            Team weirdos = new Team("EverMind");
            weirdos.AddDefaults();
            weirdos.AddPerson(new Researcher("Also", "Me", pubNumber: 0, sciField: "Neuroscience"));
            weirdos.AddPerson(new Programmer("Andrew", "Chernov", exp: 20, field: "Neuroscience"));
            weirdos.AddPerson(new Programmer("Aew", "Chernov", exp: 20, field: "Chemistry"));
            Contents.Add(weirdos);

            Team realistic = (Team)weirdos.DeepCopy();
            realistic.TeamName = "Justice League";
            realistic.Members[0].FirstName = "Dohn";
            realistic.Members[0].LastName = "Joe";
            realistic.Members[1].FirstName = "Niklaus";
            realistic.Members[1].LastName = "Wirth";
            realistic.Members[2].FirstName = "Marie";
            realistic.Members[2].LastName = "Curie";
            ((Programmer)realistic.Members[1]).Exp = 51;
            ((Programmer)realistic.Members[1]).Field = "Algorithms";
            ((Researcher)realistic.Members[2]).PubNumber = 10;
            ((Researcher)realistic.Members[2]).SciField = "Chemistry";
            realistic.AddPerson(new Programmer("John", "Chernov", exp: 20, field: "SOmething"));
            realistic.AddPerson(new Programmer("Ivan", "Chernov", exp: 20, field: "SOmething"));
            realistic.AddPerson(new Researcher("Johann", "Me", pubNumber: 0, sciField: "SOmething"));
            realistic.AddPerson(new Programmer("Andrew", "Chernov", exp: 20, field: "C++"));
            realistic.AddPerson(new Person("It's", "Me"));
            realistic.AddPerson(new Researcher("Also", "Me", pubNumber: 0, sciField: "Neuroscience"));

            Contents.Add(realistic);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder($"This list of teams contains {Contents.Count} teams, which are:\n");
            foreach (var team in Contents)
            {
                builder.Append(team.ToString() + "\n");
            }
            return builder.ToString();
        }
    }
}
