using System;
using System.Data;
using System.Windows.Forms;
using PizzaApp.Helpers;

namespace PizzaApp
{
    public partial class UserManagementForm : Form
    {
        private DataGridView dataGridViewUsers;
        private TextBox txtLogin;
        private TextBox txtPassword;
        private ComboBox cmbRole;

        public UserManagementForm()
        {
            InitializeComponent();
            this.Text = "Управление пользователями - ООО \"Два сеньора\"";
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupUI();
            LoadUsers();
        }

        private void SetupUI()
        {
            Panel panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                Padding = new Padding(10)
            };

            Label lblLogin = new Label { Text = "Логин:", Location = new System.Drawing.Point(10, 15), Size = new System.Drawing.Size(60, 25) };
            txtLogin = new TextBox { Location = new System.Drawing.Point(80, 15), Size = new System.Drawing.Size(150, 25) };

            Label lblPassword = new Label { Text = "Пароль:", Location = new System.Drawing.Point(10, 50), Size = new System.Drawing.Size(60, 25) };
            txtPassword = new TextBox { Location = new System.Drawing.Point(80, 50), Size = new System.Drawing.Size(150, 25) };

            Label lblRole = new Label { Text = "Роль:", Location = new System.Drawing.Point(10, 85), Size = new System.Drawing.Size(60, 25) };
            cmbRole = new ComboBox { Location = new System.Drawing.Point(80, 85), Size = new System.Drawing.Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new object[] { "Администратор", "Пользователь" });
            cmbRole.SelectedIndex = 1;

            Button btnAdd = new Button
            {
                Text = "Добавить",
                Location = new System.Drawing.Point(250, 15),
                Size = new System.Drawing.Size(100, 30)
            };
            btnAdd.Click += BtnAdd_Click;

            Button btnUnblock = new Button
            {
                Text = "Разблокировать",
                Location = new System.Drawing.Point(250, 55),
                Size = new System.Drawing.Size(100, 30)
            };
            btnUnblock.Click += BtnUnblock_Click;

            Button btnDelete = new Button
            {
                Text = "Удалить",
                Location = new System.Drawing.Point(250, 95),
                Size = new System.Drawing.Size(100, 30)
            };
            btnDelete.Click += BtnDelete_Click;

            panelTop.Controls.AddRange(new Control[] { lblLogin, txtLogin, lblPassword, txtPassword, lblRole, cmbRole, btnAdd, btnUnblock, btnDelete });

            dataGridViewUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.Controls.Add(dataGridViewUsers);
            this.Controls.Add(panelTop);
        }

        private void LoadUsers()
        {
            DataTable dt = DatabaseHelper.GetAllUsers();
            dataGridViewUsers.DataSource = dt;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = cmbRole.SelectedItem.ToString();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DatabaseHelper.UserExists(login))
            {
                MessageBox.Show($"Пользователь с логином '{login}' уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DatabaseHelper.AddUser(login, password, role);
            MessageBox.Show("Пользователь успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadUsers();
        }

        private void BtnUnblock_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                string login = dataGridViewUsers.SelectedRows[0].Cells["логин"].Value.ToString();
                DatabaseHelper.UnblockUser(login);
                MessageBox.Show($"Пользователь '{login}' разблокирован!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                string login = dataGridViewUsers.SelectedRows[0].Cells["логин"].Value.ToString();
                DialogResult result = MessageBox.Show($"Удалить пользователя '{login}'?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    DatabaseHelper.DeleteUser(login);
                    LoadUsers();
                }
            }
        }
    }
}