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

            /// APR cannot be greater than 100%
            /// While this is not technically incorrect, it likely occured 
            /// because the APR was a percentage instead of decimal 
            /// (3.25% is 0.0325 not 3.25)
            if (aPR >= 1.0)
            {
                throw new APROutOfRange();
            }
            else
            {
                APR = aPR;
            }
            
            MinPayment = minPayment;
            PaymentStartDate = paymentStartDate;
            Principle = startingPrinciple;
        }

        public class APROutOfRange : Exception
        {
            public APROutOfRange()
            {
            }
        }
    }
}
