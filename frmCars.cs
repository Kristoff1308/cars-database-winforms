using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using CarsClassLibrary;

namespace CarsDatabase
{
    public partial class frmCars : Form
    {
        string dbFileName = Application.StartupPath + @"\Hire.db";
        DataTable carsTable = new DataTable();
        int currentIndex = 0;
        frmSearch searchForm;
        SQLiteManager sqliteManager;

        public frmCars()
        {
            InitializeComponent();
        }

        private void frmCars_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-IE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-IE");
            this.Text = "Task A Krzysztof Kaminski 08/06/2026";
            sqliteManager = new SQLiteManager(dbFileName);
            LoadCars();
            if (carsTable.Rows.Count > 0)
            {
                currentIndex = 0;
                DisplayCar();
            }
            toolTip1.SetToolTip(txtVehicleRegNo, "Enter the vehicle registration number");
            toolTip1.SetToolTip(txtMake, "Enter the make of the vehicle");
            toolTip1.SetToolTip(txtRentalPerDay, "Enter the rental price per day");
        }


        private void LoadCars()
        {
            try
            {
                string sqlQuery = "SELECT * FROM tblCar";
                sqliteManager.SQLQuery = sqlQuery;
                carsTable = sqliteManager.ReadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database load error: " + ex.Message);
            }
        }

        private void DisplayCar()
        {
            if (carsTable.Rows.Count == 0)
            {
                ClearTextBoxes();
                txtRecordCount.Text = "0 of 0";
                return;
            }
            DataRow row = carsTable.Rows[currentIndex];
            txtVehicleRegNo.Text = row["VehicleRegNo"].ToString();
            txtMake.Text = row["Make"].ToString();
            txtEngineSize.Text = Convert.ToDouble(row["EngineSize"]).ToString("0.0");
            dtpDateRegistered.Value = Convert.ToDateTime(row["DateRegistered"]);
            txtRentalPerDay.Text = Convert.ToDecimal(row["RentalPerDay"]).ToString("0.00");
            chkAvailable.Checked = Convert.ToBoolean(row["Available"]);
            DisplayRecordCount();
        }

        private void DisplayRecordCount()
        {
            txtRecordCount.Text = (currentIndex + 1).ToString() + " of " + carsTable.Rows.Count.ToString();
        }

        private void ClearTextBoxes()
        {
            txtVehicleRegNo.Clear();
            txtMake.Clear();
            txtEngineSize.Clear();
            txtRentalPerDay.Clear();
            dtpDateRegistered.Value = DateTime.Now;
            chkAvailable.Checked = false;
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (carsTable.Rows.Count > 0)
            {
                currentIndex = 0;
                DisplayCar();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                DisplayCar();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentIndex < carsTable.Rows.Count - 1)
            {
                currentIndex++;
                DisplayCar();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            if (carsTable.Rows.Count > 0)
            {
                currentIndex = carsTable.Rows.Count - 1;
                DisplayCar();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
           
                Car car = new Car();
                car.VehicleRegNo = txtVehicleRegNo.Text;
                car.Make = txtMake.Text;
                car.EngineSize = Convert.ToDouble(txtEngineSize.Text);
            car.DateRegistered = dtpDateRegistered.Value; 
                car.RentalPerDay = Convert.ToDecimal(txtRentalPerDay.Text);
                car.Available = chkAvailable.Checked;
                bool added = sqliteManager.AddRecord(car);
                if (added)
                {
                MessageBox.Show("Record Added");
                LoadCars();
                    currentIndex = carsTable.Rows.Count - 1;
                    DisplayCar();
                }
                else
                {
                    MessageBox.Show("Add error");
                }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Car car = new Car();
            car.ID = Convert.ToInt32(carsTable.Rows[currentIndex]["ID"]);
            car.VehicleRegNo = txtVehicleRegNo.Text;
            car.Make = txtMake.Text;
            car.EngineSize = Convert.ToDouble(txtEngineSize.Text);
            car.DateRegistered = dtpDateRegistered.Value;
            car.RentalPerDay = Convert.ToDecimal(txtRentalPerDay.Text);
            car.Available = chkAvailable.Checked;
            bool updated = sqliteManager.UpdateRecord(car);
            if (updated)
            {
                MessageBox.Show("Record updated");
                LoadCars();
                DisplayCar();
            }
            else
            {
                MessageBox.Show("Update error");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show("Delete this record?","Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (answer == DialogResult.Yes)
            {
                int id = Convert.ToInt32(carsTable.Rows[currentIndex]["ID"]);
                bool deleted = sqliteManager.DeleteRecord(id);
                if (deleted)
                {
                    MessageBox.Show("Record deleted");
                    LoadCars();
                    if (carsTable.Rows.Count > 0)
                    {
                        if (currentIndex > carsTable.Rows.Count - 1)
                        {
                            currentIndex = carsTable.Rows.Count - 1;
                        }
                        DisplayCar();
                    }
                    else
                    {
                        ClearTextBoxes();
                        txtRecordCount.Text = "0 of 0";
                    }
                }
                else
                {
                    MessageBox.Show("Delete error");
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            LoadCars();
            DisplayCar();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (searchForm == null || searchForm.IsDisposed)
            {
                searchForm = new frmSearch();
            }

            searchForm.Show();
            searchForm.BringToFront();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmCars_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult replay = MessageBox.Show("Do you wish to exit the application?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (replay == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}