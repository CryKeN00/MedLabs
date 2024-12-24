using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace MedLabs
{
    public partial class EditForm : Form
    {
        private string connectionString = "Data Source=C:\\Users\\revno\\MedLab.db;Version=3;";

        public EditForm()
        {
            InitializeComponent();
            InitializeComboBox();
            LoadData();
        }
        private void InitializeComboBox()
        {
            comboBox1.Items.Add("Patients");
            comboBox1.Items.Add("Tests");
            comboBox1.SelectedIndex = 0; // Установка первой таблицы по умолчанию
            comboBox1.SelectedIndexChanged += ComboBoxTables_SelectedIndexChanged;
        }

        private void ComboBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string selectedTable = comboBox1.SelectedItem.ToString();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter($"SELECT * FROM {selectedTable}", conn);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string selectedTable = comboBox1.SelectedItem.ToString();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = selectedTable == "Patients" ?
                    "INSERT INTO Patients (FirstName, LastName, DateOfBirth) VALUES (@FirstName, @LastName, @DateOfBirth)" :
                    "INSERT INTO Tests (TestName, TestResult, PatientId) VALUES (@TestName, @TestResult, @PatientId)";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    if (selectedTable == "Patients")
                    {
                        cmd.Parameters.AddWithValue("@FirstName", textBoxFirstName.Text);
                        cmd.Parameters.AddWithValue("@LastName", textBoxLastName.Text);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateTimePicker.Text); // Формат даты для SQLite
                    }
                    else
                    {
                        if (int.TryParse(textBoxPatientId.Text, out int patientId)) // Изменено с Value на Text
                        {
                            cmd.Parameters.AddWithValue("@TestName", textBoxTestName.Text);
                            cmd.Parameters.AddWithValue("@TestResult", textBoxTestResult.Text);
                            cmd.Parameters.AddWithValue("@PatientId", patientId);
                        }
                        else
                        {
                            MessageBox.Show("Пожалуйста, введите корректный идентификатор пациента.");
                            return; // Выход, если идентификатор некорректный
                        }
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            LoadData();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            string selectedTable = comboBox1.SelectedItem.ToString();
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем идентификатор записи
                if (dataGridView1.SelectedRows[0].Cells[0].Value != null)
                {
                    if (int.TryParse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString(), out int id)) // Проверка на корректность идентификатора
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                        {
                            conn.Open();
                            string query = selectedTable == "Patients" ?
                                "UPDATE Patients SET FirstName=@FirstName, LastName=@LastName, DateOfBirth=@DateOfBirth WHERE PatientId=@Id" :
                                "UPDATE Tests SET TestName=@TestName, TestResult=@TestResult, PatientId=@PatientId WHERE TestId=@Id";

                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Id", id);
                                if (selectedTable == "Patients")
                                {
                                    cmd.Parameters.AddWithValue("@FirstName", textBoxFirstName.Text);
                                    cmd.Parameters.AddWithValue("@LastName", textBoxLastName.Text);
                                    cmd.Parameters.AddWithValue("@DateOfBirth", dateTimePicker.Text); // Формат даты для SQLite
                                }
                                else
                                {
                                    if (int.TryParse(textBoxPatientId.Text, out int patientId)) // Изменено с Value на Text
                                    {
                                        cmd.Parameters.AddWithValue("@TestName", textBoxTestName.Text);
                                        cmd.Parameters.AddWithValue("@TestResult", textBoxTestResult.Text);
                                        cmd.Parameters.AddWithValue("@PatientId", patientId);
                                    }
                                    else
                                    {
                                        MessageBox.Show("Пожалуйста, введите корректный идентификатор пациента.");
                                        return; // Выход, если идентификатор некорректный
                                    }
                                }
                                cmd.ExecuteNonQuery();
                            }
                        }
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось преобразовать идентификатор записи в число. Пожалуйста, проверьте данные.");
                    }
                }
                else
                {
                    MessageBox.Show("Выбранная запись не содержит идентификатора.");
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для изменения.");
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBoxPatientId.Text, out int patientId))
            {
                string selectedTable = comboBox1.SelectedItem.ToString(); // Предполагаем, что comboBox1 содержит названия таблиц
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string query = "";
                    bool recordExists = false;

                    // Проверяем, существует ли запись перед удалением
                    if (selectedTable == "Patients")
                    {
                        query = "SELECT COUNT(*) FROM Patients WHERE PatientId = @PatientId";
                    }
                    else if (selectedTable == "Tests")
                    {
                        query = "SELECT COUNT(*) FROM Tests WHERE PatientId = @PatientId";
                    }
                    else
                    {
                        MessageBox.Show("Выберите корректную таблицу для удаления.");
                        return;
                    }

                    // Проверяем существование записи
                    using (SQLiteCommand checkCmd = new SQLiteCommand(query, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@PatientId", patientId);
                        recordExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                    }

                    // Если запись существует, выполняем удаление
                    if (recordExists)
                    {
                        if (selectedTable == "Patients")
                        {
                            query = "DELETE FROM Patients WHERE PatientId = @PatientId";
                        }
                        else if (selectedTable == "Tests")
                        {
                            query = "DELETE FROM Tests WHERE PatientId = @PatientId";
                        }

                        using (SQLiteCommand deleteCmd = new SQLiteCommand(query, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@PatientId", patientId);
                            int rowsAffected = deleteCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Запись успешно удалена.");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Запись с указанным идентификатором не найдена.");
                    }
                }
                LoadData(); // Обновите данные после удаления
            }
            else
            {
                MessageBox.Show("Введите ID пациента/теста.");
            }
        }
        

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Обработчик события, если он нужен
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Обработчик события, если он нужен
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
