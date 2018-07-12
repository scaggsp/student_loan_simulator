using System;
using System.Collections.Generic;
using System.IO;

namespace StudentLoanSimulator
{
    public class StudentLoanSchedule
    {
        #region Constants
        const string SIMPLE_LOG_FILENAME = "Simple Payment Schedule.csv";
        const string EXPANDED_LOG_FILENAME = "Detailed Payment Schedule.csv";
        #endregion

        #region Properties

        private List<StudentLoan> fullListOfLoans;
        private DateTime currentPayDate;

        private List<ScheduledPayment> scheduledPayments;
        private int paymentIndex;

        private string logFileDirectory;

        #endregion

        #region Constructors

        public StudentLoanSchedule(List<StudentLoan> listOfLoans, List<ScheduledPayment> listOfPayments, string setLogFileDirectory = @".\Payment Schedules\")
        {
            if (listOfLoans.Count == 0)
            {
                throw new ConstructorException("List of Loans cannot be empty!");
            }

            fullListOfLoans = listOfLoans;
            scheduledPayments = listOfPayments;
            paymentIndex = 0;

            if (scheduledPayments != null)
            {
                scheduledPayments.Sort((x, y) => DateTime.Compare(x.PaymentDate, y.PaymentDate));
            }

            logFileDirectory = setLogFileDirectory;

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

        private StudentLoan FindHighestAPRLoan(List<StudentLoan> listOfLoans)
        {
            StudentLoan highestAPRLoan = listOfLoans[0];

            foreach (StudentLoan loan in listOfLoans)
            {
                if (loan.APR > highestAPRLoan.APR)
                {
                    // this loan has a higher APR
                    highestAPRLoan = loan;
                }
                else if (loan.APR == highestAPRLoan.APR)
                {
                    // if the apr is the same, prioritize the lower principle loan
                    if (loan.Principle < highestAPRLoan.Principle)
                    {
                        highestAPRLoan = loan;
                    }
                }
            }

            return highestAPRLoan;
        }

        private void MakeExtraPayments(List<StudentLoan> listOfLoans, ref decimal moneypot)
        {
            decimal remainingMoneypot = moneypot;
            decimal totalPaymentsMade = 0m;

            while ((remainingMoneypot != 0) && (listOfLoans.Count != 0))
            {
                StudentLoan highestAPRLoan = FindHighestAPRLoan(listOfLoans);
                decimal paymentAmount = 0;
                if (highestAPRLoan.PayoffAmount <= remainingMoneypot)
                {
                    // extra payment will pay off loan
                    paymentAmount = highestAPRLoan.PayoffAmount;
                    highestAPRLoan.MakePayment(paymentAmount);
                    LockPayments(highestAPRLoan); // lock the loan to reject any future payments
                }
                else
                {
                    paymentAmount = remainingMoneypot;
                    highestAPRLoan.MakePayment(paymentAmount);
                }

                totalPaymentsMade += paymentAmount;
                remainingMoneypot -= paymentAmount;

                // remove any loans paid off when making minimum payments
                listOfLoans.RemoveAll(loan => loan.Principle == 0m);
            }

            moneypot = remainingMoneypot;
        }

        /// <summary>
        /// Finds the next payment date in scheduledPayments
        /// currentPayDate is set to this next payment date
        /// </summary>
        private void AdvanceToNextPayCycleDate()
        {
            paymentIndex++;

            // check for out of bounds
            if (paymentIndex > (scheduledPayments.Count - 1))
            {
                throw new ScheduledPaymentsException("No other scheduled payments exist!");
            }

            // advance date
            currentPayDate = scheduledPayments[paymentIndex].PaymentDate;
        }

        private void SetupLogDirectory()
        {
            Directory.CreateDirectory(logFileDirectory);
        }

        private void CreateSimpleLogFile()
        {
            string header = "Date,Payment Total";

            foreach (StudentLoan loan in fullListOfLoans)
            {
                // expected loan header "<LenderName>: <AccountNumber>"
                string loanHeader = loan.LenderName + ": " + loan.AccountNumber;
                header = header + "," + loanHeader;
            }

            // create simple log file
            string simpleLogFilePath = Path.Combine(logFileDirectory, SIMPLE_LOG_FILENAME);
            using (FileStream fs = File.Create(simpleLogFilePath))  // auto-dispose filestream
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(header);
                }
            }
        }

        private void CreateExpandedLogFile()
        {
            string header = "Date,Principle Total,Interest Total,Payment Total";

            foreach (StudentLoan loan in fullListOfLoans)
            {
                string loanHeader = loan.LenderName + ": " + loan.AccountNumber;
                // three seperate headers (principle, interest, payment)
                string principleHeader = loanHeader + " - Principle";
                string interestHeader = loanHeader + " - Interest";
                string paymentHeader = loanHeader + " - Payment";

                // add the three loan headers to log header
                header = header + "," + principleHeader;
                header = header + "," + interestHeader;
                header = header + "," + paymentHeader;
            }

            // create simple log file
            string expandedLogFilePath = Path.Combine(logFileDirectory, EXPANDED_LOG_FILENAME);
            using (FileStream fs = File.Create(expandedLogFilePath))  // auto-dispose filestream
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(header);
                }
            }
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

        public class ScheduledPaymentsException : Exception
        {
            public ScheduledPaymentsException()
            {
            }

            public ScheduledPaymentsException(string message)
                : base(message)
            {
            }

            public ScheduledPaymentsException(string message, Exception inner)
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
