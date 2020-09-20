using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ClassLibrary
{
    [Serializable]
    public class TeamObservable : System.Collections.ObjectModel.ObservableCollection<Person>, INotifyPropertyChanged, IDeserializationCallback, IDataErrorInfo
    {
        //---------------
        // Private fields
        //---------------
        private string name;
        private List<string> researchAreas;
        private bool changesNotSaved;
        private double researcherFraction;


        //------------------
        // Public properties
        //------------------
        public string GroupName
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged();
                ChangesNotSaved = true;
            }
        }

        public List<string> ResearchAreas => researchAreas;

        public double ResearcherFraction
        {
            get { return researcherFraction; }
        }

        public bool ChangesNotSaved {
            get { return changesNotSaved; }
            set
            {
                changesNotSaved = value;    
                NotifyPropertyChanged();
            }
        }

        // Part of INotifyPropertyChanged
        [field:NonSerializedAttribute()]
        public new event PropertyChangedEventHandler PropertyChanged;

        // Part of IDataErrorInfo
        public string Error { get { throw new NotImplementedException(); } }

        // Part of IDataErrorInfo
        public string this[string propertyName]
        {
            get
            {
                string msg = null;
                switch (propertyName)
                {
                    case "GroupName":
                        if (GroupName == null || GroupName == "") { msg = "GroupName must be non-empty."; }
                        break;
                    default:
                        break;
                }

                return msg;
            }
        }

        //-------------
        // Constructors
        //-------------
        public TeamObservable() : this("")
        { }

        public TeamObservable(string name)
        {
            GroupName = name;
            researchAreas = new List<string>()
            {
                "Physics",
                "Mathematics",
                "Computer Science",
                "Biology",
                "Chemistry",
                "Other"
            };
            ChangesNotSaved = true;
            this.CollectionChanged += this.OnCollectionChanged;
        }

        //---------------
        // Private methods
        //---------------

        // Part of INotifyPropertyChanged
        private void NotifyPropertyChanged([CallerMemberName] String propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateResearcherFraction()
        {
            double oldValue = researcherFraction;
            if (Count == 0) { researcherFraction = 0.0; }
            else
            {
                double researchersNum = (from person in Items where person is Researcher select person).Count();
                researcherFraction = researchersNum / Count;
            }
            if (researcherFraction != oldValue) { NotifyPropertyChanged("ResearcherFraction"); }
        }

        //---------------
        // Public methods
        //---------------
        public void AddPerson(params Person[] persons)
        {
            foreach (var person in persons)
            {
                Add(person);
            }
        }

        public void RemovePersonAt(int index)
        {
            if (index >= 0) { RemoveAt(index); }
        }

        public void AddDefaults()
        {
            AddPerson(new Person("Richard", "Nixon", birthdate: new DateTime(1913, 1, 9)));
            AddPerson(new Researcher("Richard", "Feynman", birthdate: new DateTime(1918, 6, 11), sciField: "Physics", pubNumber: 42));
            AddPerson(new Programmer("Edsger", "Dijkstra", birthdate: new DateTime(1930, 6, 11), exp: 40.0, field: "Computer Science"));
        }

        public void AddDefaultProgrammer()
        {
            AddPerson(new Programmer("John", "Carmack", birthdate: new DateTime(1970, 8, 20), exp: 30.0, field: "Computer Science"));
        }

        public void AddDefaultResearcher()
        {
            AddPerson(new Researcher("Freeman", "Dyson", birthdate: new DateTime(1923, 12, 15), sciField: "Physics", pubNumber: 23));
        }

        public override string ToString()
        {
            // Generate a StringBuilder
            StringBuilder result = new StringBuilder($"The group named {GroupName} has researcher fraction of {ResearcherFraction}.\n");
            
            // Add scientific areas to output
            result.AppendLine("The scientific areas developed are the following:");
            foreach (var area in researchAreas)
            {
                result.AppendLine(area);
            }
            result.AppendLine();

            // Add save's condition to output
            result.AppendLine($"Are the changes saved to the file: {!ChangesNotSaved}");
            result.AppendLine();

            // Add members to the output
            result.AppendLine("The team consists of the following people:");
            foreach (var person in Items)
            {
                result.AppendLine(person.ToString());
            }
            result.AppendLine();


            return result.ToString();
        }

        // Part of ObservableCollection
        public void OnCollectionChanged(object sender, EventArgs e)
        {
            this.ChangesNotSaved = true;
            this.UpdateResearcherFraction();
        }

        // Part of IDeserealizationCallback
        public void OnDeserialization(object sender)
        {
            this.CollectionChanged += this.OnCollectionChanged;
        }

        //---------------
        // Static methods
        //---------------
        public static bool Save(string filename, TeamObservable obj)
        {
            bool success = false; 
            FileStream fileStream = null;
            
            try
            {
                fileStream = File.Open(filename, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(fileStream, obj);
                obj.ChangesNotSaved = false;
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong during serialization: {ex.Message}");
            }
            finally
            {
                fileStream?.Close();
            }

            return success;
        }

        public static bool Load(string filename, ref TeamObservable obj)
        {
            bool success = false;
            FileStream fileStream = null;

            try
            {
                fileStream = File.Open(filename, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();

                obj = formatter.Deserialize(fileStream) as TeamObservable;
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong during deserialization: {ex.Message}");
            }
            finally
            {
                fileStream?.Close();
            }

            return success;
        }
    }
}
