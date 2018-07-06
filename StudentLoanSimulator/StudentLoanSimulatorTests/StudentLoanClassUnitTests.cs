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
            Assert.AreEqual(studentLoanPropInfo.PropertyType, typeof(double), "APR property is an incorrect type");
        }

        /// <summary>
        /// APR cannot be greater than 100%
        /// While this is not technically incorrect, it likely occured because the APR was a percentage instead of decimal (3.25% is 0.0325 not 3.25)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StudentLoan.APROutOfRange))]
        public void TestAPROutOfRange()
        {
            StudentLoan aPROutOfRangeLoan = NewSafeLoan(DateTime.Now, testAPR: 1.0325);
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
            Assert.AreEqual(studentLoanPropInfo.PropertyType, typeof(double), "MinPayment property is an incorrect type");
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
            Assert.AreEqual(studentLoanPropInfo.PropertyType, typeof(double), "Principle property is an incorrect type");
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
            double testAPR = 0.0325;
            double testMinPayment = 25.73;
            DateTime testPaymentStartDate = DateTime.Now;
            double testStartingPrinciple = 4001.00;

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
        /// The payment components need to specified the total payment, interest paid, and principle paid
        /// </summary>


        /// <summary>
        /// 1 Month's interest is correctly calculated for a loan
        /// The expected accrued interest value was detmined using an online interest calculator
        /// </summary>


        /// <summary>
        /// 1 Month's interest is correctly calculated for a loan
        /// The expected accrued interest value was detmined using an online interest calculator
        /// </summary>


        /// <summary>
        /// Payments applied to StudentLoan reduces the principle correctly
        /// </summary>

        /// <summary>
        /// Payments can only be made when payments are unlocked
        /// Payment attempts when payments not unlocked returns exception
        /// </summary>

        /// <summary>
        /// Locking payments verifies the mimimum payment was made this month
        /// </summary>

        #endregion

        #region Helper Methods

        /// <summary>
        /// Sucessfully creates a StudentLoan object with safe values
        /// This allows tests to isolate manipulated values which intentionally trigger an exception
        /// </summary>
        private StudentLoan NewSafeLoan(DateTime testPaymentStartDate,
                                        String testLenderName = "Safe Loan",
                                        String testAccountNumber = "TEST-SAFE",
                                        double testAPR = 0.0325,
                                        double testMinPayment = 10.0,
                                        double testStartingPrinciple = 50.0
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

        #endregion
    }
}
