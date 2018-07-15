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

            // allow listOfPayments to be null for unit testing
            if ((scheduledPayments != null) && (scheduledPayments.Count > 0))
            {
                scheduledPayments.Sort((x, y) => DateTime.Compare(x.PaymentDate, y.PaymentDate));

                // set the current pay date to the earliest payment date
                currentPayDate = scheduledPayments[0].PaymentDate;
            }

            logFileDirectory = setLogFileDirectory;
        }

        #endregion

        public void GenerateSchedule()
        {
            CreateLogFiles();

            while ((scheduledPayments.Count != 0) && (GetActiveLoanCount() != 0))
            {
                ProcessPayCycle();

                // advance pay cycle to next scheduled payment
                AdvanceToNextPayCycleDate();
            }
        }

        #region Helper Methods

        private void ProcessPayCycle()
        {
            // get this pay cycle's subset of loans
            List<StudentLoan> loans = GetThisPayCyclesLoans();

            // get this pay cycle's moneypot
            decimal moneypot = GetThisPayCyclesMoneypot();

            // unlock payments
            UnlockPayments(loans);

            // apply minimum payments to loans
            // reduce the moneypot by the total of payments made
            MakeMinimumPayments(loans, ref moneypot);

            // apply extra payments to loans
            MakeExtraPayments(loans, ref moneypot);

            // lock payments
            LockPayments(loans);

            // log last payment details
            LogLastPaymentDetails();
        }

        /// <summary>
        /// returns the number of loans from fullListOfLoans that are not paid off
        /// </summary>
        private uint GetActiveLoanCount()
        {
            uint activeLoanCount = 0;

            foreach (StudentLoan loan in fullListOfLoans)
            {
                if (loan.PaidOff == false)
                {
                    activeLoanCount++;
                }
            }

            return activeLoanCount;
        }

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

        private void MakeMinimumPayments(List<StudentLoan> listOfLoans, ref decimal moneypot)
        {
            // apply the minimum payment or pay off amount (whichever is less)
            foreach (StudentLoan loan in listOfLoans)
            {
                decimal minPayment = loan.MinPayment;
                decimal payOffAmount = loan.PayoffAmount;

                if (payOffAmount < minPayment)
                {
                    moneypot -= payOffAmount;
                    loan.MakePayment(payOffAmount); // pay off the loan
                    LockPayments(loan); // lock the loan to reject any future payments
                }
                else
                {
                    moneypot -= minPayment;
                    loan.MakePayment(minPayment);
                }
            }

            // remove any loans paid off when making minimum payments
            listOfLoans.RemoveAll(loan => loan.Principle == 0m);
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
            // check for out of bounds
            if (scheduledPayments.Count == 0)
            {
                throw new ScheduledPaymentsException("No other scheduled payments exist!");
            }

            // remove the last payment from the list
            scheduledPayments.RemoveAt(0);

            if (scheduledPayments.Count != 0)
            {
                // sort the list of payments so the earliest date is at index 0
                scheduledPayments.Sort((x, y) => DateTime.Compare(x.PaymentDate, y.PaymentDate));

                // advance date
                currentPayDate = scheduledPayments[0].PaymentDate;
            }
        }

        private void CreateLogFiles()
        {
            SetupLogDirectory();
            CreateSimpleLogFile();
            CreateExpandedLogFile();
        }

        private void SetupLogDirectory()
        {
            Directory.CreateDirectory(logFileDirectory);
        }

        private void CreateSimpleLogFile()
        {
            CreateLogFile(PaymentLogType.SimplePaymentLog, SIMPLE_LOG_FILENAME);
        }

        private void CreateExpandedLogFile()
        {
            CreateLogFile(PaymentLogType.ExpandedPaymentLog, EXPANDED_LOG_FILENAME);
        }

        /// <summary>
        /// Created the requested log file and populate it with the appropriate CSV header
        /// </summary>
        /// <param name="logType">Specifies if the simple or expanded (detailed) schedule is requested</param>
        /// <param name="logFilename">Filename of log file</param>
        private void CreateLogFile(PaymentLogType logType, string logFilename)
        {
            StringBuilder header = new StringBuilder();

            header.Append("Date,");
            if (logType == PaymentLogType.ExpandedPaymentLog)
            {
                header.Append("Principle Total,Interest Total,");
            }
            header.Append("Payment Total,");


            foreach (StudentLoan loan in fullListOfLoans)
            {
                string loanHeader = loan.LenderName + ": " + loan.AccountNumber;

                if (logType == PaymentLogType.SimplePaymentLog)
                {
                    header.Append(loanHeader).Append(",");
                }
                else
                {
                    // add the three loan headers to log header
                    header.Append(loanHeader + " - Principle").Append(",");
                    header.Append(loanHeader + " - Interest").Append(",");
                    header.Append(loanHeader + " - Payment").Append(",");
                }
            }
            header.Length--; // remove the last comma

            // create simple log file
            string logFilePath = Path.Combine(logFileDirectory, logFilename);
            using (FileStream fs = File.Create(logFilePath))  // auto-dispose filestream
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(header);
                }
            }
        }

        private void LogLastPaymentDetails()
        {
            LogSimplePaymentDetails();
            LogExpandedPaymentDetails();
        }

        /// <summary>
        /// Wrapper function that requests the latest payment details be added to the simple log file
        /// </summary>
        private void LogSimplePaymentDetails()
        {
            LogPaymentDetails(PaymentLogType.SimplePaymentLog, SIMPLE_LOG_FILENAME);
        }

        /// <summary>
        /// Wrapper function that requests the latest payment details be added to the expanded log file
        /// </summary>
        private void LogExpandedPaymentDetails()
        {
            LogPaymentDetails(PaymentLogType.ExpandedPaymentLog, EXPANDED_LOG_FILENAME);
        }

        /// <summary>
        /// Add last payment details to the requested log file
        /// </summary>
        /// <param name="logType">Specifies if the simple or expanded (detailed) schedule is requested</param>
        /// <param name="logFilename">Filename of log file</param>
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
                paymentLine.Append(totalPrinciple.ToString("0.00")).Append(",");
                paymentLine.Append(totalInterest.ToString("0.00")).Append(",");
            }
            paymentLine.Append(totalPayment.ToString("0.00")).Append(",");

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
                    paymentLine.Append(paymentPrinciple.ToString("0.00")).Append(",");
                    paymentLine.Append(paymentInterest.ToString("0.00")).Append(",");
                }
                paymentLine.Append(paymentTotal.ToString("0.00")).Append(",");
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
