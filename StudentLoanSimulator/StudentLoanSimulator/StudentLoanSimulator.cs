using System;

namespace StudentLoanSimulator
{
    public class StudentLoan
    {
        public String LenderName { get; set; }
        public String AccountNumber { get; set; }
        public decimal APR { get; set; }
        public decimal MinPayment { get; set; }
        public DateTime PaymentStartDate { get; set; }
        public decimal Principle { get; set; }

        public LastPaymentDetails LastPayment { get; set; }

        private decimal dailyInterest;

        public StudentLoan(String lenderName,
                           String accountNumber,
                           decimal aPR,
                           decimal minPayment,
                           DateTime paymentStartDate,
                           decimal startingPrinciple
                           )
        {
            LenderName = lenderName;
            AccountNumber = accountNumber;

            /// APR cannot be greater than 100%
            /// While this is not technically incorrect, it likely occured 
            /// because the APR was a percentage instead of decimal 
            /// (3.25% is 0.0325 not 3.25)
            if (aPR >= 1.0m)
            {
                throw new APROutOfRange();
            }
            else
            {
                APR = aPR;
                // daily interest is the APR / number of days in a year
                dailyInterest = aPR / 365;
            }

            MinPayment = minPayment;
            PaymentStartDate = paymentStartDate;
            Principle = startingPrinciple;

            LastPayment = new LastPaymentDetails();
            LastPayment.PaymentDate = paymentStartDate;
        }

        public decimal CalcInterest(DateTime paymentDate)
        {
            decimal daysSinceLastPayment = (paymentDate - LastPayment.PaymentDate).Days;

            decimal accruedInterest = Principle * (dailyInterest * daysSinceLastPayment);
            accruedInterest = Math.Round(accruedInterest, 2); // round to two decimal places

            return accruedInterest;
        }

        public void MakePayment(DateTime paymentDate, decimal payment)
        {
            decimal interest = this.CalcInterest(paymentDate);

            this.LastPayment.PrinciplePayment = (payment - interest);
        }


        public class LastPaymentDetails
        {
            public decimal TotalPayment { get; set; }
            public decimal InterestPayment { get; set; }
            public decimal PrinciplePayment { get; set; }
            public DateTime PaymentDate { get; set; }
        }

        public class APROutOfRange : Exception
        {
            public APROutOfRange()
            {
            }
        }
    }
}
