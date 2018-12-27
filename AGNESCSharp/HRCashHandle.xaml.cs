using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace AGNESCSharp
{
    /// <summary>
    /// Interaction logic for HRCashHandle.xaml
    /// </summary>
    public partial class HRCashHandle : Window
    {
        #region Variables
        string nameToInsert;
        private string empCostCenter;
        string firstName;
        string lastName;
        byte type;
        long empID;
        DateTime hireDate;
        DateTime selectedDate;
        #endregion

        #region Main
        public HRCashHandle(string emp, int empNum)
        {
            InitializeComponent();
            nameToInsert = emp;
            var name = emp.Split(',');
            lastName = name[0].Trim();
            firstName = name[1].Trim();
            empID = empNum;
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

            (int count, double occurrencePoints) = CountOccurrences(selectedDate, empID);

            FileInfo myFile = new FileInfo(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
            bool exists = myFile.Exists;

            if (occurrencePoints == 2)
            {
                BIMessageBox.Show("Warning", firstName + " Has 2 Occurrence Points For Cash Handling Violations, the Next Full Violation Will Result in a Progressive" +
                    " Written Counseling", MessageBoxButton.OK);
            }

            if (occurrencePoints >= 3 || type == 2)
            {
                if (occurrencePoints >= 3)
                {
                    BIMessageBox.Show("Counseling Form Dialog", firstName + " Has " + occurrencePoints + " Occurrence Points, Please Fill Out and Print This WRITTEN Warning Form" +
                                        "That I Will Open For You", MessageBoxButton.OK);
                    if (exists == true)
                    {
                        //Microsoft.Office.Interop.Word.Application ap = new Microsoft.Office.Interop.Word.Application();
                        //webClient.OpenWrite("https://microsoft.sharepoint-df.com/teams/CGHR/Shared%20Documents/Employee%20Relations/Performance%20Management/Associate%20Counseling%20Report%20Template%20-%20ENG.docx");
                        //Process.Start("https://microsoft.sharepoint-df.com/teams/CGHR/Shared%20Documents/Employee%20Relations/Performance%20Management/Associate%20Counseling%20Report%20Template%20-%20ENG.docx");
                        Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                    }
                    else
                    {
                        MessageBox.Show("Oops there was a problem trying to open the Progressive Counseling Form, Please contact Business Intelligence and let them know!");
                    }
                }
                else
                {
                    BIMessageBox.Show("Counseling Form Dialog", firstName + " Has a Variance Greater Than $3.00 but Less Than $20.00 This is an Automatic Progressive Counseling" +
                        " Please Fill Out and Print This Form I Will Open For You", MessageBoxButton.OK);
                    Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\ProgressiveCounselingForm.docx");
                }
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

        //This function returns a Tuple that counts how many occurrences and how many occurence points an associate has
        private (int count, double occurencePoints) CountOccurrences(DateTime date, long empID)
        {
            int count = 0;
            double occurrencePoints = 0;
            AGNESEntity agnesdb = new AGNESEntity();

            DateTime cutOffDate = date.AddYears(-1);

            var query = from employeeTable in agnesdb.CashHandles
                        where employeeTable.PersNumber == empID && employeeTable.Date >= cutOffDate
                        select employeeTable;

            var result = query.ToList();

            foreach (var row in query)
            {
                occurrencePoints += Convert.ToInt32(row.Type);
                count++;
            }

            occurrencePoints = occurrencePoints / 2;

            return (count, occurrencePoints);
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
