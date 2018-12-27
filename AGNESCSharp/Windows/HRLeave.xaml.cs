using AGNESCSharp.Entity_Models;
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
    /// Interaction logic for HRLeave.xaml
    /// </summary>
    public partial class HRLeave : Window
    {
        #region Variables
        private string lastName;
        private string firstName;
        private long empId;
        private string empCostCenter;
        #endregion

        #region Main
        public HRLeave(string emp, int empNum)
        {
            InitializeComponent();

            var name = emp.Split(',');
            lastName = name[0].Trim();
            firstName = name[1].Trim();
            empId = empNum;
            TopTextBox.Text = "Please Enter The Details For " + firstName + " " + lastName + "'S" + " Leave";
            var query = from employeeTable in MainWindow.bidb.EmployeeLists
                        where employeeTable.PersNumber == empNum
                        select employeeTable;

            foreach (var result in query)
            {
                empCostCenter = result.CostCenter;
            }

            BeginLeave.DisplayDateStart = DateTime.Now.AddDays(-60);
            BeginLeave.DisplayDateEnd = DateTime.Now.AddDays(60);
        }
        #endregion

        #region Private Methods
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            PendingBox.IsChecked = false;
            ApprovedBox.IsChecked = false;
            ClosedBox.IsChecked = false;
            ParentalBox.IsChecked = false;
            BeginLeave.SelectedDate = null;
            EndLeave.SelectedDate = null;
            DescriptionTb.Clear();
            return;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            LOA leave = new LOA();
            string type = "Leave";//selectedLeave;
            DateTime? beginDate = BeginLeave.SelectedDate;
            DateTime? endDate = EndLeave.SelectedDate;
            string notes = DescriptionTb.Text;
            byte isChecked = 0;

            FileInfo myFile = new FileInfo(@"\\compasspowerbi\compassbiapplications\occurrencetracker\LOANotification.pdf");
            bool exists = myFile.Exists;

            if (beginDate > endDate || beginDate == endDate)
            {
                MessageBox.Show("The Ending Date For The Leave Must Be After The Begin Date For The Leave");
                return;
            }

            if (PendingBox.IsChecked == false && ApprovedBox.IsChecked == false && ClosedBox.IsChecked == false && ParentalBox.IsChecked == false)
            {
                MessageBox.Show("Please Check At Least One of the Checkboxes");
                return;
            }

            leave.PersNumber = empId;
            leave.CostCenter = empCostCenter;
            lastName.TrimStart();
            leave.LastName = lastName;
            leave.FirstName = firstName.TrimStart();
            leave.Type = type;
            leave.DateStart = beginDate;
            leave.DateEnd = endDate;
            leave.Notes = notes;
            if (PendingBox.IsChecked == true)
            {
                isChecked = 1;
            }
            leave.Pending = isChecked;
            isChecked = 0;

            if (ApprovedBox.IsChecked == true)
            {
                isChecked = 1;
            }
            leave.Approved = isChecked;
            isChecked = 0;

            if (ClosedBox.IsChecked == true)
            {
                isChecked = 1;
            }
            leave.Closed = isChecked;
            isChecked = 0;

            if (ParentalBox.IsChecked == true)
            {
                isChecked = 1;
            }
            leave.Parental = isChecked;
            isChecked = 0;

            if (InterBox.IsChecked == true)
            {
                isChecked = 1;
            }
            leave.Intermittent = isChecked;
            isChecked = 0;

            if (ContBox.IsChecked == true)
            {
                isChecked = 1;
            }
            leave.Continuous = isChecked;
            isChecked = 0;

            try
            {
                MainWindow.agnesdb.LOAs.Add(leave);
                MainWindow.agnesdb.SaveChanges();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
            MessageBox.Show("The Leave for " + firstName + " Has Been Added, Please Fill Out This Form And Submit To HR.");

            if (exists == true)
            {
                Process.Start(@"\\compasspowerbi\compassbiapplications\occurrencetracker\LOANotification.pdf");
            }

            PendingBox.IsChecked = false;
            ApprovedBox.IsChecked = false;
            ClosedBox.IsChecked = false;
            ParentalBox.IsChecked = false;
            BeginLeave.SelectedDate = null;
            EndLeave.SelectedDate = null;
            DescriptionTb.Clear();
        }
        #endregion
    }
}
