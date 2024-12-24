using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedLabs
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            
        }
        public static class FormSwitcher
        {
            public static void SwitchMainForm(Form LoginForm, Form SearchTestsFrom)
            {
                LoginForm.Hide();
                SearchTestsFrom.Show();
                SearchTestsFrom.FormClosed += (s, args) => LoginForm.Show();
            }
        }
        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Простой пример проверки логина и пароля
            if (textBox1.Text == "admin" && textBox2.Text == "admin")
            {
                SearchTestsForm newForm = new SearchTestsForm();
                FormSwitcher.SwitchMainForm(this, newForm);

            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
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
                Application.ExitThread(); // Завершить приложение
            }
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
