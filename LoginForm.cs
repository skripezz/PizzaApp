using System;
using System.Drawing;
using System.Windows.Forms;
using PizzaApp.Helpers;

namespace PizzaApp
{
    public partial class LoginForm : Form
    {
        private int failedAttempts = 0;
        private bool puzzleSolved = false;
        private PictureBox[] puzzlePieces;
        private Point[] correctPositions;
        private Point[] currentPositions;
        private int[] pieceOrder;
        private int rows = 3;
        private int cols = 3;
        private int puzzleSize = 300;
        private TextBox txtLogin;
        private TextBox txtPassword;

        public LoginForm()
        {
            InitializeComponent();
            this.Text = "Авторизация - ООО \"Два сеньора\"";
            this.MinimumSize = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            SetupPuzzle();
        }

        private void SetupPuzzle()
        {
            Panel puzzlePanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(puzzleSize + 40, puzzleSize + 40),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };

            Bitmap originalImage = CreatePuzzleImage();
            int pieceWidth = originalImage.Width / cols;
            int pieceHeight = originalImage.Height / rows;

            puzzlePieces = new PictureBox[rows * cols];
            correctPositions = new Point[rows * cols];
            currentPositions = new Point[rows * cols];
            pieceOrder = new int[rows * cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int index = i * cols + j;
                    correctPositions[index] = new Point(j * pieceWidth, i * pieceHeight);
                    currentPositions[index] = correctPositions[index];
                    pieceOrder[index] = index;
                }
            }

