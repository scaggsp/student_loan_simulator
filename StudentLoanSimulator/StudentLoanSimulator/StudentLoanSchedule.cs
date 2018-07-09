using System;
using System.Collections.Generic;

namespace StudentLoanSimulator
{
    public class StudentLoanSchedule
    {
        #region Properties

        private List<StudentLoan> fullListOfLoans;
        private DateTime currentPayDate;

        private List<ScheduledPayment> scheduledPayments;

        #endregion

        #region Constructors

        public StudentLoanSchedule(List<StudentLoan> listOfLoans, List<ScheduledPayment> listOfPayments)
        {
            if (listOfLoans.Count == 0)
            {
                throw new ConstructorException("List of Loans cannot be empty!");
            }

            fullListOfLoans = listOfLoans;
            scheduledPayments = listOfPayments;

            // set the current pay date to the loan list's earliest start date
            currentPayDate = GetEarliestStartDate();
        }

        #endregion

        #region Helper Methods
        private DateTime GetEarliestStartDate()
        {
            // set the current pay date to the loan list's earliest start date
            DateTime earliestStartDate = fullListOfLoans[0].PaymentStartDate;

            foreach (StudentLoan loan in fullListOfLoans)
            {
                if (loan.PaymentStartDate < earliestStartDate)
                {
                    earliestStartDate = loan.PaymentStartDate;
                }
            }

            return earliestStartDate;
        }

        private List<StudentLoan> GetThisPayCyclesLoans()
        {
            List<StudentLoan> loanSubset = new List<StudentLoan>();

            foreach (StudentLoan loan in fullListOfLoans)
            {
                if ((true == loan.InRepayment(currentPayDate)) && (false == loan.PaidOff))
                {
                    loanSubset.Add(loan);
                }
            }

            return loanSubset;
        }

        private decimal GetThisPayCyclesMoneypot()
        {
            decimal thisPayCycleMoneypot = 0m;

            ScheduledPayment thisPayCyclePayment;
            thisPayCyclePayment = scheduledPayments.Find(x => x.PaymentDate == currentPayDate);

            if (null != thisPayCyclePayment)
            {
                thisPayCycleMoneypot = thisPayCyclePayment.TotalPayment;
            }
            else
            {
                throw new MoneypotException("Not scheduled payment exists for this pay cycle!");
            }

            return thisPayCycleMoneypot;
        }
        
        private void UnlockPayments(List<StudentLoan> listOfLoans)
        {
            foreach (StudentLoan loan in listOfLoans)
            {
                loan.UnlockPayments(currentPayDate);
            }
        }

        private void LockPayments(StudentLoan loan)
        {
            loan.LockPayments();
        }

        private void LockPayments(List<StudentLoan> listOfLoans)
        {
            foreach (StudentLoan loan in listOfLoans)
            {
                loan.LockPayments();
            }
        }

        private decimal GetMinimumPayments(List<StudentLoan> listOfLoans)
        {
            decimal totalMinPayment = 0m;

            foreach (StudentLoan loan in listOfLoans)
            {
                decimal minPayment = loan.MinPayment;
                decimal payOffAmount = loan.PayoffAmount;

                if (payOffAmount < minPayment)
                {
                    totalMinPayment += payOffAmount;
                }
                else
                {
                    totalMinPayment += minPayment;
                }
            }

            return totalMinPayment;
        }

        private void LoadMoneypot(List<StudentLoan> listOfLoans)
        {
            decimal moneypot = GetThisPayCyclesMoneypot();
            UnlockPayments(listOfLoans);
            decimal totalMinPayment = GetMinimumPayments(listOfLoans);

            if (moneypot < totalMinPayment)
            {
                throw new MoneypotException("This pay cycle's total payment does not cover the loan minimum payments!");
            }
        }

        private decimal MakeMinimumPayments(List<StudentLoan> listOfLoans)
        {
            decimal totalPaymentMade = 0m;

            foreach (StudentLoan loan in listOfLoans)
            {
                decimal minPayment = loan.MinPayment;
                decimal payOffAmount = loan.PayoffAmount;

                if (payOffAmount < minPayment)
                {
                    totalPaymentMade += payOffAmount;
                    loan.MakePayment(payOffAmount); // pay off the loan
                    LockPayments(loan); // lock the loan to reject any future payments
                }
                else
                {
                    totalPaymentMade += minPayment;
                    loan.MakePayment(minPayment);
                }
            }

            // remove any loans paid off when making minimum payments
            listOfLoans.RemoveAll(loan => loan.Principle == 0m);

            return totalPaymentMade;
        }
        #endregion

        #region Exceptions

        public class ConstructorException : Exception
        {
            public ConstructorException()
            {
            }

            public ConstructorException(string message)
                : base(message)
            {
            }

            public ConstructorException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }

        public class MoneypotException : Exception
        {
            public MoneypotException()
            {
            }

            public MoneypotException(string message)
                : base(message)
            {
            }

            public MoneypotException(string message, Exception inner)
                : base(message, inner)
            {
            }
        }
        #endregion
    }

    public class ScheduledPayment
    {
        public DateTime PaymentDate;
        public decimal TotalPayment;
    }
}
