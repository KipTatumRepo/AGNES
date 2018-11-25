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

namespace HRManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public EmployeeList EL = new EmployeeList();
        public MainWindow()
        {
            InitializeComponent();
            LoadCostCenters();
        }

        private void LoadCostCenters()
        {
            foreach (char element in EL.CostCenter)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                string str = element.ToString();
                cbi.Content = str;
                cbxCostCenters.Items.Add(cbi);
            }
        }

    }
}
