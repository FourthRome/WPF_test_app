using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ClassLibrary;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    

    public partial class MainWindow : Window
    {
        //----------------------
        // Private member fields
        //----------------------
        private TeamObservable team;
        private Researcher researcherStub;


        //---------------------
        // Public static fields
        //---------------------
        public static RoutedCommand AddCustomResearcherCommand = new RoutedCommand("AddCustomResearcher", typeof(MainWindow));


        //---------------------------------------------
        // Constructors + window's basic event handlers
        //---------------------------------------------

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            team = this.FindResource("key_team") as TeamObservable;
            researcherStub = this.FindResource("key_researcherStub") as Researcher;

            this.dontUseDataTemplateRadioButton.IsChecked = true;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (this.ProceedWithCollectionReplacement() == false) { e.Cancel = true; }
        }

        //--------------------
        // Elements' callbacks
        //--------------------

        private void FilterResearchers(object source, FilterEventArgs args)
        {
            if (args.Item as Researcher != null) args.Accepted = true;
            else args.Accepted = false;
        }

        //---------------
        // Event handlers
        //---------------

        private void OnCheckedDontUseDataTemplate(object sender, RoutedEventArgs e)
        {
            if (teamObservableListBox != null) { this.teamObservableListBox.ItemTemplate = null; }
            
        }

        private void OnCheckedUseDataTemplate(object sender, RoutedEventArgs e)
        {
            if (teamObservableListBox != null)
            {
                DataTemplate dataTemplate = TryFindResource("key_PersonListDataTemplate") as DataTemplate;
                if (dataTemplate != null) teamObservableListBox.ItemTemplate = dataTemplate;
            }
        }

        private void OnClickAddCustomResearcher(object sender, RoutedEventArgs e)
        {
            bool inputErrors = false;
            foreach(FrameworkElement child in newResearcherGrid.Children)
            {
                if (Validation.GetHasError(child))
                {
                    inputErrors = true;
                    break;
                }
            }

            if (inputErrors)
            {
                MessageBox.Show(
                    "Some fields with information about new researcher are filled incorrectly. Please check them.",
                    "TeamObservable Editor",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            } else
            {
                Researcher newResearcher = researcherStub.DeepCopy() as Researcher;
                team.Add(newResearcher);
            }
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
                    DataContext = team;
                }
            }
        }

        private void OnClickSave(object sender, RoutedEventArgs e)
        {
            if (ValidateTeamBeforeSave()) { SaveCollection(); }
        }

        private void OnClickNew(object sender, RoutedEventArgs e)
        {
            if (ProceedWithCollectionReplacement())
            {
                team = new TeamObservable();
                DataContext = this.team;
            }
        }

        private void OnClickRemove(object sender, RoutedEventArgs e)
        {
            if (this.teamObservableListBox.SelectedIndex >= 0)
            {
                team.RemoveAt(this.teamObservableListBox.SelectedIndex);
            }
        }


        //------------------
        // Command handlers
        //------------------
        private void OpenCommandHandler(object sender, ExecutedRoutedEventArgs e)
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
                    DataContext = team;
                }
            }
        }

        private void CanSaveCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ValidateTeamBeforeSave();
        }

        private void SaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            SaveCollection();
        }

        private void CanRemoveCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (teamObservableListBox == null) { e.CanExecute = false; }
            else { e.CanExecute = (teamObservableListBox.SelectedIndex >= 0); }
            
        }

        private void RemoveCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            team.RemoveAt(this.teamObservableListBox.SelectedIndex);
        }

        private void CanAddCustomResearcherCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (newResearcherGrid == null) {
                e.CanExecute = false;
                return;
            }
            bool inputErrors = false;
            foreach (FrameworkElement child in newResearcherGrid.Children)
            {
                if (Validation.GetHasError(child))
                {
                    inputErrors = true;
                    break;
                }
            }

            e.CanExecute = !inputErrors;
        }

        private void AddCustomResearcherCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Researcher newResearcher = researcherStub.DeepCopy() as Researcher;
            team.Add(newResearcher);
        }


        //----------------------
        // Inner logic functions
        //----------------------
        private bool ValidateTeamBeforeSave()
        {
            if (this.teamObservableInfoGrid == null) { return false; }

            bool inputErrors = false;
            foreach (FrameworkElement child in teamObservableInfoGrid.Children)
            {
                if (Validation.GetHasError(child))
                {
                    inputErrors = true;
                    break;
                }
            }

            return !inputErrors;
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
                MessageBoxResult result = MessageBox.Show(
                    messageBoxText:"Do you want to save the changes into the current team? Your changes will be lost if you don't save them.",
                    caption: "TeamObservable Editor",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning,
                    MessageBoxResult.Yes);

                if (result == MessageBoxResult.Cancel) { return false; }
                else if (result == MessageBoxResult.Yes) {
                    bool validation = ValidateTeamBeforeSave();
                    if (!validation)
                    {
                        MessageBox.Show(
                            "Some fields with information about the team are filled incorrectly. Please check them.",
                            "TeamObservable Editor",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                    return validation && SaveCollection(); }
            }
            return true;
        }
    }
}

