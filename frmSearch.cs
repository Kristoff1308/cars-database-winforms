using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarsDatabase
{
    public partial class frmSearch : Form
    {
        public frmSearch()
        {
            InitializeComponent();
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {
            this.Text = "Task A Search Krzysztof Kaminski 29/05/2026";
            cboField.Items.Add("Make");
            cboField.Items.Add("EngineSize");
            cboField.Items.Add("RentalPerDay");
            cboField.Items.Add("Available");
            cboOperator.Items.Add("=");
            cboOperator.Items.Add("<");
            cboOperator.Items.Add(">");
            cboOperator.Items.Add("<=");
            cboOperator.Items.Add(">=");
            cboField.DropDownStyle = ComboBoxStyle.DropDownList;
            cboOperator.DropDownStyle = ComboBoxStyle.DropDownList;
            cboField.SelectedIndex = 0;
            cboOperator.SelectedIndex = 0;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            bool missingData =cboField.Text == "" || cboOperator.Text == "" || txtValue.Text == "";
            if (missingData)
            {
                MessageBox.Show("Enter all search criteria");
                return;
            }
            try
            {
                string dbFileName = Application.StartupPath + @"\Hire.db";
                string connectionString = "Data Source=" + dbFileName + ";Version=3;";
                SQLiteConnection connection = new SQLiteConnection(connectionString);
                connection.Open();
                string sqlQuery ="SELECT VehicleRegNo, Make, EngineSize, DateRegistered, RentalPerDay, Available " + "FROM tblCar WHERE " + cboField.Text + " " + cboOperator.Text + " @Value";
                SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);

                if (cboField.Text == "RentalPerDay")
                {
                    command.Parameters.AddWithValue("@Value", Convert.ToDecimal(txtValue.Text));
                }
                else if (cboField.Text == "Available")
                {
                    string value = txtValue.Text.ToLower();

                    if (value == "yes" || value == "true")
                    {
                        command.Parameters.AddWithValue("@Value", true);
                    }
                    else if (value == "no" || value == "false")
                    {
                        command.Parameters.AddWithValue("@Value", false);
                    }
                    else
                    {
                        MessageBox.Show("For Available enter Yes or No");
                        connection.Close();
                        return;
                    }
                }
                else
                {
                    command.Parameters.AddWithValue("@Value", txtValue.Text);
                }

                SQLiteDataReader dataReader = command.ExecuteReader();
                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);
                dataGridView1.DataSource = dataTable;
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search error: " + ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
