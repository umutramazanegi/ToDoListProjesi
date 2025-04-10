﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
namespace ToDoListProjesi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string connectionString = "Server=localHost;port=5432;Database=ToDoListProjesi;user ID=postgres;Password=1234";
        void ToDoList()
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string query = "Select * From ToDoLists";
            var command = new NpgsqlCommand(query, connection);
            var adapter = new NpgsqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
            connection.Close();
        }
        void CategoryList()
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string query = "Select * From Categories";
            var command = new NpgsqlCommand(query, connection);
            var adapter = new NpgsqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryId";
            cmbCategory.DataSource = dataTable;
            connection.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ToDoList();
            CategoryList();
        }

        private void btnCategoryList_Click(object sender, EventArgs e)
        {
            FrmCategory frm = new FrmCategory();
            frm.Show();
        }

        private void btnList_Click(object sender, EventArgs e)
        {
            ToDoList();
        }

        private void btnAllList_Click(object sender, EventArgs e)
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string query = "Select todolistid,title,description,status,priority,categoryname from todolists inner join categories on todolists.categoryid=categories.categoryid";
            var command = new NpgsqlCommand(query, connection);
            var adapter = new NpgsqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView1.DataSource = dataTable;
            connection.Close();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            int categoryId = int.Parse(cmbCategory.SelectedValue.ToString());
            string title = txtTitle.Text;
            string priority = txtPriority.Text;
            string description = txtDescription.Text;
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string query = "insert into ToDoLists (title,description,status,priority,categoryid) values (@title,@description,B'1',@priority,@categoryid)";
            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@description", description);
                // command.Parameters.AddWithValue("@status", "B'1'");
                command.Parameters.AddWithValue("@priority", priority);
                command.Parameters.AddWithValue("@categoryid", categoryId);
                command.ExecuteNonQuery();
                MessageBox.Show("Yapılacak İşlem Eklendi");
                ToDoList();
            }
            connection.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtId.Text);

            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string query = "Delete From ToDoLists Where todolistid=@todolistid";
            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@todolistid", id);
                command.ExecuteNonQuery();
                MessageBox.Show("Veri başarıyla silindi");
                CategoryList();
            }
            connection.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtId.Text);
            int categoryId = int.Parse(cmbCategory.SelectedValue.ToString());
            string title = txtTitle.Text;
            string priority = txtPriority.Text;
            string description = txtDescription.Text;

            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string query = "Update ToDoLists Set Title=@title,Description=@description,priority=@priority,categoryid=@categoryid where todolistid=@todolistid";
            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@todolistid", id);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@priority", priority);
                command.Parameters.AddWithValue("@categoryid", categoryId);
                command.ExecuteNonQuery();
                MessageBox.Show("İşlem başarıyla güncellendi");
                CategoryList();
            }
            connection.Close();
        }

        private void btnCheckedList_Click(object sender, EventArgs e)
        {
            var connection = new NpgsqlConnection(connectionString);
            try
            {
                connection.Open();
                // Sorguyu BIT tipine uygun hale getirin: Status = B'1'
                string query = "SELECT * FROM ToDoLists WHERE Status = B'1'";
                var command = new NpgsqlCommand(query, connection);
                var adapter = new NpgsqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri listelenirken bir hata oluştu: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void btnContinueList_Click(object sender, EventArgs e)
        {
            var connection = new NpgsqlConnection(connectionString);
            try // Hata yakalama eklemek iyi bir pratiktir
            {
                connection.Open();
                // Sorguyu BIT tipine uygun hale getirin: Status = B'0'
                string query = "SELECT * FROM ToDoLists WHERE Status = B'0'";
                var command = new NpgsqlCommand(query, connection);
                var adapter = new NpgsqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri listelenirken bir hata oluştu: " + ex.Message);
            }
            finally // Bağlantının her zaman kapanmasını garantileyin
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}
