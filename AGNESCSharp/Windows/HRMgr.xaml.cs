using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AGNESCSharp
{
    /// <summary>
    /// Interaction logic for HRMgr.xaml
    /// </summary>
    public partial class HRMgr : Window
    {
        #region Properties
        public static string empCostCenter;
        private List<string> CostCenters = new List<string>();
        private Dictionary<long, string> Employees = new Dictionary<long, string>();
        DateTime today = DateTime.Now;
        DateTime hireDate;
        // private HRActionCanvas AC;
        private ListBoxItem lbi;
        private long empId;
        public static int empInProbationPeriod = 0;
        private string costCenterSel;
        #endregion

        #region Constructor/Main

        public HRMgr()
        {
            InitializeComponent();
            LoadCostCenters();
            cbxCostCenters.SelectedIndex = 0;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        private void LoadCostCenters()
        {
            var qal = from al in MainWindow.bidb.EmployeeLists select al;

            foreach (var al in qal)
            {
                CostCenters.Add(al.CostCenter);
            }

            CostCenters.Sort();

            ComboBoxItem cball = new ComboBoxItem()
            { Content = "All" };
            cbxCostCenters.Items.Add(cball);

            foreach (string ccn in CostCenters.Distinct())
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = ccn;
                cbxCostCenters.Items.Add(cbi);
            }
        }

        private void LoadEmployees(string ShowType)
        {
            Employees.Clear();
            lbxAssociates.Items.Clear();

            var qal = from al in MainWindow.bidb.EmployeeLists
                      where al.CostCenter == ShowType
                      orderby al.LastName
                      select al;

            foreach (var al in qal)
            {
                string fnm = al.LastName + ", " + al.FirstName;
                empId = al.PersNumber;
                Employees.Add(empId, fnm);
            }

            foreach (var item in Employees.Distinct())
            {
                lbi = new ListBoxItem();
                lbi.Tag = item.Key;
                lbi.Content = item.Value;
                lbi.MouseDoubleClick += new MouseButtonEventHandler(EmployeeChosen);
                lbxAssociates.Items.Add(lbi);
            }
        }

        private void LoadEmployees()
        {
            Employees.Clear();
            lbxAssociates.Items.Clear();

            var qal = from al in MainWindow.bidb.EmployeeLists
                      orderby al.LastName
                      select al;

            foreach (var al in qal)
            {

                string fnm = al.LastName + ", " + al.FirstName;
                empId = al.PersNumber;
                Employees.Add(empId, fnm);
            }

            foreach (var item in Employees)
            {
                ListBoxItem lbi = new ListBoxItem();
                lbi.Tag = item.Key;
                lbi.Content = item.Value;
                lbi.MouseDoubleClick += new MouseButtonEventHandler(EmployeeChosen);
                lbxAssociates.Items.Add(lbi);
            }
        }

        private void CostCenterChosen(object sender, SelectionChangedEventArgs e)
        {
            costCenterSel = (cbxCostCenters.SelectedItem as ComboBoxItem).Content.ToString();

            if (costCenterSel == "All")
            {
                LoadEmployees();
            }
            else
            {
                LoadEmployees(costCenterSel);
            }
        }

        private void EmployeeChosen(object sender, MouseButtonEventArgs e)
        {
            lbi = (ListBoxItem)sender;
            empId = (long)lbi.Tag;
            OccStackPanel.Visibility = Visibility.Collapsed;
            LOAStackPanel.Visibility = Visibility.Collapsed;
            CashHandleStackPanel.Visibility = Visibility.Collapsed;
            DateTime cutOffDate = today.AddYears(-1);

            var query = from employeeTable in MainWindow.agnesdb.Occurrences
                        where empId == employeeTable.PersNumber && employeeTable.Date >= cutOffDate
                        select new
                        {
                            employeeTable.PID,
                            employeeTable.PersNumber,
                            employeeTable.Type,
                            employeeTable.Date,
                            employeeTable.Notes,
                            employeeTable.AttendanceViolation
                        };

            var LoaQuery = from loaEmployeeTable in MainWindow.agnesdb.LOAs
                           where empId == loaEmployeeTable.PersNumber
                           select new
                           {
                               loaEmployeeTable.PID,
                               loaEmployeeTable.PersNumber,
                               loaEmployeeTable.Type,
                               loaEmployeeTable.DateStart,
                               loaEmployeeTable.DateEnd,
                               loaEmployeeTable.Notes
                           };

            var cashHandleQuery = from chEmployeeTable in MainWindow.agnesdb.CashHandles
                                  where empId == chEmployeeTable.PersNumber
                                  select new
                                  {
                                      chEmployeeTable.PID,
                                      chEmployeeTable.PersNumber,
                                      chEmployeeTable.Type,
                                      chEmployeeTable.Date,
                                      chEmployeeTable.Notes
                                  };

            var result = query.ToList();
            var loaResult = LoaQuery.ToList();
            var cashHandleResult = cashHandleQuery.ToList();

            if (result.Count >= 1)
            {
                OccStackPanel.Visibility = Visibility.Visible;
                OccurrenceDataGrid.ItemsSource = result;
            }

            if (loaResult.Count >= 1)
            {
                LOAStackPanel.Visibility = Visibility.Visible;
                LoaDataGrid.ItemsSource = loaResult;
            }

            if (cashHandleResult.Count >= 1)
            {
                CashHandleStackPanel.Visibility = Visibility.Visible;
                CashHandleDataGrid.ItemsSource = cashHandleResult;
            }
            lbxHistory.Visibility = Visibility.Visible;
        }

        private void FilteredEmployeeChosenLoa(object sender, MouseButtonEventArgs e)
        {
            lbi = (ListBoxItem)sender;
            empId = (long)lbi.Tag;
            var LoaQuery = from loaEmployeeTable in MainWindow.agnesdb.LOAs
                           where empId == loaEmployeeTable.PersNumber
                           select new
                           {
                               loaEmployeeTable.PID,
                               loaEmployeeTable.PersNumber,
                               loaEmployeeTable.Type,
                               loaEmployeeTable.DateStart,
                               loaEmployeeTable.DateEnd,
                               loaEmployeeTable.Notes
                           };

            var loaResult = LoaQuery.ToList();

            if (loaResult.Count >= 1)
            {
                LOAStackPanel.Visibility = Visibility.Visible;
                LoaDataGrid.ItemsSource = loaResult;
            }
            lbxHistory.Visibility = Visibility.Visible;
        }

        private void FilteredEmployeeChosenOcc(object sender, MouseButtonEventArgs e)
        {
            lbi = (ListBoxItem)sender;
            empId = (long)lbi.Tag;
            var OccQuery = from OccEmployeeTable in MainWindow.agnesdb.Occurrences
                           where empId == OccEmployeeTable.PersNumber
                           select new
                           {
                               OccEmployeeTable.PID,
                               OccEmployeeTable.PersNumber,
                               OccEmployeeTable.Type,
                               OccEmployeeTable.Date,
                               OccEmployeeTable.Notes,
                               OccEmployeeTable.AttendanceViolation
                           };

            var occResult = OccQuery.ToList();

            if (occResult.Count >= 1)
            {
                OccStackPanel.Visibility = Visibility.Visible;
                OccurrenceDataGrid.ItemsSource = occResult;
            }
            lbxHistory.Visibility = Visibility.Visible;
        }

        private void FilteredEmployeeChosenCash(object sender, MouseButtonEventArgs e)
        {
            lbi = (ListBoxItem)sender;
            empId = (long)lbi.Tag;
            var OccQuery = from CHEmployeeTable in MainWindow.agnesdb.CashHandles
                           where empId == CHEmployeeTable.PersNumber
                           select new
                           {
                               CHEmployeeTable.PID,
                               CHEmployeeTable.PersNumber,
                               CHEmployeeTable.Type,
                               CHEmployeeTable.Date,
                               CHEmployeeTable.Notes,
                           };

            var CHResult = OccQuery.ToList();

            if (CHResult.Count >= 1)
            {
                CashHandleStackPanel.Visibility = Visibility.Visible;
                CashHandleDataGrid.ItemsSource = CHResult;
            }
            lbxHistory.Visibility = Visibility.Visible;
        }

        private void LeaveButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            OccStackPanel.Visibility = Visibility.Collapsed;
            LOAStackPanel.Visibility = Visibility.Collapsed;
            CashHandleStackPanel.Visibility = Visibility.Collapsed;
            if (button.IsChecked == true)
            {
                button.BorderBrush = Brushes.Gray;
                button.BorderThickness = new Thickness(2, 2, 2, 2);
                OccButton.IsChecked = false;
                CashHandleButton.IsChecked = false;

                if (lbxAssociates.SelectedIndex == -1)
                {
                    lbxAssociates.Items.Clear();
                    if (costCenterSel == "All")
                    {
                        var query = from employeeTable in MainWindow.agnesdb.LOAs
                                    select new
                                    {
                                        employeeTable.PersNumber,
                                        employeeTable.LastName,
                                        employeeTable.FirstName
                                    };

                        foreach (var item in query.Distinct())
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            lbi.Tag = item.PersNumber;
                            lbi.Content = item.LastName + ", " + item.FirstName.Trim();
                            lbi.MouseDoubleClick += new MouseButtonEventHandler(FilteredEmployeeChosenLoa);
                            lbxAssociates.Items.Add(lbi);
                        }
                    }
                    else
                    {
                        var query = from employeeTable in MainWindow.agnesdb.LOAs
                                    where employeeTable.CostCenter == costCenterSel
                                    select new
                                    {
                                        employeeTable.PersNumber,
                                        employeeTable.LastName,
                                        employeeTable.FirstName
                                    };
                        var result = query;
                        foreach (var item in query.Distinct())
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            lbi.Tag = item.PersNumber;
                            lbi.Content = item.LastName + ", " + item.FirstName.Trim();
                            lbi.MouseDoubleClick += new MouseButtonEventHandler(FilteredEmployeeChosenLoa);
                            lbxAssociates.Items.Add(lbi);
                        }
                    }
                }
                else
                {
                    lbi = lbxAssociates.SelectedItem as ListBoxItem;
                    string name = lbi.Content.ToString();
                    int empId = Convert.ToInt32(lbi.Tag.ToString());
                    Window newWindow = new HRLeave(name, empId);
                    LeaveButton.IsChecked = false;
                    OccButton.IsChecked = false;
                    CashHandleButton.IsChecked = false;
                    newWindow.ShowDialog();
                }
            }
            else
            {
                button.BorderThickness = new Thickness(0, 0, 0, 0);
                if (costCenterSel == "All")
                {
                    LoadEmployees();
                }
                else
                {
                    LoadEmployees(costCenterSel);
                }
            }
        }

        private void OccButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            OccStackPanel.Visibility = Visibility.Collapsed;
            LOAStackPanel.Visibility = Visibility.Collapsed;
            CashHandleStackPanel.Visibility = Visibility.Collapsed;
            if (button.IsChecked == true)
            {
                button.BorderBrush = Brushes.Gray;
                button.BorderThickness = new Thickness(2, 2, 2, 2);
                LeaveButton.IsChecked = false;
                CashHandleButton.IsChecked = false;

                if (lbxAssociates.SelectedIndex == -1)
                {
                    lbxAssociates.Items.Clear();
                    if (costCenterSel == "All")
                    {
                        var query = (from employeeTable in MainWindow.agnesdb.Occurrences
                                     select new
                                     {
                                         employeeTable.PersNumber,
                                         employeeTable.LastName,
                                         employeeTable.FirstName
                                     }).Distinct();
                        query.ToList();

                        foreach (var item in query)
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            lbi.Tag = item.PersNumber;
                            lbi.Content = item.LastName + ", " + item.FirstName;
                            lbi.MouseDoubleClick += new MouseButtonEventHandler(FilteredEmployeeChosenOcc);
                            lbxAssociates.Items.Add(lbi);
                        }
                    }
                    else
                    {
                        var query = (from employeeTable in MainWindow.agnesdb.Occurrences
                                     where employeeTable.CostCenter == costCenterSel
                                     select new
                                     {
                                         employeeTable.PersNumber,
                                         employeeTable.LastName,
                                         employeeTable.FirstName
                                     }).Distinct();
                        query.ToList();

                        foreach (var item in query)
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            lbi.Tag = item.PersNumber;
                            lbi.Content = item.LastName + ", " + item.FirstName;
                            lbi.MouseDoubleClick += new MouseButtonEventHandler(FilteredEmployeeChosenOcc);
                            lbxAssociates.Items.Add(lbi);
                        }
                    }

                }
                else
                {
                    lbi = lbxAssociates.SelectedItem as ListBoxItem;
                    string name = lbi.Content.ToString();
                    int empId = Convert.ToInt32(lbi.Tag.ToString());

                    var query = from employeeTable in MainWindow.bidb.EmployeeLists
                                where employeeTable.PersNumber == empId
                                select employeeTable;

                    var results = query.ToList();

                    foreach (var result in query)
                    {
                        hireDate = result.DateOfHire;
                    }

                    if (hireDate.AddDays(90) >= today)
                    {
                        empInProbationPeriod = 1;
                    }

                    Window newPage = new HROccurrence(name, empId, empInProbationPeriod);
                    LeaveButton.IsChecked = false;
                    OccButton.IsChecked = false;
                    CashHandleButton.IsChecked = false;
                    newPage.ShowDialog();
                }
            }
            else
            {
                button.BorderThickness = new Thickness(0, 0, 0, 0);
                if (costCenterSel == "All")
                {
                    LoadEmployees();
                }
                else
                {
                    LoadEmployees(costCenterSel);
                }
            }
        }

        private void CashHandleButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            OccStackPanel.Visibility = Visibility.Collapsed;
            LOAStackPanel.Visibility = Visibility.Collapsed;
            CashHandleStackPanel.Visibility = Visibility.Collapsed;
            if (button.IsChecked == true)
            {
                button.BorderBrush = Brushes.Gray;
                button.BorderThickness = new Thickness(2, 2, 2, 2);
                LeaveButton.IsChecked = false;
                OccButton.IsChecked = false;
                if (lbxAssociates.SelectedIndex == -1)
                {
                    lbxAssociates.Items.Clear();

                    if (costCenterSel == "All")
                    {
                        var query = (from employeeTable in MainWindow.agnesdb.CashHandles
                                     select new
                                     {
                                         employeeTable.PersNumber,
                                         employeeTable.LastName,
                                         employeeTable.FirstName
                                     }).Distinct();
                        query.ToList();

                        foreach (var item in query)
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            lbi.Tag = item.PersNumber;
                            lbi.Content = item.LastName + ", " + item.FirstName;
                            lbi.MouseDoubleClick += new MouseButtonEventHandler(FilteredEmployeeChosenCash);
                            lbxAssociates.Items.Add(lbi);
                        }
                    }
                    else
                    {
                        var query = (from employeeTable in MainWindow.agnesdb.CashHandles
                                     where employeeTable.CostCenter == costCenterSel
                                     select new
                                     {
                                         employeeTable.PersNumber,
                                         employeeTable.LastName,
                                         employeeTable.FirstName
                                     }).Distinct();
                        query.ToList();

                        foreach (var item in query)
                        {
                            ListBoxItem lbi = new ListBoxItem();
                            lbi.Tag = item.PersNumber;
                            lbi.Content = item.LastName + ", " + item.FirstName;
                            lbi.MouseDoubleClick += new MouseButtonEventHandler(FilteredEmployeeChosenCash);
                            lbxAssociates.Items.Add(lbi);
                        }
                    }
                }
                else
                {
                    lbi = lbxAssociates.SelectedItem as ListBoxItem;
                    string name = lbi.Content.ToString();
                    int empId = Convert.ToInt32(lbi.Tag.ToString());

                    var query = from employeeTable in MainWindow.bidb.EmployeeLists
                                where employeeTable.PersNumber == empId
                                select employeeTable;

                    var results = query.ToList();

                    foreach (var result in query)
                    {
                        hireDate = result.DateOfHire;
                    }

                    if (hireDate.AddDays(90) >= today)
                    {
                        empInProbationPeriod = 1;
                    }
                    Window newWindow = new HRCashHandle(name, empId, empInProbationPeriod);
                    LeaveButton.IsChecked = false;
                    OccButton.IsChecked = false;
                    CashHandleButton.IsChecked = false;
                    newWindow.ShowDialog();
                }
            }
            else
            {
                button.BorderThickness = new Thickness(0, 0, 0, 0);
                if (costCenterSel == "All")
                {
                    LoadEmployees();
                }
                else
                {
                    LoadEmployees(costCenterSel);
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            OccButton.IsChecked = false;
            CashHandleButton.IsChecked = false;
            LeaveButton.IsChecked = false;
            OccStackPanel.Visibility = Visibility.Collapsed;
            LOAStackPanel.Visibility = Visibility.Collapsed;
            CashHandleStackPanel.Visibility = Visibility.Collapsed;
            Window newWindow = new HRSearch();
            newWindow.ShowDialog();
        }
        #endregion
    }
}
