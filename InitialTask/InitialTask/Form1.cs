using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InitialTask
{
    public partial class ContourFind : Form
    {
        //работа с основной графикой:
        private Bitmap originBit;
        private Bitmap currBit;
        private Graphics g;

        public ContourFind()
        {
            InitializeComponent();

            //окна для картинок:
            originBit = helpFunc.CreateNewBitmap(pictureBox1.Width, pictureBox1.Height);
            currBit = helpFunc.CreateNewBitmap(pictureBox1.Width, pictureBox1.Height);

            //отображение картинок:
            pictureBox1.Image = (Image)originBit;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;


        }

        //Нужны:
        //1.Алгоритмы Собеля и Превита
        //!2.Алгоритм связывания контура
        //!!!3.Преобразование Хаффа (?)
        //!!4.Правильная отрисовка прямых

        private void btnPerformHaff_Click(object sender, EventArgs e)
        {
            //anotherWayToPerformHough(15, 5, 0.5);
            //0.Картинка уже загружена
            Int32 w = currBit.Width;
            Int32 h = currBit.Height;

            //1.Шаг theta и p выбираются автоматически (диагональ и 360) 
            double temp = Math.Pow((double)w, 2.0) + Math.Pow((double)h, 2.0);
            int p_max = (int)(Math.Sqrt(temp)) + 1; //диагональ
            int theta_max = 360; //окружность
            int stepP = 20;
            int stepTheta = 20;

            //большой цикл:
            for (int k = 0; k < 3; k++)
            {

                //2.Преобразование Хаффа
                Stack<int>[,] H = new Stack<int>[1, 1];
                ImageProc.doHoughTransform(ref originBit, ref H, p_max, theta_max + 1);

                MessageBox.Show("HoughTransform has been completed", "Progress");

                //3. Находим максимум из таблицы Хаффа:
                int iMax = 0;
                int jMax = 0;
                int countMax = 0;

                for (int i = 0; i < p_max; i++)
                {
                    for (int j = 0; j < theta_max; j++)
                    {
                        if (H[i, j].Count >= countMax)
                        {
                            countMax = H[i, j].Count;
                            iMax = i;
                            jMax = j;
                        }
                    }
                }

                //Делим на связные множества:
                int[] points = H[iMax, jMax].ToArray();
                List<Point> _lpoints = new List<Point>();
                for (int i = 0; i < points.Length - 1; i += 2)
                {
                    _lpoints.Add(new Point(points[i], points[i + 1]));

                }
                List<Point> f = new List<Point>();
                ImageProc.divideLinkedPoints(ref f, ref _lpoints, 7.0);

                //4. Выбираем граничные точки для построения прямой:
                Point P1 = new Point(f[0].X, f[0].Y);
                Point P2 = new Point(f[1].X, f[1].Y);
                
                //искомые:
                double rMax = Math.Sqrt((P1.X - P2.Y) * (P1.X - P2.Y) + (P1.Y - P2.Y) * (P1.Y - P2.Y));
                Point start = P1;
                Point finish = P2;

                foreach (Point i in f)
                {
                    foreach (Point j in f)
                    {
                        double newMax = Math.Sqrt((i.X - j.X) * (i.X - j.X) + (i.Y - j.Y) * (i.Y - j.Y));
                        if (rMax < newMax)
                        {
                            rMax = newMax;
                            start = new Point(i.X, i.Y);
                            finish = new Point(j.X, j.Y);
                        }
                    }
                }

                //5. Строим прямую:
                //Графика для отрисовки:
                Graphics g = Graphics.FromImage((Image)currBit);
                g.DrawLine(new Pen(Color.Red, 2f), start, finish);

                //4. Выбираем граничные точки для построения прямой:
                P1 = new Point(_lpoints[0].X, _lpoints[0].Y);
                P2 = new Point(_lpoints[1].X, _lpoints[1].Y);

                //искомые:
                rMax = Math.Sqrt((P1.X - P2.Y) * (P1.X - P2.Y) + (P1.Y - P2.Y) * (P1.Y - P2.Y));
                start = P1;
                finish = P2;

                foreach (Point i in f)
                {
                    foreach (Point j in f)
                    {
                        double newMax = Math.Sqrt((i.X - j.X) * (i.X - j.X) + (i.Y - j.Y) * (i.Y - j.Y));
                        if (rMax < newMax)
                        {
                            rMax = newMax;
                            start = new Point(i.X, i.Y);
                            finish = new Point(j.X, j.Y);
                        }
                    }
                }

                //5. Строим прямую:
                //Графика для отрисовки:
                g.DrawLine(new Pen(Color.Red, 2f), start, finish);
                //6. Удаляем точки (все, кроме граничных):
                _lpoints.Remove(start);
                _lpoints.Remove(finish);
                foreach (Point po in _lpoints)
                {
                    originBit.SetPixel(po.X, po.Y, Color.Black);
                }
                

                pictureBox1.Refresh();

                MessageBox.Show("Progress");

                pictureBox1.Image = (Image)currBit;
                pictureBox1.Refresh();
            }

            pictureBox1.Image = (Image)originBit;
            pictureBox1.Refresh();
            MessageBox.Show("Rastr");

            //////4.Выделим точки для проверки:
            ////int[] points = H[iMax, jMax].ToArray();
            ////for (int i = 0; i < points.Length - 1; i += 2)
            ////{
            ////    currBit.SetPixel(points[i], points[i + 1], Color.Red);

            ////}

            ////MessageBox.Show("Points have been painted", "Progress");

            //////4.Методом наименьших квадратов проведем прямую:
            ////double a = 0.0, b = 0.0;
            ////ImageProc.doMinSquaresMethod(points, ref a, ref b);

            MessageBox.Show("MinSquaresMethod has been performed", "Progress");

            pictureBox1.Image = currBit;
            pictureBox1.Refresh();
        }

        //способ реализации алгоритма не по погрешности, а по "ячейкам":
        public void anotherWayToPerformHough(int stepP, int stepTheta, double accuracy)
        {
            //0.Картинка уже загружена
            Int32 w = currBit.Width;
            Int32 h = currBit.Height;

            //1.Шаг theta и p выбираются автоматически (диагональ и 360) 
            double temp = Math.Pow((double)w, 2.0) + Math.Pow((double)h, 2.0);
            int p_max = (int)(Math.Sqrt(temp)) + 1; //диагональ
            int theta_max = 360; //окружность

            //////большой цикл:
            //for (int k = 0; k < 3; k++)
            //{
                //2.Преобразование Хаффа
                Stack<int>[,] H = new Stack<int>[1, 1];
                ImageProc.doHoughTransformCells(ref originBit, ref H, p_max, theta_max + 1, stepP, stepTheta, accuracy);

                int P = (p_max / stepP) + ((p_max % stepP == 0) ? 0 : 1);
                int T = (theta_max / stepTheta) + ((theta_max % stepTheta == 0) ? 0 : 1);

                //3. Находим максимум из таблицы Хаффа:
                int iMax = 0;
                int jMax = 0;
                int countMax = 0;

                for (int i = 0; i < P; i++)
                {
                    for (int j = 0; j < T; j++)
                    {
                        if (H[i, j].Count >= countMax)
                        {
                            countMax = H[i, j].Count;
                            iMax = i;
                            jMax = j;
                        }
                    }
                }

                MessageBox.Show("" + countMax, "Number of dots:");

                //Рисуем эти точки:
                int[] points = H[iMax, jMax].ToArray();
                for (int i = 0; i < points.Length - 1; i += 2)
                {
                    currBit.SetPixel(points[i], points[i + 1], Color.Red);

                }

                //Делим точки:
                Stack<List<Point>> Dots = new Stack<List<Point>>();
                List<Point> _lpoints = new List<Point>();
                for (int i = 0; i < points.Length - 1; i += 2)
                {
                    _lpoints.Add(new Point(points[i], points[i + 1]));

                }
                //ImageProc.divideLinkedPoints(ref Dots, ref _lpoints, 5.0);

                foreach (Point i in Dots.Pop())
                {
                    currBit.SetPixel(i.X, i.Y, Color.Yellow);
                }

                foreach (Point i in Dots.Pop())
                {
                    currBit.SetPixel(i.X, i.Y, Color.Blue);
                }


            //    //4. Выбираем граничные точки для построения прямой:
            //    //int[] points = H[iMax, jMax].ToArray();
            //    int x1 = points[0], x2 = points[0];
            //    int y1 = points[1], y2 = points[1];

            //    //искомые:
            //    double rMax = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            //    Point start = new Point();
            //    Point finish = new Point();

            //    for (int i = 0; i < points.Length - 1; i += 2)
            //    {
            //        for (int j = 0; j < points.Length - 1; j += 2)
            //        {
            //            double newMax = Math.Sqrt((points[i] - points[j]) * (points[i] - points[j]) +
            //                                        (points[i + 1] - points[j + 1]) * (points[i + 1] - points[j + 1]));
            //            if (rMax < newMax)
            //            {
            //                rMax = newMax;
            //                start.X = points[i];
            //                start.Y = points[i + 1];
            //                finish.X = points[j];
            //                finish.Y = points[j + 1];
            //            }
            //        }
            //    }

            //    //5. Строим прямую:
            //    //Графика для отрисовки:
            //    Graphics g = Graphics.FromImage((Image)currBit);
            //    g.DrawLine(new Pen(Color.Red, 2f), start, finish);

            //    //6. Удаляем точки (все, кроме граничных):
            //    for (int i = 2; i < points.Length - 3; i += 2)
            //    {
            //        originBit.SetPixel(points[i], points[i + 1], Color.Black);
            //    }

            //    pictureBox1.Refresh();

            //    MessageBox.Show("Progress");

            //    pictureBox1.Image = (Image)currBit;
            //    pictureBox1.Refresh();
            //}

            pictureBox1.Image = currBit;
            pictureBox1.Refresh();
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.BMP, *.JPG, *.PNG)|*.jpg;*.bmp;*.png";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            originBit = new Bitmap(openFileDialog.FileName);
            currBit = new Bitmap(openFileDialog.FileName);

            pictureBox1.Image = (Image)currBit;
     
        }
    }
}
