using System;
using System.Collections.Generic;
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
using SFP_Planner.Data;
using SFP_Planner.Views;
using SFPCalculator;

namespace SFP_Planner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PlanData = (PlanData)GetRes().First(x => x.GetType() == typeof(PlanData));
        }

        readonly PlanData PlanData;

        #region Miscallaneous
        //Get All Window Resources
        private List<object> GetRes()
        {
            List<object> res = new();
            foreach (var item in frmMain.Resources.Values)
            {
                res.Add(item);
            }
            return res;
        }

        //Refrech Views
        private void Refrech(bool TreeRefrech = true)
        {
            PlanData.AllPropChanged();
            dgrOutputs.Items.Refresh();
            dgrInputs.Items.Refresh();
            if (TreeRefrech)
                trvPlan.Items.Refresh();
        }

        //Imports A Plan From Save File
        public void ImportPlan(Process Plan)
        {
            PlanData.ProductionPlan = Plan;
            Refrech();
        }
        #endregion

        #region Main
        //Change Process On List View Select Item
        private void DisplayProcess(object? sender = null, RoutedPropertyChangedEventArgs<object>? e = null)
        {
            if (PlanData == null || trvPlan.SelectedItem == null) return;
            object Tag = "";
            if (sender != null)
                Tag = e?.NewValue ?? "";
            cmbRP.IsEnabled = true;
            if (Tag.ToString() == "")
            {
                PlanData.ActivePlan = PlanData.ProductionPlan;
                cmbRP.IsEnabled = false;
                if (trvPlan.ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem item)
                    item.IsSelected = false;
            }
            else
                PlanData.ActivePlan = (Process)Tag;
            if (PlanData.ActivePlan == null) return;
            PlanData.Resepies = SFPCalculator.Recipes.Data.GetOutput(
                PlanData.ActivePlan.MainID).Select(x => x.ToString()).ToArray();
            PlanData.Selected = true;

            Refrech(false);
            cmbRP.SelectedItem = PlanData.ActivePlan.Recipe.ToString();
        }

        //Recipie Center Info Panle Combo Box Select
        private void RPSelect(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;
            var Item = e.AddedItems[0]?.ToString();
            if (PlanData == null || string.IsNullOrEmpty(Item) || string.IsNullOrEmpty(cmbRP.Text) 
                || PlanData.ActivePlan == null) return;

            var NewPlan = PlanData.MakePlan(Item, PlanData.ActivePlan.Product.PerMin);
            PlanData.ActivePlan.MainID = NewPlan.MainID;
            PlanData.ActivePlan.Recipe = NewPlan.Recipe;
            PlanData.ActivePlan.Inputs = NewPlan.Inputs;
            PlanData.ActivePlan.Outputs = NewPlan.Outputs;
            PlanData.ActivePlan.Building = NewPlan.Building;
            PlanData.ActivePlan.Children = NewPlan.Children;

            Refrech();
        }

        //Main Left Panel Combo Box Select
        private void DisplayProcessMain(object sender, RoutedEventArgs e) =>
            DisplayProcess();

        //Wheter A Child Sould Be Prodused Check Box Click
        private void ProductionChecked(object sender, RoutedEventArgs e)
        {
            if (PlanData.ActivePlan == null) return;
            var Item = (SFPCalculator.Items.ItemPair)((CheckBox)sender).Tag;
            int I = PlanData.ActivePlan.Inputs.ToList().IndexOf(Item);
            PlanData.ActivePlan.Children.ToList()[I].Production = Item.Production;

            Refrech();
        }

        //The NumUpDown Value Changed
        private void PerMinChanged(object sender, EventArgs e) =>
            Refrech(false);
        #endregion

        //Highlight For trvPlan TextBlocks / Items 
        #region Extra Controls
        private void lblHiMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock LBL)
                LBL.Background = Brushes.LightSkyBlue;
        }

        private void lblHiMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock LBL)
                LBL.Background = Brushes.Transparent;
        }
        #endregion

        //Menu Window Commands
        #region Commands
        private void NewComm(object sender, ExecutedRoutedEventArgs e) =>
            PlanData.ProductionPlan = null;

        private void OpenCom(object sender, ExecutedRoutedEventArgs e) =>
            FileManager.Open(this);

        private void CloseComm(object sender, ExecutedRoutedEventArgs e)
        {
            PlanData.ProductionPlan = null;
            FileManager.OpenedFilePath = "";
        }

        private void SaveCom(object sender, ExecutedRoutedEventArgs e) =>
            FileManager.SaveAs(FileManager.OpenedFilePath);

        private void SaveAsCom(object sender, ExecutedRoutedEventArgs e) =>
            FileManager.SaveAs();

        private void RefrechCom(object sender, ExecutedRoutedEventArgs e) =>
            Refrech();

        private void ExitCom(object sender, ExecutedRoutedEventArgs e) =>
            Application.Current.Shutdown();

        private void ModMenu_Click(object sender, RoutedEventArgs e)
        {
            new WinMods().ShowDialog();
            if(MessageBox.Show("For The Changes To Take Place, You Must Restart The Program.","Mods",
                MessageBoxButton.OKCancel,MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                System.Windows.Forms.Application.Restart();
                Application.Current.Shutdown();
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Settings Is Not Avalabel Right Now.\r\n" +
                "Do You Want To Leave A Message?", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);

        private void HelpCom(object sender, ExecutedRoutedEventArgs e) =>
            new WinHelp().ShowDialog();
        #endregion
    }
}
