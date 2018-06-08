using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestCYK
{
    public partial class Form1 : Form
    {
        public void Createbutton(int witdh, int height, string w, string[,] table, int end)
        {
            int n = w.Length;
            Button oldbtn = new Button() { Width = 0, Location = new Point( 0 , n* height) };
            int tmp;
           
            for (int i = 0; i <= end; i++)
            {
                for (int j = 0; j <= n + 1; j++)
                {
                    tmp = j;
                    Button btn = new Button()
                    {
                        Width = witdh,
                        Height = height,
                        Location = new Point(oldbtn.Width + oldbtn.Location.X, oldbtn.Location.Y),
                        BackColor = Color.White,
                        TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                        Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    };

                    pnl_Main.Controls.Add(btn);
                    if (tmp == n + 1) tmp = n;
                    btn.Text = table[tmp, i];
                    if (i == 0 || j == 0)
                    {
                        btn.BackColor = Color.LightBlue;
                    }
                    oldbtn = btn;
                    //Thread.Sleep(5000);
                }
                oldbtn.Location = new Point(0, oldbtn.Location.Y - height);
                oldbtn.Width = 0;
                oldbtn.Height = 0;

            }
        }

        class Rule
        {
            public char left;
            public string right;
            public Rule(char l, string r)
            {
                left = l; right = r;
            }
            public string ToFineString()
            {
                string s = left + " -->";
                for (int i = 0; i < right.Length; i++)
                    s += " " + right[i];
                return s;
            }
        }
        class PTCYK
        {
            public List<Rule> rules = new List<Rule>();
            string[,] table = null;
            string w = null;
            int n = 0;

            // thêm luật left --> right vào tập luật
            public void AddRule(char left, string right)
            {
                rules.Add(new Rule(left, right));
            }

            public List<Rule> PrintAllRules()
            {
                return rules;
                //Console.WriteLine("<bo luat van pham>");
                //foreach (Rule r in rules)
                //    Console.WriteLine("  " + r.ToFineString());
                //Console.WriteLine();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            void InitData(string x)
            {
                w = x;
                n = x.Length;
                table = new string[n + 1, n + 1];
                for (int i = 1; i <= n; i++)
                    for (int j = 1; j <= n; j++) table[i, j] = "";
            }
            /// <summary>
            /// 
            /// </summary>
            void FirstProcess()
            {
                for (int i = 1; i <= n; i++)
                    foreach (Rule r in rules)
                        if (r.right.Length == 1)
                            if (w[i - 1] == r.right[0]) table[i, 1] += r.left;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="w"></param>
            /// <returns></returns>
            bool IsIn(char x, string w)
            {
                return w.IndexOf(x) != -1;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="j"></param>
            /// <param name="i"></param>
            /// <param name="r"></param>
            /// <returns></returns>
            bool CanGen(int j, int i, Rule r)
            {
                for (int k = 1; k <= j - 1; k++)
                    if (IsIn(r.right[0], table[i, k]))
                        if (IsIn(r.right[1], table[i + k, j - k]))
                            return true;
                return false;
            }

            void GenTable()
            {
                for (int j = 2; j <= n; j++)
                    for (int i = 1; i <= n - j + 1; i++)
                        foreach (Rule r in rules)
                            if (r.right.Length == 2)
                                if (!IsIn(r.left, table[i, j]))
                                    if (CanGen(j, i, r))
                                    {
                                        table[i, j] += r.left;
                                        // Createbutton(90, 90, w, table);
                                    }
            }
            /// <summary>
            /// run CYK :3
            /// </summary>
            /// <param name="x"></param>
            public void Process(string x)
            {
                InitData(x);
                FirstProcess();
                GenTable();
            }

            //hiện thị ra ký tự test các bước
            public string[,] PrintResult()
            {
                for (int i = 1; i <= n; i++)
                {
                    table[0, i] = i.ToString();
                    table[i, 0] = w[i - 1].ToString();
                }

                return table;

            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //lbl_list.Text = a;
            button1.Enabled = false;
        }

        string[,] table;
        int kk = 0;

        private void btn_test_Click(object sender, EventArgs e)
        {
            PTCYK parser = new PTCYK();

            List<Rule> rules = new List<Rule>();

            // nạp bộ luật vào bộ phân tích
            parser.AddRule('S', "AB");
            parser.AddRule('S', "XB");
            parser.AddRule('T', "AB");
            parser.AddRule('T', "XB");
            parser.AddRule('X', "AT");
            parser.AddRule('A', "a");
            parser.AddRule('B', "b");
            rules = parser.PrintAllRules();

            //gán bộ luật vào lbllist
            string a = "";
            foreach (Rule r in rules)
            {
                a += r.ToFineString() + "\n";
            }

            lbl_list.Text = a;
            //
            //thực hiện gán giá trị vào button và show ra pnl_Main
            parser.Process(txt_input.Text.ToString());
            table = parser.PrintResult();
            timer.Start();
            //kiểm tra nếu đã chạy hết thì dừng timer, xóa text nhập và enable btnRefresh
            if (kk == txt_input.Text.Length+1)
            {
                timer.Stop();
                button1.Enabled = true;
                MessageBox.Show("CYK done!","Infomation");
                txt_input.Text = "";
                kk = 0;
            }
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            //MessageBox.Show(kk.ToString());
            //Createbutton(60, 50, txt_input.Text, table, kk);
            Createbutton(Convert.ToInt32(txtW_btn.Text), Convert.ToInt32(txtH_btn.Text), txt_input.Text, table, kk);
            kk++;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pnl_Main.Controls.Clear();//remove all controls
            button1.Enabled = false;
            ////remove controls is button
            //foreach (Control item in pnl_Main.Controls.OfType<Button>())
            //{
            //    pnl_Main.Controls.Remove(item);
            //}
            //button1.Enabled = false;
        }
    }
}
