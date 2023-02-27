using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Drawing2D;

namespace SonicFrontiersPuzzle
{
    public partial class Form1 : Form
    {
        readonly List<int> queue = new();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawLine(e.Graphics, new Point(250, 82), new Point(444, 233));
            DrawLine(e.Graphics, new Point(207, 233), new Point(207, 90));
            DrawLine(e.Graphics, new Point(531, 233), new Point(531, 90));
            DrawLine(e.Graphics, new Point(444, 82), new Point(250, 233));
            DrawLine(e.Graphics, new Point(250, 75), new Point(445, 75));
        }
        public static void DrawLine(Graphics g, Point from, Point to)
        {
            var pen = new Pen(Color.Black, 1)
            {
                CustomEndCap = new AdjustableArrowCap(5, 5)
            };

            g.DrawLine(pen, from, to);
        }
        public void ManualIncrement(int[] arr, int modulo)
        {
            int[] abcd = textBox1.Text.Split(' ').Select(int.Parse).ToArray();
            Increment(arr, abcd.First(), modulo);
            abcd = abcd.Skip(1).ToArray();
            textBox1.Text = string.Join(" ", abcd);
        }
        public void SolutionCall(int[] arr, int modulo)
        {
            List<List<List<int>>> biglist = new();
            List<List<int>> list = new();
            List<int> small = new();
            int i = 0;
            while (i != 4)
            {
                for (int j = 0; j < 7; j++)
                {
                    for (int k = 0; k <= j; k++)
                    {
                        small.Add(i);
                    }
                    list.Add(small);
                    small = new();
                }
                biglist.Add(list);
                list = new();
                i++;
            }
            if (Solution(arr, biglist, modulo))
            {
                queue.Sort();
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            int a = int.Parse(ATopLeft.Text);
            int b = int.Parse(BTopRight.Text);
            int c = int.Parse(CBottomLeft.Text);
            int d = int.Parse(DBottomRight.Text);
            int modulo = int.Parse(MOD.Text);
            int[] arr = { a, b, c, d };
            //ManualIncrement(arr);
            SolutionCall(arr, modulo);
            textBox1.Text = string.Join(" ", queue).Replace('0', 'a').Replace('1', 'b').Replace('2', 'c').Replace('3', 'd');
            ATopLeft.Text = arr[0].ToString();
            BTopRight.Text = arr[1].ToString();
            CBottomLeft.Text = arr[2].ToString();
            DBottomRight.Text = arr[3].ToString();
        }
        private bool Solution(int[] arr, List<List<List<int>>> biglist, int modulo)
        {
            int[] origin = new int[4];
            Array.Copy(arr, origin, 4);
            var list0 = biglist[0];
            var list1 = biglist[1];
            var list2 = biglist[2];
            var list3 = biglist[3];
            foreach(var i in list0)
            {
                foreach(var j in list1)
                {
                    foreach (var k in list2)
                    {
                        foreach (var t in list3)
                        {
                            var joincommand = i.Concat(j).Concat(k).Concat(t);
                            foreach (var v in joincommand)
                            {
                                queue.Add(v);
                                Increment(arr, v, modulo);
                            }
                            if (!Goal(arr))
                            {
                                queue.Clear();
                                Array.Copy(origin, arr, 4);
                            }
                            else return true;
                        }
                    }
                }
            }
            return false;
        }
        private static void Increment(int[] arr, int index, int modulo)
        {
            if (index == 0)
            {
                arr[0]++;
                arr[0] %= modulo;
                arr[1]++;
                arr[1] %= modulo;
                arr[3]++;
                arr[3] %= modulo;
            }
            else if (index == 1)
            {
                arr[1]++;
                arr[1] %= modulo;
                arr[2]++;
                arr[2] %= modulo;
            }
            else if (index == 2)
            {
                arr[2]++;
                arr[2] %= modulo;
                arr[0]++;
                arr[0] %= modulo;
            }
            else
            {
                arr[3]++;
                arr[3] %= modulo;
                arr[1]++;
                arr[1] %= modulo;
            }
        }
        public static bool Goal(int[] arr)
        {
            if (arr[0] == 2 && arr[1] == 5 && arr[2] == 5 && arr[3] == 4) return true;
            return false;
        }
    }
}