using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StudentLoanSimulator;

namespace StudentLoanSimulatorTests
{
    /// <summary>
    /// Unit tests of the StudentLoanSimulator class (i.e. Manage loan payments through the life of the loan)
    /// </summary>
    [TestClass]
    public class StudentLoanScheduleClassUnitTests
    {
        // instance of unit test helper methods
        UTestHelper UHelper = new UTestHelper();
        List<ScheduledPayment> DummyPayments = new List<ScheduledPayment>();

        /// <summary>
        /// Parse an XML file for a single loan's details
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestParseForSingleLoan()
        {
        }

        /// <summary>
        /// Parse an XML file for multiple loan details
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestParseForMulipleLoans()
        {
        }

        /// <summary>
        /// Parse an XML file for payment dates and amounts pairs
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestParseForPayments()
        {
        }

        /// <summary>
        /// The StudentLoanSchedule constructor accepts a list of loans
        /// </summary>
        [TestMethod]
        public void TestConstructorAcceptsListOfLoans()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            var privateObject = new PrivateObject(testSchedule);
            var fullListOfLoans = privateObject.GetField("fullListOfLoans");

            Assert.AreEqual(listOfLoans, fullListOfLoans);
        }

        /// <summary>
        /// If the StudentLoanSchedule constructor receives an empty loan list, throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoanSchedule.ConstructorException))]
        public void TestConstructorRejectsEmptyListOfLoans()
        {
            List<StudentLoan> listOfLoans = new List<StudentLoan>();

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);
        }

        /// <summary>
        /// From a list of loans, find the earliest start date
        /// Set the "current" date to the earliest start date
        /// </summary>
        [TestMethod]
        public void TestFindEarliestStartDate()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            DateTime expectedStartDate = new DateTime(2016, 1, 1);

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            var privateObject = new PrivateObject(testSchedule);
            var startingDate = privateObject.GetField("currentPayDate");

            Assert.AreEqual(expectedStartDate, startingDate);
        }


        /// <summary>
        /// Get a subset list of all loans in repayment and not paid off
        /// </summary>
        [TestMethod]
        public void TestGetPayCycleSubsetOfLoans()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            var privateObject = new PrivateObject(testSchedule);
            privateObject.SetField("currentPayDate", new DateTime(2018, 7, 1));
            var _subsetOfLoans = privateObject.Invoke("GetThisPayCyclesLoans");
            List<StudentLoan> subsetOfLoans = (List<StudentLoan>)_subsetOfLoans;

            // subset should include the first 3 loans, but not the last 4 loans
            Assert.AreEqual(3, subsetOfLoans.Count);
            Assert.IsTrue(subsetOfLoans.Contains(listOfLoans[0]));
            Assert.IsTrue(subsetOfLoans.Contains(listOfLoans[1]));
            Assert.IsTrue(subsetOfLoans.Contains(listOfLoans[2]));
            Assert.IsFalse(subsetOfLoans.Contains(listOfLoans[3]));
            Assert.IsFalse(subsetOfLoans.Contains(listOfLoans[4]));
            Assert.IsFalse(subsetOfLoans.Contains(listOfLoans[5]));
            Assert.IsFalse(subsetOfLoans.Contains(listOfLoans[6]));
        }


        /// <summary>
        /// If all loans are paid off, end simulation
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestEndSimulation()
        {
        }


        /// <summary>
        /// Constructor sorts the payments from earliest to latest
        /// </summary>
        [TestMethod]
        public void TestSortPaymentList()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            List<ScheduledPayment> listOfPayments = new List<ScheduledPayment>
            {
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 5, 1), TotalPayment = 100m},
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 3, 1), TotalPayment = 100m},
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 4, 1), TotalPayment = 100m},
            };

            List<ScheduledPayment> sortedListOfPayments = new List<ScheduledPayment>
            {
                listOfPayments[1],
                listOfPayments[2],
                listOfPayments[0],
            };

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, listOfPayments);

            var privateObject = new PrivateObject(testSchedule);
            var scheduledPayments = privateObject.GetField("scheduledPayments");
            
            CollectionAssert.AreEqual(sortedListOfPayments, listOfPayments);
        }


        /// <summary>
        /// Set the moneypot to the current date's pay cycle total payment amount
        /// </summary>
        [TestMethod]
        public void TestGetNextMoneypotAmount()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            List<ScheduledPayment> listOfPayments = new List<ScheduledPayment>
            {
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 3, 1), TotalPayment = 200m},
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 4, 1), TotalPayment = 100m},
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 5, 1), TotalPayment = 200m},
            };

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, listOfPayments);

            var privateObject = new PrivateObject(testSchedule);
            privateObject.SetField("currentPayDate", new DateTime(2018, 4, 1));
            var moneypot = privateObject.Invoke("GetThisPayCyclesMoneypot");

            Assert.AreEqual(100m, moneypot);
        }


        /// <summary>
        /// If the current date cannot determine this pay cycle's moneypot, throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoanSchedule.MoneypotException))]
        public void TestNoMoneypotForThisPayCycle()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            List<ScheduledPayment> listOfPayments = new List<ScheduledPayment>
            {
                new ScheduledPayment() { PaymentDate = new DateTime(2017, 3, 1), TotalPayment = 100m},
                new ScheduledPayment() { PaymentDate = new DateTime(2017, 4, 1), TotalPayment = 200m},
                new ScheduledPayment() { PaymentDate = new DateTime(2017, 5, 1), TotalPayment = 300m},
            };

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, listOfPayments);

            var privateObject = new PrivateObject(testSchedule);
            privateObject.SetField("currentPayDate", new DateTime(2018, 4, 1));
            try
            {
                var moneypot = privateObject.Invoke("GetThisPayCyclesMoneypot");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
            }
        }


        /// <summary>
        /// If the subset list is empty and this pay cycle's moneypot != 0, throw exception
        /// Payment of loans when they're not yet in repayment is not supported
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestGapInRepayment()
        {
        }


        /// <summary>
        /// Unlock payments for the only the subset of loans
        /// </summary>
        [TestMethod]
        public void TestUnlockSubsetOfLoans()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("currentPayDate", new DateTime(2018, 7, 1));
            var _subsetOfLoans = privateSchedule.Invoke("GetThisPayCyclesLoans");
            List<StudentLoan> subsetOfLoans = (List<StudentLoan>)_subsetOfLoans;
            privateSchedule.Invoke("UnlockPayments", subsetOfLoans);

            foreach (StudentLoan loan in listOfLoans)
            {
                var privateLoan = new PrivateObject(loan);
                var loanLock = privateLoan.GetField("paymentLock");

                if (true == subsetOfLoans.Contains(loan))
                {
                    Assert.AreEqual(StudentLoan.PaymentLock.PaymentsUnlocked, loanLock);
                }
                else
                {
                    Assert.AreEqual(StudentLoan.PaymentLock.PaymentsLocked, loanLock);
                }
            }
        }


        /// <summary>
        /// Determine this pay cycle's total minimum payment required by the subset loan list
        /// </summary>
        [TestMethod]
        public void TestPayCycleMinimumPayment()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("currentPayDate", new DateTime(2018, 7, 1));
            var _subsetOfLoans = privateSchedule.Invoke("GetThisPayCyclesLoans");
            List<StudentLoan> subsetOfLoans = (List<StudentLoan>)_subsetOfLoans;
            privateSchedule.Invoke("UnlockPayments", subsetOfLoans);

            var totalMinimumPayment = privateSchedule.Invoke("GetMinimumPayments", subsetOfLoans);

            Assert.AreEqual(30m, totalMinimumPayment);
        }


        /// <summary>
        /// Determine this pay cycle's total minimum payment required when a loan's minimum payment is > it's payoff amount
        /// i.e. when paying a loan's minimum payment will reduce the principle < 0
        /// </summary>
        [TestMethod]
        public void TestPayCycleMinimumPaymentWithPayOff()
        {
            List<StudentLoan> listOfLoans = new List<StudentLoan>
            {
                UHelper.NewSafeLoan(testLenderName: "Loan 1", testPaymentStartDate: new DateTime(2016, 1, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 2", testPaymentStartDate: new DateTime(2017, 4, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 3", testPaymentStartDate: new DateTime(2018, 4, 1), testStartingPrinciple: 5m),
                UHelper.NewSafeLoan(testLenderName: "Loan 4", testPaymentStartDate: new DateTime(2021, 4, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 5", testPaymentStartDate: new DateTime(2022, 3, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 6", testPaymentStartDate: new DateTime(2023, 3, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 7", testPaymentStartDate: new DateTime(2024, 3, 1)),
            };

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("currentPayDate", new DateTime(2018, 4, 1));
            var _subsetOfLoans = privateSchedule.Invoke("GetThisPayCyclesLoans");
            List<StudentLoan> subsetOfLoans = (List<StudentLoan>)_subsetOfLoans;
            privateSchedule.Invoke("UnlockPayments", subsetOfLoans);

            var totalMinimumPayment = privateSchedule.Invoke("GetMinimumPayments", subsetOfLoans);

            Assert.AreEqual(25m, totalMinimumPayment);
        }


        /// <summary>
        /// If the moneypot is < the minimum payments, throw an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoanSchedule.MoneypotException))]
        public void TestMoneypotDoesNotCoverMinimumPayments()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            List<ScheduledPayment> listOfPayments = new List<ScheduledPayment>
            {
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 4, 1), TotalPayment = 10m},
            };

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, listOfPayments);

            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("currentPayDate", new DateTime(2018, 4, 1));
            var _subsetOfLoans = privateSchedule.Invoke("GetThisPayCyclesLoans");
            List<StudentLoan> subsetOfLoans = (List<StudentLoan>)_subsetOfLoans;

            // The pay cycle total payment (moneypot) is only 10, but the minimum payment should be 30
            try
            {
                privateSchedule.Invoke("LoadMoneypot", subsetOfLoans);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
            }
        }


        /// <summary>
        /// Lock payments for the only a single loan
        /// </summary>
        [TestMethod]
        public void TestLockSingleLoan()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            // force the payment lock to Unlocked
            StudentLoan singleLoan = listOfLoans[0];
            var privateLoan = new PrivateObject(singleLoan);
            privateLoan.SetField("paymentLock", StudentLoan.PaymentLock.PaymentsUnlocked);

            // force the principle to 0 so lock will not check the minimum payment has been made
            privateLoan.SetProperty("Principle", 0m);

            // attempt to lock payments
            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.Invoke("LockPayments", singleLoan);

            // get the loan's payment loan value and assert it is now locked
            var loanLock = privateLoan.GetField("paymentLock");
            Assert.AreEqual(StudentLoan.PaymentLock.PaymentsLocked, loanLock);
        }


        /// <summary>
        /// Lock payments for the only the subset of loans after all payments complete
        /// </summary>
        [TestMethod]
        public void TestLockSubsetOfLoans()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            // the subset of loans only includes the first three loans
            List<StudentLoan> subsetOfLoans = new List<StudentLoan>()
            {
                listOfLoans[0],
                listOfLoans[1],
                listOfLoans[2],
            };

            // set all loans to unlocked so only the subset of loans are confirmed to be locked by method
            foreach (StudentLoan loan in listOfLoans)
            {
                // force the payment lock to Unlocked
                StudentLoan singleLoan = loan;
                var privateLoan = new PrivateObject(singleLoan);
                privateLoan.SetField("paymentLock", StudentLoan.PaymentLock.PaymentsUnlocked);

                // force the principle to 0 so lock will not check the minimum payment has been made
                privateLoan.SetProperty("Principle", 0m);
            }

            // attempt to lock payments of only the subset of loans
            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.Invoke("LockPayments", subsetOfLoans);

            foreach (StudentLoan loan in listOfLoans)
            {
                var privateLoan = new PrivateObject(loan);
                var loanLock = privateLoan.GetField("paymentLock");

                if (true == subsetOfLoans.Contains(loan))
                {
                    Assert.AreEqual(StudentLoan.PaymentLock.PaymentsLocked, loanLock);
                }
                else
                {
                    Assert.AreEqual(StudentLoan.PaymentLock.PaymentsUnlocked, loanLock);
                }
            }
        }


        /// <summary>
        /// Apply the mimimum payment to this pay cycle's loans
        /// Each payment should reduce the moneypot appropriately
        /// </summary>
        [TestMethod]
        public void TestApplyMinimumPayments()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            // start a pay cycle by setting the current date
            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("currentPayDate", new DateTime(2018, 7, 1));

            // get the subset of loans for this pay cycle
            var _subsetOfLoans = privateSchedule.Invoke("GetThisPayCyclesLoans");
            List<StudentLoan> subsetOfLoans = (List<StudentLoan>)_subsetOfLoans;

            // unlock the subset of loans so interest can be calculted
            privateSchedule.Invoke("UnlockPayments", subsetOfLoans);

            // get the total minimum payment required for the subset of loans
            var totalMinimumPayment = privateSchedule.Invoke("GetMinimumPayments", subsetOfLoans);

            // make the minimum payments to subset of loans and verify payment amount
            var paymentMade = privateSchedule.Invoke("MakeMinimumPayments", subsetOfLoans);
            Assert.AreEqual(totalMinimumPayment, paymentMade);

            // attempt to lock the loans
            // loans will only lock if their minimum payment was made
            privateSchedule.Invoke("LockPayments", subsetOfLoans);
        }


        /// <summary>
        /// Paying off a loan when paying it's minimum payment locks payment and removes it from this pay cycle's loan list
        /// </summary>
        [TestMethod]
        public void TestMinimumPaymentPaysOffLoan()
        {
            List<StudentLoan> listOfLoans = new List<StudentLoan>
            {
                UHelper.NewSafeLoan(testLenderName: "Loan 1", testPaymentStartDate: new DateTime(2016, 1, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 2", testPaymentStartDate: new DateTime(2017, 4, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 3", testPaymentStartDate: new DateTime(2018, 4, 1), testStartingPrinciple: 5m),
                UHelper.NewSafeLoan(testLenderName: "Loan 4", testPaymentStartDate: new DateTime(2021, 4, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 5", testPaymentStartDate: new DateTime(2022, 3, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 6", testPaymentStartDate: new DateTime(2023, 3, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 7", testPaymentStartDate: new DateTime(2024, 3, 1)),
            };
            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            // start a pay cycle by setting the current date
            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("currentPayDate", new DateTime(2018, 4, 1));

            // get the subset of loans for this pay cycle
            var _subsetOfLoans = privateSchedule.Invoke("GetThisPayCyclesLoans");
            List<StudentLoan> subsetOfLoans = (List<StudentLoan>)_subsetOfLoans;

            // unlock the subset of loans so interest can be calculted
            privateSchedule.Invoke("UnlockPayments", subsetOfLoans);

            // get the total minimum payment required for the subset of loans
            var totalMinimumPayment = privateSchedule.Invoke("GetMinimumPayments", subsetOfLoans);

            // make the minimum payments to subset of loans and verify payment amount
            var paymentMade = privateSchedule.Invoke("MakeMinimumPayments", subsetOfLoans);

            // the third loan should no longer be part of the subset of loans
            StudentLoan singleLoan = listOfLoans[2];
            Assert.IsFalse(subsetOfLoans.Contains(singleLoan));

            // verify the third loan is paid off and locked
            var privateLoan = new PrivateObject(singleLoan);

            // assert the loan is paid off
            var paidOff = privateLoan.GetProperty("PaidOff");
            Assert.AreEqual(true, paidOff);

            // get the loan's payment lock value and assert it is now locked
            var loanLock = privateLoan.GetField("paymentLock");
            Assert.AreEqual(StudentLoan.PaymentLock.PaymentsLocked, loanLock);
        }


        /// <summary>
        /// Determine this pay cycle's extra payment amount
        /// extra payment amount = moneypot - total minimum payments
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestDetermineExtraPaymentAmount()
        {
        }


        /// <summary>
        /// Determine which loan of the subset of loans has the highest APR
        /// If two loan's have the same APR, the lower principle loan is prioritized
        /// </summary>
        [TestMethod]
        public void TestFindHighestAPRInSubset()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            // the subset of loans only includes the first three loans
            List<StudentLoan> subsetOfLoans = new List<StudentLoan>()
            {
                UHelper.NewSafeLoan(testLenderName: "Loan 1", testAPR: 0.03m, testStartingPrinciple: 250m),
                UHelper.NewSafeLoan(testLenderName: "Loan 2", testAPR: 0.06m, testStartingPrinciple: 500m),
                UHelper.NewSafeLoan(testLenderName: "Loan 3", testAPR: 0.06m, testStartingPrinciple: 1000m),
            };

            // attempt to lock payments of only the subset of loans
            var privateSchedule = new PrivateObject(testSchedule);
            var highestAPRLoan = privateSchedule.Invoke("FindHighestAPRLoan", subsetOfLoans);

            Assert.AreSame(subsetOfLoans[1], highestAPRLoan);
        }


        /// <summary>
        /// Paying off a loan when making an extra payment locks payment and removes it from this pay cycle's loan list
        /// </summary>
        [TestMethod]
        public void TestExtraPaymentPaysOffLoan()
        {
            List<StudentLoan> listOfLoans = new List<StudentLoan>
            {
                UHelper.NewSafeLoan(testLenderName: "Loan 1", testPaymentStartDate: new DateTime(2016, 1, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 2", testPaymentStartDate: new DateTime(2017, 4, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 3", testPaymentStartDate: new DateTime(2018, 4, 1), testAPR: 0.06m, testStartingPrinciple: 15m),
                UHelper.NewSafeLoan(testLenderName: "Loan 4", testPaymentStartDate: new DateTime(2021, 4, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 5", testPaymentStartDate: new DateTime(2022, 3, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 6", testPaymentStartDate: new DateTime(2023, 3, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 7", testPaymentStartDate: new DateTime(2024, 3, 1)),
            };
            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            // start a pay cycle by setting the current date
            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("currentPayDate", new DateTime(2018, 4, 1));

            // get the subset of loans for this pay cycle
            var _subsetOfLoans = privateSchedule.Invoke("GetThisPayCyclesLoans");
            List<StudentLoan> subsetOfLoans = (List<StudentLoan>)_subsetOfLoans;

            // unlock the subset of loans so interest can be calculted
            privateSchedule.Invoke("UnlockPayments", subsetOfLoans);

            // get the total minimum payment required for the subset of loans
            var totalMinimumPayment = privateSchedule.Invoke("GetMinimumPayments", subsetOfLoans);

            // make the minimum payments to subset of loans and verify payment amount
            var paymentMade = privateSchedule.Invoke("MakeMinimumPayments", subsetOfLoans);

            // make extra payments to subset of loans from the moneypot
            // the highest APR loan (loan 3) should recieve the extra payment
            decimal moneypot = 5m;
            privateSchedule.Invoke("MakeExtraPayments", subsetOfLoans, moneypot);

            // the third loan should no longer be part of the subset of loans
            StudentLoan singleLoan = listOfLoans[2];
            Assert.IsFalse(subsetOfLoans.Contains(singleLoan));

            // verify the third loan is paid off and locked
            var privateLoan = new PrivateObject(singleLoan);

            // assert the loan is paid off
            var paidOff = privateLoan.GetProperty("PaidOff");
            Assert.AreEqual(true, paidOff);

            // get the loan's payment lock value and assert it is now locked
            var loanLock = privateLoan.GetField("paymentLock");
            Assert.AreEqual(StudentLoan.PaymentLock.PaymentsLocked, loanLock);
        }


        /// <summary>
        /// Apply extra payments to loans
        /// The first payment pays off an loan. The remaining moneypot is applied to the loan with the next highest APR
        /// </summary>
        [TestMethod]
        public void TestApplyExtraPayments()
        {
            List<StudentLoan> listOfLoans = new List<StudentLoan>
            {
                UHelper.NewSafeLoan(testLenderName: "Loan 1", testPaymentStartDate: new DateTime(2016, 1, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 2", testPaymentStartDate: new DateTime(2018, 4, 1), testAPR: 0.06m, testStartingPrinciple: 50m),
                UHelper.NewSafeLoan(testLenderName: "Loan 3", testPaymentStartDate: new DateTime(2018, 4, 1), testAPR: 0.06m, testStartingPrinciple: 15m),
                UHelper.NewSafeLoan(testLenderName: "Loan 4", testPaymentStartDate: new DateTime(2021, 4, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 5", testPaymentStartDate: new DateTime(2022, 3, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 6", testPaymentStartDate: new DateTime(2023, 3, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 7", testPaymentStartDate: new DateTime(2024, 3, 1)),
            };
            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            // start a pay cycle by setting the current date
            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("currentPayDate", new DateTime(2018, 4, 1));

            // get the subset of loans for this pay cycle
            var _subsetOfLoans = privateSchedule.Invoke("GetThisPayCyclesLoans");
            List<StudentLoan> subsetOfLoans = (List<StudentLoan>)_subsetOfLoans;

            // unlock the subset of loans so interest can be calculted
            privateSchedule.Invoke("UnlockPayments", subsetOfLoans);

            // get the total minimum payment required for the subset of loans
            var totalMinimumPayment = privateSchedule.Invoke("GetMinimumPayments", subsetOfLoans);

            // make the minimum payments to subset of loans and verify payment amount
            var paymentMade = privateSchedule.Invoke("MakeMinimumPayments", subsetOfLoans);

            // make extra payments to subset of loans from the moneypot
            // the highest APR loan (loan 3) should recieve the extra payment first, then loan two
            decimal moneypot = 15m;
            privateSchedule.Invoke("MakeExtraPayments", subsetOfLoans, moneypot);

            // the third loan should no longer be part of the subset of loans
            StudentLoan singleLoan = listOfLoans[2];
            Assert.IsFalse(subsetOfLoans.Contains(singleLoan));

            // the second loan should still be part of the subset of loans
            // the principle should be reduced by 10 min payment + 10 extra payment
            singleLoan = listOfLoans[1];
            Assert.IsTrue(subsetOfLoans.Contains(singleLoan));
            Assert.AreEqual(30m, singleLoan.Principle);
        }


        /// <summary>
        /// Paying off all the subset loans ends this pay cycle leaving the moneypot > 0
        /// </summary>
        [TestMethod]
        public void TestMoneypotNotFullyUsed()
        {
            List<StudentLoan> listOfLoans = new List<StudentLoan>
            {
                UHelper.NewSafeLoan(testLenderName: "Loan 1", testPaymentStartDate: new DateTime(2018, 4, 1), testAPR: 0.05m, testStartingPrinciple: 100m),
                UHelper.NewSafeLoan(testLenderName: "Loan 2", testPaymentStartDate: new DateTime(2018, 4, 1), testAPR: 0.06m, testStartingPrinciple: 50m),
                UHelper.NewSafeLoan(testLenderName: "Loan 3", testPaymentStartDate: new DateTime(2018, 4, 1), testAPR: 0.06m, testStartingPrinciple: 15m),
                UHelper.NewSafeLoan(testLenderName: "Loan 4", testPaymentStartDate: new DateTime(2021, 4, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 5", testPaymentStartDate: new DateTime(2022, 3, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 6", testPaymentStartDate: new DateTime(2023, 3, 1)),
                UHelper.NewSafeLoan(testLenderName: "Loan 7", testPaymentStartDate: new DateTime(2024, 3, 1)),
            };
            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, DummyPayments);

            // start a pay cycle by setting the current date
            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("currentPayDate", new DateTime(2018, 4, 1));

            // get the subset of loans for this pay cycle
            var _subsetOfLoans = privateSchedule.Invoke("GetThisPayCyclesLoans");
            List<StudentLoan> subsetOfLoans = (List<StudentLoan>)_subsetOfLoans;

            // unlock the subset of loans so interest can be calculted
            privateSchedule.Invoke("UnlockPayments", subsetOfLoans);

            // get the total minimum payment required for the subset of loans
            var totalMinimumPayment = privateSchedule.Invoke("GetMinimumPayments", subsetOfLoans);

            // make the minimum payments to subset of loans and verify payment amount
            var paymentMade = privateSchedule.Invoke("MakeMinimumPayments", subsetOfLoans);

            // make extra payments to subset of loans from the moneypot
            // this should more than pay off all loans in the subset
            // only 135 is needed to pay off all loans => 15 should remain in moneypot
            decimal moneypot = 150m;
            object[] args = new object[] { subsetOfLoans, moneypot };
            privateSchedule.Invoke("MakeExtraPayments", args);
            // Invoke handles ref parameters by passing by value and replacing the args array value back through the array (not the ref object in the array
            moneypot = (decimal)args[1];

            // the subset should not contain any loans
            Assert.AreEqual(0, subsetOfLoans.Count);

            // the moneypot should still have 15 remaining
            Assert.AreEqual(15m, moneypot);
        }


        /// <summary>
        /// Advance the "current" date ahead to the next payment date
        /// </summary>
        [TestMethod]
        public void TestAdvanceDateToNextPayCycle()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            List<ScheduledPayment> listOfPayments = new List<ScheduledPayment>
            {
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 4, 1), TotalPayment = 10m},
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 5, 1), TotalPayment = 10m},
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 6, 1), TotalPayment = 10m},
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 7, 1), TotalPayment = 10m},
            };

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, listOfPayments);

            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("paymentIndex", 1);
            privateSchedule.Invoke("AdvanceToNextPayCycleDate");

            var currentDate = privateSchedule.GetField("currentPayDate");

            Assert.AreEqual(new DateTime(2018, 6, 1), currentDate);
        }


        /// <summary>
        /// Advancing the current date when no future payments exist throws an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoanSchedule.ScheduledPaymentsException))]
        public void TestAdvanceDatePastEndOfScheduledPayments()
        {
            List<StudentLoan> listOfLoans = UHelper.NewSafeLoanList();
            List<ScheduledPayment> listOfPayments = new List<ScheduledPayment>
            {
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 4, 1), TotalPayment = 10m},
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 5, 1), TotalPayment = 10m},
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 6, 1), TotalPayment = 10m},
                new ScheduledPayment() { PaymentDate = new DateTime(2018, 7, 1), TotalPayment = 10m},
            };

            StudentLoanSchedule testSchedule = new StudentLoanSchedule(listOfLoans, listOfPayments);

            var privateSchedule = new PrivateObject(testSchedule);
            privateSchedule.SetField("paymentIndex", 3);

            try
            {
                privateSchedule.Invoke("AdvanceToNextPayCycleDate");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
            }
        }


        /// <summary>
        /// Accept an optional directory parameter to specify the csv file location
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestAcceptLogDirectoryOptionalArgument()
        {
        }


        /// <summary>
        /// Create a csv file that will record simple payment details
        /// Only check that the header are correct. No payments will be included
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestCreateSimplePaymentLogFile()
        {
        }


        /// <summary>
        /// Create a csv file that will record expanded payment details
        /// Only check that the header are correct. No payments will be included
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestCreateExpandedPaymentLogFile()
        {
        }


        /// <summary>
        /// Add a payment to the simple payment file
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestAddSimplePaymentToLogFile()
        {
        }

        /// <summary>
        /// Add a payment to the expanded payment file
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestAddExpandedPaymentToLogFile()
        {
        }

        /// <summary>
        /// Only loans whose last payment date == current date are recorded
        /// All other loans record 0 for all payment details
        /// </summary>
        [TestMethod]
        [Ignore]
        public void TestOnlyLogLoansWithPaymentsThisPayCycle()
        {
        }
    }

    /// <summary>
    /// Unit tests of the StudentLoan class
    /// </summary>
    [TestClass]
    public class StudentLoanClassUnitTests
    {
        // instance of unit test helper methods
        UTestHelper UHelper = new UTestHelper();

        #region Loan Details

        /// <summary>
        /// Student Loan Class has LenderName field
        /// </summary>
        [TestMethod]
        public void TestHasLenderNameProperty()
        {
            // Get the Type object corresponding to StudentLoan.
            Type studentLoanType = typeof(StudentLoan);
            // Get the PropertyInfo object by passing the property name.
            PropertyInfo studentLoanPropInfo = studentLoanType.GetProperty("LenderName");

            // Verify property exists
            Assert.AreNotEqual(null, studentLoanPropInfo, "LenderName property does not exist in StudentLoan Class");

            // Verify property has the correct type
            Assert.AreEqual(studentLoanPropInfo.PropertyType, typeof(String), "LenderName property is an incorrect type");
        }

        /// <summary>
        /// Student Loan Class has AccountNumber field
        /// </summary>
        [TestMethod]
        public void TestHasAccountNumberProperty()
        {
            // Get the Type object corresponding to StudentLoan.
            Type studentLoanType = typeof(StudentLoan);
            // Get the PropertyInfo object by passing the property name.
            PropertyInfo studentLoanPropInfo = studentLoanType.GetProperty("AccountNumber");

            // Verify property exists
            Assert.AreNotEqual(null, studentLoanPropInfo, "AccountNumber property does not exist in StudentLoan Class");

            // Verify property has the correct type
            Assert.AreEqual(studentLoanPropInfo.PropertyType, typeof(String), "AccountNumber property is an incorrect type");
        }

        /// <summary>
        /// Student Loan Class has APR field
        /// </summary>
        [TestMethod]
        public void TestHasAPRProperty()
        {
            // Get the Type object corresponding to StudentLoan.
            Type studentLoanType = typeof(StudentLoan);
            // Get the PropertyInfo object by passing the property name.
            PropertyInfo studentLoanPropInfo = studentLoanType.GetProperty("APR");

            // Verify property exists
            Assert.AreNotEqual(null, studentLoanPropInfo, "APR property does not exist in StudentLoan Class");

            // Verify property has the correct type
            Assert.AreEqual(studentLoanPropInfo.PropertyType, typeof(decimal), "APR property is an incorrect type");
        }

        /// <summary>
        /// APR cannot be greater than 100%
        /// While this is not technically incorrect, it likely occured because the APR was a percentage instead of decimal (3.25% is 0.0325 not 3.25)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.APROutOfRangeException))]
        public void TestAPROutOfRange()
        {
            StudentLoan aPROutOfRangeLoan = UHelper.NewSafeLoan(testAPR: 1.0325m);
        }

        /// <summary>
        /// Student Loan Class has MinPayment field
        /// </summary>
        [TestMethod]
        public void TestHasMinPaymentProperty()
        {
            // Get the Type object corresponding to StudentLoan.
            Type studentLoanType = typeof(StudentLoan);
            // Get the PropertyInfo object by passing the property name.
            PropertyInfo studentLoanPropInfo = studentLoanType.GetProperty("MinPayment");

            // Verify property exists
            Assert.AreNotEqual(null, studentLoanPropInfo, "MinPayment property does not exist in StudentLoan Class");

            // Verify property has the correct type
            Assert.AreEqual(studentLoanPropInfo.PropertyType, typeof(decimal), "MinPayment property is an incorrect type");
        }

        /// <summary>
        /// Student Loan Class has PaymentStartDate field
        /// </summary>
        [TestMethod]
        public void TestHasPaymentStartDateProperty()
        {
            // Get the Type object corresponding to StudentLoan.
            Type studentLoanType = typeof(StudentLoan);
            // Get the PropertyInfo object by passing the property name.
            PropertyInfo studentLoanPropInfo = studentLoanType.GetProperty("PaymentStartDate");

            // Verify property exists
            Assert.AreNotEqual(null, studentLoanPropInfo, "PaymentStartDate property does not exist in StudentLoan Class");

            // Verify property has the correct type
            Assert.AreEqual(studentLoanPropInfo.PropertyType, typeof(DateTime), "PaymentStartDate property is an incorrect type");
        }

        /// <summary>
        /// Student Loan Class has Principle field
        /// </summary>
        [TestMethod]
        public void TestHasPrincipleProperty()
        {
            // Get the Type object corresponding to StudentLoan.
            Type studentLoanType = typeof(StudentLoan);
            // Get the PropertyInfo object by passing the property name.
            PropertyInfo studentLoanPropInfo = studentLoanType.GetProperty("Principle");

            // Verify property exists
            Assert.AreNotEqual(null, studentLoanPropInfo, "Principle property does not exist in StudentLoan Class");

            // Verify property has the correct type
            Assert.AreEqual(studentLoanPropInfo.PropertyType, typeof(decimal), "Principle property is an incorrect type");
        }

        #endregion

        /// <summary>
        /// Student Loan Class consturcor is called without throwing an exception
        /// The constuctor values are read back out of the new object for correctness
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            String testLenderName = "testName";
            String testAccountNumber = "ABC-123456";
            decimal testAPR = 0.0325m;
            decimal testMinPayment = 25.73m;
            DateTime testPaymentStartDate = DateTime.Now.Date;
            decimal testStartingPrinciple = 4001.00m;

            StudentLoan testStudentLoan = new StudentLoan(testLenderName,
                                                          testAccountNumber,
                                                          testAPR,
                                                          testMinPayment,
                                                          testPaymentStartDate,
                                                          testStartingPrinciple
                                                          );

            Assert.AreEqual(testLenderName, testStudentLoan.LenderName);
            Assert.AreEqual(testAccountNumber, testStudentLoan.AccountNumber);
            Assert.AreEqual(testAPR, testStudentLoan.APR);
            Assert.AreEqual(testMinPayment, testStudentLoan.MinPayment);
            Assert.AreEqual(testPaymentStartDate, testStudentLoan.PaymentStartDate);
            Assert.AreEqual(testStartingPrinciple, testStudentLoan.Principle);
        }

        #region Make a Payment

        /// <summary>
        /// Last payment information can be retreived
        /// </summary>
        [TestMethod]
        public void TestHasLastPaymentObject()
        {
            // Get the Type object corresponding to StudentLoan.
            Type studentLoanType = typeof(StudentLoan);
            // Get the PropertyInfo object by passing the property name.
            PropertyInfo studentLoanPropInfo = studentLoanType.GetProperty("LastPayment");

            // Verify property exists
            Assert.AreNotEqual(null, studentLoanPropInfo, "LastPayment property does not exist in StudentLoan Class");
        }

        /// <summary>
        /// The last payment components need to specified the total payment, interest paid, and principle paid
        /// </summary>
        [TestMethod]
        public void TestHasLastPaymentProperties()
        {
            // Get the Type object corresponding to StudentLoan.
            Type studentLoanType = typeof(StudentLoan.LastPaymentDetails);
            // Get the PropertyInfo object by passing the property name and verify property exists
            PropertyInfo lastPaymentPropInfo;
            lastPaymentPropInfo = studentLoanType.GetProperty("TotalPayment");
            Assert.AreNotEqual(null, lastPaymentPropInfo, "TotalPayment property does not exist in LastPaymentDetails Class");

            lastPaymentPropInfo = studentLoanType.GetProperty("InterestPayment");
            Assert.AreNotEqual(null, lastPaymentPropInfo, "InterestPayment property does not exist in LastPaymentDetails Class");

            lastPaymentPropInfo = studentLoanType.GetProperty("PrinciplePayment");
            Assert.AreNotEqual(null, lastPaymentPropInfo, "PrinciplePayment property does not exist in LastPaymentDetails Class");

            lastPaymentPropInfo = studentLoanType.GetProperty("PaymentDate");
            Assert.AreNotEqual(null, lastPaymentPropInfo, "PaymentDate property does not exist in LastPaymentDetails Class");
        }

        /// <summary>
        /// The private function calculates daily interest correctly
        /// </summary>
        [TestMethod]
        public void TestCalculateInterest()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan(testPaymentStartDate: DateTime.Now);
            var privateObject = new PrivateObject(testPaymentLoan);

            var interest = privateObject.Invoke("CalcInterest", DateTime.Now.AddDays(73));

            Assert.AreEqual(10m, interest);
        }

        /// <summary>
        /// Accrued interest is based on day boundries, not 24 hr days
        /// </summary>
        [TestMethod]
        public void TestInterestBasedOnDayBoundries()
        {
            // payment start date is 1 hour from midnight (day boundry)
            // principle and interest rate are very high so rounding does not hide behavior
            DateTime loanStartDate = new DateTime(2018, 7, 1, 23, 0, 0);
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan(testPaymentStartDate: loanStartDate, testStartingPrinciple: 100000m, testAPR: 0.50m);

            var privateObject = new PrivateObject(testPaymentLoan);

            // payment date is 1am on the 73rd day => accrued interest should be 10000
            // note: this is less than 73x 24hr days later. The interest is correct only if it's based on calendar days not 24hr days
            var interest = privateObject.Invoke("CalcInterest", loanStartDate.AddDays(72).AddHours(2));

            Assert.AreEqual(10000m, interest);
        }

        /// <summary>
        /// Payments applied to StudentLoan reduces the principle correctly
        /// Every 73 days, the accrued interest results in an integer. This makes the principle reduction math easier.
        /// </summary>
        [TestMethod]
        public void TestPaymentReducesPrinciple()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            // unlock payments first
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73));

            // expected accrued interest = 10.0
            // 30.0 payment - 10.0 interest = 20.0 principle reduction
            // 1000.0 starting principle - 20.0 principle reduction = 980.0 remaining principle
            UHelper.MakeLoanPayment(testPaymentLoan, 30.0m, expectedPrinciple: 980.0m);
        }

        /// <summary>
        /// Payments applied to StudentLoan pays interest first
        /// </summary>
        [TestMethod]
        public void TestPaymentReducesAccruedInterestFirst()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            // unlock payments first
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73));

            // pay less than total accrued interest (expected to be 10.0)
            // only interest should be reduced, the principle should remain unchanged
            UHelper.MakeLoanPayment(testPaymentLoan, 4.0m, expectedPrinciple: 1000.0m);
        }

        /// <summary>
        /// Payments reduce the principle after interest paid
        /// </summary>
        [TestMethod]
        public void TestPaymentReducesPrincipleAfterInterest()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            // unlock payments first
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73));

            // expected accrued interest = 10.0
            // 30.0 payment - 10.0 interest = 20.0 principle reduction
            // 1000.0 starting principle - 20.0 principle reduction = 980.0 remaining principle
            UHelper.MakeLoanPayment(testPaymentLoan, 30.0m, expectedPrinciple: 980.0m);
        }

        /// <summary>
        /// Payments reduce the principle after interest paid
        /// </summary>
        [TestMethod]
        public void TestExtraPaymentFurtherReducesPrinciple()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            // unlock payments first
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73));

            // expected accrued interest = 10.0
            // 30.0 payment - 10.0 interest = 20.0 principle reduction
            // 1000.0 starting principle - 20.0 principle reduction = 980.0 remaining principle
            UHelper.MakeLoanPayment(testPaymentLoan, 30.0m, expectedPrinciple: 980.0m);

            // apply an extra payment after interest paid
            // expected accrued interest = 0.0
            // 30.0 payment - 0.0 interest = 30.0 principle reduction
            // 980.0 starting principle - 30.0 principle reduction = 950.0 remaining principle
            UHelper.MakeLoanPayment(testPaymentLoan, 30.0m, expectedPrinciple: 950.0m);
        }

        /// <summary>
        /// Payments are detailed in LastPayment
        /// </summary>
        [TestMethod]
        public void TestPaymentDetails()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            // unlock payments first
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73));

            // expected accrued interest = 10.0
            // 30.0 payment - 10.0 interest = 20.0 principle reduction
            // 1000.0 starting principle - 20.0 principle reduction = 980.0 remaining principle
            UHelper.MakeLoanPayment(testPaymentLoan, 30.0m, expectedPrinciple: 980.0m, expectedTotalPayment: 30.0m, expectedInterestPayment: 10.0m, expectedPrinciplePayment: 20.0m);

            // expected accrued interest = 0.0
            // 30.0 payment - 0.0 interest = 30.0 principle reduction
            // 980.0 starting principle - 30.0 principle reduction = 950.0 remaining principle
            UHelper.MakeLoanPayment(testPaymentLoan, 30.0m, expectedPrinciple: 950.0m, expectedTotalPayment: 60.0m, expectedInterestPayment: 10.0m, expectedPrinciplePayment: 50.0m);
        }

        /// <summary>
        /// Payments can only be made when payments are unlocked
        /// Payment attempts when payments not unlocked returns exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.PaymentsLockException))]
        public void TestLockedPayment()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            // apply an abitrary payment without unlocking payment
            testPaymentLoan.MakePayment(30.0m);
        }

        /// <summary>
        /// Unlocking payments should set the last payment date to the date parameter passed
        /// </summary>
        [TestMethod]
        public void TestUnlockPaymentSetsLastPaymentDate()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            DateTime newPaymentDate = DateTime.Now.AddDays(73);
            testPaymentLoan.UnlockPayments(newPaymentDate); // unlock payment (sucessful)

            // new payment date within 1 day of date parameter
            Assert.IsTrue(0 == (newPaymentDate - testPaymentLoan.LastPayment.PaymentDate).Days);
        }

        /// <summary>
        /// Attempting to unlock payments while payments already unlocked results in an exception
        /// The payment lock triggers setup/closeout actions when transitioning. If a redundant unlock attempt is made, there is either an error in the calling function or the developer does not understand the intended payment flow
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.PaymentsLockException))]
        public void TestRedundantUnlockPayment()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73)); // unlock payment (sucessful)
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73)); // unlock payment (exception)
        }

        /// <summary>
        /// Attempting to lock payments while payments already locked results in an exception
        /// The payment lock triggers setup/closeout actions when transitioning. If a redundant lock attempt is made, there is either an error in the calling function or the developer does not understand the intended payment flow
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.PaymentsLockException))]
        public void TestRedundantLockPayment()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            testPaymentLoan.LockPayments(); // unlock payment (sucessful)
            testPaymentLoan.LockPayments(); // unlock payment (exception)
        }

        /// <summary>
        /// Locking payments verifies the mimimum payment was made this month
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.PaymentException))]
        public void TestMinimumPaymentMade()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73)); // first unlock payments

            // immediately lock payments again leaving the last total payment == 0
            testPaymentLoan.LockPayments();
        }

        /// <summary>
        /// Locking payments does not clear the last payment details, but unlocking does clear details.
        /// </summary>
        [TestMethod]
        public void TestPreserveAndClearPaymentDetails()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();

            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73)); // first unlock payments

            // make at least the minimum payment so payments can be locked again
            // expected accrued interest = 10.0
            // 30.0 payment - 10.0 interest = 20.0 principle reduction
            // 1000.0 starting principle - 20.0 principle reduction = 980.0 remaining principle
            UHelper.MakeLoanPayment(testPaymentLoan, 30.0m);

            testPaymentLoan.LockPayments();

            UHelper.CheckLastPaymentDetails(testPaymentLoan, expectedPrinciple: 980.0m, expectedTotalPayment: 30.0m, expectedInterestPayment: 10.0m, expectedPrinciplePayment: 20.0m);

            // unlocking payments should clear the last payment details without affecting the principle
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(146));

            UHelper.CheckLastPaymentDetails(testPaymentLoan, expectedPrinciple: 980.0m, expectedTotalPayment: 0m, expectedInterestPayment: 0m, expectedPrinciplePayment: 0m);
        }

        /// <summary>
        /// Attempting to get the payoff amount when payment is locked throws an exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.PaymentsLockException))]
        public void TestLockedPayoffAmount()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();
            // assume new loan's payments are locked 
            decimal payoffAmount = testPaymentLoan.PayoffAmount;
        }

        /// <summary>
        /// The loan is able return a payment amount that would pay off the loan (principle + interest)
        /// </summary>
        [TestMethod]
        public void TestPayoffAmount()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73)); // first unlock payments

            // expected accrued interest = 10.0
            // payoff amount = 1000.0 starting principle + 10.0 interest
            Assert.AreEqual(1010.0m, testPaymentLoan.PayoffAmount);
        }

        /// <summary>
        /// Payments which reduce the principle less than 0 throw an exception
        /// This also covers the case of making a payment on a loan that's paid off
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.PaymentException))]
        public void TestCannotOverpayLoan()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73)); // first unlock payments
            // make a payment significantly greater than the loan principle
            UHelper.MakeLoanPayment(testPaymentLoan, 100000.0m);
        }

        /// <summary>
        /// Reducing the principle to 0 marks the loan paid off
        /// </summary>
        [TestMethod]
        public void TestPayOffLoan()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73)); // first unlock payments

            // verify the loan is not paid off yet
            Assert.AreEqual(false, testPaymentLoan.PaidOff);

            // apply a payment that pays off the loan
            UHelper.MakeLoanPayment(testPaymentLoan, testPaymentLoan.PayoffAmount);

            // verify the loan is now paid off
            Assert.AreEqual(true, testPaymentLoan.PaidOff);
            UHelper.CheckLastPaymentDetails(testPaymentLoan, expectedPrinciple: 0m);
        }

        /// <summary>
        /// Locking payment on a paid off loan when the total payment is < minimum payment does not throw an exception
        /// </summary>
        /// <remarks>
        /// It is not necessary to lock a paid off loan. Payment attempts after loan is paid off result in an exception
        /// </remarks>
        [TestMethod]
        public void TestLockPaidOffLoan()
        {
            StudentLoan testPaymentLoan = UHelper.NewPaymentLoan();
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73)); // first unlock payments

            // apply a payment that leaves 1 penny in the loan
            UHelper.MakeLoanPayment(testPaymentLoan, (testPaymentLoan.PayoffAmount - 0.01m));

            testPaymentLoan.LockPayments();

            // unlock the loan again 2 days later
            // the accrued interest on 1 penny is negligible
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(75)); // first unlock payments

            // apply a payment that pays off the loan and verify it's paid off
            UHelper.MakeLoanPayment(testPaymentLoan, testPaymentLoan.PayoffAmount);
            Assert.AreEqual(true, testPaymentLoan.PaidOff);

            testPaymentLoan.LockPayments();
        }

        /// <summary>
        /// Loan is able to signal if start date has been reached and the loan needs to start being repaid
        /// </summary>
        [TestMethod]
        public void TestLoanInRepayment()
        {
            StudentLoan testStudentLoan = UHelper.NewSafeLoan(testPaymentStartDate: DateTime.Now.AddYears(1));

            Assert.AreEqual(false, testStudentLoan.InRepayment(DateTime.Now));
            Assert.AreEqual(true, testStudentLoan.InRepayment(DateTime.Now.AddYears(2)));
        }

        #endregion

    }

    public class UTestHelper
    {

        /// <summary>
        /// Sucessfully creates a StudentLoan object with safe values
        /// This allows tests to isolate manipulated values which intentionally trigger an exception
        /// </summary>
        public StudentLoan NewSafeLoan(String testLenderName = "Safe Loan",
                                    String testAccountNumber = "TEST-SAFE",
                                    decimal testAPR = 0.0325m,
                                    decimal testMinPayment = 10.0m,
                                    DateTime? testPaymentStartDate = null,
                                    decimal testStartingPrinciple = 50.0m
                                    )
        {
            DateTime testPaymentStartDateParam;

            if (testPaymentStartDate == null)
            {
                testPaymentStartDateParam = DateTime.Now;
            }
            else
            {
                testPaymentStartDateParam = (DateTime)testPaymentStartDate;
            }

            StudentLoan safeLoan = new StudentLoan(testLenderName,
                                                   testAccountNumber,
                                                   testAPR,
                                                   testMinPayment,
                                                   testPaymentStartDateParam,
                                                   testStartingPrinciple
                                                   );

            return safeLoan;
        }

        /// <summary>
        /// Sucessfully creates a list of StudentLoan objects with safe values
        /// </summary>
        public List<StudentLoan> NewSafeLoanList()
        {
            List<StudentLoan> listOfLoans = new List<StudentLoan>
            {
                NewSafeLoan(testLenderName: "Loan 1", testPaymentStartDate: new DateTime(2016, 1, 1)),
                NewSafeLoan(testLenderName: "Loan 2", testPaymentStartDate: new DateTime(2017, 4, 1)),
                NewSafeLoan(testLenderName: "Loan 3", testPaymentStartDate: new DateTime(2018, 4, 1)),
                NewSafeLoan(testLenderName: "Loan 4", testPaymentStartDate: new DateTime(2021, 4, 1)),
                NewSafeLoan(testLenderName: "Loan 5", testPaymentStartDate: new DateTime(2022, 3, 1)),
                NewSafeLoan(testLenderName: "Loan 6", testPaymentStartDate: new DateTime(2023, 3, 1)),
                NewSafeLoan(testLenderName: "Loan 7", testPaymentStartDate: new DateTime(2024, 3, 1)),
            };

            return listOfLoans;
        }

        /// <summary>
        /// Sucessfully creates a StudentLoan object with values that are easy to work with when testing loan payments
        /// </summary>
        public StudentLoan NewPaymentLoan(String testLenderName = "Payment Loan",
                                           String testAccountNumber = "TEST-454590",
                                           decimal testAPR = 0.05m,
                                           decimal testMinPayment = 10.61m,
                                           DateTime? testPaymentStartDate = null,
                                           decimal testStartingPrinciple = 1000.00m
                                          )
        {
            DateTime testPaymentStartDateParam;

            if (testPaymentStartDate == null)
            {
                testPaymentStartDateParam = DateTime.Now;
            }
            else
            {
                testPaymentStartDateParam = (DateTime)testPaymentStartDate;
            }

            StudentLoan paymentLoan = new StudentLoan(testLenderName,
                                                          testAccountNumber,
                                                          testAPR,
                                                          testMinPayment,
                                                          testPaymentStartDateParam,
                                                          testStartingPrinciple
                                                          );


            return paymentLoan;
        }

        public void MakeLoanPayment(StudentLoan loan, decimal payment, decimal expectedPrinciple = -1m, decimal expectedTotalPayment = -1m, decimal expectedInterestPayment = -1m, decimal expectedPrinciplePayment = -1m)
        {
            loan.MakePayment(payment);

            CheckLastPaymentDetails(loan, expectedPrinciple, expectedTotalPayment, expectedInterestPayment, expectedPrinciplePayment);
        }

        public void CheckLastPaymentDetails(StudentLoan loan, decimal expectedPrinciple = -1m, decimal expectedTotalPayment = -1m, decimal expectedInterestPayment = -1m, decimal expectedPrinciplePayment = -1m)
        {
            // check payment was handled correctly
            if (expectedPrinciple != -1m)
            {
                Assert.AreEqual(expectedPrinciple, loan.Principle);
            }

            if (expectedTotalPayment != -1m)
            {
                Assert.AreEqual(expectedTotalPayment, loan.LastPayment.TotalPayment);
            }

            if (expectedInterestPayment != -1m)
            {
                Assert.AreEqual(expectedInterestPayment, loan.LastPayment.InterestPayment);
            }

            if (expectedPrinciplePayment != -1m)
            {
                Assert.AreEqual(expectedPrinciplePayment, loan.LastPayment.PrinciplePayment);
            }
        }
    }
}
