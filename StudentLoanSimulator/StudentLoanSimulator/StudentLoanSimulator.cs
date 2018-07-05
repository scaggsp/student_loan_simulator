using System;

namespace StudentLoanSimulator
{
    public class StudentLoan
    {
        public String LenderName { get; set; }
        public String AccountNumber { get; set; }
        public double APR { get; set; }
        public double MinPayment { get; set; }
        public DateTime PaymentStartDate { get; set; }
        public double Principle { get; set; }

        public StudentLoan(String lenderName,
                           String accountNumber,
                           double aPR,
                           double minPayment,
                           DateTime paymentStartDate,
                           double startingPrinciple
                           )
        {
            LenderName = lenderName;
            AccountNumber = accountNumber;
            APR = aPR;
            MinPayment = minPayment;
            PaymentStartDate = paymentStartDate;
            Principle = startingPrinciple;
        }
    }
}
