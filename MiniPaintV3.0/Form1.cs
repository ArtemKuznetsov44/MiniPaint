using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniPaintV3._0
{
    public partial class Form1 : Form
    {
        // Область объявления глобальных переменных:
        Bitmap map;
        Bitmap mapForFigures;
        Graphics graphics;

        Pen pen = new Pen(Color.Black, 3f);
        int x1, y1;
        int xFirstClickForFigures, yFirstClickForFigures;
        Modes modes;
        private enum Modes
        {
            Pen,
            Rectangle,
            Triangle,
            Circle,
            StraightLine,

        }
        //========================================//
        public Form1()
        {
            InitializeComponent();
            SetSize();
        }
        private void SetSize()
        {

            map = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            graphics = Graphics.FromImage(map);

            // Зададим наконечникик для нашей ручки, для придания более округлых линий:
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            // Зададим начальные координаты мыши:
            x1 = 0;
            y1 = 0;
        }
        // Работа с меню.
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (button11.BackColor == Color.Aquamarine)
                button11.BackColor = Color.OldLace;
            switch (e.ClickedItem.Text)
            {
                case "Show toolbar":
                    panel1.Visible = true;
                    break;
                case "Hide toolbar":
                    panel1.Visible = false;
                    break;
                case "Save":
                    saveFileDialog1.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        if (saveFileDialog1.FileName != "" || pictureBox1.Image != null)
                            pictureBox1.Image.Save(saveFileDialog1.FileName);
                    }
                    break;
                case "Clear All":
                    graphics.Clear(pictureBox1.BackColor);
                    pictureBox1.Image = map;
                    break;
                case "Open":
                    openFileDialog1.Filter = "JPEG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png";

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        map = (Bitmap)Image.FromFile(openFileDialog1.FileName);
                        pictureBox1.Image = map;
                        graphics = Graphics.FromImage(map);
                    }
                    break;
            }
        }
        // Событие, срабатывающее при самостоятельном выборе цвета в colorDialog1.
        private void button10_Click(object sender, EventArgs e)
        {
            if (button11.BackColor == Color.Aquamarine)
                button11.BackColor = Color.OldLace;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pen.Color = colorDialog1.Color;
                button10.BackColor = colorDialog1.Color; 
            }
        }
        // Событие, отвечающие за измение цвета кисти при нажатии на кноку с установленным цветом. 
        private void button9_Click(object sender, EventArgs e)
        {
            if (button11.BackColor == Color.Aquamarine)
                button11.BackColor = Color.OldLace;
            pen.Color = ((Button)sender).BackColor;
        }
        // Регулировка ширины кисти при помощи трэкбара. 
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            pen.Width = trackBar1.Value;
        }
        // Установка коардинотов точки в момент нажатия на ЛКМ. 
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            xFirstClickForFigures = e.X;
            yFirstClickForFigures = e.Y;
        }

        // Событие выбора режима в листбоксе. 
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (listBox1.SelectedIndex)
            {
                case 0:
                    modes = Modes.Pen;
                    break;
                case 1:
                    modes = Modes.Rectangle;
                    break;
                case 2:
                    modes = Modes.Triangle;
                    break;
                case 3:
                    modes = Modes.Circle;
                    break;
                case 4:
                    modes = Modes.StraightLine;
                    break;
                default:
                    modes = Modes.Pen;
                    break;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //xEndClickForFigures = e.X;
            //yEndClickForFigures = e.Y; 

            switch (modes)
            {
                case Modes.Rectangle: // Режим - прямоугольник. 
                    if (e.X < xFirstClickForFigures && e.Y < yFirstClickForFigures)
                        graphics.DrawRectangle(pen, e.X, e.Y, xFirstClickForFigures - e.X, yFirstClickForFigures - e.Y);
                    else if (e.X > xFirstClickForFigures && e.Y > yFirstClickForFigures)
                        graphics.DrawRectangle(pen, xFirstClickForFigures, yFirstClickForFigures, e.X - xFirstClickForFigures, e.Y - yFirstClickForFigures);
                    else if (e.X > xFirstClickForFigures && e.Y < yFirstClickForFigures)
                        graphics.DrawRectangle(pen, xFirstClickForFigures, e.Y, e.X - xFirstClickForFigures, yFirstClickForFigures - e.Y);
                    else if (e.X < xFirstClickForFigures && e.Y > yFirstClickForFigures)
                        graphics.DrawRectangle(pen, e.X, yFirstClickForFigures, xFirstClickForFigures - e.X, e.Y - yFirstClickForFigures);
                    pictureBox1.Image = map;
                    break;
                case Modes.Triangle: // Режим - треугольник. 
                    Point[] points = new Point[3];
                    points[0].X = xFirstClickForFigures; points[0].Y = yFirstClickForFigures;
                    points[2].X = e.X; points[2].Y = e.Y;
                    points[1].X = e.X; points[1].Y = yFirstClickForFigures;
                    graphics.DrawPolygon(pen, points);
                    pictureBox1.Image = map;
                    break;
                case Modes.Circle: // Режим - круг. 
                    graphics.DrawEllipse(pen, xFirstClickForFigures, yFirstClickForFigures, e.X - xFirstClickForFigures, e.Y - yFirstClickForFigures);
                    pictureBox1.Image = map;
                    break;
                case Modes.StraightLine: // Режим - прямая линия. 
                    graphics.DrawLine(pen, xFirstClickForFigures, yFirstClickForFigures, e.X, e.Y);
                    pictureBox1.Image = map;
                    break;

            }
        }

     

        // Кнопак - Eraser(ластик). 
        private void button11_Click(object sender, EventArgs e)
        {
            if (button11.BackColor == Color.OldLace)
            {
                if (modes != Modes.Pen)
                    modes = Modes.Pen; 
                button11.BackColor = Color.Aquamarine;
                pen.Color = pictureBox1.BackColor;
            }
            else
            {
                button11.BackColor = Color.OldLace;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            /*
             * Так как событие MouseMove выполняется многократно,
             * следовательно в конце фукнции мы будем записывать текущие координаты, в качестве начальных 
             * -> при следующем вызове данного события, нашие пременные остануться в памяти.
             */
            if (e.Button == MouseButtons.Left) // Нажата ли ЛКМ. 
            {
                switch (modes)
                {
                    case Modes.Pen:
                        graphics.DrawLine(pen, x1, y1, e.X, e.Y);
                        pictureBox1.Image = map;
                        break;
                }

            }
            x1 = e.X;
            y1 = e.Y;
        }
    }
}
        
