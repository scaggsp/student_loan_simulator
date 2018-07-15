using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StudentLoanSimulatorCLI;
using StudentLoanSimulator;

namespace StudentLoanSimulatorCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            List<StudentLoan> listOfLoans = GetLoanList();
            List<ScheduledPayment> listOfPayments = GetPaymentList();

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, listOfPayments);
            testSchedule.GenerateSchedule();
        }

        static List<StudentLoan> GetLoanList()
        {
            List<StudentLoan> listOfLoans = new List<StudentLoan>
            {
                new StudentLoan("Test Lender","123456-1111", 0.0325m, 10.0m, new DateTime(2018, 1, 1), 50.0m),
                new StudentLoan("Test Lender","123456-2222", 0.0325m, 10.0m, new DateTime(2018, 3, 1), 50.0m),
            };

            return listOfLoans;
        }
        
        static List<ScheduledPayment> GetPaymentList()
        {
            List<ScheduledPayment> listOfPayments = new List<ScheduledPayment>
            {
                // first payment should only apply to loan 1 (loan 2 not in repayment yet)
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 2, 1), TotalPayment = 40m},
                // second payment should pay off both loans
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 3, 1), TotalPayment = 61m},
            };

            return listOfPayments;
        }
    }
}
