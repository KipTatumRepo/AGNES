using AGNESCSharp.Entity_Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

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
            InterBox.IsChecked = false;
            ContBox.IsChecked = false;
            BeginLeave.SelectedDate = null;
            EndLeave.SelectedDate = null;
            
            DescriptionTb.Clear();
            return;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            LOA leave = new LOA();
            string type = "Leave";
            DateTime? beginDate = BeginLeave.SelectedDate;
            DateTime? endDate = EndLeave.SelectedDate;
            string notes = DescriptionTb.Text;
            byte isChecked = 0;

            FileInfo myFile = new FileInfo(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\LOANotification.pdf");
            bool LOAExists = myFile.Exists;

            if (beginDate == null || endDate == null)
            {
                MessageBox.Show("Please Enter a Beginning Date AND Estimated Ending Date For The Leave");
                return;
            }
            if (beginDate > endDate || beginDate == endDate)
            {
                MessageBox.Show("The Ending Date For The Leave Must Be After The Begin Date For The Leave");
                return;
            }

            if (PendingBox.IsChecked == false && ApprovedBox.IsChecked == false && ClosedBox.IsChecked == false && ParentalBox.IsChecked == false)
            {
                MessageBox.Show("Please Select Pending, Approved, or Closed");
                return;
            }

            if (PendingBox.IsChecked == true && ApprovedBox.IsChecked == true || PendingBox.IsChecked == true && ClosedBox.IsChecked == true || ApprovedBox.IsChecked == true && ClosedBox.IsChecked == true)
            {
                MessageBox.Show("There Can Only Be One Option of Pending, Approved, or Closed Selected At A Time");
                return;
            }

            if (ParentalBox.IsChecked == true && PendingBox.IsChecked == true || ParentalBox.IsChecked == true && ApprovedBox.IsChecked == true || ParentalBox.IsChecked == true && ClosedBox.IsChecked == true || ParentalBox.IsChecked == true &&
                InterBox.IsChecked == true || ParentalBox.IsChecked == true && ContBox.IsChecked == true )
            {
                MessageBox.Show("If Parental Leave is Selected, No Other Selections May be Made");
                return;
            }

            if (InterBox.IsChecked == true && ContBox.IsChecked == true)
            {
                MessageBox.Show("There Can Only Intermittent or Continuous Leave, Both Cannot Be Selected at The Same Time, Please Select Just One");
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

            if (LOAExists == true)
            {
                Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\LOANotification.pdf");
                
            }
            else
            {
                MessageBox.Show("Oops there was a problem trying to open the Leave Of Abscence Form, Please contact Business Intelligence and let them know!");
            }

            PendingBox.IsChecked = false;
            ApprovedBox.IsChecked = false;
            ClosedBox.IsChecked = false;
            ParentalBox.IsChecked = false;
            BeginLeave.SelectedDate = null;
            EndLeave.SelectedDate = null;
            DescriptionTb.Clear();
            this.Close();
        }
        #endregion
    }
}
