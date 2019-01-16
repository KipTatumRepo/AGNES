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
        private string empCostCenter;
        string firstName;
        string lastName;
        byte type;
        long empID;
        DateTime hireDate;
        DateTime selectedDate;
        #endregion

        #region Main
        public HRCashHandle(string emp, int empNum, int empInProbationPeriod)
        {
            InitializeComponent();
            nameToInsert = emp;
            var name = emp.Split(',');
            lastName = name[0].Trim();
            firstName = name[1].Trim();
            empID = empNum;
            empInProbation = empInProbationPeriod;

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
            (DateTime earlyDate, double occurrencePoints) = CountOccurrences(selectedDate, empID);
            int anyPriorZero = CountZeroPoints(selectedDate, empID);

            FileInfo myFile = new FileInfo(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
            bool exists = myFile.Exists;

            switch (empInProbation)
            {
                //Associate past 1st 90 days
                case 0:

                    if (type == 2)
                    {
                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has a Variance Greater Than $3.00 but Less Than $20.00 This is an Automatic Progressive Counseling" +
                            " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
                        Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                    }

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

                    if (occurrencePoints < 2)
                    {
                        MessageBox.Show(firstName + " Has " + occurrencePoints + " Occurrence Points");
                    }

                    //Warning Letting User Know Associate is N Number of Points Away From a Written Prog Counseling
                    else if (occurrencePoints >= 2 && occurrencePoints < 3)
                    {
                        BIMessageBox.Show("Warning", firstName + " Has " + occurrencePoints +" Occurrence Points For Cash Handling Violations, "  + (3-occurrencePoints) +
                            " More Points Before " + earlyDate.ToShortDateString() +  " Will Result in a Progressive Written Counseling", MessageBoxButton.OK); 
                    }

                    else if(occurrencePoints >= 3)
                    {
                        BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occurrencePoints + " Cash Handling Occurrence Points, Please Fill Out" 
                            + " and Print This WRITTEN Warning Form That I Will Open For You", MessageBoxButton.OK); 
                                                
                        if (exists == true)
                        {
                            Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                        }
                        else
                        {
                            MessageBox.Show("Oops there was a problem trying to open the Progressive Counseling Form, Please contact Business Intelligence and let them know!");
                        }
                      
                    }
                    break;
                //Associate is IN 90 Probationary Period
                case 1:

                    break;
            }

            CashCB.SelectedItem = null;
            CHOccurrenceDP.SelectedDate = null;
            DescriptionTb.Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CashCB.SelectedItem = null;
            CHOccurrenceDP.SelectedDate = null;
            DescriptionTb.Clear();
        }

        //This function returns a Tuple that finds the earliest valid date (-1 year from given date) and how many occurence points an associate has
        private (DateTime EarlyDate, double occurencePoints) CountOccurrences(DateTime date, long empID)
        {
            double occurrencePoints = 0;
            AGNESEntity agnesdb = new AGNESEntity();

            DateTime cutOffDate = date.AddYears(-1);

            var query = from employeeTable in agnesdb.CashHandles
                        where employeeTable.PersNumber == empID && employeeTable.Date >= cutOffDate
                        orderby employeeTable.Date ascending
                        select employeeTable;

            var result = query.ToList();

            var eTEarly = result[0];

            DateTime CHEarly = (DateTime)eTEarly.Date;
            DateTime EarlyDate = CHEarly.AddYears(1);

            foreach (var row in query)
            {
                occurrencePoints += Convert.ToInt32(row.Type);
            }

            occurrencePoints = occurrencePoints / 2;

            return (EarlyDate, occurrencePoints);
        }

        private int CountZeroPoints(DateTime date, long empID)
        {
            AGNESEntity agnesdb = new AGNESEntity();
            DateTime cutOffDate = date.AddYears(-1);

            var query = from employeeTable in agnesdb.CashHandles
                        where employeeTable.PersNumber == empID && employeeTable.Date >= cutOffDate && employeeTable.Type == 0
                        select employeeTable;

            int count = query.Count();
            return count;
        }

        private void CashCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
        #endregion
    }
}
