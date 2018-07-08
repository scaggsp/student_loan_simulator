using System;
using System.Collections.Generic;

namespace StudentLoanSimulator
{
    public class StudentLoanSchedule
    {
        #region Properties

        private List<StudentLoan> fullListOfLoans;
        private List<StudentLoan> thisPayCyclesLoans;
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
