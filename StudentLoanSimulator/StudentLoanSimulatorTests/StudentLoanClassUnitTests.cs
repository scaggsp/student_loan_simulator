using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StudentLoanSimulator;

namespace StudentLoanSimulatorTests
{
    [TestClass]
    public class StudentLoanClassUnitTests
    {
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
            StudentLoan aPROutOfRangeLoan = NewSafeLoan(DateTime.Now, testAPR: 1.0325m);
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
            DateTime testPaymentStartDate = DateTime.Now;
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
        /// 1 Month's interest is correctly calculated for a loan
        /// The expected accrued interest value was detmined using an online interest calculator
        /// </summary>
        /// <remarks>
        /// This test will roll into the make payment test. The interest should not be accessible to an outside caller, but the interest portion of a payment is.
        /// </remarks>
        [TestMethod]
        public void TestCalculateInterest()
        {
            StudentLoan testPaymentLoan = NewPaymentLoan();

            decimal testInterest = testPaymentLoan.CalcInterest(DateTime.Now.AddDays(30));
            Assert.AreEqual(4.11m, testInterest);

            testInterest = testPaymentLoan.CalcInterest(DateTime.Now.AddDays(60));
            Assert.AreEqual(8.22m, testInterest);
        }

        /// <summary>
        /// Payments applied to StudentLoan reduces the principle correctly
        /// Every 73 days, the accrued interest results in an integer. This makes the principle reduction math easier.
        /// </summary>
        [TestMethod]
        public void TestPaymentReducesPrinciple()
        {
            StudentLoan testPaymentLoan = NewPaymentLoan();

            // unlock payments first
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73));

