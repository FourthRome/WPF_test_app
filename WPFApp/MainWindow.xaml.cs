using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassLibrary;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    

    public partial class MainWindow : Window
    {
        private TeamObservable team;
        private Researcher researcherStub;

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            team = this.FindResource("team") as TeamObservable;
            team.AddDefaults();
            team.AddDefaultResearcher();

            Binding bd1 = new Binding();
            bd1.Source = researcherStub;
            bd1.Path = new PropertyPath("FirstName");
            bd1.Mode = BindingMode.OneWayToSource;
            newFirstNameTextBox.SetBinding(TextBox.TextProperty, bd1);

            Binding bd2 = new Binding();
            bd2.Source = researcherStub;
            bd2.Path = new PropertyPath("LastName");
            bd2.Mode = BindingMode.OneWayToSource;
            newLastNameTextBox.SetBinding(TextBox.TextProperty, bd2);

            Binding bd3 = new Binding();
            bd3.Source = researcherStub;
            bd3.Path = new PropertyPath("Birthdate");
            bd3.Mode = BindingMode.OneWayToSource;
            newBirthdateDatePicker.SetBinding(DatePicker.SelectedDateProperty, bd3);

            Binding bd4 = new Binding();
            bd4.Source = researcherStub;
            bd4.Path = new PropertyPath("SciField");
            bd4.Mode = BindingMode.OneWayToSource;
            newSciFieldComboBox.SetBinding(ComboBox.SelectedValueProperty, bd4);

            Binding bd5 = new Binding();
            bd5.Source = researcherStub;
            bd5.Path = new PropertyPath("PubNumber");
            bd5.Mode = BindingMode.OneWayToSource;
            bd5.Converter = new PubNumberConverter();
            bd5.ValidatesOnExceptions = true;
            newPubNumberTextBox.SetBinding(TextBox.TextProperty, bd5);

            //researcherStub = this.FindResource("key_ResearcherStub") as Researcher;
        }

        public MainWindow()
        {
            researcherStub = new Researcher();
            InitializeComponent();
        }

        private void FilterResearchers(object source, FilterEventArgs args)
        {
            if (args.Item as Researcher != null) args.Accepted = true;
            else args.Accepted = false;
        }

        private void DontUseDataTemplate(object sender, RoutedEventArgs e)
        {
            teamObservableListBox.ItemTemplate = null;
        }

        private void UseDataTemplate(object sender, RoutedEventArgs e)
        {
            DataTemplate dataTemplate = this.TryFindResource("key_PersonListDataTemplate") as DataTemplate;

            if (dataTemplate != null) teamObservableListBox.ItemTemplate = dataTemplate;
        }

        private void AddCustomResearcher(object sender, RoutedEventArgs e)
        {
            Researcher newResearcher = researcherStub.DeepCopy() as Researcher;

            team.Add(newResearcher);
        }

        private void AddDefaultResearcher(object sender, RoutedEventArgs e)
        {
            team.AddDefaultResearcher();
        }

        private void AddDefaultProgrammer(object sender, RoutedEventArgs e)
        {
            team.AddDefaultProgrammer();
        }

        private void AddDefaults(object sender, RoutedEventArgs e)
        {
            team.AddDefaults();
        }

        private void OnClickSave(object sender, RoutedEventArgs e)
        {
            SaveCollection();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (ProceedWithCollectionReplacement() == false) { e.Cancel = true; }
        }

        private void OnClickNew(object sender, RoutedEventArgs e)
        {
            if (ProceedWithCollectionReplacement())
            {
                this.team = new TeamObservable();
                this.DataContext = this.team;

                this.researcherStub.FirstName = null;
                this.researcherStub.LastName = null;
                this.researcherStub.Birthdate = new DateTime();
                this.researcherStub.SciField = null;
                this.researcherStub.PubNumber = 0;
            }
        }

        private void OnClickRemove(object sender, RoutedEventArgs e)
        {
            team.RemoveAt(this.teamObservableListBox.SelectedIndex);
        }

        private bool SaveCollection()
        {
            //BindingOperations.GetBindingExpression(groupNameTextBox, TextBox.TextProperty).UpdateSource();

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.Filter = "TeamObservable serialized object(*teamobservable)|*.teamobservable|All(*.*)|*.*";
            dlg.FilterIndex = 0;
            dlg.OverwritePrompt = true;

            if (dlg.ShowDialog() == true)
            {
                team.ChangesNotSaved = false;
                TeamObservable.Save(dlg.FileName, this.team);
                // Here is no potential exception, but the file can be not saved properly.
                // Need a messagebox here.
                return true;
            }
            else { return false; }
        }

        private bool ProceedWithCollectionReplacement()
        {
            if (team.ChangesNotSaved)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show(
                    messageBoxText:"Do you want to save the changes into the current team? Your changes will be lost if you don't save them.",
                    caption: "TeamObservable Editor",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning,
                    MessageBoxResult.Yes);

                if (result == MessageBoxResult.Cancel) { return false; }
                else if (result == MessageBoxResult.Yes) { return this.SaveCollection(); }
            }
            return true;
        }

        private void OpenCollection(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "TeamObservable serialized object (*teamobservable)|*.teamobservable|All(*.*)|*.*";
            dlg.FilterIndex = 0;
            dlg.CheckFileExists = true;

            if (dlg.ShowDialog() == true)
            {
                if (ProceedWithCollectionReplacement())
                {
                    TeamObservable.Load(dlg.FileName, ref team);
                    // Here is no potential exception, but the file could be not opened properly.
                    // Need a messagebox here.
                    this.DataContext = team;
                }
            }
            
        }
    }
}

