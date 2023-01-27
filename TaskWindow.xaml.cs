using System;
using System.Drawing;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace GeneticPlacement
{
    public partial class TaskWindow
    {
        public PictureBox pictureBox1 = new ();
        public TaskWindow(double tmpHeight, double tmpWidth)
        {
            Width = tmpWidth;
            Height = tmpHeight;
            InitializeComponent();
            
            pictureBox1.Height = Convert.ToInt32(Height);
            pictureBox1.Width = Convert.ToInt32(Width);
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.BackColor = Color.Green;
            FormsHost.Child = pictureBox1;
            pictureBox1.Paint += picturebox1_Paint;
        }

        private static void picturebox1_Paint(object? sender, PaintEventArgs e)
        {
            const string pathDirectory = @"C:\Users\bgima\OneDrive\Изображения\";
            var positions = Population.BestPlacementOfElements;
            for (int i = 1; i <= positions.Count; i++ )
            {
                var xi = MainWindow.elements.Coordinates[i].x;
                var yi = MainWindow.elements.Coordinates[i].y;
                var ulPoint2 = new System.Drawing.Point(xi, yi);
                for (int j = 1; j <= positions.Count; j++)
                {
                    var xj = MainWindow.elements.Coordinates[j].x;
                    var yj = MainWindow.elements.Coordinates[j].y;
                    if (MainWindow.MatrixConnection[i-1][j-1] == 1 )
                    {
                        var lineColor = new Pen(Color.Gold, 3);
                        var ulPoint3 = new System.Drawing.Point(xj, yj);
                        e.Graphics.DrawLine(lineColor, ulPoint2, ulPoint3);
                    }
                    if (MainWindow.MatrixConnection[i-1][j-1] > 1 )
                    {
                        var lineColor = new Pen(Color.Gold, 3*MainWindow.MatrixConnection[i-1][j-1]);
                        var ulPoint3 = new System.Drawing.Point(xj, yj);
                        e.Graphics.DrawLine(lineColor, ulPoint2, ulPoint3);
                    }
                }
            }
            for (int i = 1; i <= positions.Count; i++ )
            {
                var colorRect = new Pen(Color.Black);
                var ai = Convert.ToInt32(MainWindow.elements.Dimensions[i].length);
                var bi = Convert.ToInt32(MainWindow.elements.Dimensions[i].width);
                var xi = MainWindow.elements.Coordinates[i].x;
                var yi = MainWindow.elements.Coordinates[i].y;
                var xicorner = xi - (ai / 2);
                var yicorner = yi - (bi / 2);
                var bmp = new Bitmap(pathDirectory+$"chip.png");
                var b2 = new Bitmap(bmp, new System.Drawing.Size(ai, bi));
                var ulPoint1 = new System.Drawing.Point(xicorner, yicorner);
                e.Graphics.DrawRectangle(colorRect, xicorner, yicorner, ai, bi);
                e.Graphics.DrawImage(b2, ulPoint1);
            }
        }
    }
}
