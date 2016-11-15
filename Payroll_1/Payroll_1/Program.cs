using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Payroll_1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> mystaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0;
            int year = 0;

            while(year == 0)
            {
                Console.Write("\nPlease enter the year: ");

                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.Write("year error");
                }
            }

            while (month == 0)
            {
                Console.Write("\nPlease enter the month: ");

                try
                {
                    month = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.Write("month error");
                }
            }

            mystaff = fr.ReadFile();

            for(int i = 0; i < mystaff.Count; i++)
            {
                try
                {
                    Console.Write("Enter hours worked for " + mystaff[i].NameOfStaff);
                    mystaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    mystaff[i].CalculatePay();
                    Console.WriteLine(mystaff[i].ToString());
                }catch(Exception e)
                {
                    Console.WriteLine("Error");
                    i--;
                }

                PaySlip ps = new PaySlip(month,year);
                ps.GeneratePaySlip(mystaff);
                ps.GenerateSummary(mystaff);
            }
        }
    }

    class Staff
    {
        //variable
        public float hourlyRate;
        public int hWorked;

        //properties
        public float TotalPay
        {
            get;
            set;
        }

        public float BasicPay
        {
            get;
            set;
        }

        public string NameOfStaff
        {
            get;
            set;
        }

        public int HoursWorked
        {
            get
            {
                if(hWorked > 0)
                {
                    return hWorked;
                }
                else
                {
                    hWorked = 0;
                    return hWorked;
                }
            }
            set
            {
                hWorked = value;
            }
        }
        //constructor

        public Staff(string name, float rate)
        {
            this.NameOfStaff = name;
            this.hourlyRate = rate;
        }


        //Method
        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating Pay...");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }

        public override string ToString()
        {
            return BasicPay.ToString() + " " + TotalPay.ToString();
        }
    }

    class Manager : Staff
    {
        //variable
        private const float managerHourlyRate = 50;

        //Properties
        public int Allowance
        {
            get;
            set;
        }

        //Constructor
        public Manager(string name) : base(name,managerHourlyRate)
        {
            
        }


        //Method
        public override void CalculatePay()
        {
            base.CalculatePay();
            Allowance = 1000;

            if (HoursWorked > 160)
            {
                TotalPay = TotalPay + Allowance;
            }
        }

        public override string ToString()
        {
            return TotalPay.ToString();
        }
    }

    class Admin : Staff
    {
        //Variable
        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30;

        //Properties
        public float Overtime
        {
            get;
            private set;
        }
        
        //Constructor
        public Admin(string name) : base(name,adminHourlyRate)
        {

        }

        //Method
        public override void CalculatePay()
        {
            base.CalculatePay();

            if(HoursWorked > 160)
            {
                Overtime = Overtime * (HoursWorked - 160);
                TotalPay = TotalPay + Overtime;
            }
        }

        public override string ToString()
        {
            return TotalPay.ToString();
        }

    }

    class FileReader
    {
        //Method
        public List<Staff> ReadFile()
        {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "Staff.txt";
            string[] separator = { ", " };
            int i = 2;
            if (File.Exists(@"Staff.txt"))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string s;
                    while (!sr.EndOfStream)
                    {
                        s = sr.ReadLine();
                        result = s.Split(separator,StringSplitOptions.None);

                        if (result[1] == "Manager")
                        {
                            Manager manager = new Manager(result[0]);
                            myStaff.Add(manager);
                        }else if(result[1] == "Admin")
                        {
                            Admin admin = new Admin(result[0]);
                            myStaff.Add(admin);
                        }
                    }
                    sr.Close();
                }
            }else
            {
                Console.WriteLine("File Not Found!");
            }
            
            return myStaff;

        }
    }

    class PaySlip
    {
        //Variables
        private int month;
        private int year;

        //Enum
        enum MonthsOfYear {JAN = 1,FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC}
        //Properties

        //Constructor
        public PaySlip(int payMonth, int payYear)
        {
            this.month = payMonth;
            this.year = payYear;
        }
        //Methods
        public void GeneratePaySlip(List<Staff> myStaff)
        {
            string path;

            foreach (Staff f in myStaff)
            {
                path = f.NameOfStaff + ".txt";
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("PAYSLIP FOR " + (MonthsOfYear)month + " " + (MonthsOfYear)year);
                sw.WriteLine("======================");
                sw.WriteLine("Name of Staff : " + f.NameOfStaff);
                sw.WriteLine("Hours Worked : " + f.HoursWorked);
                sw.WriteLine("");
                sw.WriteLine("Basic Pay" + f.BasicPay);

                if (f.GetType() == typeof(Manager))
                {
                    sw.WriteLine("Allowance : " + ((Manager)f).Allowance);
                }
                else if (f.GetType() == typeof(Admin))
                {
                    sw.WriteLine("Overtime : " + ((Admin)f).Overtime);
                }

                sw.WriteLine("");
                sw.WriteLine("======================");
                sw.WriteLine("Total Pay : " + f.TotalPay);
                sw.WriteLine("======================");

                sw.Close();
            }
        }

        public void GenerateSummary(List<Staff> myStaff)
        {
            string path = "Summary.txt";
            var result = from s in myStaff
                         where s.HoursWorked < 10
                         orderby s
                         select new {s.NameOfStaff, s.HoursWorked };
           
            Console.WriteLine("Staff with less than 10 working hours");

            foreach(Staff f in myStaff)
            {
                StreamWriter sw = new StreamWriter(path);
                foreach(var r in result)
                {
                    sw.WriteLine("Name of Staff : " + r.NameOfStaff + ", Hours Worked : " + r.HoursWorked);
                }
                sw.Close();
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
