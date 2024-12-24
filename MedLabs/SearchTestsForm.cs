using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace MedLabs
{
    public partial class SearchTestsForm : Form
    {
        private string connectionString = "Data Source=C:\\Users\\revno\\MedLab.db;Version=3;";

        public SearchTestsForm()
        {
            InitializeComponent();
           
        }
        public static class FormSwitcher
        {
            public static void SwitchMainForm(Form SearchTestsForm, Form EditForm)
            {
                SearchTestsForm.Hide();
                EditForm.Show();
                EditForm.FormClosed += (s, args) => SearchTestsForm.Show();
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string query = @"
                SELECT p.FirstName, p.LastName, p.DateOfBirth, t.TestName, t.TestResult 
                FROM Patients p 
                JOIN Tests t ON p.PatientId = t.PatientId 
                WHERE p.FirstName || ' ' || p.LastName LIKE @fullName";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@fullName", "%" + textBox1.Text + "%");
                    connection.Open();

                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            // Выводим данные о пациенте (ФИО и дата рождения)
                            var firstRow = dataTable.Rows[0];
                            label3.Text = $"{firstRow["FirstName"]} {firstRow["LastName"]}";
                            label4.Text = Convert.ToDateTime(firstRow["DateOfBirth"]).ToShortDateString();

                            // Заполняем DataGridView результатами анализов
                            dataGridView1.DataSource = dataTable.DefaultView.ToTable(false, "TestName", "TestResult");
                        }
                        else
                        {
                            MessageBox.Show("Пациент не найден или нет доступных анализов.");
                            label3.Text = string.Empty;
                            label4.Text = string.Empty;
                            dataGridView1.DataSource = null;
                        }
                    }
                }
            }
        }

        private void SearchTestsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите закрыть приложение?", "Закрытие приложения",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // Отменяем закрытие
            }
            else
            {
                // Проверяем, есть ли другие открытые формы
                if (Application.OpenForms.Count == 1)
                {
                    Application.Exit(); // Завершить приложение, если это последняя форма
                }
            }
        }

        private void SearchTestsForm_Load(object sender, EventArgs e)
        {
            // Здесь вы можете добавить код, который должен выполняться при загрузке формы
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Обработчик события для label1 (если нужен)
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Обработчик события для label3 (если нужен)
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EditForm newForm = new EditForm();
            FormSwitcher.SwitchMainForm(this, newForm);
            // Скрываем текущую форму
        }
    }
}


