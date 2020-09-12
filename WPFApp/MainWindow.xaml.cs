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
            researcherStub = this.FindResource("key_ResearcherStub") as Researcher;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FilterResearchers(object source, FilterEventArgs args)
        {
            if (args.Item as Researcher != null) args.Accepted = true;
            else args.Accepted = false;
        }

        private void OnCheckedDontUseDataTemplate(object sender, RoutedEventArgs e)
        {
            teamObservableListBox.ItemTemplate = null;
        }

        private void OnCheckedUseDataTemplate(object sender, RoutedEventArgs e)
        {
            DataTemplate dataTemplate = this.TryFindResource("key_PersonListDataTemplate") as DataTemplate;

            if (dataTemplate != null) teamObservableListBox.ItemTemplate = dataTemplate;
        }

        private void OnClickAddCustomResearcher(object sender, RoutedEventArgs e)
        {
            Researcher newResearcher = researcherStub.DeepCopy() as Researcher;

            team.Add(newResearcher);
        }

        private void OnClickAddDefaultResearcher(object sender, RoutedEventArgs e)
        {
            team.AddDefaultResearcher();
        }

        private void OnClickAddDefaultProgrammer(object sender, RoutedEventArgs e)
        {
            team.AddDefaultProgrammer();
        }

        private void OnClickAddDefaults(object sender, RoutedEventArgs e)
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
            }
        }

        private void OnClickRemove(object sender, RoutedEventArgs e)
        {
            team.RemoveAt(this.teamObservableListBox.SelectedIndex);
        }

        private bool SaveCollection()
        {
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

        private void OnClickOpen(object sender, RoutedEventArgs e)
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

