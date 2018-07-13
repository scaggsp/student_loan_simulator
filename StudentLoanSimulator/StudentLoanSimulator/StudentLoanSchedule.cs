using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            StringBuilder header = new StringBuilder();
            header.Append("Date,Payment Total,");

            foreach (StudentLoan loan in fullListOfLoans)
            {
                // expected loan header "<LenderName>: <AccountNumber>"
                header.Append(loan.LenderName + ": " + loan.AccountNumber).Append(",");
            }
            header.Length--; // remove the last comma

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
            StringBuilder header = new StringBuilder();
            header.Append("Date,Principle Total,Interest Total,Payment Total,");

            foreach (StudentLoan loan in fullListOfLoans)
            {
                string loanHeader = loan.LenderName + ": " + loan.AccountNumber;

                // add the three loan headers to log header
                header.Append(loanHeader + " - Principle").Append(",");
                header.Append(loanHeader + " - Interest").Append(",");
                header.Append(loanHeader + " - Payment").Append(",");
            }
            header.Length--; // remove the last comma

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

        private void LogSimplePaymentDetails()
        {
            StringBuilder paymentLine = new StringBuilder();

            // add the payment Date
            paymentLine.Append("\"").Append(currentPayDate.ToString("MMMM, yyyy")).Append("\"").Append(",");

            // determine this pay cycle's total payment amount
            decimal totalPayment = 0m;
            foreach (StudentLoan loan in fullListOfLoans)
            {
                if (loan.LastPayment.PaymentDate.Date == currentPayDate.Date)
                {
                    totalPayment += loan.LastPayment.TotalPayment;
                }
            }

            // add total payment to payment line
            paymentLine.Append(totalPayment).Append(",");

            // add each loan's total payment
            foreach (StudentLoan loan in fullListOfLoans)
            {
                // only log payments that match the current pay cycle date
                if (loan.LastPayment.PaymentDate.Date == currentPayDate.Date)
                {
                    paymentLine.Append(loan.LastPayment.TotalPayment).Append(",");
                }
                else
                {
                    paymentLine.Append(0).Append(",");
                }
            }
            paymentLine.Length--; // remove the last comma

            // create simple log file
            string simpleLogFilePath = Path.Combine(logFileDirectory, SIMPLE_LOG_FILENAME);
            using (StreamWriter sw = File.AppendText(simpleLogFilePath))
            {
                sw.WriteLine(paymentLine);
            }
            LogPaymentDetails(PaymentLogType.SimplePaymentLog, SIMPLE_LOG_FILENAME);
        }

        private void LogExpandedPaymentDetails()
        {
            LogPaymentDetails(PaymentLogType.ExpandedPaymentLog, EXPANDED_LOG_FILENAME);
        }

        private void LogPaymentDetails(PaymentLogType logType, string logFilename)
        {
            StringBuilder paymentLine = new StringBuilder();

            // add the payment Date
            paymentLine.Append("\"").Append(currentPayDate.ToString("MMMM, yyyy")).Append("\"").Append(",");

            // determine this pay cycle's total payment amount
            decimal totalPayment = 0m;
            decimal totalInterest = 0m;
            decimal totalPrinciple = 0m;
            foreach (StudentLoan loan in fullListOfLoans)
            {
                if (loan.LastPayment.PaymentDate.Date == currentPayDate.Date)
                {
                    totalPrinciple += loan.LastPayment.PrinciplePayment;
                    totalInterest += loan.LastPayment.InterestPayment;
                    totalPayment += loan.LastPayment.TotalPayment;
                }
            }

            // add total payment details to payment line
            if (logType == PaymentLogType.ExpandedPaymentLog)
            {
                paymentLine.Append(totalPrinciple).Append(",");
                paymentLine.Append(totalInterest).Append(",");
            }
            paymentLine.Append(totalPayment).Append(",");

            // add each loan's total payment
            foreach (StudentLoan loan in fullListOfLoans)
            {
                // only log payments that match the current pay cycle date
                decimal paymentPrinciple = 0;
                decimal paymentInterest = 0;
                decimal paymentTotal = 0;

                if (loan.LastPayment.PaymentDate.Date == currentPayDate.Date)
                {
                    paymentPrinciple = loan.LastPayment.PrinciplePayment;
                    paymentInterest = loan.LastPayment.InterestPayment;
                    paymentTotal = loan.LastPayment.TotalPayment;
                }

                if (logType == PaymentLogType.ExpandedPaymentLog)
                {
                    paymentLine.Append(paymentPrinciple).Append(",");
                    paymentLine.Append(paymentInterest).Append(",");
                }
                paymentLine.Append(paymentTotal).Append(",");
            }
            paymentLine.Length--; // remove the last comma

            // add payment to log file
            string logFilePath = Path.Combine(logFileDirectory, logFilename);
            using (StreamWriter sw = File.AppendText(logFilePath))
            {
                sw.WriteLine(paymentLine);
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

        private enum PaymentLogType
        {
            SimplePaymentLog,
            ExpandedPaymentLog
        }
    }

    public class ScheduledPayment
    {
        public DateTime PaymentDate;
        public decimal TotalPayment;
    }
}
