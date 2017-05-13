using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace InitialTask
{
    //искючение вместо break:
    class SuddenThrow : Exception
    {
        public override string ToString(){ return "Внезапный выход"; }

        public override string Message{
            get { return "Произошло исключение Sudden Throw"; }
        }
    }


    //основной класс:
    public static class helpFunc
    {
        //create a new bitmap:
        public static Bitmap CreateNewBitmap(Int32 width, Int32 height)
        {
            Bitmap _map = new Bitmap(width, height);
            ClearBitmap(ref _map);
            return _map;

        }

        //make the bitmap clear
        public static void ClearBitmap(ref Bitmap tempB)
        {
            Int32 W = tempB.Width;
            Int32 H = tempB.Height;

            BitmapData bmData = tempB.LockBits(new Rectangle(0, 0, W, H),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* ptr = (byte*)bmData.Scan0;
                for (int j = 0; j < H; j++){
                    for (int i = 0; i < W; i++){
                        byte white = 255;
                        ptr[0] = white;
                        ptr[1] = white;
                        ptr[2] = white;

                        ptr += 3;
                    }
                    ptr += bmData.Stride - W * 3;
                }
            }
            tempB.UnlockBits(bmData);

        }

        //draw the point in the (X,Y):
        public static void DrawPoint(ref Bitmap tempB, Int32 X, Int32 Y)
        {
            Int32 W = tempB.Width;
            Int32 H = tempB.Height;

            BitmapData bmData = tempB.LockBits(new Rectangle(0, 0, W, H),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            unsafe
            {
                try
                {
                    byte* ptr = (byte*)bmData.Scan0;
                    for (int j = 0; j < H; j++)
                    {
                        for (int i = 0; i < W; i++)
                        {
                            if (i == X && j == Y)
                            {
                                byte black = 0;
                                ptr[0] = black;
                                ptr[1] = black;
                                ptr[2] = black;

                                throw new SuddenThrow();

                            }
                            ptr += 3;
                        }
                        ptr += bmData.Stride - W * 3;
                    }
                }
                catch { }
                finally { tempB.UnlockBits(bmData); }

            }

        }

        //send test rezults to file
        public static void SendMessageTo(string filename, string message)
        {
            FileStream fout;
            try { fout = new FileStream(filename, FileMode.Append); }
            catch (IOException exc)
            {
                Console.Out.WriteLine("НЕ удается открыть файл:\n " + exc.Message);
                return;
            }

            //поток для файла:
            StreamWriter fstr_out = new StreamWriter(fout);
            
            //запись времени?:
            DateTime thisDay = DateTime.Today;
            thisDay = DateTime.UtcNow;
            fstr_out.WriteLine(String.Format("{0:dd MM yyyy}_{1:HH}:{2:mm} ", thisDay, thisDay, thisDay));
            fstr_out.WriteLine(message);
            
            //закрываем все:
            fstr_out.Close();
            fout.Close();

        }


        //override methods:
        //draw the line:
        public static void DrawLine() { }

        //draw the circle:
        public static void DrawCircle() { }




    }
}
