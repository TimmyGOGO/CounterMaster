using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Windows.Forms;

namespace InitialTask
{
    public struct toch
    {
        public int R;
        public int O;
    }

    struct cirToch
    {
        public int A;
        public int B;
        public int R;
    }

    //обработка изображений и дополнительные алгоритмы
    public class ImageProc
    {
        //convert to grayscale image
        public static void ToGrayScale(ref Bitmap img){
            Int32 W = img.Width;
            Int32 H = img.Height;

            BitmapData bmData = img.LockBits(new Rectangle(0, 0, W, H), 
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            
            unsafe
            {
                byte* ptr = (byte*)bmData.Scan0;

                for (int y = 0; y < H; y++)
                {
                    for (int x = 0; x < W; x++)
                    {
                        byte r = ptr[0];
                        byte g = ptr[1];
                        byte b = ptr[2];

                        byte gray = (byte)(0.30 * r + 0.59 * g + 0.11 * b);

                        ptr[0] = (byte)gray;
                        ptr[1] = (byte)gray;
                        ptr[2] = (byte)gray;
                        ptr += 3;
                    }
                    ptr += bmData.Stride - W * 3;
                }
            }
            img.UnlockBits(bmData);

        }
        
        //get full histogram of an image:
        public static void getFullHistogramm(out int[] hist, Bitmap bmp)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //histogramm definition:
            hist = new int[256];

            unsafe
            {
                byte* ptr = (byte*)bmData.Scan0;

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        hist[(int)(ptr[0])]++; //red
                        ptr += 3;
                    }
                    ptr += bmData.Stride - bmp.Width * 3;
                }
            }
            bmp.UnlockBits(bmData);

        }
        
        //compute an entropy of an image:
        public static double calculateEntropy(Bitmap bmp)
        {
            int[] hist = null;
            getFullHistogramm(out hist, bmp);

            long N = bmp.Width * bmp.Height;

            double H = 0.0;

            for (int i = 0; i < 256; i++)
            {
                if (hist[i] != 0)
                {
                    H += hist[i] * Math.Log((double)(N / hist[i]), 2.0);
                }
            }
            H /= N;

            return Math.Abs(H);
        }

        //преобразование Хаффа вместе с параметрами (деление на ячейки):
        public static void doHoughTransformCells(ref Bitmap img, ref Stack<int>[,] H, int p_max, int theta_max, int stepP, int stepT, double accuracy = 10.0)
        {

            //предварительные вычисления:
            Int32 w = img.Width;
            Int32 h = img.Height;

            //2. делим диапазон значений:
            int P = (p_max / stepP) + ((p_max % stepP == 0) ? 0 : 1);
            int T = (theta_max / stepT) + ((theta_max % stepT == 0) ? 0 : 1);
            H = new Stack<int>[P, T];
            for (int i = 0; i < P; i++)
            {
                for (int j = 0; j < T; j++)
                {
                    H[i, j] = new Stack<int>();
                }
            }

            //3. цикл по белым точкам:
            BitmapData bmData = img.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* ptr = (byte*)bmData.Scan0;

                //массив по всем точкам растра:
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        //если точка белая:
                        if (ptr[0] == 255 && ptr[1] == 255 && ptr[2] == 255)
                        {
                            for (int a = 0; a < T; a++) //цикл по отрезкам углов:
                            {
                                for (int r = 0; r < P; r++) //цикл по отрезкам расстояний:
                                {
                                    //выбираем прогон для данной ячейки:
                                    int begA = (theta_max / stepT) * a;
                                    int endA = (a != T - 1) ? ((theta_max / stepT) * (a+1)) : T;
                                    int begP = (p_max / stepP) * r;
                                    int endP = (r != P - 1) ? ((p_max / stepP) * (r+1)) : P;

                                    for (; begA < endA; begA++) //цикл по отрезкам углов:
                                    {
                                        for (; begP < endP; begP++) //цикл по отрезкам расстояний:
                                        {

                                            double theta = Math.PI * ((double)begA / 180.0);
                                            // Если решение уравнения достаточно хорошее (точность больше заданой)
                                            if (Math.Abs((y * Math.Sin(theta) + x * Math.Cos(theta)) - begP) < accuracy)
                                            {
                                                H[r, a].Push(y);
                                                H[r, a].Push(x);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        ptr += 3;
                    }
                    ptr += bmData.Stride - w * 3;
                }
            }
            img.UnlockBits(bmData);

            return;

        }

        //Преобразование Хаффа (составление таблицы H): (РАБОЧЕЕ)
        public static void doHoughTransform(ref Bitmap img, ref Stack<int>[,] H, int p_max, int theta_max, double accuracy=10.0)
        {
            
            //предварительные вычисления:
            Int32 w = img.Width;
            Int32 h = img.Height;

            //2. занулим массив H:
            H = new Stack<int>[p_max, theta_max];
            for (int i = 0; i < p_max; i++)
            {
                for (int j = 0; j < theta_max; j++)
                {
                    H[i, j] = new Stack<int>();
                }
            }

            //3. цикл по белым точкам:
            BitmapData bmData = img.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* ptr = (byte*)bmData.Scan0;

                //массив по всем точкам растра:
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        //если точка белая:
                        if (ptr[0] == 255 && ptr[1] == 255 && ptr[2] == 255)
                        {
                            for (int T = 0; T < theta_max; T++) //цикл по всевозможным углам:
                            {
                                for (int p = 0; p < p_max; p++) //цикл по всевозможным расстояниям:
                                {
                                    double theta = Math.PI * ((double)T / 180.0);
                                    // Если решение уравнения достаточно хорошее (точность больше заданой)
                                    if (Math.Abs(( y * Math.Sin(theta) + x * Math.Cos(theta)) - p) < accuracy)
                                    {
                                        H[Math.Abs((int)p), T].Push(y);
                                        H[Math.Abs((int)p), T].Push(x);
                                    }

                                }
                            }
                        }
                        ptr += 3;
                    }
                    ptr += bmData.Stride - w * 3;
                }
            }
            img.UnlockBits(bmData);

            return;
           
        }

        //деление на связные множества точек:
        public static bool divideLinkedPoints(ref List<Point> first, ref List<Point> points, double accuracy=10.0)
        {
            first = new List<Point>();
            first.Add(points[0]);
            points.RemoveAt(0);

            bool isThereNewDot = true;
            while (isThereNewDot)
            {
                isThereNewDot = false;
                for (int i = 0; i < points.Count; i++)
                {
                    foreach (Point p in first)
                    {
                        double distance = Math.Sqrt((points[i].X - p.X) * (points[i].X - p.X) +
                                                    (points[i].Y - p.Y) * (points[i].Y - p.Y));
                        if (distance <= 10)
                        {
                            first.Add(points[i]);
                            points.RemoveAt(i);
                            i--;
                            isThereNewDot = true;
                            break;
                        }
                        
                    }

                }

            }
            
            return (points.Count == 0)? false : true; 
            
            

        }

        //Метод наименьших квадратов (получаем коэффициенты прямой)
        //на вход идет массив: "x y x y..."
        public static void doMinSquaresMethod(int[] points, ref double a, ref double b)
        {
            int n = points.Length;
            //находим суммы для формул:
            int summXY = 0;
            int summY = 0;
            int summXX = 0;
            int summX = 0;

            for (int i = 0; i < n - 1; i += 2)
            {
                summXY += points[i] * points[i + 1];
                summXX += points[i] * points[i];
                summY += points[i + 1];
                summX += points[i];
            }

            //a: (коэффициэнт при x)
            double numerator = n * summXY - summX * summY;
            double denominator = n * summXX - summX * summX;
            
            if ((int)denominator == 0)
            {
                a = 0;
            }
            else
            {
                a = numerator / denominator;
            }

            //b: (свободный член)
            b = (summY - a * summX) / (double)n;

        }

        //алгоритм для поиска оружностей на растре
        public static ArrayList doCircleSearch(ref Bitmap img)
        {
            //1. Предварительные вычисления:
            Int32 w = img.Width;
            Int32 h = img.Height;

            double temp = Math.Pow((double)w, 2.0) + Math.Pow((double)h, 2.0);
            int R = (int)(Math.Sqrt(temp)) + 1;
            int A = w;
            int B = h;

            int[, ,] H = new int[w, h, R];
            //2. занулим массив H:
            for (int i = 0; i < A; i++)
            {
                for (int j = 0; j < B; j++)
                {
                    for (int k = 0; k < R; k++)
                    {
                        H[i,j,k] = 0;
                    }
                }
            }
            MessageBox.Show("Hooray!");

            //3. цикл по черным точкам:
            BitmapData bmData = img.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* ptr = (byte*)bmData.Scan0;

                //массив по всем точкам растра:
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        //если точка черная:
                        if (ptr[0] == 0 && ptr[1] == 0 && ptr[2] == 0)
                        {
                            //по всем А:
                            for (int a = 0; a < A; a++)
                            {
                                //по всем В:
                                for (int b = 0; b < B; b++)
                                {
                                    
                                    double r = Math.Sqrt(Math.Pow((double)(x - a), 2.0) + Math.Pow((double)(x - b), 2.0));
                                    //MessageBox.Show(r.ToString());
                                    H[a, b, (int)(r)]++;
                                
                                }
                            }
                            
                        }
                        ptr += 3;
                    }
                    ptr += bmData.Stride - w * 3;
                }
            }
            img.UnlockBits(bmData);

            //определяем порог и вспомогательные структуры
            int p = 100;
            cirToch point;
            point.A = 0;
            point.B = 0;
            point.R = 0;
            ArrayList passT = new ArrayList();

            //5. находим окружность по выбранной ячейке
            for (int i = 0; i < A; i++)
            {
                for (int j = 0; j < B; j++)
                {
                    for (int k = 0; k < R; k++)
                    {
                        if (H[i, j, k] >= p)
                        {
                            point.A = i;
                            point.B = j;
                            point.R = k;
                            passT.Add(point);
                        }
                    }
                }
            }
            //6. что можно считать окружностью?
            
            return passT;

        }
        

        public void MyActions()
        {
            //1. для каждой точки на растре
            /* в предположении что мы рисуем O( 360 ) прямых
               нарисовать на другом растре характерный график:
                r= 0...sqrt(w^2 + h^2)
                O = 0...359
                
                O = 180/0 => r = r0
                O = 90 => r = 0
                
             * осталось придумать перевод координат -> и все)
               
               координатная плоскость: O x r
            */


        }




        internal static void doLineSearch(Bitmap originBit, int[,] H)
        {
            throw new NotImplementedException();
        }
    }
}
