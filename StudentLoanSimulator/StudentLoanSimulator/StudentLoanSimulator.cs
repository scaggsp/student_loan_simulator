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
        private PaymentLock paymentLock;
        public decimal AccruedInterest { get; set; }


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
                throw new APROutOfRangeException();
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
            
            paymentLock = PaymentLock.PaymentsLocked;
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

        public void UnlockPayments(DateTime paymentDate)
        {
            if (paymentLock == PaymentLock.PaymentsLocked)
            {
                AccruedInterest = CalcInterest(paymentDate);
                paymentLock = PaymentLock.PaymentsUnlocked;
            }
            else
            {
                throw new PaymentsLockException("Payments already unlocked! Please review the payment flow.");
            }
        }

        public void MakePayment(decimal payment)
        {
            if (paymentLock == PaymentLock.PaymentsLocked)
            {
                throw new PaymentsLockException("Payments Locked!");
            }
            else
            {
                // record total payment details
                LastPayment.TotalPayment += payment;

                // payment interest and principle component trackers
                decimal interestPaid = 0.0m;
                decimal principlePaid = 0.0m;

                if (payment < AccruedInterest)
                {
                    // payment only covers interest
                    interestPaid = payment;
                    AccruedInterest -= payment;
                }
                else
                {
                    // pay interest first
                    interestPaid = AccruedInterest;
                    payment -= AccruedInterest;
                    AccruedInterest = 0.0m;

                    // remaining payment reduces principle
                    principlePaid = payment;
                    Principle -= principlePaid;
                }

                // record payment details
                LastPayment.InterestPayment += interestPaid;
                LastPayment.PrinciplePayment += principlePaid;
            }
        }

        public void LockPayments()
        {
            if (paymentLock == PaymentLock.PaymentsUnlocked)
            {
                if (LastPayment.TotalPayment < MinPayment)
                {
                    throw new PaymentException("Minimum payment was not made!");
                }
                else
                {
                    paymentLock = PaymentLock.PaymentsLocked;
                }
            }
            else
            {
                throw new PaymentsLockException("Payments already Locked! Please review the payment flow.");
            }
        }

        public class LastPaymentDetails
        {
            public decimal TotalPayment { get; set; }
            public decimal InterestPayment { get; set; }
            public decimal PrinciplePayment { get; set; }
            public DateTime PaymentDate { get; set; }
        }

        private enum PaymentLock
        {
            PaymentsLocked,
            PaymentsUnlocked
        }

        #region Exceptions

        public class APROutOfRangeException : Exception
        {
            public APROutOfRangeException()
            {
            }

            public APROutOfRangeException(string message)
                : base(message)
            {
            }

            public APROutOfRangeException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class PaymentsLockException : Exception
        {
            public PaymentsLockException()
            {
            }

            public PaymentsLockException(string message)
                : base(message)
            {
            }

            public PaymentsLockException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class PaymentException : Exception
        {
            public PaymentException()
            {
            }

            public PaymentException(string message)
                : base(message)
            {
            }

            public PaymentException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
        #endregion
    }
}
