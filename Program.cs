using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace SimpleCalculator
{
    public class CalculatorForm : Form
    {
        private TextBox txtDisplay;
        private Label lblFormula;
        private Button btn0, btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9;
        private Button btnDecimal, btnNegate, btnBackspace;
        private Button btnAdd, btnSubtract, btnMultiply, btnDivide, btnEquals, btnClear;
        private Panel titleBar;
        private Label lblTitle;
        private Button btnClose;
        private bool isDragging = false;
        private Point lastCursorPosition;
        private double result;
        private string operation;
        private bool newCalculation = true;
        private string lastFormula = "";

        public CalculatorForm()
        {
            this.Width = 330;
            this.Height = 330;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(33, 33, 33);
            this.KeyPreview = true;
            this.AcceptButton = null;

            ApplyRoundedCorners(this, 10);

            // Title bar
            titleBar = new Panel();
            titleBar.BackColor = Color.FromArgb(33, 33, 33);
            titleBar.Location = new System.Drawing.Point(5, 5);
            titleBar.Size = new Size(320, 24);

            lblTitle = new Label();
            lblTitle.Text = "Calculator";
            lblTitle.ForeColor = Color.FromArgb(220, 220, 220);
            lblTitle.Location = new System.Drawing.Point(5, 5);
            lblTitle.AutoSize = true;

            btnClose = new Button();
            btnClose.Text = "×";
            btnClose.Location = new System.Drawing.Point(290, 0);
            btnClose.Size = new Size(30, 24);
            btnClose.BackColor = Color.FromArgb(33, 33, 33);
            btnClose.ForeColor = Color.FromArgb(220, 220, 220);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Arial", 12, FontStyle.Bold);
            btnClose.TextAlign = ContentAlignment.MiddleCenter;
            btnClose.Click += (s, e) => this.Close();
            ApplyRoundedCorners(btnClose, 4);

            // Formula label (faded)
            lblFormula = new Label();
            lblFormula.Location = new System.Drawing.Point(20, 34);
            lblFormula.Size = new Size(290, 20);
            lblFormula.BackColor = Color.FromArgb(50, 50, 50);
            lblFormula.ForeColor = Color.FromArgb(100, 100, 100);
            lblFormula.TextAlign = ContentAlignment.MiddleRight;
            lblFormula.Font = new Font("Arial", 10);
            lblFormula.Text = "";

            // Display
            txtDisplay = new TextBox();
            txtDisplay.Location = new System.Drawing.Point(20, 54);
            txtDisplay.Width = 290;
            txtDisplay.Height = 40;
            txtDisplay.ReadOnly = true;
            txtDisplay.BackColor = Color.FromArgb(50, 50, 50);
            txtDisplay.ForeColor = Color.FromArgb(220, 220, 220);
            txtDisplay.BorderStyle = BorderStyle.None;
            txtDisplay.TextAlign = HorizontalAlignment.Right;
            txtDisplay.Font = new Font("Arial", 16, FontStyle.Bold);
            // No rounded corners—leave as rectangle
            txtDisplay.Paint += TxtDisplay_Paint;

            // Number buttons (numpad layout)
            btn7 = CreateButton("7", 20, 110);
            btn8 = CreateButton("8", 80, 110);
            btn9 = CreateButton("9", 140, 110);
            btnDivide = CreateButton("÷", 200, 110);
            btnClear = CreateButton("C", 260, 110);

            btn4 = CreateButton("4", 20, 160);
            btn5 = CreateButton("5", 80, 160);
            btn6 = CreateButton("6", 140, 160);
            btnMultiply = CreateButton("×", 200, 160);
            btnNegate = CreateButton("±", 260, 160);

            btn1 = CreateButton("1", 20, 210);
            btn2 = CreateButton("2", 80, 210);
            btn3 = CreateButton("3", 140, 210);
            btnSubtract = CreateButton("-", 200, 210);
            btnBackspace = CreateButton("⌫", 260, 210);

            btn0 = CreateButton("0", 20, 260, 110);
            btnDecimal = CreateButton(".", 140, 260);
            btnAdd = CreateButton("+", 200, 260);
            btnEquals = CreateButton("=", 260, 260);

            // Add controls
            this.Controls.Add(titleBar);
            titleBar.Controls.Add(lblTitle);
            titleBar.Controls.Add(btnClose);
            this.Controls.Add(lblFormula);
            this.Controls.Add(txtDisplay);
            this.Controls.Add(btn0);
            this.Controls.Add(btn1);
            this.Controls.Add(btn2);
            this.Controls.Add(btn3);
            this.Controls.Add(btn4);
            this.Controls.Add(btn5);
            this.Controls.Add(btn6);
            this.Controls.Add(btn7);
            this.Controls.Add(btn8);
            this.Controls.Add(btn9);
            this.Controls.Add(btnDecimal);
            this.Controls.Add(btnNegate);
            this.Controls.Add(btnBackspace);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnSubtract);
            this.Controls.Add(btnMultiply);
            this.Controls.Add(btnDivide);
            this.Controls.Add(btnEquals);
            this.Controls.Add(btnClear);

            // Dragging events
            titleBar.MouseDown += TitleBar_MouseDown;
            titleBar.MouseMove += TitleBar_MouseMove;
            titleBar.MouseUp += TitleBar_MouseUp;

            // Button hover effects
            AddHoverEffects();

            // Keyboard events
            this.Paint += Form_Paint;
        }

        private Button CreateButton(string text, int x, int y, int width = 50)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new System.Drawing.Point(x, y);
            btn.Width = width;
            btn.Height = 40;
            btn.BackColor = Color.FromArgb(60, 60, 60);
            btn.ForeColor = Color.FromArgb(220, 220, 220);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Arial", 12, FontStyle.Bold);
            ApplyRoundedCorners(btn, 4);
            return btn;
        }

        private void ApplyRoundedCorners(Control control, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle bounds = new Rectangle(0, 0, control.Width, control.Height);
            path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Width - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(bounds.Width - radius * 2, bounds.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(bounds.X, bounds.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        private void AddHoverEffects()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button btn && btn != btnClose)
                {
                    btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(80, 80, 80);
                    btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(60, 60, 60);
                }
            }
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(80, 80, 80);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(33, 33, 33);
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(60, 60, 60), 2f))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                e.Graphics.DrawPath(pen, RoundedRect(rect, 10));
            }
        }

        private void TxtDisplay_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(80, 80, 80), 1f))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, txtDisplay.Width - 1, txtDisplay.Height - 1);
                e.Graphics.DrawRectangle(pen, rect); // Plain rectangle, no rounded corners
            }
            using (SolidBrush brush = new SolidBrush(txtDisplay.ForeColor))
            {
                string text = txtDisplay.Text;
                SizeF textSize = e.Graphics.MeasureString(text, txtDisplay.Font);
                float x = txtDisplay.Width - textSize.Width - 10;
                float y = (txtDisplay.Height - textSize.Height) / 2;
                e.Graphics.DrawString(text, txtDisplay.Font, brush, x, y);
            }
        }

        private void Button_Paint(object sender, PaintEventArgs e)
        {
            // No painting needed
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Width - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(bounds.Width - radius * 2, bounds.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(bounds.X, bounds.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursorPosition = e.Location;
            }
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point newPosition = Point.Subtract(this.Location, (Size)lastCursorPosition);
                newPosition = Point.Add(newPosition, (Size)e.Location);
                this.Location = newPosition;
            }
        }

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.NumPad0:
                case Keys.D0:
                    Number_Click(btn0, EventArgs.Empty);
                    return true;
                case Keys.NumPad1:
                case Keys.D1:
                    Number_Click(btn1, EventArgs.Empty);
                    return true;
                case Keys.NumPad2:
                case Keys.D2:
                    Number_Click(btn2, EventArgs.Empty);
                    return true;
                case Keys.NumPad3:
                case Keys.D3:
                    Number_Click(btn3, EventArgs.Empty);
                    return true;
                case Keys.NumPad4:
                case Keys.D4:
                    Number_Click(btn4, EventArgs.Empty);
                    return true;
                case Keys.NumPad5:
                case Keys.D5:
                    Number_Click(btn5, EventArgs.Empty);
                    return true;
                case Keys.NumPad6:
                case Keys.D6:
                    Number_Click(btn6, EventArgs.Empty);
                    return true;
                case Keys.NumPad7:
                case Keys.D7:
                    Number_Click(btn7, EventArgs.Empty);
                    return true;
                case Keys.NumPad8:
                case Keys.D8:
                    Number_Click(btn8, EventArgs.Empty);
                    return true;
                case Keys.NumPad9:
                case Keys.D9:
                    Number_Click(btn9, EventArgs.Empty);
                    return true;
                case Keys.Decimal:
                case Keys.OemPeriod:
                    Decimal_Click(btnDecimal, EventArgs.Empty);
                    return true;
                case Keys.Add:
                    Operation_Click(btnAdd, EventArgs.Empty);
                    return true;
                case Keys.Subtract:
                    Operation_Click(btnSubtract, EventArgs.Empty);
                    return true;
                case Keys.Multiply:
                    Operation_Click(btnMultiply, EventArgs.Empty);
                    return true;
                case Keys.Divide:
                case Keys.OemQuestion:
                    Operation_Click(btnDivide, EventArgs.Empty);
                    return true;
                case Keys.Enter:
                    Equals_Click(btnEquals, EventArgs.Empty);
                    return true;
                case Keys.Escape:
                    Clear_Click(btnClear, EventArgs.Empty);
                    return true;
                case Keys.Back:
                    Backspace_Click(btnBackspace, EventArgs.Empty);
                    return true;
                case Keys.OemMinus:
                    if (string.IsNullOrEmpty(operation) || txtDisplay.Text.EndsWith(operation + " "))
                    {
                        Negate_Click(btnNegate, EventArgs.Empty);
                        return true;
                    }
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Number_Click(object sender, EventArgs e)
        {
            string display = txtDisplay.Text.Trim();
            if (newCalculation)
            {
                txtDisplay.Text = ((Button)sender).Text;
                newCalculation = false;
            }
            else if (operation != "" && display.EndsWith(operation + " "))
            {
                txtDisplay.Text = display + ((Button)sender).Text;
            }
            else if (operation == "" || double.TryParse(display.Split(' ').Last(), out _))
            {
                txtDisplay.Text = display + ((Button)sender).Text;
            }
            else
            {
                txtDisplay.Text += ((Button)sender).Text;
            }
            lblFormula.Text = txtDisplay.Text;
        }

        private void Decimal_Click(object sender, EventArgs e)
        {
            string display = txtDisplay.Text.Trim();
            string currentNumber = display.Split(' ').Last();
            if (!currentNumber.Contains("."))
            {
                if (newCalculation)
                {
                    txtDisplay.Text = "0.";
                    newCalculation = false;
                }
                else if (operation != "" && display.EndsWith(operation + " "))
                {
                    txtDisplay.Text = display + "0.";
                }
                else
                {
                    txtDisplay.Text += ".";
                }
            }
            lblFormula.Text = txtDisplay.Text;
        }

        private void Negate_Click(object sender, EventArgs e)
        {
            string[] parts = txtDisplay.Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                string lastPart = parts.Last();
                if (double.TryParse(lastPart, out double num))
                {
                    num = -num;
                    parts[parts.Length - 1] = num.ToString();
                    txtDisplay.Text = string.Join(" ", parts);
                }
                else if (lastPart == "")
                {
                    txtDisplay.Text += "-";
                    newCalculation = false;
                }
            }
            else
            {
                txtDisplay.Text = "-";
                newCalculation = false;
            }
            lblFormula.Text = txtDisplay.Text;
        }

        private void Backspace_Click(object sender, EventArgs e)
        {
            if (txtDisplay.Text.Length > 0 && !newCalculation)
            {
                string display = txtDisplay.Text.TrimEnd();
                if (display.EndsWith(" + ") || display.EndsWith(" - ") || 
                    display.EndsWith(" × ") || display.EndsWith(" ÷ "))
                {
                    txtDisplay.Text = display.Substring(0, display.Length - 3).TrimEnd();
                    operation = "";
                    if (txtDisplay.Text.Length > 0)
                    {
                        double.TryParse(txtDisplay.Text.Split(' ').Last(), out result);
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else if (display.Contains(" "))
                {
                    string[] parts = display.Split(' ');
                    parts[parts.Length - 1] = parts.Last().Substring(0, parts.Last().Length - 1);
                    if (parts.Last() == "")
                    {
                        parts = parts.Take(parts.Length - 1).ToArray();
                        txtDisplay.Text = string.Join(" ", parts).Trim() + " ";
                    }
                    else
                    {
                        txtDisplay.Text = string.Join(" ", parts).Trim();
                    }
                }
                else
                {
                    txtDisplay.Text = display.Substring(0, display.Length - 1);
                }
                lblFormula.Text = txtDisplay.Text;
            }
        }

        private void Operation_Click(object sender, EventArgs e)
        {
            if (txtDisplay.Text.Length > 0)
            {
                if (operation != "" && !newCalculation)
                {
                    Equals_Click(sender, e);
                }
                if (double.TryParse(txtDisplay.Text.Split(' ').Last(), out result))
                {
                    operation = ((Button)sender).Text;
                    txtDisplay.Text += " " + operation + " ";
                    newCalculation = false;
                }
                lblFormula.Text = txtDisplay.Text;
            }
        }

        private void Equals_Click(object sender, EventArgs e)
        {
            if (operation != "" && txtDisplay.Text.Length > 0)
            {
                lastFormula = txtDisplay.Text;
                string[] parts = txtDisplay.Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3 && double.TryParse(parts[parts.Length - 1], out double number2))
                {
                    switch (operation)
                    {
                        case "+":
                            result += number2;
                            break;
                        case "-":
                            result -= number2;
                            break;
                        case "×":
                            result *= number2;
                            break;
                        case "÷":
                            if (number2 != 0)
                                result /= number2;
                            else
                            {
                                txtDisplay.Text = "Error: Divide by Zero";
                                result = 0;
                                operation = "";
                                newCalculation = true;
                                lblFormula.Text = lastFormula;
                                return;
                            }
                            break;
                    }
                    txtDisplay.Text = result.ToString();
                    operation = "";
                    newCalculation = true;
                    lblFormula.Text = lastFormula;
                }
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            txtDisplay.Text = "";
            lblFormula.Text = "";
            result = 0;
            operation = "";
            newCalculation = true;
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new CalculatorForm());
        }
    }
}