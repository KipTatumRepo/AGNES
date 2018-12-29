﻿using AGNESCSharp.Entity_Models;
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
        int selectedSearch;
        int selectedSearchType;
        long SelectOccurrence;
        long? assocNumber;
        ComboBoxItem searchTable;
        string selectedOccurrence;
        string searchTableItem;
        string lastName;
        string firstName;
        Dictionary<string, int> cbDictionary = new Dictionary<string, int>();
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
            UpdateButton.Visibility = Visibility.Collapsed;
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

                if (result.Count > 1)
                {
                    MultipleNameDG.Visibility = Visibility.Visible;
                    MultipleNameDG.ItemsSource = result;

                }
                else
                {
                    long queryMain = (from employeeTable in MainWindow.bidb.EmployeeLists
                                      where employeeTable.FirstName == firstName && employeeTable.LastName == lastName
                                      select employeeTable.PersNumber).SingleOrDefault();

                    assocNumber = queryMain;//Convert.ToInt64(queryMain); //result[0]?? result[2].ToString()
                    SearchDb(buttonName, searchTable.Content.ToString(), assocNumber);

                }
                //assocNumber = GetAssocNumber(firstName, lastName);
            }



            #region TODO DELETE ME


            //#region Occurrence Search
            //if (searchTableItem == "Occurrence Search")
            //{
            //    var query = from occTable in agnesdb.Occurrences
            //                where assocNumber == occTable.PersNumber //firstName == occTable.FirstName && lastName == occTable.LastName
            //                orderby occTable.Date ascending
            //                select occTable;

            //    var result = query.ToList();

            //    if (result.Count <= 0)
            //    {
            //        MessageBox.Show("There Are No Occurrences For " + firstName);
            //        return;
            //    }
            //    else
            //    {
            //        if (result.Count == 1)
            //        {
            //            string attendanceViolation;
            //            MultipleOccurrenceView.Visibility = Visibility.Visible;
            //            SearchOccDisplayGrid.Visibility = Visibility.Visible;
            //            foreach (var row in result)
            //            {
            //                OccNumber.Text = row.PersNumber.ToString();
            //                OccName.Text = row.FirstName + " " + row.LastName;
            //                OccDate.SelectedDate = row.Date;
            //                CHOccNumber.Text = row.PID.ToString();

            //                if (row.AttendanceViolation != null)
            //                {
            //                    OccCB.Visibility = Visibility.Visible;
            //                    AttLabel.Visibility = Visibility.Visible;
            //                    attendanceViolation = row.AttendanceViolation.ToString();

            //                    //set the selected index to match what is in DB
            //                    OccCB.SelectedIndex = SetIndex(attendanceViolation);
            //                }
            //                else
            //                {
            //                    OccCB.SelectedIndex = -1;
            //                    OccCB.Visibility = Visibility.Collapsed;
            //                    AttLabel.Visibility = Visibility.Collapsed;
            //                }
            //                //get type value from db to select correct radio button
            //                byte? type = row.Type;

            //                //if (type == 1)
            //                //{
            //                //    OccHalf.IsChecked = true;
            //                //}
            //                //else
            //                //{
            //                //    OccFull.IsChecked = true;
            //                //}
            //                OccNotes.Text = row.Notes;
            //            }
            //            UpdateButton.Visibility = Visibility.Visible;
            //        }
            //        else
            //        {
            //            MultipleOccurrenceView.Visibility = Visibility.Visible;
            //            MultipleOccurrencDG.Visibility = Visibility.Visible;
            //            MultipleOccurrencDG.ItemsSource = result;
            //        }

            //    }
            //}
            //#endregion

            //#region LOA Search 
            //else if (searchTableItem == "LOA Search")
            //{
            //    var query = from LOATable in agnesdb.LOAs
            //                where assocNumber == LOATable.PersNumber //firstName == LOATable.FirstName && lastName == LOATable.LastName
            //                orderby LOATable.DateStart ascending
            //                select LOATable;

            //    var result = query.ToList();

            //    result = query.ToList();

            //    if (result.Count <= 0)
            //    {
            //        MessageBox.Show("There Are No Leave of Abscenses For " + firstName);
            //        return;
            //    }
            //    else
            //    {
            //        if (result.Count == 1)
            //        {
            //            MultipleLOAView.Visibility = Visibility.Visible;
            //            LOADisplayGrid.Visibility = Visibility.Visible;
            //            foreach (var row in result)
            //            {
            //                LeaveNumber.Text = row.PID.ToString();
            //                BeginLeave.SelectedDate = row.DateStart;
            //                EndLeave.SelectedDate = row.DateEnd;
            //                LOANote.Text = row.Notes;
            //                if (row.Pending == 1)
            //                {
            //                    PendingBox.IsChecked = true;
            //                }
            //                if (row.Approved == 1)
            //                {
            //                    ApprovedBox.IsChecked = true;
            //                }
            //                if (row.Closed == 1)
            //                {
            //                    ClosedBox.IsChecked = true;
            //                }
            //                if (row.Parental == 1)
            //                {
            //                    ParentalBox.IsChecked = true;
            //                }
            //                if (row.Intermittent == 1)
            //                {
            //                    InterBox.IsChecked = true;
            //                }
            //                if (row.Continuous == 1)
            //                {
            //                    ContBox.IsChecked = true;
            //                }
            //            }
            //            UpdateButton.Visibility = Visibility.Visible;
            //        }
            //        else
            //        {
            //            MultipleLOAView.Visibility = Visibility.Visible;
            //            MultipleLOADG.Visibility = Visibility.Visible;
            //            MultipleLOADG.ItemsSource = result;
            //        }
            //    }
            //}
            //#endregion

            //#region Cash Handle Search
            //else
            //{
            //    var query = from CashTable in agnesdb.CashHandles
            //                where assocNumber == CashTable.PersNumber //firstName == CashTable.FirstName && lastName == CashTable.LastName
            //                orderby CashTable.Date ascending
            //                select CashTable;

            //    var result = query.ToList();


            //    result = query.ToList();

            //    if (result.Count <= 0)
            //    {
            //        MessageBox.Show("There Are No Cash Handling Violations For " + firstName);
            //        return;
            //    }
            //    else
            //    {
            //        if (result.Count == 1)
            //        {
            //            MultipleCashHandleView.Visibility = Visibility.Visible;
            //            CashHandleDisplayGrid.Visibility = Visibility.Visible;
            //            foreach (var row in result)
            //            {
            //                CashHandleNumber.Text = row.PID.ToString();
            //                CashCB.SelectedIndex = Convert.ToInt32(row.Type);
            //                CHOccurrenceDP.SelectedDate = row.Date;
            //                CHNote.Text = row.Notes;
            //            }
            //            UpdateButton.Visibility = Visibility.Visible;
            //        }
            //        else
            //        {
            //            MultipleCashHandleView.Visibility = Visibility.Visible;
            //            MultipleCashHandleDG.Visibility = Visibility.Visible;
            //            MultipleCashHandleDG.ItemsSource = result;
            //        }

            //    }
            //}
            //#endregion

            #endregion
        }

        private void PID_CellClicked(object sender, MouseButtonEventArgs e)
        {
            UpdateButton.Visibility = Visibility.Visible;
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
                SearchOccDisplayGrid.Visibility = Visibility.Visible;
                OccNumber.Text = filteredRow.PersNumber.ToString();
                OccName.Text = filteredRow.FirstName + " " + filteredRow.LastName;
                OccDate.SelectedDate = filteredRow.Date;
                CHOccNumber.Text = filteredRow.PID.ToString();

                if (filteredRow.AttendanceViolation != null)
                {
                    attendanceViolation = filteredRow.AttendanceViolation.ToString();
                    OccCB.Visibility = Visibility.Visible;
                    AttLabel.Visibility = Visibility.Visible;
                    OccCB.SelectedIndex = SetIndex(attendanceViolation);
                }
                else
                {
                    OccCB.SelectedIndex = -1;
                    OccCB.Visibility = Visibility.Collapsed;
                    AttLabel.Visibility = Visibility.Collapsed;
                }

                byte? type = filteredRow.Type;
                
                OccNotes.Text = filteredRow.Notes;
            }
            UpdateButton.Visibility = Visibility.Visible;
        }

        private void LOA_PID_CellClicked(object sender, MouseButtonEventArgs e)
        {
            DateTime? dateStart;
            DateTime? dateEnd;
            MultipleLOADG.Visibility = Visibility.Collapsed;
            UpdateButton.Visibility = Visibility.Visible;
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
                LOADisplayGrid.Visibility = Visibility.Visible;
                LeaveNumber.Text = filteredRow.PID.ToString();
                dateStart = filteredRow.DateStart;
                dateEnd = filteredRow.DateEnd;
                BeginLeave.SelectedDate = dateStart;
                EndLeave.SelectedDate = dateEnd;

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

                LOANote.Text = filteredRow.Notes;

            }
            UpdateButton.Visibility = Visibility.Visible;
        }

        private void CashCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CashHandle_PID_CellClicked(object sender, MouseButtonEventArgs e)
        {
            UpdateButton.Visibility = Visibility.Visible;
            MultipleCashHandleDG.Visibility = Visibility.Collapsed;
            
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
                CashHandleDisplayGrid.Visibility = Visibility.Visible;
                CashHandleNumber.Text = filteredRow.PID.ToString();
                CashCB.SelectedIndex = Convert.ToInt32(filteredRow.Type);
                CHOccurrenceDP.SelectedDate = filteredRow.Date;
                CHNote.Text = filteredRow.Notes;
            }
            UpdateButton.Visibility = Visibility.Visible;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            AGNESEntity agnesdb = new AGNESEntity();

            //Occurrence Violation Selected
            if (selectedSearchType == 0)

            //Leave Of Absense Selected
            else if (selectedSearchType == 1)
            {
                string selectOccurrence = LeaveNumber.Text;
                SelectOccurrence = Convert.ToInt64(selectOccurrence);
                byte PendingBoxValue = 0;
                byte ApprovedBoxValue = 0;
                byte ClosedBoxValue = 0;
                byte ParentalBoxValue = 0;
                byte InterBoxValue = 0;
                byte ContBoxValue = 0;
                if (PendingBox.IsChecked == true)
                {
                    PendingBoxValue = 1;
                }
                if (ApprovedBox.IsChecked == true)
                {
                    ApprovedBoxValue = 1;
                }
                if (ClosedBox.IsChecked == true)
                {
                    ClosedBoxValue = 1;
                }
                if (ParentalBox.IsChecked == true)
                {
                    ParentalBoxValue = 1;
                }
                if (InterBox.IsChecked == true)
                {
                    InterBoxValue = 1;
                }
                if (ContBox.IsChecked == true)
                {
                    ContBoxValue = 1;
                }
                using (var db = new AGNESEntity())
                {
                    var result = db.LOAs.SingleOrDefault(f => f.PID == SelectOccurrence);
                    if (result != null)
                    {
                        result.Intermittent = InterBoxValue;
                        result.Continuous = ContBoxValue;
                        result.Pending = PendingBoxValue;
                        result.Approved = ApprovedBoxValue;
                        result.Closed = ClosedBoxValue;
                        result.Parental = ParentalBoxValue;
                        result.DateStart = BeginLeave.SelectedDate;
                        result.DateEnd = EndLeave.SelectedDate;
                        result.Notes = LOANote.Text;
                        try
                        {
                            db.SaveChanges();
                            MessageBox.Show("Leave of Abscence Record Has Been Updated.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("There was a problem updating the LOA record in the database please contact Business Intelligence " + ex);
                        }
                    }
                }
            }

            //Cash Handle Occurrence Selected
            else
            {
                byte type;

                if (CashCB.SelectedIndex == 0)
                {
                    type = 0;
                }
                else if (CashCB.SelectedIndex == 1)
                {
                    type = 1;
                }
                else
                {
                    type = 2;
                }
                using (var db = new AGNESEntity())
                {
                    string selectOccurrence = CashHandleNumber.Text;
                    SelectOccurrence = Convert.ToInt64(selectOccurrence);

                    var result = db.CashHandles.SingleOrDefault(f => f.PID == SelectOccurrence);
                    if (result != null)
                    {
                        result.Type = type;
                        result.Date = CHOccurrenceDP.SelectedDate;
                        result.Notes = CHNote.Text;
                        try
                        {
                            db.SaveChanges();
                            MessageBox.Show("Cash Handling Record Has Been Updated.");
                            if (type == 2)
                            {
                                BIMessageBox.Show("Counseling Form Dialog", firstName + "'s Variance Type Was Changed To Greater Than $3.00 but Less Than $20.00, This is an Automatic Progressive Counseling" +
                                                    " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
                                Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("There was a problem updating the CASH HANDLING record in the database please contact Business Intelligence " + ex);
                        }

                    }
                }
            }
        }

        private void MultipleNameDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object row = MultipleNameDG.SelectedValue;
            string AssocNumber = (MultipleNameDG.SelectedCells[0].Column.GetCellContent(row) as TextBlock).Text;
            assocNumber = Convert.ToInt64(AssocNumber);
            SearchDb(null, searchTable.Content.ToString(), assocNumber);
        } 

        private int SetIndex(string cbSelection)
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

        //private long GetAssocNumber(string firstName, string lastName)
        //{
        //    //long PersNumber = (from employeeTable in MainWindow.bidb.EmployeeLists
        //    //                  where employeeTable.FirstName == firstName && employeeTable.LastName == lastName
        //    //                  select employeeTable.PersNumber).SingleOrDefault();



        //    //return PersNumber;
        //}

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
                    if (result.Count == 1)
                    {
                        string attendanceViolation;
                        MultipleOccurrenceView.Visibility = Visibility.Visible;
                        SearchOccDisplayGrid.Visibility = Visibility.Visible;
                        foreach (var row in result)
                        {
                            OccNumber.Text = row.PersNumber.ToString();
                            OccName.Text = row.FirstName + " " + row.LastName;
                            OccDate.SelectedDate = row.Date;
                            CHOccNumber.Text = row.PID.ToString();

                            if (row.AttendanceViolation != null)
                            {
                                OccCB.Visibility = Visibility.Visible;
                                AttLabel.Visibility = Visibility.Visible;
                                attendanceViolation = row.AttendanceViolation.ToString();

                                //set the selected index to match what is in DB
                                OccCB.SelectedIndex = SetIndex(attendanceViolation);
                            }
                            else
                            {
                                OccCB.SelectedIndex = -1;
                                OccCB.Visibility = Visibility.Collapsed;
                                AttLabel.Visibility = Visibility.Collapsed;
                            }
                            //get type value from db to select correct radio button
                            byte? type = row.Type;
                            
                            OccNotes.Text = row.Notes;
                        }
                        UpdateButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MultipleOccurrenceView.Visibility = Visibility.Visible;
                        MultipleOccurrencDG.Visibility = Visibility.Visible;
                        MultipleOccurrencDG.ItemsSource = result;
                    }

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

                result = query.ToList();

                if (result.Count <= 0)
                {
                    MessageBox.Show("There Are No Leave of Abscenses For " + firstName);
                    return;
                }
                else
                {
                    if (result.Count == 1)
                    {
                        MultipleLOAView.Visibility = Visibility.Visible;
                        LOADisplayGrid.Visibility = Visibility.Visible;
                        foreach (var row in result)
                        {
                            LeaveNumber.Text = row.PID.ToString();
                            BeginLeave.SelectedDate = row.DateStart;
                            EndLeave.SelectedDate = row.DateEnd;
                            LOANote.Text = row.Notes;
                            if (row.Pending == 1)
                            {
                                PendingBox.IsChecked = true;
                            }
                            if (row.Approved == 1)
                            {
                                ApprovedBox.IsChecked = true;
                            }
                            if (row.Closed == 1)
                            {
                                ClosedBox.IsChecked = true;
                            }
                            if (row.Parental == 1)
                            {
                                ParentalBox.IsChecked = true;
                            }
                            if (row.Intermittent == 1)
                            {
                                InterBox.IsChecked = true;
                            }
                            if (row.Continuous == 1)
                            {
                                ContBox.IsChecked = true;
                            }
                        }
                        UpdateButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MultipleLOAView.Visibility = Visibility.Visible;
                        MultipleLOADG.Visibility = Visibility.Visible;
                        MultipleLOADG.ItemsSource = result;
                    }
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


                result = query.ToList();

                if (result.Count <= 0)
                {
                    MessageBox.Show("There Are No Cash Handling Violations For " + firstName);
                    return;
                }
                else
                {
                    if (result.Count == 1)
                    {
                        MultipleCashHandleView.Visibility = Visibility.Visible;
                        CashHandleDisplayGrid.Visibility = Visibility.Visible;
                        foreach (var row in result)
                        {
                            CashHandleNumber.Text = row.PID.ToString();
                            CashCB.SelectedIndex = Convert.ToInt32(row.Type);
                            CHOccurrenceDP.SelectedDate = row.Date;
                            CHNote.Text = row.Notes;
                        }
                        UpdateButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MultipleCashHandleView.Visibility = Visibility.Visible;
                        MultipleCashHandleDG.Visibility = Visibility.Visible;
                        MultipleCashHandleDG.ItemsSource = result;
                    }

                }
            }
            #endregion
        }
        #endregion
    }
}
