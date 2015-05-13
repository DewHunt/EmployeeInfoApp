using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EmployeeInfoApp
{
    public partial class EmployeeInfoUI : Form
    {
        public EmployeeInfoUI()
        {
            InitializeComponent();
        }

        private int employeeId;

        EmplyeeInfo emploInfo = new EmplyeeInfo();
        private void saveButton_Click(object sender, EventArgs e)
        {
            emploInfo.name = nameTextBox.Text;
            emploInfo.address = addressTextBox.Text;
            emploInfo.email = emailTextBox.Text;
            emploInfo.salary = Convert.ToDouble(salaryTextBox.Text);

            string connectionString = @"server = PC-301-24\SQLEXPRESS; Database = EmployeeInfoDB; Integrated Security = true";
            SqlConnection connection = new SqlConnection(connectionString);

            if (saveButton.Text == "Update")
            {
                string updateQuery = "UPDATE employee SET name ='" + emploInfo.name + "', address ='" + emploInfo.address + "', email = '" + emploInfo.email + "', salary = '" + emploInfo.salary + "' WHERE id = '" + employeeId + "'";

                SqlCommand updateCommand = new SqlCommand(updateQuery, connection);

                connection.Open();
                int rowAffected = updateCommand.ExecuteNonQuery();
                connection.Close();

                if (rowAffected > 0)
                {
                    MessageBox.Show("Updated Successfully!");

                    saveButton.Text = "Save";
                    employeeId = 0;
                    ShowAllInfo();
                    nameTextBox.Clear();
                    addressTextBox.Clear();
                    emailTextBox.Clear();
                    salaryTextBox.Clear();
                }
                else
                {
                    MessageBox.Show("Update Failed!");
                }
            }
            else
            {

                string emailExitsQuery = "SELECT * FROM employee WHERE email ='" + emploInfo.email + "'";
                SqlCommand emailExitsCommand = new SqlCommand(emailExitsQuery, connection);

                connection.Open();
                SqlDataReader reader = emailExitsCommand.ExecuteReader();
                bool status = reader.HasRows;
                connection.Close();

                if (status == true)
                {
                    MessageBox.Show("Email Alredy Taken.");
                }
                else
                {
                    string insertQuery = "INSERT INTO employee VALUES('" + emploInfo.name + "','" + emploInfo.address +
                                         "','" + emploInfo.email + "','" + emploInfo.salary + "')";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);

                    connection.Open();
                    int rowAffected = insertCommand.ExecuteNonQuery();
                    connection.Close();

                    if (rowAffected > 0)
                    {
                        MessageBox.Show("Data Successfully Saved");
                        nameTextBox.Clear();
                        addressTextBox.Clear();
                        emailTextBox.Clear();
                        salaryTextBox.Clear();
                        ShowAllInfo();
                    }
                    else
                    {
                        MessageBox.Show("Sorry, Saved Faild");
                    }
                }
            }
        }

        private void EmployeeInfoUI_Load(object sender, EventArgs e)
        {
            ShowAllInfo();
        }

        public void ShowAllInfo()
        {
            List<EmplyeeInfo> showAllInfoList = new List<EmplyeeInfo>();

            string connectionString = @"server = PC-301-24\SQLEXPRESS; Database = EmployeeInfoDB; Integrated Security = true";
            SqlConnection connection = new SqlConnection(connectionString);

            string showAllInfoQuery = "SELECT * FROM employee";
            SqlCommand showAllInfoCommand = new SqlCommand(showAllInfoQuery, connection);

            connection.Open();
            SqlDataReader showAllReader = showAllInfoCommand.ExecuteReader();

            while (showAllReader.Read())
            {
                EmplyeeInfo emplyeeInfo = new EmplyeeInfo();
                emplyeeInfo.id = int.Parse(showAllReader["id"].ToString());
                emplyeeInfo.name = showAllReader["name"].ToString();
                emplyeeInfo.address = showAllReader["address"].ToString();
                emplyeeInfo.email = showAllReader["email"].ToString();
                emplyeeInfo.salary = Convert.ToDouble(showAllReader["salary"].ToString());

                showAllInfoList.Add(emplyeeInfo);
            }
            showAllReader.Close();
            connection.Close();

            showAllListView.Items.Clear();
            foreach (var showAllInfo in showAllInfoList)
            {
                ListViewItem item = new ListViewItem(showAllInfo.id.ToString());
                item.SubItems.Add(showAllInfo.name);
                item.SubItems.Add(showAllInfo.address);
                item.SubItems.Add(showAllInfo.email);
                item.SubItems.Add(showAllInfo.salary.ToString());

                showAllListView.Items.Add(item);
            }
        }

        private void showAllListView_DoubleClick(object sender, EventArgs e)
        {
            string connectionString = @"server = PC-301-24\SQLEXPRESS; Database = EmployeeInfoDB; Integrated Security = true";
            SqlConnection connection = new SqlConnection(connectionString);

            ListViewItem item = showAllListView.SelectedItems[0];

            employeeId = int.Parse(item.Text.ToString());

            string doubleClickQuery = "SELECT * FROM employee WHERE ID ='" + employeeId + "'";

            SqlCommand doubleClickCommand = new SqlCommand(doubleClickQuery, connection);

            connection.Open();
            SqlDataReader doubleClickReader = doubleClickCommand.ExecuteReader();

            while (doubleClickReader.Read())
            {
                EmplyeeInfo aEmplyeeInfo = new EmplyeeInfo();
                aEmplyeeInfo.id = int.Parse(doubleClickReader["ID"].ToString());
                nameTextBox.Text = aEmplyeeInfo.name = doubleClickReader["Name"].ToString();
                addressTextBox.Text = aEmplyeeInfo.address = doubleClickReader["Address"].ToString();
                emailTextBox.Text = aEmplyeeInfo.email = doubleClickReader["Email"].ToString();
                salaryTextBox.Text = (aEmplyeeInfo.salary = Convert.ToDouble(doubleClickReader["Salary"])).ToString();
            }

            doubleClickReader.Close();
            connection.Close();

            saveButton.Text = "Update";
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            string connectionString = @"server = PC-301-24\SQLEXPRESS; Database = EmployeeInfoDB; Integrated Security = true";
            SqlConnection connection = new SqlConnection(connectionString);

            string deleteQuery = "DELETE FROM employee WHERE id = '" + employeeId + "'";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);

            connection.Open();
            int rowAffected = deleteCommand.ExecuteNonQuery();
            connection.Close();

            if (rowAffected > 0)
            {
                DialogResult deleteDialogeResult = MessageBox.Show("Are you Sure", "Warning", MessageBoxButtons.YesNo);

                if (deleteDialogeResult == DialogResult.Yes)
                {
                    MessageBox.Show("Deleted Successfully!");

                    saveButton.Text = "Save";
                    employeeId = 0;
                    ShowAllInfo();
                    nameTextBox.Clear();
                    addressTextBox.Clear();
                    emailTextBox.Clear();
                    salaryTextBox.Clear();
                }
            }
            else
            {
                MessageBox.Show("Delete Failed!");
            }
        }
    }
}
