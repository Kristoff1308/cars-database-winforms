using System;
using System.Data;
using System.Windows.Forms;
using CarsClassLibrary;

namespace CarsDatabase
{
    public partial class frmSearch : Form
    {
        SQLiteManager sqliteManager;
        public frmSearch()
        {
            InitializeComponent();
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {
           string dbFileName = Application.StartupPath + @"\Hire.db";
            sqliteManager = new SQLiteManager(dbFileName);
            this.Text = "Task A Search Krzysztof Kaminski 08/06/2026";

            cboField.Items.Clear();
            cboField.Items.Add("Make");
            cboField.Items.Add("EngineSize");
            cboField.Items.Add("RentalPerDay");
            cboField.Items.Add("Available");

            cboOperator.Items.Clear();
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
            if (cboField.Text == "" || cboOperator.Text == "" || txtValue.Text == "")
            {
                MessageBox.Show("Enter all search criteria");
                return;
            }
            try
            {
                string sqlQuery =
                    "SELECT VehicleRegNo, Make, EngineSize, DateRegistered, RentalPerDay, Available " +
                    "FROM tblCar WHERE " +
                    cboField.Text + " " +
                    cboOperator.Text + " '" +
                    txtValue.Text + "'COLLATE NOCASE";

                sqliteManager.SQLQuery = sqlQuery;
                DataTable dataTable = sqliteManager.ReadData();
                dataGridView1.DataSource = dataTable;
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
