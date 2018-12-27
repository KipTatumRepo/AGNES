using AGNESCSharp.Entity_Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        //byte NCNS;
        int empInProbationPeriod = 0;
        int selectedIndex;
        int layoutFlag;
        private string empCostCenter;
        DateTime hireDate;
        DateTime today = DateTime.Now;
        //DateTime? SelectedDate;
        DateTime? selectedDate;
        Dictionary<string, int> cbDictionary = new Dictionary<string, int>();
        #endregion

        #region Main
        public HROccurrence(string emp, int empNum)
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

            TopTextBox.Text = "Occurrence Details for " + firstName + " " + lastName;

            var query = from employeeTable in MainWindow.bidb.EmployeeLists
                        where employeeTable.PersNumber == empNum
                        select employeeTable;

            var results = query.ToList();
            foreach (var result in query)
            {
                empCostCenter = result.CostCenter;
            }

            if (hireDate.AddDays(90) >= today)
            {
                empInProbationPeriod = 1;
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
            //else
            //{
            //    selectedDate = OOccurrenceDP.SelectedDate; 
            //}

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


            if (NoCallFromDB == null)
            {
                date = new DateTime(1001, 1, 1);

            }
            else
            {
                date = NoCallFromDB.Date;
            }

            //type = 1;

            string notes = DescriptionTb.Text;

            //if (AFullButton.IsChecked == true || OFullButton.IsChecked == true)
            //{
            //    type = 2;
            //}

            oc.PersNumber = empID;
            oc.CostCenter = empCostCenter;
            oc.LastName = lastName;
            oc.FirstName = firstName;
            oc.Type = type;
            oc.Date = selectedDate;
            oc.Notes = notes;
            oc.AttendanceViolation = AttType;
            //oc.NoCallNoShow = NCNS;

            MainWindow.agnesdb.Occurrences.Add(oc);
            MainWindow.agnesdb.SaveChanges();
            MessageBox.Show("The Occurrence for " + firstName + " has been added");


            //get Write up form ready
            FileInfo myFile = new FileInfo(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
            bool exists = myFile.Exists;

            if (AttType == "No Call No Show" && date != new DateTime(1001, 1, 1))
            {
                BIMessageBox.Show("No Call No Show Dialog", "This No Call No Show is " + firstName + "'s Second In Less Than a Year And Requires Termination.  Please Fill Out And Print This Form", MessageBoxButton.OK);
                Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\TermLetter.docx");
                this.Close();
                return;
            }

            else if (AttType == "No Call No Show")
            {
                BIMessageBox.Show("No Call No Show Dialog", "This No Call No Show Requires An Automatic Written Progressive Counseling, Please Fill Out And Print This Form", MessageBoxButton.OK);
                Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                Thread.Sleep(4000);
            }

            (DateTime earlyDate, double occurrencePoints) = CountOccurrences(fDate, empID);

            switch (empInProbationPeriod)
            {
                case 0:
                    switch (occurrencePoints)
                    {
                        case 4:
                            BIMessageBox.Show(firstName + " Has 4 Occurrence Points, 1 More Before " + earlyDate.ToShortDateString() + " Will Require A Written Progressive Counseling.");
                            break;

                        case 4.5:
                            BIMessageBox.Show(firstName + " Has 4.5 Occurrence Points, .5 More Before " + earlyDate.ToShortDateString() + " Will Require A Written Progressive Counseling.");
                            break;

                        case 5:
                            BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occurrencePoints + " Occurrence Points, Please Fill Out and Print This WRITTEN Warning Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                            if (exists == true)
                            {
                                Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                            }
                            else
                            {
                                MessageBox.Show("Oops there was a problem trying to open the Progressive Counseling Form, Please contact Business Intelligence and let them know!");
                            }
                            break;

                        case 6:
                            BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occurrencePoints + " Occurrence Points, Please Fill Out and Print This FINAL Warning Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                            if (exists == true)
                            {
                                Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                            }
                            else
                            {
                                MessageBox.Show("Oops there was a problem trying to open the Progressive Counseling Form, Please contact Business Intelligence and let them know!");
                            }
                            break;

                        case 7:
                            BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occurrencePoints + " Occurrence Points, Please Print This DISCHARGE Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                            Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\TermLetter.docx");
                            break;

                        default:
                            MessageBox.Show(firstName + " Has " + occurrencePoints + " Occurrence Points");
                            break;
                    }
                    break;

                case 1:
                    switch (occurrencePoints)
                    {
                        case 1:
                            BIMessageBox.Show("Counseling Form Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occurrencePoints + " Occurrence Points, Please Please Fill Out" +
                                " and Print This FINAL Warning Form That I will Open For You", MessageBoxButton.OK);
                            if (exists == true)
                            {
                                Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                            }
                            else
                            {
                                MessageBox.Show("Oops there was a problem trying to open the Progressive Counseling Form, Please contact Business Intelligence and let them know!");
                            }
                            break;

                        case 2:
                            BIMessageBox.Show("Counseling Form Dialog", firstName + " Is In The Associates 90 Day Probationary Period and Has " + occurrencePoints + " Occurrence Points, Please Print This DISCHARGE Form" +
                                "That I Will Open For You", MessageBoxButton.OK);
                            Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\TermLetter.docx");
                            break;
                    }
                    break;
            }
            DescriptionTb.Clear();
            //OFullButton.IsChecked = false;
            //OHalfButton.IsChecked = true;
            //OOccurrenceDP.SelectedDate = null;
            //AFullButton.IsChecked = false;
            //AHalfButton.IsChecked = true;
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
            //OFullButton.IsChecked = false;
            //OHalfButton.IsChecked = true;
            //OOccurrenceDP.SelectedDate = null;
            //AFullButton.IsChecked = false;
            //AHalfButton.IsChecked = true;
            AOccurrenceDP.SelectedDate = null;
            AttendanceType.SelectedItem = null;
        }

        //This function returns a Tuple that gets the earliest valid occurrence and how many occurence points an associate has
        //The earliest valid occurrence is 1 year prior to selected write up date
        public static (DateTime EarlyDate, double occurencePoints) CountOccurrences(DateTime date, long empID)
        {
            //int count = 0;
            double occurrencePoints = 0;
            AGNESEntity agnesdb = new AGNESEntity();

            DateTime cutOffDate = date.AddYears(-1);
            //DateTime CHEarly;

            var query = from employeeTable in agnesdb.Occurrences
                        where employeeTable.PersNumber == empID && employeeTable.Date >= cutOffDate
                        orderby employeeTable.Date ascending
                        select employeeTable;

            //var CHQuery = from CHTable in agnesdb.CashHandles
            //              where CHTable.PersNumber == empID & CHTable.Date >= cutOffDate
            //              orderby CHTable.Date ascending
            //              select CHTable;

            var eTQueryResult = query.ToList();
            //var cHQueryResult = CHQuery.ToList();

            var eTEarly = eTQueryResult[0];
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


            DateTime occEarly = (DateTime)eTEarly.Date;
            DateTime EarlyDate = occEarly.AddYears(1);

            //DateTime occEarlyAddYear 
            //DateTime cHEarlyAddYear = CHEarly.AddYears(1);



            //if (occEarly < CHEarly)
            //{

            //}
            //else
            //{
            //    EarlyDate = cHEarlyAddYear;
            //}



            foreach (var row in query)
            {
                occurrencePoints += Convert.ToInt32(row.Type);
                //count++;
            }

            //foreach (var row in CHQuery)
            //{
            //    occurrencePoints += Convert.ToInt32(row.Type);
            //}

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
            //else
            //{
            //    OccurrenceSelection.Visibility = Visibility.Collapsed;
            //    //OtherGrid.Visibility = Visibility.Visible;
            //    DescriptionTbLable.Visibility = Visibility.Visible;
            //    DescriptionSV.Visibility = Visibility.Visible;
            //    ButtonGrid.Visibility = Visibility.Visible;
            //    //SelectedDate = OOccurrenceDP.SelectedDate;
            //    //selectedDate = OOccurrenceDP.SelectedDate;
            //    //selectedDate = (DateTime)SelectedDate;
            //    layoutFlag = 1;
            //}
        }
        #endregion
    }
}
