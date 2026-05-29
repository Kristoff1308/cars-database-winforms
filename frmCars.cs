using System;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CarsDatabase
{
    public partial class frmCars : Form
    {
        string dbFileName = Application.StartupPath + @"\Hire.db";
        DataTable carsTable = new DataTable();
        int currentIndex = 0;

        public frmCars()
        {
            InitializeComponent();
        }

        private void frmCars_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-IE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-IE");
            this.Text = "Task A Search Krzysztof Kaminski 29/05/2026";
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

        private string GetConnectionString()
        {
            return "Data Source=" + dbFileName + ";Version=3;";
        }

        private void LoadCars()
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection(GetConnectionString());
                connection.Open();
                string sqlQuery = "SELECT * FROM tblCar";
                SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
                SQLiteDataReader dataReader = command.ExecuteReader();
                carsTable = new DataTable();
                carsTable.Load(dataReader);
                connection.Close();
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
            txtEngineSize.Text = row["EngineSize"].ToString();
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
            try
            {
                SQLiteConnection connection = new SQLiteConnection(GetConnectionString());
                connection.Open();
                string sqlQuery = "INSERT INTO tblCar " + "(VehicleRegNo, Make, EngineSize, DateRegistered, RentalPerDay, Available) " + "VALUES " + "(@VehicleRegNo, @Make, @EngineSize, @DateRegistered, @RentalPerDay, @Available)";
                SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@VehicleRegNo", txtVehicleRegNo.Text);
                command.Parameters.AddWithValue("@Make", txtMake.Text);
                command.Parameters.AddWithValue("@EngineSize", txtEngineSize.Text);
                command.Parameters.AddWithValue("@DateRegistered", dtpDateRegistered.Value.ToString("dd/MM/yyyy"));
                command.Parameters.AddWithValue("@RentalPerDay", Convert.ToDecimal(txtRentalPerDay.Text));
                command.Parameters.AddWithValue("@Available", chkAvailable.Checked);
                command.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Record added");
                LoadCars();
                currentIndex = carsTable.Rows.Count - 1;
                DisplayCar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add error: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                SQLiteConnection connection = new SQLiteConnection(GetConnectionString());
                connection.Open();
                string sqlQuery = "UPDATE tblCar SET " + "Make = @Make, " + "EngineSize = @EngineSize, " +"DateRegistered = @DateRegistered, " + "RentalPerDay = @RentalPerDay, " + "Available = @Available " +"WHERE VehicleRegNo = @VehicleRegNo";
                SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@VehicleRegNo", txtVehicleRegNo.Text);
                command.Parameters.AddWithValue("@Make", txtMake.Text);
                command.Parameters.AddWithValue("@EngineSize", txtEngineSize.Text);
                command.Parameters.AddWithValue("@DateRegistered", dtpDateRegistered.Value.ToString("dd/MM/yyyy"));
                command.Parameters.AddWithValue("@RentalPerDay", Convert.ToDecimal(txtRentalPerDay.Text));
                command.Parameters.AddWithValue("@Available", chkAvailable.Checked);
                command.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Record updated");
                LoadCars();
                DisplayCar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show("Delete this record?","Delete",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (answer == DialogResult.Yes)
            {
                try
                {
                    SQLiteConnection connection = new SQLiteConnection(GetConnectionString());
                    connection.Open();
                    string sqlQuery = "DELETE FROM tblCar WHERE VehicleRegNo = @VehicleRegNo";
                    SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@VehicleRegNo", txtVehicleRegNo.Text);
                    command.ExecuteNonQuery();
                    connection.Close();
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
                        currentIndex = 0;
                        txtVehicleRegNo.Clear();
                        txtMake.Clear();
                        txtEngineSize.Clear();
                        txtRentalPerDay.Clear();
                        chkAvailable.Checked = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete error: " + ex.Message);
                }
            }
        }
        
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DisplayCar();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            frmSearch searchForm = new frmSearch();

            searchForm.ShowDialog();
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