            Random rnd = new Random();
            for (int i = pieceOrder.Length - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                int temp = pieceOrder[i];
                pieceOrder[i] = pieceOrder[j];
                pieceOrder[j] = temp;
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int index = i * cols + j;
                    int pieceIndex = pieceOrder[index];
                    int correctRow = pieceIndex / cols;
                    int correctCol = pieceIndex % cols;

                    Rectangle srcRect = new Rectangle(
                        correctCol * pieceWidth,
                        correctRow * pieceHeight,
                        pieceWidth,
                        pieceHeight
                    );

                    Bitmap pieceBitmap = new Bitmap(pieceWidth, pieceHeight);
                    using (Graphics g = Graphics.FromImage(pieceBitmap))
                    {
                        g.DrawImage(originalImage, 0, 0, srcRect, GraphicsUnit.Pixel);
                    }

                    puzzlePieces[index] = new PictureBox
                    {
                        Image = pieceBitmap,
                        Size = new Size(pieceWidth - 2, pieceHeight - 2),
                        Location = new Point(
                            20 + currentPositions[index].X,
                            20 + currentPositions[index].Y
                        ),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Tag = index,
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle,
                        Cursor = Cursors.Hand
                    };

                    puzzlePieces[index].MouseDown += PuzzlePiece_MouseDown;
                    puzzlePieces[index].MouseMove += PuzzlePiece_MouseMove;
                    puzzlePieces[index].MouseUp += PuzzlePiece_MouseUp;

                    puzzlePanel.Controls.Add(puzzlePieces[index]);
                }
            }

            this.Controls.Add(puzzlePanel);

            Button btnCheck = new Button
            {
                Text = "Проверить пазл",
                Location = new Point(20, puzzleSize + 80),
                Size = new Size(150, 30)
            };
            btnCheck.Click += BtnCheck_Click;
            this.Controls.Add(btnCheck);

            Label lblLogin = new Label { Text = "Логин:", Location = new Point(20, puzzleSize + 130), Size = new Size(80, 25) };
            txtLogin = new TextBox { Location = new Point(100, puzzleSize + 130), Size = new Size(150, 25) };

            Label lblPassword = new Label { Text = "Пароль:", Location = new Point(20, puzzleSize + 165), Size = new Size(80, 25) };
            txtPassword = new TextBox { Location = new Point(100, puzzleSize + 165), Size = new Size(150, 25), PasswordChar = '*' };

            Button btnLogin = new Button
            {
                Text = "Войти",
                Location = new Point(100, puzzleSize + 200),
                Size = new Size(100, 35)
            };
            btnLogin.Click += BtnLogin_Click;

            this.Controls.AddRange(new Control[] { lblLogin, txtLogin, lblPassword, txtPassword, btnLogin });
        }

        private Bitmap CreatePuzzleImage()
        {
            Bitmap bmp = new Bitmap(300, 300);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightBlue);
                g.FillRectangle(Brushes.Green, 50, 50, 200, 200);
                g.DrawEllipse(Pens.Red, 100, 100, 100, 100);
                g.DrawString("Pizza", new Font("Arial", 30), Brushes.White, 100, 130);
            }
            return bmp;
        }

        private Point mouseDownPos;
        private int selectedIndex = -1;

        private void PuzzlePiece_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            selectedIndex = (int)pb.Tag;
            mouseDownPos = e.Location;
            pb.BringToFront();
        }

        private void PuzzlePiece_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedIndex == -1) return;
            PictureBox pb = sender as PictureBox;
            if (e.Button == MouseButtons.Left)
            {
                pb.Left += e.X - mouseDownPos.X;
                pb.Top += e.Y - mouseDownPos.Y;
            }
        }

        private void PuzzlePiece_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectedIndex == -1) return;
            PictureBox pb = sender as PictureBox;

            int gridX = (int)Math.Round((double)(pb.Left - 20) / (puzzleSize / cols));
            int gridY = (int)Math.Round((double)(pb.Top - 20) / (puzzleSize / rows));

            gridX = Math.Max(0, Math.Min(cols - 1, gridX));
            gridY = Math.Max(0, Math.Min(rows - 1, gridY));

            int targetIndex = gridY * cols + gridX;

            if (targetIndex != selectedIndex)
            {
                Point tempPos = puzzlePieces[targetIndex].Location;
                puzzlePieces[targetIndex].Location = new Point(
                    20 + currentPositions[selectedIndex].X,
                    20 + currentPositions[selectedIndex].Y
                );
                pb.Location = new Point(
                    20 + tempPos.X - 20,
                    20 + tempPos.Y - 20
                );

                Point tempCurrent = currentPositions[selectedIndex];
                currentPositions[selectedIndex] = currentPositions[targetIndex];
                currentPositions[targetIndex] = tempCurrent;

                int tempOrder = pieceOrder[selectedIndex];
                pieceOrder[selectedIndex] = pieceOrder[targetIndex];
                pieceOrder[targetIndex] = tempOrder;
            }
            else
            {
                pb.Location = new Point(
                    20 + currentPositions[selectedIndex].X,
                    20 + currentPositions[selectedIndex].Y
                );
            }

            selectedIndex = -1;
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            bool isCorrect = true;
            for (int i = 0; i < pieceOrder.Length; i++)
            {
                if (pieceOrder[i] != i)
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                puzzleSolved = true;
                MessageBox.Show("Пазл собран верно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                puzzleSolved = false;
                MessageBox.Show("Пазл собран неверно! Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (!puzzleSolved)
            {
                MessageBox.Show("Соберите пазл перед авторизацией!",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Поля 'Логин' и 'Пароль' обязательны для заполнения!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DatabaseHelper.IsUserBlocked(login))
            {
                MessageBox.Show("Вы заблокированы. Обратитесь к администратору.",
                    "Доступ запрещен", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DatabaseHelper.ValidateUser(login, password))
            {
                string role = DatabaseHelper.GetUserRole(login);
                MessageBox.Show($"Вы успешно авторизовались!\nРоль: {role}",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                MainForm mainForm = new MainForm(login, role);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                failedAttempts++;

                if (failedAttempts >= 3)
                {
                    DatabaseHelper.BlockUser(login);
                    MessageBox.Show("Вы заблокированы. Обратитесь к администратору.",
                        "Доступ запрещен", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Вы ввели неверный логин или пароль. Пожалуйста проверьте ещё раз введенные данные.\nПопытка {failedAttempts} из 3",
                        "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}