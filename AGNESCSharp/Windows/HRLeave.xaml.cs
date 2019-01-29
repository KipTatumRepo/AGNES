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
        private long? empId;
        private long SelectOccurrence;
        private string empCostCenter;
        private int NavFromSearch;
        private string nameToInsert;
        #endregion

        #region Main
        public HRLeave(string emp, long? empNum, int navFromSearch)
        {
            InitializeComponent();
            empId = empNum;
            NavFromSearch = navFromSearch;

            if (NavFromSearch == 0)
            {
                UpdateButton.Visibility = Visibility.Collapsed;
                var name = emp.Split(',');
                lastName = name[0].Trim();
                firstName = name[1].Trim();
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
                SaveButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Visible;
                
            }
            else if (NavFromSearch == 1)
            {
                nameToInsert = emp;
                var name = emp.Split(',');
                firstName = name[0].Trim();
                SaveButton.Visibility = Visibility.Collapsed;
                TopTextBox.Text = "Please Enter The Details For " + firstName + "'S" + " Leave";
                //CashCB.SelectedIndex = HRSearch.SelectedIndexV;
                BeginLeave.SelectedDate = HRSearch.LOADateStartV;
                EndLeave.SelectedDate = HRSearch.LOADateEndV;
                DescriptionTb.Text = HRSearch.LOANoteV;
                UpdateButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Collapsed;
                if (HRSearch.Approved == 1)
                {
                    ApprovedBox.IsChecked = true;
                }
                if (HRSearch.Pending == 1)
                {
                    PendingBox.IsChecked = true;
                }
                if (HRSearch.ClosedV == 1)
                {
                    ClosedBox.IsChecked = true;
                }
                if (HRSearch.Parental == 1)
                {
                    ParentalBox.IsChecked = true;
                }
                if (HRSearch.Continuous == 1)
                {
                    ContBox.IsChecked = true;
                }
                if (HRSearch.Intermittent == 1)
                {
                    InterBox.IsChecked = true;
                }
            }
            else
            {
                nameToInsert = emp;
                var name = emp.Split(',');
                firstName = name[0].Trim();
                SaveButton.Visibility = Visibility.Collapsed;
                TopTextBox.Text = "Please Enter The Details For " + firstName + "'S" + " Leave";
                //CashCB.SelectedIndex = HRMgr.SelectedIndexV;
                //.SelectedDate = HRMgr.Date;
                BeginLeave.SelectedDate = HRMgr.LOADateStartV;
                EndLeave.SelectedDate = HRMgr.LOADateEndV;
                DescriptionTb.Text = HRMgr.LOANoteV;
                UpdateButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Collapsed;
                if (HRMgr.Approved == 1)
                {
                    ApprovedBox.IsChecked = true;
                }
                if (HRMgr.Pending == 1)
                {
                    PendingBox.IsChecked = true;
                }
                if (HRMgr.ClosedV == 1)
                {
                    ClosedBox.IsChecked = true;
                }
                if (HRMgr.Parental == 1)
                {
                    ParentalBox.IsChecked = true;
                }
                if (HRMgr.Continuous == 1)
                {
                    ContBox.IsChecked = true;
                }
                if (HRMgr.Intermittent == 1)
                {
                    InterBox.IsChecked = true;
                }
            }
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


            if (LOAExists == true && ClosedBox.IsChecked == false && ApprovedBox.IsChecked == false)
            {
                MessageBox.Show("The Leave for " + firstName + " Has Been Added, Please Fill Out This Form And Submit To HR.");
                Process.Start(@"\\compasspowerbi\compassbiapplications\AGNES\Docs\LOANotification.pdf");

            }
            else
            {
                MessageBox.Show("The Leave for " + firstName + " Has Been Added.");
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

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AGNESEntity())
            {
                if (NavFromSearch == 1)
                {
                    string selectOccurrence = HRSearch.LOANumberV;
                    SelectOccurrence = Convert.ToInt64(selectOccurrence);
                }
                else
                {
                    string selectOccurrence = HRMgr.LOANumberV;
                    SelectOccurrence = Convert.ToInt64(selectOccurrence);
                }
                var result = db.LOAs.SingleOrDefault(f => f.PID == SelectOccurrence);
                if (result != null)
                {
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
                    result.Intermittent = InterBoxValue;
                    result.Continuous = ContBoxValue;
                    result.Pending = PendingBoxValue;
                    result.Approved = ApprovedBoxValue;
                    result.Closed = ClosedBoxValue;
                    result.Parental = ParentalBoxValue;
                    result.DateStart = BeginLeave.SelectedDate;
                    result.DateEnd = EndLeave.SelectedDate;
                    result.Notes = DescriptionTb.Text;

                    if (BeginLeave.SelectedDate == null || BeginLeave.SelectedDate == null)
                    {
                        MessageBox.Show("Please Enter a Beginning Date AND Estimated Ending Date For The Leave");
                        return;
                    }
                    if (BeginLeave.SelectedDate > EndLeave.SelectedDate || BeginLeave.SelectedDate == EndLeave.SelectedDate)
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
                        InterBox.IsChecked == true || ParentalBox.IsChecked == true && ContBox.IsChecked == true)
                    {
                        MessageBox.Show("If Parental Leave is Selected, No Other Selections May be Made");
                        return;
                    }

                    if (InterBox.IsChecked == true && ContBox.IsChecked == true)
                    {
                        MessageBox.Show("There Can Only Intermittent or Continuous Leave, Both Cannot Be Selected at The Same Time, Please Select Just One");
                        return;
                    }
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
            this.Close();
        }
        #endregion
    }
}
