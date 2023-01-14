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
using System.Windows.Shapes;
using Microsoft.Win32;
using SFPCalculator;

namespace SFP_Planner.Views
{
    /// <summary>
    /// Interaction logic for WinMods.xaml
    /// </summary>
    public partial class WinMods : Window
    {
        public WinMods()
        {
            InitializeComponent();

            ModNames = SFPMods.ListMods();
            grvModList.ItemsSource = ModNames;
        }

        public string[] ModNames { get; set; }

        private void Refresh()
        {
            ModNames = SFPMods.ListMods();
            grvModList.ItemsSource = ModNames;
        }

        private void InstallMod(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdMods = new() { Filter = "Mod File|*.zip", Multiselect = true };
            ofdMods.ShowDialog();
            SFPMods.AddMods(ofdMods.FileNames);
            Refresh();
        }

        private void UninstallMod(object sender, RoutedEventArgs e)
        {
            if (grvModList.SelectedItem is not string Mod)
            {
                MessageBox.Show("No Mod Was Sellected To Be Uninstalled", "Mods",
                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show($"Are You Sure You Want To Remove ({Mod})\r\nThis Action Can Not Be Undone!", "Mods",
                MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK) 
                SFPMods.DeleteMods(Mod);
            Refresh();
        }
    }
}
