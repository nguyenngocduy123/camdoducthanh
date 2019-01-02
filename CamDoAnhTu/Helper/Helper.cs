using CamDoAnhTu.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;


namespace CamDoAnhTu.Helper
{
    public class Helper
    {
        public static string[] masotragopArr = { "BA", "CA", "MA", "ZA", "YA", "TA", "QA" };
        public static string[] masotradungArr = { "BD", "CD", "MD", "ZD", "YD", "TD" };
        public static bool IsTraGop(string code)
        {
            if (masotragopArr.Any(code.Contains))
            {
                return true;
            }
            else return false;
        }
        public static Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            return result;
        }

        public static int GetUserByUsername(string uname)
        {
            Customer user = null;
            int a = 0;
            string code = uname;
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                user = ctx.Customers.Where(u => u.Code == code).FirstOrDefault();
            }
            if (user == null)
            {
                a = 1;
            }
            return a;
        }

        public static int GetUserByUsername1(string uname)
        {
            Customer user = null;
            int a = 0;
            string code = uname;
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                user = ctx.Customers.Where(u => u.Code == code && u.type == 12).FirstOrDefault();


            }
            if (user == null)
            {
                a = 1;
            }
            return a;
        }
        public static bool CheckHetNo(Customer cs)
        {
            bool kq = false;

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                List<Customer> lstCus = ctx.Customers.ToList();

                int day = 0;
                if (cs.Price == null || Int32.Parse(cs.Price.ToString()) == 0)
                {
                    day = 0;
                }
                else
                    day = Int32.Parse(cs.Loan.ToString()) / Int32.Parse(cs.Price.ToString());

                if (cs.DayPaids == day || cs.Description == "End")
                {
                    kq = true;
                }
            }
            return kq;
        }
        public static List<Customer> GetCustomer(string searchString)
        {
            List<Customer> lst = new List<Customer>();

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                lst = ctx.Customers.Where(p => p.Name.StartsWith(searchString)).ToList();
            }
            return lst;
        }

        public static List<Customer> GetCustomer1(string searchString)
        {
            List<Customer> lst = new List<Customer>();

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                lst = ctx.Customers.Where(p => p.Code.StartsWith(searchString)).ToList();
            }
            return lst;
        }

        public static List<Customer> GetCustomer2(string searchString)
        {
            List<Customer> lst = new List<Customer>();

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                lst = ctx.Customers.Where(p => p.Phone.StartsWith(searchString)).ToList();
            }
            return lst;
        }
        
        public static List<Customer> GetCustomer3(string searchString)
        {
            List<Customer> lst = new List<Customer>();

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                lst = ctx.Customers.Where(p => p.Address.StartsWith(searchString)).ToList();
            }
            return lst;
        }
        public static void UpdateAllSongaydatra()
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                var lstCustomer = ctx.Customers.ToList();
                int count = 0;
                foreach (Customer cs in lstCustomer)
                {
                    
                    foreach (var loan in cs.Loans)
                    {
                        if (loan.Status == 1)
                        {
                            count++;
                        }
                    }
                    cs.DayPaids = count;
                }
                ctx.SaveChanges();
            }

        }
       
        public static void UpdateLoanCustomer(Customer cs)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                cs.NgayNo = 0;
                
                int countMax = 0;

                DateTime EndDate = DateTime.Now;

                List<Loan> t = ctx.Loans.Where(p => p.IDCus == cs.Code).OrderBy(p => p.Date).ToList();

                Loan t1 = new Loan();

                if (t.Count != 0)
                {
                    t1 = t.First();
                }

                DateTime StartDate = t1.Date;

                List<Loan> query = t.Where(p => p.Date >= StartDate && p.Date <= EndDate).ToList();
                int count = 0;
                foreach (Loan temp in query)
                {
                    if (temp.Status == 0)
                    {
                        count++;
                        countMax = count;
                    }
                    else
                    {
                        count = 0;
                    }
                }

                cs.NgayNo = countMax;
                ctx.SaveChanges();
            }
        }

        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }


        private static string Chu(string gNumber)
        {
            string result = "";
            switch (gNumber)
            {
                case "0":
                    result = "không";
                    break;
                case "1":
                    result = "một";
                    break;
                case "2":
                    result = "hai";
                    break;
                case "3":
                    result = "ba";
                    break;
                case "4":
                    result = "bốn";
                    break;
                case "5":
                    result = "năm";
                    break;
                case "6":
                    result = "sáu";
                    break;
                case "7":
                    result = "bảy";
                    break;
                case "8":
                    result = "tám";
                    break;
                case "9":
                    result = "chín";
                    break;
            }
            return result;
        }
        private static string Donvi(string so)
        {
            string Kdonvi = "";

            if (so.Equals("1"))
                Kdonvi = "";
            if (so.Equals("2"))
                Kdonvi = "nghìn";
            if (so.Equals("3"))
                Kdonvi = "triệu";
            if (so.Equals("4"))
                Kdonvi = "tỷ";
            if (so.Equals("5"))
                Kdonvi = "nghìn tỷ";
            if (so.Equals("6"))
                Kdonvi = "triệu tỷ";
            if (so.Equals("7"))
                Kdonvi = "tỷ tỷ";

            return Kdonvi;
        }

        private static string Tach(string tach3)
        {
            string Ktach = "";
            if (tach3.Equals("000"))
                return "";
            if (tach3.Length == 3)
            {
                string tr = tach3.Trim().Substring(0, 1).ToString().Trim();
                string ch = tach3.Trim().Substring(1, 1).ToString().Trim();
                string dv = tach3.Trim().Substring(2, 1).ToString().Trim();
                if (tr.Equals("0") && ch.Equals("0"))
                    Ktach = " không trăm lẻ " + Chu(dv.ToString().Trim()) + " ";
                if (!tr.Equals("0") && ch.Equals("0") && dv.Equals("0"))
                    Ktach = Chu(tr.ToString().Trim()).Trim() + " trăm ";
                if (!tr.Equals("0") && ch.Equals("0") && !dv.Equals("0"))
                    Ktach = Chu(tr.ToString().Trim()).Trim() + " trăm lẻ " + Chu(dv.Trim()).Trim() + " ";
                if (tr.Equals("0") && Convert.ToInt32(ch) > 1 && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = " không trăm " + Chu(ch.Trim()).Trim() + " mươi " + Chu(dv.Trim()).Trim() + " ";
                if (tr.Equals("0") && Convert.ToInt32(ch) > 1 && dv.Equals("0"))
                    Ktach = " không trăm " + Chu(ch.Trim()).Trim() + " mươi ";
                if (tr.Equals("0") && Convert.ToInt32(ch) > 1 && dv.Equals("5"))
                    Ktach = " không trăm " + Chu(ch.Trim()).Trim() + " mươi lăm ";
                if (tr.Equals("0") && ch.Equals("1") && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = " không trăm mười " + Chu(dv.Trim()).Trim() + " ";
                if (tr.Equals("0") && ch.Equals("1") && dv.Equals("0"))
                    Ktach = " không trăm mười ";
                if (tr.Equals("0") && ch.Equals("1") && dv.Equals("5"))
                    Ktach = " không trăm mười lăm ";
                if (Convert.ToInt32(tr) > 0 && Convert.ToInt32(ch) > 1 && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm " + Chu(ch.Trim()).Trim() + " mươi " + Chu(dv.Trim()).Trim() + " ";
                if (Convert.ToInt32(tr) > 0 && Convert.ToInt32(ch) > 1 && dv.Equals("0"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm " + Chu(ch.Trim()).Trim() + " mươi ";
                if (Convert.ToInt32(tr) > 0 && Convert.ToInt32(ch) > 1 && dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm " + Chu(ch.Trim()).Trim() + " mươi lăm ";
                if (Convert.ToInt32(tr) > 0 && ch.Equals("1") && Convert.ToInt32(dv) > 0 && !dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm mười " + Chu(dv.Trim()).Trim() + " ";

                if (Convert.ToInt32(tr) > 0 && ch.Equals("1") && dv.Equals("0"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm mười ";
                if (Convert.ToInt32(tr) > 0 && ch.Equals("1") && dv.Equals("5"))
                    Ktach = Chu(tr.Trim()).Trim() + " trăm mười lăm ";

            }


            return Ktach;

        }

        public static string So_chu(decimal gNum)
        {
            if (gNum == 0)
                return "Không đồng";

            string lso_chu = "";
            string tach_mod = "";
            string tach_conlai = "";
            decimal Num = Math.Round(gNum, 0);
            string gN = Convert.ToString(Num);
            int m = Convert.ToInt32(gN.Length / 3);
            int mod = gN.Length - m * 3;
            string dau = "[+]";

            // Dau [+ , - ]
            if (gNum < 0)
                dau = "[-]";
            dau = "";

            // Tach hang lon nhat
            if (mod.Equals(1))
                tach_mod = "00" + Convert.ToString(Num.ToString().Trim().Substring(0, 1)).Trim();
            if (mod.Equals(2))
                tach_mod = "0" + Convert.ToString(Num.ToString().Trim().Substring(0, 2)).Trim();
            if (mod.Equals(0))
                tach_mod = "000";
            // Tach hang con lai sau mod :
            if (Num.ToString().Length > 2)
                tach_conlai = Convert.ToString(Num.ToString().Trim().Substring(mod, Num.ToString().Length - mod)).Trim();

            ///don vi hang mod
            int im = m + 1;
            if (mod > 0)
                lso_chu = Tach(tach_mod).ToString().Trim() + " " + Donvi(im.ToString().Trim());
            /// Tach 3 trong tach_conlai

            int i = m;
            int _m = m;
            int j = 1;
            string tach3 = "";
            string tach3_ = "";

            while (i > 0)
            {
                tach3 = tach_conlai.Trim().Substring(0, 3).Trim();
                tach3_ = tach3;
                lso_chu = lso_chu.Trim() + " " + Tach(tach3.Trim()).Trim();
                m = _m + 1 - j;
                if (!tach3_.Equals("000"))
                    lso_chu = lso_chu.Trim() + " " + Donvi(m.ToString().Trim()).Trim();
                tach_conlai = tach_conlai.Trim().Substring(3, tach_conlai.Trim().Length - 3);

                i = i - 1;
                j = j + 1;
            }
            if (lso_chu.Trim().Substring(0, 1).Equals("k"))
                lso_chu = lso_chu.Trim().Substring(10, lso_chu.Trim().Length - 10).Trim();
            if (lso_chu.Trim().Substring(0, 1).Equals("l"))
                lso_chu = lso_chu.Trim().Substring(2, lso_chu.Trim().Length - 2).Trim();
            if (lso_chu.Trim().Length > 0)
                lso_chu = dau.Trim() + " " + lso_chu.Trim().Substring(0, 1).Trim().ToUpper() + lso_chu.Trim().Substring(1, lso_chu.Trim().Length - 1).Trim() + " đồng.";

            return lso_chu.ToString().Trim();

        }

        public static User GetUserInfo()
        {
            if (HttpContext.Current.Session["User"] != null)
            {
                return HttpContext.Current.Session["User"] as User;
            }

            return null;
        }
    }
}