            testPaymentLoan.MakePayment(30.0m);
            // expected accrued interest = 10.0
            // 30.0 payment - 10.0 interest = 20.0 principle reduction
            // 1000.0 starting principle - 20.0 principle reduction = 980.0 remaining principle
            Assert.AreEqual(980.0m, testPaymentLoan.Principle);
        }

        /// <summary>
        /// Payments applied to StudentLoan pays interest first
        /// </summary>
        [TestMethod]
        public void TestPaymentReducesAccruedInterestFirst()
        {
            StudentLoan testPaymentLoan = NewPaymentLoan();

            // unlock payments first
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73));
            Assert.AreEqual(10.0m, testPaymentLoan.AccruedInterest);

            testPaymentLoan.MakePayment(4.0m);
            // pay less than total accrued interest
            // only interest should be reduced, the principle should remain unchanged
            Assert.AreEqual(1000.0m, testPaymentLoan.Principle);
            Assert.AreEqual(6.0m, testPaymentLoan.AccruedInterest);
        }

        /// <summary>
        /// Payments reduce the principle after interest paid
        /// </summary>
        [TestMethod]
        public void TestPaymentReducesPrincipleAfterInterest()
        {
            StudentLoan testPaymentLoan = NewPaymentLoan();

            // unlock payments first
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73));

            testPaymentLoan.MakePayment(30.0m);
            // expected accrued interest = 10.0
            // 30.0 payment - 10.0 interest = 20.0 principle reduction
            // 1000.0 starting principle - 20.0 principle reduction = 980.0 remaining principle
            Assert.AreEqual(980.0m, testPaymentLoan.Principle);
            Assert.AreEqual(0.0m, testPaymentLoan.AccruedInterest);
        }

        /// <summary>
        /// Payments reduce the principle after interest paid
        /// </summary>
        [TestMethod]
        public void TestExtraPaymentFurtherReducesPrinciple()
        {
            StudentLoan testPaymentLoan = NewPaymentLoan();

            // unlock payments first
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73));

            testPaymentLoan.MakePayment(30.0m);
            // expected accrued interest = 10.0
            // 30.0 payment - 10.0 interest = 20.0 principle reduction
            // 1000.0 starting principle - 20.0 principle reduction = 980.0 remaining principle
            Assert.AreEqual(980.0m, testPaymentLoan.Principle);
            Assert.AreEqual(0.0m, testPaymentLoan.AccruedInterest);

            // apply an extra payment after interest paid
            testPaymentLoan.MakePayment(30.0m);
            // expected accrued interest = 0.0
            // 30.0 payment - 0.0 interest = 30.0 principle reduction
            // 980.0 starting principle - 30.0 principle reduction = 950.0 remaining principle
            Assert.AreEqual(950.0m, testPaymentLoan.Principle);
            Assert.AreEqual(0.0m, testPaymentLoan.AccruedInterest);
        }

        /// <summary>
        /// Payments are detailed in LastPayment
        /// </summary>
        [TestMethod]
        public void TestPaymentDetails()
        {
            StudentLoan testPaymentLoan = NewPaymentLoan();

            // unlock payments first
            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73));

            testPaymentLoan.MakePayment(30.0m);
            // expected accrued interest = 10.0
            // 30.0 payment - 10.0 interest = 20.0 principle reduction
            // 1000.0 starting principle - 20.0 principle reduction = 980.0 remaining principle
            Assert.AreEqual(980.0m, testPaymentLoan.Principle);
            Assert.AreEqual(0.0m, testPaymentLoan.AccruedInterest);

            Assert.AreEqual(30.0m, testPaymentLoan.LastPayment.TotalPayment);
            Assert.AreEqual(10.0m, testPaymentLoan.LastPayment.InterestPayment);
            Assert.AreEqual(20.0m, testPaymentLoan.LastPayment.PrinciplePayment);

            // apply an extra payment after interest paid
            testPaymentLoan.MakePayment(30.0m);
            // expected accrued interest = 0.0
            // 30.0 payment - 0.0 interest = 30.0 principle reduction
            // 980.0 starting principle - 30.0 principle reduction = 950.0 remaining principle
            Assert.AreEqual(950.0m, testPaymentLoan.Principle);
            Assert.AreEqual(0.0m, testPaymentLoan.AccruedInterest);

            Assert.AreEqual(60.0m, testPaymentLoan.LastPayment.TotalPayment);
            Assert.AreEqual(10.0m, testPaymentLoan.LastPayment.InterestPayment);
            Assert.AreEqual(50.0m, testPaymentLoan.LastPayment.PrinciplePayment);
        }

        /// <summary>
        /// Payments can only be made when payments are unlocked
        /// Payment attempts when payments not unlocked returns exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.PaymentsLockException))]
        public void TestLockedPayment()
        {
            StudentLoan testPaymentLoan = NewPaymentLoan();

            // apply an abitrary payment without unlocking payment
            testPaymentLoan.MakePayment(30.0m);
        }

        /// <summary>
        /// Attempting to unlock payments while payments already unlocked results in an exception
        /// The payment lock triggers setup/closeout actions when transitioning. If a redundant unlock attempt is made, there is either an error in the calling function or the developer does not understand the intended payment flow
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.PaymentsLockException))]
        public void TestRedundantUnlockPayment()
        {
            StudentLoan testPaymentLoan = NewPaymentLoan();

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
            StudentLoan testPaymentLoan = NewPaymentLoan();

            testPaymentLoan.LockPayments(); // unlock payment (sucessful)
            testPaymentLoan.LockPayments(); // unlock payment (exception)
        }

        /// <summary>
        /// Unlocking payments calculates the accrued intest since the last payment
        /// </summary>
        /// <remarks>
        /// This is a temporary test to verify unlock functionality. Future testing to ingetegrate this test by checking a payment's details i.e. accrued interest was calculated correctly by checking the last payment interest component.
        /// </remarks>
        [TestMethod]
        public void TestAccrueInterest()
        {
            StudentLoan testPaymentLoan = NewPaymentLoan();

            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73)); // first unlock payments
            Assert.AreEqual(10.0m, testPaymentLoan.AccruedInterest);
        }

        /// <summary>
        /// Locking payments verifies the mimimum payment was made this month
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.PaymentException))]
        public void TestMinimumPaymentMade()
        {
            StudentLoan testPaymentLoan = NewPaymentLoan();

            testPaymentLoan.UnlockPayments(DateTime.Now.AddDays(73)); // first unlock payments

            // immediately lock payments again leaving the last total payment == 0
            testPaymentLoan.LockPayments();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Sucessfully creates a StudentLoan object with safe values
        /// This allows tests to isolate manipulated values which intentionally trigger an exception
        /// </summary>
        private StudentLoan NewSafeLoan(DateTime testPaymentStartDate,
                                    String testLenderName = "Safe Loan",
                                    String testAccountNumber = "TEST-SAFE",
                                    decimal testAPR = 0.0325m,
                                    decimal testMinPayment = 10.0m,
                                    decimal testStartingPrinciple = 50.0m
                                    )
        {
            StudentLoan safeLoan = new StudentLoan(testLenderName,
                                                   testAccountNumber,
                                                   testAPR,
                                                   testMinPayment,
                                                   testPaymentStartDate,
                                                   testStartingPrinciple
                                                   );

            return safeLoan;
        }

        /// <summary>
        /// Sucessfully creates a StudentLoan object with values that are easy to work with when testing loan payments
        /// </summary>
        private StudentLoan NewPaymentLoan()
        {
            String testLenderName = "Payment Loan";
            String testAccountNumber = "TEST-454590";
            decimal testAPR = 0.05m;
            decimal testMinPayment = 10.61m;
            DateTime testPaymentStartDate = DateTime.Now;
            decimal testStartingPrinciple = 1000.00m;

            StudentLoan paymentLoan = new StudentLoan(testLenderName,
                                                          testAccountNumber,
                                                          testAPR,
                                                          testMinPayment,
                                                          testPaymentStartDate,
                                                          testStartingPrinciple
                                                          );


            return paymentLoan;
        }
        #endregion
    }
}
