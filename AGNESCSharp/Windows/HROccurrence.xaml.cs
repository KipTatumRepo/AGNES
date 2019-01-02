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
    /// Interaction logic for HROccurrence.xaml
    /// </summary>
    public partial class HROccurrence : Window
    {
        #region Variables
        string firstName;
        string lastName;
        string AttType;
        long empID;
        byte type;
        int empInProbation;
        int selectedIndex;
        int layoutFlag;
        private string empCostCenter;
        DateTime hireDate;
        DateTime today = DateTime.Now;
        DateTime? selectedDate;
        Dictionary<string, int> cbDictionary = new Dictionary<string, int>();
        #endregion

        #region Main
        public HROccurrence(string emp, int empNum, int empInProbationPeriod)
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
                AttendanceType.Items.Add(cbi);
            }

            var name = emp.Split(',');
            lastName = name[0].Trim();
            firstName = name[1].Trim();
            empID = empNum;
            empInProbation = empInProbationPeriod;

            TopTextBox.Text = "Occurrence Details for " + firstName + " " + lastName;

            var query = from employeeTable in MainWindow.bidb.EmployeeLists
                        where employeeTable.PersNumber == empNum
                        select employeeTable;

            var results = query.ToList();
            foreach (var result in query)
            {
                empCostCenter = result.CostCenter;
                hireDate = result.DateOfHire;
            }
            
            AOccurrenceDP.DisplayDateStart = DateTime.Now.AddYears(-1);
            AOccurrenceDP.DisplayDateEnd = DateTime.Now;
            //OOccurrenceDP.DisplayDateStart = DateTime.Now.AddYears(-1);
            //OOccurrenceDP.DisplayDateEnd = DateTime.Now;
        }
        #endregion

        #region Private Methods 
        private void AttendanceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            ComboBoxItem cbi = (ComboBoxItem)cb.SelectedItem;
            string PointValue;

            if (cbi == null)
            {
                return;
            }

            PointValue = cbi.Tag.ToString();
            AttType = cbi.Content.ToString();
            type = Convert.ToByte(PointValue);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Occurrence oc = new Occurrence();

            if (layoutFlag == 0)
            {
                selectedDate = AOccurrenceDP.SelectedDate;
                if (AttendanceType.SelectedIndex == -1)
                {
                    MessageBox.Show("Please Select An Absence Type");
                    return;
                }
            }
            
            if (selectedDate == null)
            {
                MessageBox.Show("Please Select a Valid Date");
                return;
            }

            DateTime fDate = DateTime.Parse(selectedDate.ToString());
            //TODO: THIS CUTOFFDATE CALCULATION MAY CHANGE, RIGHT NOW IT IS 1 YEAR PRIOR TO SELECTED DATE OF WRITE UP
            DateTime cutOffDate = fDate.AddYears(-1);
            //TODO: THIS CUTOFFDATE MAY CHANGE, RIGHT NOW IT IS 1 YEAR PRIOR TO SELECTED DATE OF WRITE UP
            DateTime? date;
            var NoCallFromDB = (from table in MainWindow.agnesdb.Occurrences
                                where table.PersNumber == empID && table.AttendanceViolation == "No Call No Show" && table.Date >= cutOffDate
                                orderby table.Date ascending
                                select new
                                {
                                    table.AttendanceViolation,
                                    table.Date
                                }).FirstOrDefault();

            //Need to Check is Associate has a previous No Call No Show in the previous year
            if (NoCallFromDB == null)
            {
                date = new DateTime(1001, 1, 1);
            }
            else
            {
                date = NoCallFromDB.Date;
            }
            
            string notes = DescriptionTb.Text;

            oc.PersNumber = empID;
            oc.CostCenter = empCostCenter;
            oc.LastName = lastName;
            oc.FirstName = firstName;
            oc.Type = type;
            oc.Date = selectedDate;
            oc.Notes = notes;
            oc.AttendanceViolation = AttType;

            MainWindow.agnesdb.Occurrences.Add(oc);
            MainWindow.agnesdb.SaveChanges();
            MessageBox.Show("The Occurrence for " + firstName + " has been added");

            //get Write up form ready
            FileInfo myFileTerm = new FileInfo(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
            FileInfo myFileProg = new FileInfo(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
            
            bool TermExists = myFileTerm.Exists;
            bool ProgExists = myFileProg.Exists;
            
            //right now earliest date is -1 year from incident date
            (DateTime earlyDate, double? occPoints) = CountOccurrences(fDate, empID);

            switch (empInProbation)
            {
                //Associate NOT in Probationary Period
                case 0:

                    if (AttType == "No Call No Show" && date != new DateTime(1001, 1, 1))
                    {
                        BIMessageBox.Show("No Call No Show Dialog", "This No Call No Show is " + firstName + "'s Second In Less Than a Year And Requires Termination.  Please Fill Out And Print This Form", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
                        this.Close();
                        return;
                    }
                    else if (AttType == "No Call No Show")
                    {
                        BIMessageBox.Show("No Call No Show Dialog", "This No Call No Show Requires An Automatic FINAL Written Progressive Counseling, Please Fill Out And Print This Form", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\ProgressiveCounselingForm.docx");
                    }

                    if (occPoints < 4)
                    {
                        MessageBox.Show(firstName + " Has " + occPoints + " Occurrence Points");
                    }
                    else if (occPoints >= 4 && occPoints < 5)
                    {
                        BIMessageBox.Show("Warning Dialog", firstName + " Has " + occPoints + " Occurrence Points " + (5 - occPoints) + " More Before " + earlyDate.ToShortDateString() + " Will Require A Written Progressive Counseling.", MessageBoxButton.OK);
                    }
                    else if (occPoints >= 5 && occPoints < 6)
                    {
                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occPoints + " Occurrence Points, Please Fill Out and Print This WRITTEN Warning Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                        if (ProgExists == true)
                        {
                            Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
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
                        if (ProgExists == true)
                        {
                            Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                        }
                        else
                        {
                            MessageBox.Show("Oops there was a problem trying to open the Progressive Counseling Form, Please contact Business Intelligence and let them know!");
                        }
                    }
                    else
                    {
                        BIMessageBox.Show("Termination Form Dialog", firstName + " Has " + occPoints + " Occurrence Points, Please Fill Out and Print This DISCHARGE Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\TermLetter.docx");
                    }
                    break;

                //Associate IS In Probationary Period
                case 1:

                    if (AttType == "No Call No Show")
                    {
                        BIMessageBox.Show("No Call No Show Dialog", firstName + "Is In The Associates 90 Probationary Period, This No Call No Show Requires Automatic Termination " +
                            "Please Fill Out And Print This DISCHARG Form", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
                        this.Close();
                        return; 
                        
                    }

                    if (occPoints < 1)
                    {
                        BIMessageBox.Show("Warning Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occPoints + " Occurrence Points.  " + (1 - occPoints) + " More Points Before " +
                            hireDate.AddDays(90).ToShortDateString() + " Will Require A Written Progressive Counseling", MessageBoxButton.OK);
                    }

                    else if (occPoints >= 1 && occPoints < 2)
                    {
                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occPoints + " Occurrence Points, Please Please Fill Out" +
                                " and Print This FINAL Warning Form That I will Open For You", MessageBoxButton.OK);
                        if (ProgExists == true)
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
                        BIMessageBox.Show("Termination Form Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occPoints + " Occurrence Points, Please Print This DISCHARGE Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                        if (TermExists == true)
                        {
                            Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\TermLetter.docx");
                        }
                        else
                        {
                            MessageBox.Show("Oops there was a problem trying to open the Termination Form, Please contact Business Intelligence and let them know!");
                        }
                    }
                    break;
            }
            DescriptionTb.Clear();
            AOccurrenceDP.SelectedDate = null;
            AttendanceType.SelectedItem = null;
        }

        private void OccurrenceCategory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedIndex = OccurrenceCategory.SelectedIndex;
            ShowElements(selectedIndex);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DescriptionTb.Clear();
            AOccurrenceDP.SelectedDate = null;
            AttendanceType.SelectedItem = null;
        }

        //This function returns a Tuple that gets the earliest valid occurrence and how many occurence points an associate has
        //The earliest valid occurrence is 1 year prior to selected write up date
        public static (DateTime EarlyDate, double? occurencePoints) CountOccurrences(DateTime date, long empID)
        {
            double? occurrencePoints = 0;
            
            AGNESEntity agnesdb = new AGNESEntity();
            DateTime cutOffDate = date.AddYears(-1);
            //DateTime hireDate;

            var query = from employeeTable in agnesdb.Occurrences
                        where employeeTable.PersNumber == empID && employeeTable.Date >= cutOffDate
                        orderby employeeTable.Date ascending
                        select employeeTable;

            var eTQueryResult = query.ToList();
            var eTEarly = eTQueryResult[0];
            
            DateTime occEarly = (DateTime)eTEarly.Date;
            DateTime EarlyDate = occEarly.AddYears(1);

            foreach (var row in query)
            {
                occurrencePoints += Convert.ToInt32(row.Type);
            }

            occurrencePoints = occurrencePoints / 2;

            return (EarlyDate, occurrencePoints);
        }

        //Take in occurrence type and show appropriate page elements
        private void ShowElements(int cbIndex)
        {
            if (cbIndex == 0)
            {
                OccurrenceSelection.Visibility = Visibility.Collapsed;
                AttendanceGrid.Visibility = Visibility.Visible;
                DescriptionTbLable.Visibility = Visibility.Visible;
                DescriptionSV.Visibility = Visibility.Visible;
                ButtonGrid.Visibility = Visibility.Visible;
                //SelectedDate = AOccurrenceDP.SelectedDate;
                selectedDate = AOccurrenceDP.SelectedDate;
                //selectedDate = (DateTime)SelectedDate;
                layoutFlag = 0;
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            saveImage.Width = 66;
            saveImage.Height = 58;
        }

        private void SaveImage_MouseLeave(object sender, MouseEventArgs e)
        {
            saveImage.Width = 58;
            saveImage.Height = 50;
        }

        private void EraseImage_MouseEnter(object sender, MouseEventArgs e)
        {
            eraseImage.Width = 66;
            eraseImage.Height = 58;
        }

        private void EraseImage_MouseLeave(object sender, MouseEventArgs e)
        {
            eraseImage.Width = 58;
            eraseImage.Height = 50;
        }
        #endregion
    }
}

#region Future Stuff??
//This code is for searching CashHandle Table to help calculate number of occurrence points

//DateTime CHEarly;
//var CHQuery = from CHTable in agnesdb.CashHandles
//              where CHTable.PersNumber == empID & CHTable.Date >= cutOffDate
//              orderby CHTable.Date ascending
//              select CHTable;
//var cHQueryResult = CHQuery.ToList();
//if (cHQueryResult.Count < 1)
//{
//    var cHEarlies = DateTime.Now;
//    var cHEarly = cHEarlies;
//    CHEarly = (DateTime)cHEarly.Date;
//}
//else
//{
//    var cHEarlies = cHQueryResult[0];
//    var cHEarly = cHEarlies;
//    CHEarly = (DateTime)cHEarly.Date;
//}
//DateTime occEarlyAddYear 
//DateTime cHEarlyAddYear = CHEarly.AddYears(1);

//if (occEarly < CHEarly)
//{

//}
//else
//{
//    EarlyDate = cHEarlyAddYear;
//}
//foreach (var row in CHQuery)
//{
//    occurrencePoints += Convert.ToInt32(row.Type);
//}

#endregion
