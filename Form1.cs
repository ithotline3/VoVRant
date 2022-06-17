using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VoVRant
{
    public partial class Form1 : Form
    {
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);

        public static int SizeX = 32;
        public static int SizeY = 16;
        public static int ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
        public static int ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
        public int ScreenWidthHalf = ScreenWidth / 2;
        public int ScreenHeightHalf = ScreenHeight / 2;
        public int IDX;
        public Bitmap Visual = new Bitmap(SizeX, SizeY);
        public bool FoundLeftSide;
        public bool FoundRightSide;
        public bool CenterAligned;
        public bool BreakOut;
        public List<Point> HitPoints;
        public double AVGPoint;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Task Aim = new Task(() =>
            {
                while (true)
                {
                    if (!(GetAsyncKeyState(Keys.XButton2) < 0))
                    {
                        HitPoints = new List<Point>();
                        FoundLeftSide = false;
                        CenterAligned = false;
                        FoundRightSide = false;
                        BreakOut = false;
                        AVGPoint = 0;
                        IDX = 0;
                        using (Graphics graphics = Graphics.FromImage(Visual))
                        {
                            //Take a screenshot
                            graphics.CopyFromScreen(ScreenWidthHalf - (SizeX / 2), ScreenHeightHalf - (SizeY / 2), 0, 0, Visual.Size);
                            //Get all pixels
                            for (int i = 0; i < Visual.Width; i++)
                            {
                                for (int j = 0; j < Visual.Height; j++)
                                {
                                    Color CenterColor = Visual.GetPixel(i, j);
                                    //If good color, shoot
                                    if ((CenterColor.R > 230) && (CenterColor.B > 230) && (CenterColor.G < 200))
                                    {
                                        //Check both sides
                                        if (i < (Visual.Width / 2))
                                        {
                                            FoundLeftSide = true;
                                        }
                                        else if (i > (Visual.Width / 2))
                                        {
                                            FoundRightSide = true;
                                        }

                                        //Add it to list
                                        HitPoints.Add(new Point(i, j));

                                        //Check if list avg is head like
                                        foreach(Point p in HitPoints)
                                        {
                                            IDX++;
                                            AVGPoint += Math.Sqrt(Math.Pow(p.X - (Visual.Width / 2), 2) + Math.Pow(p.Y - (Visual.Height / 2), 2));
                                            if (((AVGPoint / IDX) < 12) && (IDX > 4))
                                            {
                                                CenterAligned = true;
                                                break;
                                            }
                                        }

                                        //Shoot it boii
                                        if ((FoundRightSide) && (FoundLeftSide) && (CenterAligned))
                                        {
                                            SendKeys.SendWait("k");
                                            BreakOut = true;
                                            break;
                                        }
                                    }
                                }
                                if (BreakOut)
                                {
                                    break;
                                    Thread.Sleep(250);
                                }
                            }
                        }
                        Thread.Sleep(6);
                    }
                }
            });
            Aim.Start();
        }
    }
}
