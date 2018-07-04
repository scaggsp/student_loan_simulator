using System;

namespace StudentLoanSimulator
{
    public class StudentLoan
    {
        public String LenderName { get; set; }
        public String AccountNumber { get; set; }
        public float APR { get; set; }
        public float MinPayment { get; set; }
        public DateTime PaymentStartDate { get; set; }
    }
}
