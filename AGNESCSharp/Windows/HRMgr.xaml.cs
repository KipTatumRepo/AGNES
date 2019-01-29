using AGNESCSharp.Entity_Models;
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
        public static int empInProbationPeriod = 2;
        private List<string> CostCenters = new List<string>();
        private Dictionary<long, string> Employees = new Dictionary<long, string>();
        private DateTime today = DateTime.Now;
        private DateTime hireDate;
        private ListBoxItem lbi;
        private long empId;
        private string costCenterSel;
        string selectedOccurrence;
        //Cash Handling Violation Passable Variables
        public static string CashHandleNumberV;
        public static int SelectedIndexV;
        public static DateTime? CHDateV;
        public static string CHNoteV;
        public static string firstName;
        public static DateTime? DateEnd;
        //Leave Of Absence Passable Variables
        public static string LOANumberV;
        public static DateTime? LOADateStartV;
        public static DateTime? LOADateEndV;
        public static String LOANoteV;
        public static int? Approved;
        public static int? Pending;
        public static int? ClosedV;
        public static int? Parental;
        public static int? Intermittent;
        public static int? Continuous;
        //Occurrence Passable Variables
        public static string OccNumberV;
        public static DateTime? OccDateV;
        public static string OccAttViolation;
        public static byte? OccType;
        public static string OccNotesV;

        #endregion

        #region Constructor/Main

        public HRMgr(long userAccess)
        {
            InitializeComponent();
            
            LoadCostCenters(userAccess);

            cbxCostCenters.SelectedIndex = 0;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        private void LoadCostCenters(long userAccess)
        {
            if (userAccess == 0)
            {
                var qal = from al in MainWindow.bidb.EmployeeLists
                          select al;

                foreach (var al in qal)
                {
                    CostCenters.Add(al.CostCenter);
                }

                ComboBoxItem cball = new ComboBoxItem()
                { Content = "All" };
                cbxCostCenters.Items.Add(cball);
                
            }
            else
            {
                var getUnit = from unitTable in MainWindow.agnesdb.UnitsUsers_Joins
                              where userAccess == unitTable.UserId
                              select unitTable.UnitNumber;

                List<long> units = getUnit.ToList();
                List<string> costCenters = new List<string>();

                if (units.Contains(11682))
                {
                    var qal = from al in MainWindow.bidb.EmployeeLists
                              select al;

                    foreach (var al in qal)
                    {
                        CostCenters.Add(al.CostCenter);
                    }

                    ComboBoxItem cball = new ComboBoxItem()
                    { Content = "All" };
                    cbxCostCenters.Items.Add(cball);
                }
                else
                {
                    foreach (long unit in units)
                    {
                        var getCostCenter = from costCenterTable in MainWindow.bidb.EmployeeLists
                                            where unit == costCenterTable.CostCenterNumber
                                            select costCenterTable.CostCenter;

                        foreach (string costCenter in getCostCenter.ToList())
                        {
                            CostCenters.Add(costCenter);
                        }
                    }

                    //var getCostCenter = from costCenterTable in MainWindow.bidb.EmployeeLists
                    //                     where getUnit == costCenterTable.CostCenterNumber
                    //                     select costCenterTable.CostCenter;

                    //List<string> costCenters = getCostCenter.ToList();

                    //foreach (string item in costCenters)
                    //{
                    //    CostCenters.Add(item);
                    //}
                    //CostCenters.Add(getCostCenter);
                }

                

                //var getCostCenter = (from costCenterTable in MainWindow.bidb.CostCenters
                //                    where getUnit == costCenterTable.CostCenter1
                //                    select costCenterTable.UnitName).SingleOrDefault();

                //CostCenters.Add(getCostCenter);
                //foreach (var unit in getUnit)
                //{

                //    CostCenters.Add(unit.UnitNumber.ToString());
                //}
            }

            CostCenters.Sort();
            
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

            var querys = from employeeTable in MainWindow.bidb.EmployeeLists
                        where employeeTable.PersNumber == empId
                        select employeeTable;

            foreach (var resultes in querys)
            {
                hireDate = resultes.DateOfHire;
            }

            if (hireDate.AddDays(90) >= today)
            {
                empInProbationPeriod = 1;
            }
            else
            {
                empInProbationPeriod = 0;
            }

            var query = from employeeTable in MainWindow.agnesdb.Occurrences
                        where empId == employeeTable.PersNumber && employeeTable.Date >= cutOffDate
                        orderby employeeTable.Date descending
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
                           orderby loaEmployeeTable.DateStart descending
                           select new
                           {
                               loaEmployeeTable.PID,
                               loaEmployeeTable.PersNumber,
                               loaEmployeeTable.Type,
                               loaEmployeeTable.DateStart,
                               loaEmployeeTable.DateEnd,
                               loaEmployeeTable.Approved,
                               loaEmployeeTable.Pending,
                               loaEmployeeTable.Closed,
                               loaEmployeeTable.Parental,
                               loaEmployeeTable.Continuous,
                               loaEmployeeTable.Intermittent,
                               loaEmployeeTable.Notes
                           };
            var cashHandleQuery = from chEmployeeTable in MainWindow.agnesdb.CashHandles
                                  where empId == chEmployeeTable.PersNumber && chEmployeeTable.Date >= cutOffDate
                                  orderby chEmployeeTable.Date descending
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
                double? Points = 0;
                OccStackPanel.Visibility = Visibility.Visible;
                OccurrenceDataGrid.ItemsSource = result;
                OccPointDisplay.Foreground = Brushes.Black;
                double? OccPoints = 0;

                foreach (var row in result)
                {
                    Points += row.Type;
                }
                OccPoints = Points / 2;

                if (OccPoints >= 4)
                {
                    OccPointDisplay.Foreground = Brushes.Red;
                }
                OccPointDisplay.Text = OccPoints.ToString();
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
                CHPointDisplay.Foreground = Brushes.Black;
                double? NoVarianceCount = 0;

                foreach (var row in cashHandleResult)
                {
                    if (row.Type == 0)
                    {
                        NoVarianceCount++;
                    }
                }

                if (NoVarianceCount >= 2)
                {
                    CHPointDisplay.Foreground = Brushes.Red;
                }
                CHPointDisplay.Text = NoVarianceCount.ToString();
            }
            lbxHistory.Visibility = Visibility.Visible;
        }

        private void FilteredEmployeeChosenLoa(object sender, MouseButtonEventArgs e)
        {
            lbi = (ListBoxItem)sender;
            empId = (long)lbi.Tag;
            var LoaQuery = from loaEmployeeTable in MainWindow.agnesdb.LOAs
                           where empId == loaEmployeeTable.PersNumber
                           orderby loaEmployeeTable.DateStart descending
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
                           orderby OccEmployeeTable.Date descending
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
                           orderby CHEmployeeTable.Date descending
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
                    Window newWindow = new HRLeave(name, empId, 0);
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

                    //var results = query.ToList();

                    foreach (var result in query)
                    {
                        hireDate = result.DateOfHire;
                    }

                    if (hireDate.AddDays(90) >= today)
                    {
                        empInProbationPeriod = 1;
                    }
                    else
                    {
                        empInProbationPeriod = 0;
                    }


                    Window newPage = new HROccurrence(name, empId, empInProbationPeriod, 0);
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
                    else
                    {
                        empInProbationPeriod = 0;
                    }
                    Window newWindow = new HRCashHandle(name, empId, empInProbationPeriod, 0);
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

        private void CashHandle_PID_CellClicked(object sender, MouseButtonEventArgs e)
        {
            if (CashHandleDataGrid.SelectedItem == null) { return; }
            object row = CashHandleDataGrid.SelectedValue;
            selectedOccurrence = (CashHandleDataGrid.SelectedCells[0].Column.GetCellContent(row) as TextBlock).Text;
            long SelectOccurrence = Int64.Parse(selectedOccurrence);

            AGNESEntity agnesdb = new AGNESEntity();

            //find corresponding occurrence in table
            var query = from FilteredCHTable in agnesdb.CashHandles
                        where SelectOccurrence == FilteredCHTable.PID
                        select FilteredCHTable;

            var result = query.ToList();

            foreach (var filteredRow in result)
            {
                //set public static variable to carry from here to LOA Window when datagrid selection is made
                firstName = filteredRow.FirstName;
                CashHandleNumberV = filteredRow.PID.ToString();
                SelectedIndexV = Convert.ToInt32(filteredRow.Type);
                CHDateV = filteredRow.Date;
                CHNoteV = filteredRow.Notes;

            }
            Window newWindow = new HRCashHandle(firstName, (int)empId, empInProbationPeriod, 2);
            lbxHistory.Visibility = Visibility.Collapsed;
            newWindow.ShowDialog();
        }

        private void LOA_PID_CellClicked(object sender, MouseButtonEventArgs e)
        {

            if (LoaDataGrid.SelectedItem == null) { return; }
            object row = LoaDataGrid.SelectedValue;
            selectedOccurrence = (LoaDataGrid.SelectedCells[0].Column.GetCellContent(row) as TextBlock).Text;
           
            long SelectOccurrence = Int64.Parse(selectedOccurrence);

            AGNESEntity agnesdb = new AGNESEntity();

            //find corresponding occurrence in table
            var query = from FilteredLOATable in agnesdb.LOAs
                        where SelectOccurrence == FilteredLOATable.PID
                        select FilteredLOATable;

            var result = query.ToList();

            foreach (var filteredRow in result)
            {
                #region Work on this for v2 
                //set public static variable to carry from here to LOA Window when datagrid selection is made
                firstName = filteredRow.FirstName;
                LOANumberV = filteredRow.PID.ToString();
                LOADateStartV = filteredRow.DateStart;
                LOADateEndV = filteredRow.DateEnd;
                LOANoteV = filteredRow.Notes;
                Approved = filteredRow.Approved;
                Pending = filteredRow.Pending;
                ClosedV = filteredRow.Closed;
                Parental = filteredRow.Parental;
                Continuous = filteredRow.Continuous;
                Intermittent = filteredRow.Intermittent;
                #endregion
            }

            Window newWindow = new HRLeave(firstName, (int)empId, 2);
            lbxHistory.Visibility = Visibility.Collapsed;
            newWindow.ShowDialog();
        }

        private void Occ_PID_CellClicked(object sender, MouseButtonEventArgs e)
        {
            if (OccurrenceDataGrid.SelectedItem == null) { return; }
            object row = OccurrenceDataGrid.SelectedValue;
            selectedOccurrence = (OccurrenceDataGrid.SelectedCells[0].Column.GetCellContent(row) as TextBlock).Text;
            long SelectOccurrence = Int64.Parse(selectedOccurrence);

            AGNESEntity agnesdb = new AGNESEntity();

            //find corresponding occurrence in table
            var query = from FilteredOccTable in agnesdb.Occurrences
                        where SelectOccurrence == FilteredOccTable.PID
                        select FilteredOccTable;

            var result = query.ToList();

            foreach (var filteredRow in result)
            {
                firstName = filteredRow.FirstName;
                OccNumberV = filteredRow.PID.ToString();
                OccDateV = filteredRow.Date;
                OccAttViolation = filteredRow.AttendanceViolation;
                OccType = filteredRow.Type;
                OccNotesV = filteredRow.Notes;
            }

            Window newWindow = new HROccurrence(firstName, (int)empId, empInProbationPeriod, 2);
            lbxHistory.Visibility = Visibility.Collapsed;
            newWindow.ShowDialog();
        }

        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            lbxHistory.Visibility = Visibility.Collapsed;
            string name = lbi.Content.ToString();
            Window newWindow = new HROccurrence(name, empId, empInProbationPeriod, 0);
            newWindow.ShowDialog();
        }

        private void LOAAddNew_Click(object sender, RoutedEventArgs e)
        {
            lbxHistory.Visibility = Visibility.Collapsed;
            string name = lbi.Content.ToString();
            Window newWindow = new HRLeave(name, empId, 0);
            newWindow.ShowDialog();
        }

        private void CHAddNew_Click(object sender, RoutedEventArgs e)
        {
            lbxHistory.Visibility = Visibility.Collapsed;
            string name = lbi.Content.ToString();
            Window newWindow = new HRCashHandle(name, empId, empInProbationPeriod, 0);
            newWindow.ShowDialog();
        }

        #endregion
    }
}
