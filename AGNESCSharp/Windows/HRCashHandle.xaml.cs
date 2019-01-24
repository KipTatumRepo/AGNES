using AGNESCSharp.Entity_Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AGNESCSharp
{
    /// <summary>
    /// Interaction logic for HRCashHandle.xaml
    /// </summary>
    public partial class HRCashHandle : Window
    {
        #region Variables
        string nameToInsert;
        int empInProbation;
        int occPoint;
        long SelectOccurrence;
        private string empCostCenter;
        string firstName;
        string lastName;
        byte type;
        long? empID;
        int NavFromSearch;
        DateTime hireDate;
        DateTime selectedDate;
        #endregion

        #region Main
        public HRCashHandle(string emp, long? empNum, int empInProbationPeriod, int navFromSearch)
        {
            InitializeComponent();
            empID = empNum;
            empInProbation = empInProbationPeriod;
            NavFromSearch = navFromSearch;

            if (NavFromSearch == 0)
            {
                UpdateButton.Visibility = Visibility.Collapsed;
                nameToInsert = emp;
                var name = emp.Split(',');
                lastName = name[0].Trim();
                firstName = name[1].Trim();


                TopTextBox.Text = "Please Enter The Details For " + firstName + " " + lastName + "'S" + " Cash Handling Violation";

                var query = from employeeTable in MainWindow.bidb.EmployeeLists
                            where employeeTable.PersNumber == empNum
                            select employeeTable;

                var results = query.ToList();
                foreach (var result in query)
                {
                    hireDate = result.DateOfHire;
                    empCostCenter = result.CostCenter;
                }
                CHOccurrenceDP.DisplayDateStart = DateTime.Now.AddDays(-60);
                CHOccurrenceDP.DisplayDateEnd = DateTime.Now;
                //SaveButton.Visibility = Visibility.Visible;
                
            }
            else if (NavFromSearch == 1)
            {
                nameToInsert = emp;
                var name = emp.Split(',');
                firstName = name[0].Trim();
                SaveButton.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Collapsed;
                TopTextBox.Text = "Please Enter The Details For " + firstName + "'S" + " Cash Handling Violation";
                CashCB.SelectedIndex = HRSearch.CHSelectedIndexV;
                CHOccurrenceDP.SelectedDate = HRSearch.CHDateV;
                DescriptionTb.Text = HRSearch.CHNoteV;
                //UpdateButton.Visibility = Visibility.Visible;
            }
            else
            {
                nameToInsert = emp;
                var name = emp.Split(',');
                firstName = name[0].Trim();
                SaveButton.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Collapsed;
                TopTextBox.Text = "Please Enter The Details For " + firstName + "'S" + " Cash Handling Violation";
                CashCB.SelectedIndex = HRMgr.SelectedIndexV;
                CHOccurrenceDP.SelectedDate = HRMgr.CHDateV;
                DescriptionTb.Text = HRMgr.CHNoteV;
                //UpdateButton.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region Public Methods
        public static int CountZeroPoints(DateTime date, long? empID)
        {
            AGNESEntity agnesdb = new AGNESEntity();
            DateTime cutOffDate = date.AddYears(-1);

            var query = from employeeTable in agnesdb.CashHandles
                        where employeeTable.PersNumber == empID && employeeTable.Date >= cutOffDate && employeeTable.Type == 0
                        select employeeTable;

            int count = query.Count();
            return count;
        }
        #endregion

        #region Prvate Methods
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            CashHandle ch = new CashHandle();
            DateTime cutOffDate;
            ch.PersNumber = empID;
            ch.CostCenter = empCostCenter;
            ch.LastName = lastName;
            ch.FirstName = firstName;
            ch.Type = type;
            ch.Date = CHOccurrenceDP.SelectedDate;
            ch.Notes = DescriptionTb.Text;

            if (CHOccurrenceDP.SelectedDate == null || CashCB.SelectedIndex == -1)
            {
                if (CHOccurrenceDP.SelectedDate == null)
                {
                    MessageBox.Show("Please Select The Date of The Cash Handling Occurrence");
                }
                else
                {
                    MessageBox.Show("Please Select The Cash Handling Variance Amount");
                }
                return;
            }
            MainWindow.agnesdb.CashHandles.Add(ch);
            MainWindow.agnesdb.SaveChanges();
            MessageBox.Show("The Cash Handling Occurrence for " + firstName + " has been added");

            selectedDate = Convert.ToDateTime(CHOccurrenceDP.SelectedDate);
            cutOffDate = selectedDate.AddYears(-1);

            //right now I am calculating the earliest valid date as -1 year from incident date, this may change
            (DateTime earlyDate, double? occurrencePoints) = HROccurrence.CountOccurrences(selectedDate, empID, 1);
            int anyPriorZero = CountZeroPoints(selectedDate, empID);

            FileInfo myFile = new FileInfo(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
            bool exists = myFile.Exists;

            HRSearch.Report(firstName, null, null, empInProbation, earlyDate, type, empID);

            CashCB.SelectedItem = null;
            CHOccurrenceDP.SelectedDate = null;
            DescriptionTb.Clear();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CashCB.SelectedItem = null;
            CHOccurrenceDP.SelectedDate = null;
            DescriptionTb.Clear();
        }

        private void CashCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NavFromSearch == 0)
            {
                CancelButton.Visibility = Visibility.Visible;
                SaveButton.Visibility = Visibility.Visible;
            }
            else
            {
                UpdateButton.Visibility = Visibility.Visible;
            }

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
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
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
                if (NavFromSearch == 1)
                {
                    string selectOccurrence = HRSearch.CashHandleNumberV;
                    SelectOccurrence = Convert.ToInt64(selectOccurrence);
                }
                else
                {
                    string selectOccurrence = HRMgr.CashHandleNumberV;
                    SelectOccurrence = Convert.ToInt64(selectOccurrence);
                }

                var result = db.CashHandles.SingleOrDefault(f => f.PID == SelectOccurrence);
                if (result != null)
                {
                    byte? oldType = result.Type;
                    result.Type = type;
                    result.Date = CHOccurrenceDP.SelectedDate;
                    result.Notes = DescriptionTb.Text;
                    string violationNotes = DescriptionTb.Text;

                    DateTime date = (DateTime)CHOccurrenceDP.SelectedDate;
                    (DateTime earlyDate, double? occPoints) = HROccurrence.CountOccurrences(date, empID, 1);

                    if (type < oldType && type == 0)
                    {
                        var messageBoxResult = BIMessageBox.Show("Cash Handle Reduction", "This Change Will Require the Removal of A Written Counseling, Do You Wish To Continue?", MessageBoxButton.YesNo);
                        if (messageBoxResult != MessageBoxResult.Yes) return;
                        db.SaveChanges();
                        MessageBox.Show("Occurrence Record Has Been Updated.");
                        (earlyDate, occPoints) = HROccurrence.CountOccurrences(date, empID, 1);
                        HRSearch.Report(firstName, violationNotes, occPoints, empInProbation, earlyDate, type, empID);
                    }

                    else if (type < oldType)
                    {
                        decimal compareOccPoints = (decimal)occPoints;
                        decimal compareType = (decimal)type;
                        decimal quotientOldType = (decimal)oldType / 2;
                        decimal quotientCompareType = (decimal)compareType / 2;
                        var messageBoxResult = BIMessageBox.Show("Occurrence Point Reduction Dialog", "The Selected Violation Will Result In A Reduction Of Occurrence Points From " + occPoints +
                           " To " + (compareOccPoints - (quotientOldType - quotientCompareType)) + " For " + firstName + " and May Require Removal of A Written Counseling, Do You Wish To Continue?", MessageBoxButton.YesNo);

                        if (messageBoxResult != MessageBoxResult.Yes) return;
                        db.SaveChanges();
                        MessageBox.Show("Occurrence Record Has Been Updated.");
                        (earlyDate, occPoints) = HROccurrence.CountOccurrences(date, empID, 1);
                        HRSearch.Report(firstName, violationNotes, occPoints, empInProbation, earlyDate, type, empID);
                    }
                    else
                    {
                        try
                        {
                            db.SaveChanges();
                            MessageBox.Show("Cash Handling Record Has Been Updated.");
                            if (type == 1)
                            {
                                BIMessageBox.Show("Counseling Form Dialog", firstName + "'s Variance Type Was Changed To Between $3.00 and $20.00, This is an Automatic Progressive Counseling" +
                                                    " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
                                Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
                            }
                            else
                            {
                                BIMessageBox.Show("Contact HRBP Dialong", "This Type of Cash Handling Violation Requires Notification of Your DM AND HRBP, Please Contact Them", MessageBoxButton.OK);

                                BIMessageBox.Show("Counseling Form Dialog", firstName + " Has a Variance Greater Than $20.00 This is an Automatic Progressive Counseling" +
                                    " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
                                Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("There was a problem updating the CASH HANDLING record in the database please contact Business Intelligence " + ex);
                        }
                    }
                }
            }
            this.Close();
        }
        #endregion
    }
}

#region DELETE ME

//This function returns a Tuple that finds the earliest valid date (-1 year from given date) and how many occurence points an associate has
//private (DateTime EarlyDate, double occurencePoints) CountOccurrences(DateTime date, long empID)
//{
//    double occurrencePoints = 0;
//    AGNESEntity agnesdb = new AGNESEntity();

//    DateTime cutOffDate = date.AddYears(-1);

//    var query = from employeeTable in agnesdb.CashHandles
//                where employeeTable.PersNumber == empID && employeeTable.Date >= cutOffDate
//                orderby employeeTable.Date ascending
//                select employeeTable;

//    var result = query.ToList();

//    var eTEarly = result[0];

//    DateTime CHEarly = (DateTime)eTEarly.Date;
//    DateTime EarlyDate = CHEarly.AddYears(1);

//    foreach (var row in query)
//    {
//        occurrencePoints += Convert.ToInt32(row.Type);
//    }

//    occurrencePoints = occurrencePoints / 2;

//    return (EarlyDate, occurrencePoints);
//}


//switch (empInProbation)
//{
//Associate past 1st 90 days
//case 0:

//    if (type == 1)
//    {
//        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has a Variance Between $3.00 and $20.00 This is an Automatic Progressive Counseling" +
//            " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
//        Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
//    }

//    if (type == 2)
//    {
//        BIMessageBox.Show("Contact HRBP Dialong", "This Type of Cash Handling Violation Requires Notification of Your DM AND HRBP, Please Contact Them", MessageBoxButton.OK);

//        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has a Variance Greater Than $20.00 This is an Automatic Progressive Counseling" +
//            " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
//        Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
//    }

//    if (anyPriorZero == 2)
//    {
//        BIMessageBox.Show("Warning", firstName + " Has 2 Prior No Variance Found Violations, 1 More Will Result in a Progressive Counseling",
//            MessageBoxButton.OK);
//    }

//    if (anyPriorZero == 3)
//    {
//        BIMessageBox.Show("Counseling Form Dialog", "This is " + firstName + "'s Thrid Occurrence For No Variance Found Violations and Requires a WRITTEN Counseling" +
//                                    " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
//        Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
//    }
//    break;
////Associate is IN 90 Probationary Period
//case 1:

//    break;
//}
#endregion
