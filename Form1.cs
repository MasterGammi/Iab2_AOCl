using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.ImgHash;
using Emgu.CV.Structure;
using Emgu.CV.Util;


namespace lab2
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> srcImg;
        double move = 0;
        PointF[] pts = new PointF[4];
        int c = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                srcImg = new Image<Bgr, byte>(fileName).Resize(640, 480, Inter.Linear);

                imageBox1.Image = srcImg;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double k = Double.Parse(textBox1.Text);
            Image<Bgr, byte> scaledImg = new Image<Bgr,byte>((int)(srcImg.Width * k), (int)(srcImg.Height * k));

            for(int i = 0; i < scaledImg.Width; i++)
                for(int j = 0; j < scaledImg.Height; j++)
                {
                    double I = (i / k);
                    double J = (j / k);
                    double baseI = Math.Floor(I);
                    double baseJ = Math.Floor(J);

                    if (baseI >= srcImg.Width - 1) continue;
                    if (baseJ >= srcImg.Height -1) continue;

                    double rI = I - baseI;
                    double rJ = J - baseJ;
                    double irI = 1 - rI;
                    double irJ = 1 - rJ;

                    Bgr c1 = new Bgr();
                    c1.Blue = srcImg.Data[(int)baseJ, (int)baseI, 0] * irI + srcImg.Data[(int)baseJ, (int)(baseI + 1), 0] * rI;
                    c1.Green = srcImg.Data[(int)baseJ, (int)baseI, 1] * irI + srcImg.Data[(int)baseJ, (int)(baseI + 1), 1] * rI;
                    c1.Red = srcImg.Data[(int)baseJ, (int)baseI, 2] * irI + srcImg.Data[(int)baseJ, (int)(baseI + 1), 2] * rI;

                    Bgr c2 = new Bgr();
                    c2.Blue = srcImg.Data[(int)(baseJ +1), (int)baseI,0] * irI + srcImg.Data[(int)(baseJ+1), (int)(baseI + 1), 0] * rI;
                    c2.Green = srcImg.Data[(int)(baseJ + 1), (int)baseI, 1] * irI + srcImg.Data[(int)(baseJ + 1), (int)(baseI + 1), 1] * rI;
                    c2.Red = srcImg.Data[(int)(baseJ + 1), (int)baseI, 2] * irI + srcImg.Data[(int)(baseJ + 1), (int)(baseI + 1), 2] * rI;

                    Bgr c = new Bgr();
                    c.Blue = c1.Blue * irJ + c2.Blue * rJ;
                    c.Green = c1.Green * irJ + c2.Green * rJ;
                    c.Red = c1.Red * irJ + c2.Red * rJ;

                    scaledImg[j, i] = c;
                }
            imageBox2.Image = scaledImg;
        }

        private void button3_Click(object sender, EventArgs e)
        {

            float sX = 1.5f;
            float sY = 1.5f;

            var newImage = new Image<Bgr, byte>((int)(srcImg.Width * sX),
            (int)(srcImg.Height * sY));
            for (int x = 0; x < srcImg.Width; x++)
            {
                for (int y = 0; y < srcImg.Height; y++)
                {

                    int newX = (int)(x * sX);
                    int newY = (int)(y * sY);

                    newImage[newY, newX] = srcImg[y, x];
                }
                imageBox2.Image = newImage;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            move = move + 0.1;

             var newImage = new Image<Bgr, byte>(srcImg.Width, srcImg.Height);


            for (int x = 0; x < srcImg.Width; x++)
            {
                for (int y = 0; y < srcImg.Height; y++)
                {

                    int newX = (int)(x + move * (srcImg.Height - y));
                    if (newX >= srcImg.Width) newX = srcImg.Width-1;

                    int newY = y;

                    newImage[newY, newX] = srcImg[y, x];
                }
                imageBox2.Image = newImage;
            }
        }

        private void imageBox1_MouseClick(object sender, MouseEventArgs e)
        {
            var imgCopy = srcImg.Copy();

            int x = (int)(e.Location.X / imageBox1.ZoomScale);
            int y = (int)(e.Location.Y / imageBox1.ZoomScale);

            pts[c] = new Point(x, y);
            c++;
            if (c >= 4)
                c = 0;

            int radius = 2;
            int thickness = 2;
            var color = new Bgr(Color.Blue).MCvScalar;

            for(int i = 0; i < 4; i++)
            CvInvoke.Circle(imgCopy, new Point((int)pts[i].X, (int)pts[i].Y), radius, color, thickness);

            imageBox1.Image = imgCopy;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            var destPoints = new PointF[]
            {

             new PointF(0, 0),
             new PointF(0, srcImg.Height - 1),
             new PointF(srcImg.Width - 1, srcImg.Height - 1),
             new PointF(srcImg.Width - 1, 0)
            };
            var homographyMatrix = CvInvoke.GetPerspectiveTransform(pts, destPoints);
            var destImage = new Image<Bgr, byte>(srcImg.Size);
            CvInvoke.WarpPerspective(srcImg, destImage, homographyMatrix, destImage.Size);

            imageBox2.Image = destImage;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
