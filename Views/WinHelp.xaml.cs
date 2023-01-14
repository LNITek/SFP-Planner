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

namespace SFP_Planner.Views
{
    /// <summary>
    /// Interaction logic for WinHelp.xaml
    /// </summary>
    public partial class WinHelp : Window
    {
        public WinHelp()
        {
            InitializeComponent();
        }

        private void SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var Item = (TreeViewItem)trvHelp.SelectedItem;
            if (Item.Tag == null) DisplayPage.Content = new AboutPage();
            else
                DisplayPage.Content = new HelpPage(Item.Tag.ToString() ?? "");
        }
    }
}
