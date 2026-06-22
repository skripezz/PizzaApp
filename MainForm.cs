using System;
using System.Windows.Forms;
using PizzaApp.Helpers;

namespace PizzaApp
{
    public partial class MainForm : Form
    {
        private string currentUser;
        private string userRole;

        public MainForm(string login, string role)
        {
            InitializeComponent();
            currentUser = login;
            userRole = role;

            this.Text = $"Главное окно - ООО \"Два сеньора\" (Пользователь: {login})";
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            SetupUI();
        }

        private void SetupUI()
        {
            MenuStrip menuStrip = new MenuStrip();

            ToolStripMenuItem menuReferences = new ToolStripMenuItem("Справочники");
            menuReferences.DropDownItems.Add("Продукция", null, (s, e) => ShowMessage("Справочник продукции"));
            menuReferences.DropDownItems.Add("Материалы", null, (s, e) => ShowMessage("Справочник материалов"));
            menuReferences.DropDownItems.Add("Заказчики", null, (s, e) => ShowMessage("Справочник заказчиков"));
            menuStrip.Items.Add(menuReferences);

            ToolStripMenuItem menuOrders = new ToolStripMenuItem("Заказы");
            menuOrders.DropDownItems.Add("Новый заказ", null, (s, e) => ShowMessage("Создание заказа"));
            menuOrders.DropDownItems.Add("Список заказов", null, (s, e) => ShowMessage("Список заказов"));
            menuStrip.Items.Add(menuOrders);

            if (userRole == "Администратор")
            {
                ToolStripMenuItem menuAdmin = new ToolStripMenuItem("Администрирование");
                menuAdmin.DropDownItems.Add("Управление пользователями", null, (s, e) =>
                {
                    UserManagementForm form = new UserManagementForm();
                    form.ShowDialog();
                });
                menuStrip.Items.Add(menuAdmin);
            }

            ToolStripMenuItem menuHelp = new ToolStripMenuItem("Справка");
            menuHelp.DropDownItems.Add("О программе", null, (s, e) =>
            {
                MessageBox.Show("Учет производства ООО \"Два сеньора\"\nВерсия 1.0",
                    "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
            menuStrip.Items.Add(menuHelp);

            ToolStripMenuItem menuExit = new ToolStripMenuItem("Выход");
            menuExit.Click += (s, e) => Application.Exit();
            menuStrip.Items.Add(menuExit);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            StatusStrip statusStrip = new StatusStrip();
            ToolStripStatusLabel statusLabel = new ToolStripStatusLabel($"Пользователь: {currentUser} | Роль: {userRole}");
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);

            Label welcomeLabel = new Label
            {
                Text = $"Добро пожаловать, {currentUser}!",
                Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(20, 50),
                AutoSize = true
            };
            this.Controls.Add(welcomeLabel);

            Label infoLabel = new Label
            {
                Text = "Выберите нужный раздел в меню для работы с системой.",
                Location = new System.Drawing.Point(20, 90),
                AutoSize = true,
                ForeColor = System.Drawing.Color.Gray
            };
            this.Controls.Add(infoLabel);
        }

        private void ShowMessage(string text)
        {
            MessageBox.Show($"Функционал '{text}' в разработке.",
                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}