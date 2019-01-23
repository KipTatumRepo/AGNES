using AGNESCSharp.Entity_Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AGNESCSharp
{
    /// <summary>
    /// Interaction logic for HRSearch.xaml
    /// </summary>
    public partial class HRSearch : Window
    {
        #region Variables
        //byte? violationAmount;
        private int selectedSearch;
        private int selectedSearchType;
        //int navFromSearch = 1;
        private long SelectOccurrence;
        private long? assocNumber;
        private ComboBoxItem searchTable;
        private string selectedOccurrence;
        private string searchTableItem;
        private string lastName;
        private string firstName;
        Dictionary<string, int> cbDictionary = new Dictionary<string, int>();
        private DateTime today = DateTime.Now;
        private int empInProbation = 0;
        private int EmpNumber;
        //Cash Handling Violation Passable Variables
        public static string CashHandleNumberV;
        public static int CHSelectedIndexV;
        public static DateTime? CHDateV;
        public static string CHNoteV;
        //Leave Of Absence Passable Variables
        public static string LOANumberV;
        public static DateTime? LOADateStartV;
        public static DateTime? LOADateEndV;
        public static string LOANoteV;
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

        #region Main
        public HRSearch()
        {
            InitializeComponent();
            cbDictionary.Add("Consecutive Unexcused Absence", 0);
            cbDictionary.Add("Early Out", 1);
            cbDictionary.Add("Failure to follow Meal/Rest break", 1);
            cbDictionary.Add("Late", 1);
            cbDictionary.Add("LOA Approved", 0);
            cbDictionary.Add("LOA Denied", 2);
            cbDictionary.Add("LOA Pending", 0);
            cbDictionary.Add("No Call No Show", 2);
            cbDictionary.Add("Sick, insufficient sick time", 1);
            cbDictionary.Add("Sick, no sick time available", 2);
            cbDictionary.Add("Unexcused Absence", 2);

            foreach (var item in cbDictionary)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = item.Key;
                cbi.Tag = item.Value;
                OccCB.Items.Add(cbi);
            }
        }
        #endregion

        #region Public Methods
        public static int SetIndex(string cbSelection)
        {

            int index;
            List<string> cbList = new List<string>();
            cbList.Add("Consecutive Unexcused Absence");
            cbList.Add("Early Out");
            cbList.Add("Failure to follow Meal/Rest break");
            cbList.Add("Late");
            cbList.Add("LOA Approved");
            cbList.Add("LOA Denied");
            cbList.Add("LOA Pending");
            cbList.Add("No Call No Show");
            cbList.Add("Sick, insufficient sick time");
            cbList.Add("Sick, no sick time available");
            cbList.Add("Unexcused Absence");

            index = cbList.FindIndex(x => x.StartsWith(cbSelection));

            return index;
        }

        public static void Report(string firstName, string violationText, double? occPoints, int empInProbation, DateTime earlyDate, int? cashHandleType, long? empId)
        {
            int? CashHandleType = cashHandleType;
            if (cashHandleType == null)
            {
                CashHandleType = 100;
            }

            //get Write up form ready
            FileInfo myFile = new FileInfo(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
            bool exists = myFile.Exists;
            switch (empInProbation)
            {
                case 0:
                    //this may be redone
                    if (CashHandleType == 0)
                    {
                        int anyPriorZero = HRCashHandle.CountZeroPoints(earlyDate, empId);

                        if (anyPriorZero == 2)
                        {
                            BIMessageBox.Show("Warning", firstName + " Has 2 Prior No Variance Found Violations, 1 More Will Result in a Progressive Counseling",
                                MessageBoxButton.OK);
                        }

                        if (anyPriorZero == 3)
                        {
                            BIMessageBox.Show("Counseling Form Dialog", "This is " + firstName + "'s Thrid Occurrence For No Variance Found Violations and Requires a WRITTEN Counseling" +
                                                        " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
                            Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                        }
                        return;
                    }

                    if (CashHandleType == 1)
                    {
                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has a Variance Between $3.00 and $20.00 This is an Automatic Progressive Counseling" +
                            " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                        return;
                    }

                    if (CashHandleType == 2)
                    {
                        BIMessageBox.Show("Contact HRBP Dialong", "This Type of Cash Handling Violation Requires Notification of Your DM AND HRBP, Please Contact Them", MessageBoxButton.OK);

                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has a Variance Greater Than $20.00 This is an Automatic Progressive Counseling" +
                            " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                        return;
                    }

                    if (occPoints < 4)
                    {
                        MessageBox.Show(firstName + " Has " + occPoints + " Occurrence Points");
                    }
                    else if (occPoints >= 4 && occPoints < 5)
                    {
                        BIMessageBox.Show(firstName + " Has " + occPoints + " Occurrence Points " + (5 - occPoints) + " More Before " + earlyDate.ToShortDateString() + " Will Require A Written Progressive Counseling.");
                    }
                    else if (occPoints >= 5 && occPoints < 6)
                    {
                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occPoints + " Occurrence Points, Please Fill Out and Print This WRITTEN Warning Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                        if (exists == true)
                        {
                            Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
                        }
                        else
                        {
                            MessageBox.Show("Oops there was a problem trying to open the Progressive Counseling Form, Please contact Business Intelligence and let them know!");
                        }
                    }
                    else if (occPoints >= 6 && occPoints < 7)
                    {
                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occPoints + " Occurrence Points, Please Fill Out and Print This FINAL Warning Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                        if (exists == true)
                        {
                            Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
                        }
                        else
                        {
                            MessageBox.Show("Oops there was a problem trying to open the Progressive Counseling Form, Please contact Business Intelligence and let them know!");
                        }
                    }
                    else
                    {
                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occPoints + " Occurrence Points, Please Print This DISCHARGE Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
                    }
                    break;

                case 1:
                    if (violationText == "No Call No Show")
                    {
                        BIMessageBox.Show("Termination Form Dialog", "This Update to No Call No Show For The Associate In Their Probationary Period Requires Termination" +
                            " Please Fill Out and Print This Form", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
                        //this.Close();
                        return;
                    }

                    if (occPoints < 1)
                    {
                        BIMessageBox.Show("Warning Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occPoints + " Occurrence Points.  " + (1 - occPoints) +
                            " Will Require A Written Progressive Counseling", MessageBoxButton.OK);
                    }

                    else if (occPoints >= 1 && occPoints < 2)
                    {
                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occPoints + " Occurrence Points, Please Please Fill Out" +
                                " and Print This FINAL Warning Form That I will Open For You", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
                    }
                    else
                    {
                        BIMessageBox.Show("Termination Form Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occPoints + " Occurrence Points, Please Print This DISCHARGE Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
                    }
                    break;
            };
        }
        #endregion

        #region Private Methods 
        private void SearchTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MultipleOccurrenceView.Visibility = Visibility.Collapsed;
            SearchOccDisplayGrid.Visibility = Visibility.Collapsed;
            MultipleLOAView.Visibility = Visibility.Collapsed;
            LOADisplayGrid.Visibility = Visibility.Collapsed;
            MultipleCashHandleView.Visibility = Visibility.Collapsed;
            CashHandleDisplayGrid.Visibility = Visibility.Collapsed;
            NameSearchButton.Visibility = Visibility.Visible;
            searchTable = (ComboBoxItem)SearchTypeCB.SelectedItem;
            searchTableItem = searchTable.Content.ToString();
            selectedSearchType = SearchTypeCB.SelectedIndex;

            AGNESEntity agnesdb = new AGNESEntity();
        }

        private void SearchByCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MultipleOccurrenceView.Visibility = Visibility.Collapsed;
            SearchOccDisplayGrid.Visibility = Visibility.Collapsed;
            MultipleLOAView.Visibility = Visibility.Collapsed;
            LOADisplayGrid.Visibility = Visibility.Collapsed;
            MultipleCashHandleView.Visibility = Visibility.Collapsed;
            selectedSearch = SearchByCB.SelectedIndex;

            if (selectedSearch == 0)
            {
                SearchNumberGrid.Visibility = Visibility.Visible;
                SearchNameGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                SearchNameGrid.Visibility = Visibility.Visible;
                SearchNumberGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void NameSearchButton_Click(object sender, RoutedEventArgs e)
        {

            Button button = (Button)e.OriginalSource;
            //UpdateButton.Visibility = Visibility.Collapsed;
            MultipleOccurrenceView.Visibility = Visibility.Collapsed;
            MultipleLOAView.Visibility = Visibility.Collapsed;
            MultipleCashHandleView.Visibility = Visibility.Collapsed;
            MultipleOccurrencDG.Visibility = Visibility.Collapsed;
            MultipleLOADG.Visibility = Visibility.Collapsed;
            MultipleCashHandleDG.Visibility = Visibility.Collapsed;
            LOADisplayGrid.Visibility = Visibility.Collapsed;
            SearchOccDisplayGrid.Visibility = Visibility.Collapsed;
            CashHandleDisplayGrid.Visibility = Visibility.Collapsed;

            lastName = LastNameBox.Text;
            firstName = FirstNameBox.Text;

            string buttonName = button.Name;

            AGNESEntity agnesdb = new AGNESEntity();
            BIEntity bidb = new BIEntity();

            if (SearchTypeCB.SelectedIndex == -1)
            {
                MessageBox.Show("Please Select What You Would Like to Search For");
                return;
            }

            //we will always search by associate number behind the scenes.
            //get first and last name and return associate number.
            if (lastName == null || lastName == "")
            {
                assocNumber = Convert.ToInt64(AssociateNumberBox.Text);
                SearchDb(buttonName, searchTable.Content.ToString(), assocNumber);
            }
            else
            {
                var query = from employeeTable in MainWindow.bidb.EmployeeLists
                            where employeeTable.FirstName == firstName && employeeTable.LastName == lastName
                            select employeeTable;

                var result = query.ToList();

                //if (result.Count > 1)
                //{
                    //MultipleNameDG.Visibility = Visibility.Visible;
                    //MultipleNameDG.ItemsSource = result;

               // }
                //else
                //{
                    long queryMain = (from employeeTable in MainWindow.bidb.EmployeeLists
                                      where employeeTable.FirstName == firstName && employeeTable.LastName == lastName
                                      select employeeTable.PersNumber).SingleOrDefault();

                    assocNumber = queryMain;
                    SearchDb(buttonName, searchTable.Content.ToString(), assocNumber);
                //}
                //assocNumber = GetAssocNumber(firstName, lastName);
            }
        }

        private void PID_CellClicked(object sender, MouseButtonEventArgs e)
        {
            //UpdateButton.Visibility = Visibility.Visible;
            MultipleOccurrencDG.Visibility = Visibility.Collapsed;
            AttLabel.Visibility = Visibility.Collapsed;
            OccCB.Visibility = Visibility.Collapsed;
            string attendanceViolation;
           
            if (MultipleOccurrencDG.SelectedItem == null) return;

            object row = MultipleOccurrencDG.SelectedValue;
            selectedOccurrence = (MultipleOccurrencDG.SelectedCells[0].Column.GetCellContent(row) as TextBlock).Text;
            SelectOccurrence = Int64.Parse(selectedOccurrence);

            AGNESEntity agnesdb = new AGNESEntity();

            //find corresponding occurrence in table
            var query = from FilteredOccTable in agnesdb.Occurrences
                        where SelectOccurrence == FilteredOccTable.PID
                        select FilteredOccTable;

            var result = query.ToList();

            foreach (var filteredRow in result)
            {
                //SearchOccDisplayGrid.Visibility = Visibility.Visible;
                //OccNumber.Text = filteredRow.PersNumber.ToString();
                //OccName.Text = filteredRow.FirstName + " " + filteredRow.LastName;
                //OccDate.SelectedDate = filteredRow.Date;
                //CHOccNumber.Text = filteredRow.PID.ToString();
                //violationAmount = filteredRow.Type;

                if (filteredRow.AttendanceViolation != null)
                {
                    OccNumberV = filteredRow.PID.ToString();
                    OccDateV = filteredRow.Date;
                    OccAttViolation = filteredRow.AttendanceViolation;
                    OccType = filteredRow.Type;
                    OccNotesV = filteredRow.Notes;


                //    attendanceViolation = filteredRow.AttendanceViolation.ToString();
                //    OccCB.Visibility = Visibility.Visible;
                //    AttLabel.Visibility = Visibility.Visible;
                //    OccCB.SelectedIndex = SetIndex(attendanceViolation);
                //}
                //else
                //{
                //    OccCB.SelectedIndex = -1;
                //    OccCB.Visibility = Visibility.Collapsed;
                //    AttLabel.Visibility = Visibility.Collapsed;
                }

                //byte? type = filteredRow.Type;
                
                //OccNotes.Text = filteredRow.Notes;
            }

            EmpNumber = (int)assocNumber;
            Window newWindow = new HROccurrence(firstName, EmpNumber, empInProbation, 1);
            newWindow.ShowDialog();
            //UpdateButton.Visibility = Visibility.Visible;
        }

        private void LOA_PID_CellClicked(object sender, MouseButtonEventArgs e)
        {
            //DateTime? dateStart;
            //DateTime? dateEnd;
            MultipleLOADG.Visibility = Visibility.Collapsed;
            //UpdateButton.Visibility = Visibility.Visible;
            if (MultipleLOADG.SelectedItem == null) return;

            object row = MultipleLOADG.SelectedValue;
            selectedOccurrence = (MultipleLOADG.SelectedCells[0].Column.GetCellContent(row) as TextBlock).Text;
            SelectOccurrence = Int64.Parse(selectedOccurrence);

            AGNESEntity agnesdb = new AGNESEntity();

            //find corresponding LOA in table
            var query = from FilteredOccTable in agnesdb.LOAs
                        where SelectOccurrence == FilteredOccTable.PID
                        select FilteredOccTable;

            var result = query.ToList();

            foreach (var filteredRow in result)
            {
                //LOADisplayGrid.Visibility = Visibility.Visible;
                //LeaveNumber.Text = filteredRow.PID.ToString();
                //dateStart = filteredRow.DateStart;
                //dateEnd = filteredRow.DateEnd;
                //BeginLeave.SelectedDate = dateStart;
                //EndLeave.SelectedDate = dateEnd;

                if (filteredRow.Pending == 1)
                {
                    PendingBox.IsChecked = true;
                }
                if (filteredRow.Approved == 1)
                {
                    ApprovedBox.IsChecked = true;
                }
                if (filteredRow.Closed == 1)
                {
                    ClosedBox.IsChecked = true;
                }
                if (filteredRow.Parental == 1)
                {
                    ParentalBox.IsChecked = true;
                }

                //LOANote.Text = filteredRow.Notes;
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

            }
            EmpNumber = (int)assocNumber;
            Window newWindow = new HRLeave(firstName, EmpNumber, 1);
            newWindow.ShowDialog();
            //UpdateButton.Visibility = Visibility.Visible;
            //UpdateButton.Visibility = Visibility.Visible;
        }

        private void CashCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CashHandle_PID_CellClicked(object sender, MouseButtonEventArgs e)
        {
            //UpdateButton.Visibility = Visibility.Visible;
            MultipleCashHandleDG.Visibility = Visibility.Collapsed;
            //int EmpNumber;
            //int PID;

            if (MultipleCashHandleDG.SelectedItem == null) return;

            object row = MultipleCashHandleDG.SelectedValue;
            selectedOccurrence = (MultipleCashHandleDG.SelectedCells[0].Column.GetCellContent(row) as TextBlock).Text;
            long SelectOccurrence = Int64.Parse(selectedOccurrence);

            AGNESEntity agnesdb = new AGNESEntity();

            //find corresponding occurrence in table
            var query = from FilteredCHTable in agnesdb.CashHandles
                        where SelectOccurrence == FilteredCHTable.PID
                        select FilteredCHTable;

            var result = query.ToList();

            foreach (var filteredRow in result)
            {
                //CashHandleDisplayGrid.Visibility = Visibility.Visible;
                //CashHandleNumber.Text = filteredRow.PID.ToString();
                //CashCB.SelectedIndex = Convert.ToInt32(filteredRow.Type);
                //CHOccurrenceDP.SelectedDate = filteredRow.Date;
                //CHNote.Text = filteredRow.Notes;
                //violationAmount = filteredRow.Type;

                //set public static variable to carry from here to LOA Window when datagrid selection is made
                CashHandleNumberV = filteredRow.PID.ToString();
                CHSelectedIndexV = Convert.ToInt32(filteredRow.Type);
                CHDateV = filteredRow.Date;
                CHNoteV = filteredRow.Notes;
               
            }
            EmpNumber = (int)assocNumber;
            Window newWindow = new HRCashHandle(firstName, EmpNumber, empInProbation, 1);
            newWindow.ShowDialog();
            //UpdateButton.Visibility = Visibility.Visible;
        }

        private void MultipleNameDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object row = MultipleNameDG.SelectedValue;
            string AssocNumber = (MultipleNameDG.SelectedCells[0].Column.GetCellContent(row) as TextBlock).Text;
            assocNumber = Convert.ToInt64(AssocNumber);
            SearchDb(null, searchTable.Content.ToString(), assocNumber);
        } 

        private void FirstNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonGrid.Visibility = Visibility.Visible;
            if (SearchByCB.SelectedIndex == 0)
            {
                NumberSearchButton.Visibility = Visibility.Visible;
            }
            else
            {
                NameSearchButton.Visibility = Visibility.Visible;
            }
        }

        private void SearchDb(string button, string table, long? assocNumber)
        {
            AGNESEntity agnesdb = new AGNESEntity();
            BIEntity bidb = new BIEntity();
            MultipleNameDG.Visibility = Visibility.Collapsed;
            DateTime hireDate;
            

            //if we are searching by name we cant search without a name
            if (button == "NameSearchButton") //button.Name
            {
                if (lastName == null || lastName == "" || firstName == null || firstName == "")
                {
                    MessageBox.Show("Please Enter First and Last Name");
                    return;
                }
            }

            //if we are searching by associate number we cant search without a number
            if (button == "NumberSearchButton") //button.Name
            {
                if (assocNumber == null)
                {
                    MessageBox.Show("Please Enter a Valid Associate Number");
                    return;
                }
                else
                {
                    //if we are searching by the associate number we still need to get the associates first name
                    firstName = (from EmployeeTable in bidb.EmployeeLists
                                 where assocNumber == EmployeeTable.PersNumber
                                 select EmployeeTable.FirstName).SingleOrDefault();
                }
            }

            hireDate = (from EmployeeTable in bidb.EmployeeLists
                        where assocNumber == EmployeeTable.PersNumber
                        select EmployeeTable.DateOfHire).SingleOrDefault();

            if (hireDate.AddDays(90) >= today)
            {
                empInProbation = 1;
            }

            #region Occurrence Search
            if (table == "Occurrence Search")
            {
                var query = from occTable in agnesdb.Occurrences
                            where assocNumber == occTable.PersNumber 
                            orderby occTable.Date ascending
                            select occTable;

                var result = query.ToList();

                if (result.Count <= 0)
                {
                    MessageBox.Show("There Are No Occurrences For " + firstName);
                    return;
                }
                else
                {

                    MultipleOccurrenceView.Visibility = Visibility.Visible;
                    MultipleOccurrencDG.Visibility = Visibility.Visible;
                    MultipleOccurrencDG.ItemsSource = result;
                    //UpdateButton.Visibility = Visibility.Visible;

                    #region DELETE AFTER TESTING
                    //if (result.Count == 1)
                    //{
                    //string attendanceViolation;
                    //MultipleOccurrenceView.Visibility = Visibility.Visible;
                    //SearchOccDisplayGrid.Visibility = Visibility.Visible;
                    //foreach (var row in result)
                    //{
                    //    OccNumber.Text = row.PersNumber.ToString();
                    //    OccName.Text = row.FirstName + " " + row.LastName;
                    //    OccDate.SelectedDate = row.Date;
                    //    CHOccNumber.Text = row.PID.ToString();
                    //    violationAmount = row.Type;

                    //    if (row.AttendanceViolation != null)
                    //    {
                    //        OccCB.Visibility = Visibility.Visible;
                    //        AttLabel.Visibility = Visibility.Visible;
                    //        attendanceViolation = row.AttendanceViolation.ToString();

                    //        //set the selected index to match what is in DB
                    //        OccCB.SelectedIndex = SetIndex(attendanceViolation);
                    //    }
                    //    else
                    //    {
                    //        OccCB.SelectedIndex = -1;
                    //        OccCB.Visibility = Visibility.Collapsed;
                    //        AttLabel.Visibility = Visibility.Collapsed;
                    //    }
                    //    //get type value from db to select correct radio button
                    //    byte? type = row.Type;

                    //    OccNotes.Text = row.Notes;
                    //}
                    //
                    //}
                    //else
                    //{
                    //    MultipleOccurrenceView.Visibility = Visibility.Visible;
                    //    MultipleOccurrencDG.Visibility = Visibility.Visible;
                    //    MultipleOccurrencDG.ItemsSource = result;
                    //}
# endregion 
                }
            }
            #endregion

            #region LOA Search 
            else if (table == "LOA Search")
            {
                var query = from LOATable in agnesdb.LOAs
                            where assocNumber == LOATable.PersNumber 
                            orderby LOATable.DateStart ascending
                            select LOATable;

                var result = query.ToList();

                if (result.Count <= 0)
                {
                    MessageBox.Show("There Are No Leave of Abscenses For " + firstName);
                    return;
                }
                else
                {
                    MultipleLOAView.Visibility = Visibility.Visible;
                    MultipleLOADG.Visibility = Visibility.Visible;
                    MultipleLOADG.ItemsSource = result;
                    //UpdateButton.Visibility = Visibility.Visible;

                    #region DELETE AFTER TESTING
                    //if (result.Count == 1)
                    //{
                    //MultipleLOAView.Visibility = Visibility.Visible;
                    //LOADisplayGrid.Visibility = Visibility.Visible;
                    //foreach (var row in result)
                    //{
                    //    LeaveNumber.Text = row.PID.ToString();
                    //    BeginLeave.SelectedDate = row.DateStart;
                    //    EndLeave.SelectedDate = row.DateEnd;
                    //    LOANote.Text = row.Notes;
                    //    if (row.Pending == 1)
                    //    {
                    //        PendingBox.IsChecked = true;
                    //    }
                    //    if (row.Approved == 1)
                    //    {
                    //        ApprovedBox.IsChecked = true;
                    //    }
                    //    if (row.Closed == 1)
                    //    {
                    //        ClosedBox.IsChecked = true;
                    //    }
                    //    if (row.Parental == 1)
                    //    {
                    //        ParentalBox.IsChecked = true;
                    //    }
                    //    if (row.Intermittent == 1)
                    //    {
                    //        InterBox.IsChecked = true;
                    //    }
                    //    if (row.Continuous == 1)
                    //    {
                    //        ContBox.IsChecked = true;
                    //    }
                    //}
                    //
                    //}
                    //else
                    //{
                    //    MultipleLOAView.Visibility = Visibility.Visible;
                    //    MultipleLOADG.Visibility = Visibility.Visible;
                    //    MultipleLOADG.ItemsSource = result;
                    //}
                    #endregion
                }
            }
            #endregion

            #region Cash Handle Search
            else
            {
                var query = from CashTable in agnesdb.CashHandles
                            where assocNumber == CashTable.PersNumber 
                            orderby CashTable.Date ascending
                            select CashTable;

                var result = query.ToList();
               
                if (result.Count <= 0)
                {
                    MessageBox.Show("There Are No Cash Handling Violations For " + firstName);
                    return;
                }
                else
                {
                    MultipleCashHandleView.Visibility = Visibility.Visible;
                    MultipleCashHandleDG.Visibility = Visibility.Visible;
                    MultipleCashHandleDG.ItemsSource = result;
                    //UpdateButton.Visibility = Visibility.Visible;

                    #region DELETE AFTER TESTING
                    //if (result.Count == 1)
                    //{
                    //MultipleCashHandleView.Visibility = Visibility.Visible;
                    //CashHandleDisplayGrid.Visibility = Visibility.Visible;
                    //foreach (var row in result)
                    //{
                    //    CashHandleNumber.Text = row.PID.ToString();
                    //    CashCB.SelectedIndex = Convert.ToInt32(row.Type);
                    //    CHOccurrenceDP.SelectedDate = row.Date;
                    //    CHNote.Text = row.Notes;
                    //    violationAmount = row.Type;
                    //}
                    //
                    //}
                    //else
                    //{
                    //    MultipleCashHandleView.Visibility = Visibility.Visible;
                    //    MultipleCashHandleDG.Visibility = Visibility.Visible;
                    //    MultipleCashHandleDG.ItemsSource = result;
                    //}
                    #endregion
                }
            }
            #endregion
        }
        #endregion
    }
}

#region DELETE ME
//private long GetAssocNumber(string firstName, string lastName)
//{
//    //long PersNumber = (from employeeTable in MainWindow.bidb.EmployeeLists
//    //                  where employeeTable.FirstName == firstName && employeeTable.LastName == lastName
//    //                  select employeeTable.PersNumber).SingleOrDefault();



//    //return PersNumber;
//}

//through here

//if (violationText == "No Call No Show" && date != new DateTime(1001, 1, 1))
//{
//    BIMessageBox.Show("No Call No Show Dialog", "This No Call No Show is " + firstName + "'s Second In Less Than a Year And Requires Termination.  Please Fill Out And Print This Progressive Counseling and Separation Form", MessageBoxButton.OK);
//    Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
//    Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
//    this.Close();
//    return;
//}

//if (violationText == "No Call No Show")
//{
//    BIMessageBox.Show("No Call No Show Dialog", "This No Call No Show Requires An Automatic Written Progressive Counseling, Please Fill Out And Print This Form", MessageBoxButton.OK);
//    Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
//}

//private void UpdateButton_Click(object sender, RoutedEventArgs e)
//{
//AGNESEntity agnesdb = new AGNESEntity();

////Occurrence Violation Selected
//if (selectedSearchType == 0)
//{
//    string selectOccurrence = CHOccNumber.Text;
//    byte type = 0;
//    string violationText;
//    SelectOccurrence = Convert.ToInt64(selectOccurrence);

//    using (var db = new AGNESEntity())
//    {
//        var result = db.Occurrences.SingleOrDefault(f => f.PID == SelectOccurrence);
//        if (result != null)
//        {
//            byte? oldType = result.Type;
//            ComboBoxItem cbi = new ComboBoxItem();
//            cbi = (ComboBoxItem)OccCB.SelectedItem;
//            type = Convert.ToByte(cbi.Tag);
//            violationText = cbi.Content.ToString();
//            result.AttendanceViolation = violationText;
//            result.Type = type;
//            result.Date = OccDate.SelectedDate;
//            result.Notes = OccNotes.Text;

//            DateTime date = (DateTime)OccDate.SelectedDate;
//            (DateTime earlyDate, double? occPoints) = HROccurrence.CountOccurrences(date, (long)assocNumber, selectedSearchType);

//            if (type < violationAmount)
//            {
//                decimal compareOccPoints = (decimal)occPoints;
//                decimal compareType = (decimal)type;
//                decimal quotientOldType = (decimal)oldType / 2;
//                decimal quotientCompareType = (decimal)compareType / 2;
//                var messageBoxResult = BIMessageBox.Show("Occurrence Point Reduction Dialog", "The Selected Violation Will Result In A Reduction Of Occurrence Points From " + occPoints +
//                   " To " + (compareOccPoints - (quotientOldType - quotientCompareType)) + " For " + firstName + " and May Require Removal of A Written Counseling, Do You Wish To Continue?", MessageBoxButton.YesNo);

//                if (messageBoxResult != MessageBoxResult.Yes) return;
//                db.SaveChanges();
//                MessageBox.Show("Occurrence Record Has Been Updated.");
//                (earlyDate, occPoints) = HROccurrence.CountOccurrences(date, (long)assocNumber, selectedSearchType);
//                Report(firstName, violationText, occPoints, empInProbation, earlyDate, null, null);
//            }
//            else
//            {
//                try
//                {
//                    db.SaveChanges();
//                    MessageBox.Show("Occurrence Record Has Been Updated.");
//                    (earlyDate, occPoints) = HROccurrence.CountOccurrences(date, (long)assocNumber, selectedSearchType);
//                    Report(firstName, violationText, occPoints, empInProbation, earlyDate, null, null);

//                    #region DELETE ME AFTER TESTING COMPLETE
//                    //get Write up form ready
//                    //FileInfo myFile = new FileInfo(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
//                    //bool exists = myFile.Exists;

//                    //switch (empInProbation)
//                    //{
//                    //    case 0:

//                    //        if (violationText == "No Call No Show")
//                    //        {
//                    //            BIMessageBox.Show("No Call No Show Dialog", "This No Call No Show Requires An Automatic Written Progressive Counseling, Please Fill Out And Print This Form", MessageBoxButton.OK);
//                    //            Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
//                    //        }

//                    //        if (occPoints < 4)
//                    //        {
//                    //            MessageBox.Show(firstName + " Has " + occPoints + " Occurrence Points");
//                    //        }
//                    //        else if (occPoints >= 4 && occPoints < 5)
//                    //        {
//                    //            BIMessageBox.Show(firstName + " Has " + occPoints + " Occurrence Points " + (5 - occPoints) + " More Before " + earlyDate.ToShortDateString() + " Will Require A Written Progressive Counseling.");
//                    //        }
//                    //        else if (occPoints >= 5 && occPoints < 6)
//                    //        {
//                    //            BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occPoints + " Occurrence Points, Please Fill Out and Print This WRITTEN Warning Form" +
//                    //                    "That I Will Open For You", MessageBoxButton.OK);
//                    //            if (exists == true)
//                    //            {
//                    //                Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
//                    //            }
//                    //            else
//                    //            {
//                    //                MessageBox.Show("Oops there was a problem trying to open the Progressive Counseling Form, Please contact Business Intelligence and let them know!");
//                    //            }
//                    //        }
//                    //        else if (occPoints >= 6 && occPoints < 7)
//                    //        {
//                    //            BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occPoints + " Occurrence Points, Please Fill Out and Print This FINAL Warning Form" +
//                    //                    "That I Will Open For You", MessageBoxButton.OK);
//                    //            if (exists == true)
//                    //            {
//                    //                Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
//                    //            }
//                    //            else
//                    //            {
//                    //                MessageBox.Show("Oops there was a problem trying to open the Progressive Counseling Form, Please contact Business Intelligence and let them know!");
//                    //            }
//                    //        }
//                    //        else
//                    //        {
//                    //            BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occPoints + " Occurrence Points, Please Print This DISCHARGE Form" +
//                    //                    "That I Will Open For You", MessageBoxButton.OK);
//                    //            Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
//                    //        }
//                    //        break;

//                    //    case 1:
//                    //        if (violationText == "No Call No Show")
//                    //        {
//                    //            BIMessageBox.Show("Termination Form Dialog", "This Update to No Call No Show For The Associate In Their Probationary Period Requires Termination" +
//                    //                " Please Fill Out and Print This Form", MessageBoxButton.OK);
//                    //            Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
//                    //            this.Close();
//                    //            return;
//                    //        }

//                    //        if (occPoints < 1)
//                    //        {
//                    //            BIMessageBox.Show("Warning Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occPoints + " Occurrence Points.  " + (1 - occPoints) +
//                    //                " Will Require A Written Progressive Counseling", MessageBoxButton.OK);
//                    //        }

//                    //        else if (occPoints >= 1 && occPoints < 2)
//                    //        {
//                    //            BIMessageBox.Show("Counseling Form Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occPoints + " Occurrence Points, Please Please Fill Out" +
//                    //                    " and Print This FINAL Warning Form That I will Open For You", MessageBoxButton.OK);
//                    //            Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
//                    //        }
//                    //        else
//                    //        {
//                    //            BIMessageBox.Show("Termination Form Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occPoints + " Occurrence Points, Please Print This DISCHARGE Form" +
//                    //                    "That I Will Open For You", MessageBoxButton.OK);
//                    //            Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
//                    //        }
//                    //        break;
//                    //};
//                    #endregion
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show("There was a problem updating the OCCURRENCE record in the database please contact Business Intelligence " + ex);
//                }
//            }
//        }
//    }
//}

//Leave Of Absense Selected
//else if (selectedSearchType == 1)
//{
//    string selectOccurrence = LeaveNumber.Text;
//    SelectOccurrence = Convert.ToInt64(selectOccurrence);
//    byte PendingBoxValue = 0;
//    byte ApprovedBoxValue = 0;
//    byte ClosedBoxValue = 0;
//    byte ParentalBoxValue = 0;
//    byte InterBoxValue = 0;
//    byte ContBoxValue = 0;
//    if (PendingBox.IsChecked == true)
//    {
//        PendingBoxValue = 1;
//    }
//    if (ApprovedBox.IsChecked == true)
//    {
//        ApprovedBoxValue = 1;
//    }
//    if (ClosedBox.IsChecked == true)
//    {
//        ClosedBoxValue = 1;
//    }
//    if (ParentalBox.IsChecked == true)
//    {
//        ParentalBoxValue = 1;
//    }
//    if (InterBox.IsChecked == true)
//    {
//        InterBoxValue = 1;
//    }
//    if (ContBox.IsChecked == true)
//    {
//        ContBoxValue = 1;
//    }
//    using (var db = new AGNESEntity())
//    {
//        var result = db.LOAs.SingleOrDefault(f => f.PID == SelectOccurrence);
//        if (result != null)
//        {
//            result.Intermittent = InterBoxValue;
//            result.Continuous = ContBoxValue;
//            result.Pending = PendingBoxValue;
//            result.Approved = ApprovedBoxValue;
//            result.Closed = ClosedBoxValue;
//            result.Parental = ParentalBoxValue;
//            result.DateStart = BeginLeave.SelectedDate;
//            result.DateEnd = EndLeave.SelectedDate;
//            result.Notes = LOANote.Text;

//            if (BeginLeave.SelectedDate == null || BeginLeave.SelectedDate == null)
//            {
//                MessageBox.Show("Please Enter a Beginning Date AND Estimated Ending Date For The Leave");
//                return;
//            }
//            if (BeginLeave.SelectedDate > EndLeave.SelectedDate || BeginLeave.SelectedDate == EndLeave.SelectedDate)
//            {
//                MessageBox.Show("The Ending Date For The Leave Must Be After The Begin Date For The Leave");
//                return;
//            }

//            if (PendingBox.IsChecked == false && ApprovedBox.IsChecked == false && ClosedBox.IsChecked == false && ParentalBox.IsChecked == false)
//            {
//                MessageBox.Show("Please Select Pending, Approved, or Closed");
//                return;
//            }

//            if (PendingBox.IsChecked == true && ApprovedBox.IsChecked == true || PendingBox.IsChecked == true && ClosedBox.IsChecked == true || ApprovedBox.IsChecked == true && ClosedBox.IsChecked == true)
//            {
//                MessageBox.Show("There Can Only Be One Option of Pending, Approved, or Closed Selected At A Time");
//                return;
//            }

//            if (ParentalBox.IsChecked == true && PendingBox.IsChecked == true || ParentalBox.IsChecked == true && ApprovedBox.IsChecked == true || ParentalBox.IsChecked == true && ClosedBox.IsChecked == true || ParentalBox.IsChecked == true &&
//                InterBox.IsChecked == true || ParentalBox.IsChecked == true && ContBox.IsChecked == true)
//            {
//                MessageBox.Show("If Parental Leave is Selected, No Other Selections May be Made");
//                return;
//            }

//            if (InterBox.IsChecked == true && ContBox.IsChecked == true)
//            {
//                MessageBox.Show("There Can Only Intermittent or Continuous Leave, Both Cannot Be Selected at The Same Time, Please Select Just One");
//                return;
//            }
//            try
//            {

//                db.SaveChanges();
//                MessageBox.Show("Leave of Abscence Record Has Been Updated.");
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("There was a problem updating the LOA record in the database please contact Business Intelligence " + ex);
//            }
//        }
//    }
//}

//Cash Handle Occurrence Selected
//else
//{
//    byte type;

//    if (CashCB.SelectedIndex == 0)
//    {
//        type = 0;
//    }
//    else if (CashCB.SelectedIndex == 1)
//    {
//        type = 1;
//    }
//    else
//    {
//        type = 2;
//    }

//    using (var db = new AGNESEntity())
//    {
//        string selectOccurrence = CashHandleNumber.Text;
//        SelectOccurrence = Convert.ToInt64(selectOccurrence);

//        var result = db.CashHandles.SingleOrDefault(f => f.PID == SelectOccurrence);
//        if (result != null)
//        {
//            byte? oldType = result.Type;
//            result.Type = type;
//            result.Date = CHOccurrenceDP.SelectedDate;
//            result.Notes = CHNote.Text;
//            string violationNotes = CHNote.Text;

//            DateTime date = (DateTime)CHOccurrenceDP.SelectedDate;
//            (DateTime earlyDate, double? occPoints) = HROccurrence.CountOccurrences(date, (long)assocNumber, selectedSearchType);

//            if (type < violationAmount && type == 0)
//            {
//                var messageBoxResult = BIMessageBox.Show("Cash Handle Reduction", "This Change Will Require the Removal of A Written Counseling, Do You Wish To Continue?", MessageBoxButton.YesNo);
//                if (messageBoxResult != MessageBoxResult.Yes) return;
//                db.SaveChanges();
//                MessageBox.Show("Occurrence Record Has Been Updated.");
//                (earlyDate, occPoints) = HROccurrence.CountOccurrences(date, (long)assocNumber, selectedSearchType);
//                Report(firstName, violationNotes, occPoints, empInProbation, earlyDate, type, assocNumber);
//            }

//            else if (type < violationAmount)
//            {
//                decimal compareOccPoints = (decimal)occPoints;
//                decimal compareType = (decimal)type;
//                decimal quotientOldType = (decimal)oldType / 2;
//                decimal quotientCompareType = (decimal)compareType / 2;
//                var messageBoxResult = BIMessageBox.Show("Occurrence Point Reduction Dialog", "The Selected Violation Will Result In A Reduction Of Occurrence Points From " + occPoints +
//                   " To " + (compareOccPoints - (quotientOldType - quotientCompareType)) + " For " + firstName + " and May Require Removal of A Written Counseling, Do You Wish To Continue?", MessageBoxButton.YesNo);

//                if (messageBoxResult != MessageBoxResult.Yes) return;
//                db.SaveChanges();
//                MessageBox.Show("Occurrence Record Has Been Updated.");
//                (earlyDate, occPoints) = HROccurrence.CountOccurrences(date, (long)assocNumber, selectedSearchType);
//                //TODO: GO BACK TO THIS
//                Report(firstName, violationNotes, occPoints, empInProbation, earlyDate, type, assocNumber);
//            }
//            else
//            {
//                try
//                {
//                    db.SaveChanges();
//                    MessageBox.Show("Cash Handling Record Has Been Updated.");
//                    if (type == 1)
//                    {
//                        BIMessageBox.Show("Counseling Form Dialog", firstName + "'s Variance Type Was Changed To Between $3.00 and $20.00, This is an Automatic Progressive Counseling" +
//                                            " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
//                        Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
//                    }
//                    else
//                    {
//                        BIMessageBox.Show("Contact HRBP Dialong", "This Type of Cash Handling Violation Requires Notification of Your DM AND HRBP, Please Contact Them", MessageBoxButton.OK);

//                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has a Variance Greater Than $20.00 This is an Automatic Progressive Counseling" +
//                            " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
//                        Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show("There was a problem updating the CASH HANDLING record in the database please contact Business Intelligence " + ex);
//                }
//            }
//        }
//    }
//}
//}
#endregion
