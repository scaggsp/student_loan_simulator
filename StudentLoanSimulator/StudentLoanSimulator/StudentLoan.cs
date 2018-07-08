using System;

namespace StudentLoanSimulator
{
    public class StudentLoan
    {
        #region Properies and Fields

        public String LenderName { get; }
        public String AccountNumber { get; }
        public decimal APR { get; }
        public decimal MinPayment { get; }
        public DateTime PaymentStartDate { get; }

        public decimal Principle { get; private set; }
        public LastPaymentDetails LastPayment { get; set; }

        public decimal PayoffAmount
        {
            get
            {
                if (paymentLock == PaymentLock.PaymentsLocked)
                {
                    throw new PaymentsLockException("Payments Locked!");
                }
                else
                {
                    return (Principle + accruedInterest);
                }
            }
        }
        public bool PaidOff { get { return (0m == Principle); } }

        public bool InRepayment(DateTime date)
        {
            return (date >= PaymentStartDate);
        }

        private decimal dailyInterest;
        private PaymentLock paymentLock;
        private decimal accruedInterest;

        #endregion

        #region Constructors

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
            PaymentStartDate = paymentStartDate.Date;
            Principle = startingPrinciple;
            
            paymentLock = PaymentLock.PaymentsLocked;
            LastPayment = new LastPaymentDetails();
            LastPayment.PaymentDate = PaymentStartDate;
        }

        #endregion

        public void UnlockPayments(DateTime paymentDate)
        {
            if (paymentLock == PaymentLock.PaymentsLocked)
            {
                ClearLastPaymentDetails();
                accruedInterest = CalcInterest(paymentDate);
                // CalcInterest uses LastPayment.PaymentDate => calculate interest before changing date
                LastPayment.PaymentDate = paymentDate;
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
            else if (payment > PayoffAmount)
            {
                throw new PaymentException("Cannot overpay a loan! Payment will reduce principle < 0.");
            }
            else
            {
                // record total payment details
                LastPayment.TotalPayment += payment;

                // payment interest and principle component trackers
                decimal interestPaid = 0.0m;
                decimal principlePaid = 0.0m;

                if (payment < accruedInterest)
                {
                    // payment only covers interest
                    interestPaid = payment;
                    accruedInterest -= payment;
                }
                else
                {
                    // pay interest first
                    interestPaid = accruedInterest;
                    payment -= accruedInterest;
                    accruedInterest = 0.0m;

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
                if ((true == PaidOff) || (LastPayment.TotalPayment >= MinPayment))
                {
                    paymentLock = PaymentLock.PaymentsLocked;
                }
                else
                {
                    throw new PaymentException("Minimum payment was not made!");
                }
            }
            else
            {
                throw new PaymentsLockException("Payments already Locked! Please review the payment flow.");
            }
        }

        #region Private Functions

        private decimal CalcInterest(DateTime paymentDate)
        {
            decimal daysSinceLastPayment = (paymentDate.Date - LastPayment.PaymentDate.Date).Days;

            decimal accruedInterest = Principle * (dailyInterest * daysSinceLastPayment);
            accruedInterest = Math.Round(accruedInterest, 2); // round to two decimal places

            return accruedInterest;
        }

        private void ClearLastPaymentDetails()
        {
            LastPayment.TotalPayment = 0m;
            LastPayment.InterestPayment = 0m;
            LastPayment.PrinciplePayment = 0m;
        }

        #endregion

        public class LastPaymentDetails
        {
            public decimal TotalPayment { get; set; }
            public decimal InterestPayment { get; set; }
            public decimal PrinciplePayment { get; set; }
            public DateTime PaymentDate { get; set; }
        }

        public enum PaymentLock
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
