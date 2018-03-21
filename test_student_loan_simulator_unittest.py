#!/usr/bin/env python
"""This script performs unit testing on the student_loan script."""

import unittest
from student_loan import StudentLoan, OverpaymentException, UnderpaymentException

DUMMY_PRINCIPLE = 0
DUMMY_APR = 0
DUMMY_MIN_PAY = 0

class TestStudentLoanClass(unittest.TestCase):
    """Unit test the StudentLoan class."""

    def setUp(self):
        pass

    def test_loan_class_create(self):
        """Create a loan class instance."""
        loan = StudentLoan(DUMMY_PRINCIPLE, DUMMY_APR, DUMMY_MIN_PAY)
        self.assertIsInstance(loan, StudentLoan)

    def test_loan_has_principle_var(self):
        """StudentLoan class has a principal variable."""
        loan = StudentLoan(DUMMY_PRINCIPLE, DUMMY_APR, DUMMY_MIN_PAY)
        self.assertTrue(hasattr(loan, 'principle'))

    def test_loan_class_init_principle(self):
        """StudentLoan class initializes principal variable."""
        princ = 10000  # starting principle
        loan = StudentLoan(princ, DUMMY_APR, DUMMY_MIN_PAY)
        self.assertEqual(loan.principle, princ)

    def test_loan_class_has_apr_var(self):
        """StudentLoan class has a apr variable."""
        loan = StudentLoan(DUMMY_PRINCIPLE, DUMMY_APR, DUMMY_MIN_PAY)
        self.assertTrue(hasattr(loan, 'apr'))

    def test_loan_class_init_apr(self):
        """StudentLoan class initializes apr variable."""
        apr = 12  # Annual interest rate (percentage)
        loan = StudentLoan(DUMMY_PRINCIPLE, apr, DUMMY_MIN_PAY)
        self.assertEqual(loan.apr, apr)

    def test_loan_class_has_min_pay_var(self):
        """StudentLoan class has a minimum_payment variable."""
        loan = StudentLoan(DUMMY_PRINCIPLE, DUMMY_APR, DUMMY_MIN_PAY)
        self.assertTrue(hasattr(loan, 'minimum_payment'))

    def test_loan_class_init_min_pay(self):
        """StudentLoan class initializes minimum_payment variable."""
        min_pay = 45.76  # Minimum payment
        loan = StudentLoan(DUMMY_PRINCIPLE, DUMMY_APR, min_pay)
        self.assertEqual(loan.minimum_payment, min_pay)

    def test_class_independence(self):
        """StudentLoan class instances are wholly separate from each other."""
        loan1 = StudentLoan(2000, 6, 50)
        loan2 = StudentLoan(4000, 12, 100)
        self.assertNotEqual(loan1.principle, loan2.principle)
        self.assertNotEqual(loan1.apr, loan2.apr)
        self.assertNotEqual(loan1.minimum_payment, loan2.minimum_payment)

    def test_loan_calculate_interest(self):
        """StudentLoan class can calculate the loan's accrued interest from a single period."""
        princ = 10000         # starting principle
        apr_perc = 12         # annual interest rate (percentage)
        loan = StudentLoan(princ, apr_perc, DUMMY_MIN_PAY)
        interest = loan.calculate_single_month_interest()
        self.assertEqual(interest, 100)

    def test_loan_has_apply_payment(self):
        """StudentLoan class has apply_payment method."""
        loan = StudentLoan(DUMMY_PRINCIPLE, DUMMY_APR, DUMMY_MIN_PAY)
        self.assertTrue(hasattr(loan, 'apply_payment'))

    def test_loan_apply_loan_payment(self):
        """apply_payment lowers principle correctly."""
        princ = 10000         # starting principle
        apr_perc = 12         # annual interest rate (percentage)
        loan = StudentLoan(princ, apr_perc, DUMMY_MIN_PAY)
        loan.apply_payment(200)
        # The payment should pay 100 to interest and the rest to principle
        self.assertEqual(loan.principle, 9900)

    def test_loan_over_pay(self):
        """Payment is greater than the principal + interest."""
        payment = 200   # payment significantly higher than payoff amount
        princ = 100     # starting principle
        apr_perc = 12   # annual interest rate (percentage)
        loan = StudentLoan(princ, apr_perc, DUMMY_MIN_PAY)
        self.assertRaises(OverpaymentException, loan.apply_payment, payment)

    def test_loan_payoff(self):
        """Payment pays remaining principal and interest."""
        princ = 10000         # starting principle
        apr_perc = 12         # annual interest rate (percentage)
        loan = StudentLoan(princ, apr_perc, DUMMY_MIN_PAY)
        payment = princ + loan.calculate_single_month_interest()
        loan.apply_payment(payment)
        self.assertEqual(loan.principle, 0)
        self.assertEqual(loan.calculate_single_month_interest(), 0)

    def test_loan_has_payment_details(self):
        """StudentLoan class has last_payment_details method."""
        loan = StudentLoan(DUMMY_PRINCIPLE, DUMMY_APR, DUMMY_MIN_PAY)
        self.assertTrue(hasattr(loan, 'last_payment_details'))

    def test_last_payment_details(self):
        """Verify last_payment_details accuracy."""
        princ = 10000         # starting principle
        apr_perc = 12         # annual interest rate (percentage)
        loan = StudentLoan(princ, apr_perc, DUMMY_MIN_PAY)
        loan.apply_payment(250)
        self.assertEqual(loan.last_payment_details["payment"], 250)
        self.assertEqual(loan.last_payment_details["principleReduction"], 150)
        self.assertEqual(loan.last_payment_details["InterestPaid"], 100)

    def test_loan_under_pay(self):
        """Payment is less than the minimum payment."""
        payment = 20    # payment significantly lower than payoff amount
        princ = 100     # starting principle
        apr_perc = 12   # annual interest rate (percentage)
        min_pay = payment * 2 # minimum payment required for the loan
        loan = StudentLoan(princ, apr_perc, min_pay)
        self.assertRaises(UnderpaymentException, loan.apply_payment, payment)
        
    def test_loan_under_pay_payoff(self):
        """Payment is payoff value but less than the minimum payment."""
        princ = 10     # starting principle
        apr_perc = 12  # annual interest rate (percentage)
        min_pay = 50   # minimum payment required for the loan
        loan = StudentLoan(princ, apr_perc, min_pay)
        payment = princ + loan.calculate_single_month_interest()
        loan.apply_payment(payment)
        self.assertEqual(loan.principle, 0)
        self.assertEqual(loan.calculate_single_month_interest(), 0)


if __name__ == '__main__':
    unittest.main()
