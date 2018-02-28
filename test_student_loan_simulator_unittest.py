#!/usr/bin/env python
"""This script performs unit testing on the student_loan_simulator script.""" 

import unittest
from student_loan_simulator import StudentLoan

dummyPrinciple = 0
dummyApr = 0
dummyCompPeriods = 0

class TestStudentLoanSimulator(unittest.TestCase):

    def setUp(self):
        pass
 
    def test_loan_class_create(self):
        """Create a loan class instance."""
        loan = StudentLoan(dummyPrinciple, dummyApr, dummyCompPeriods)
        self.assertIsInstance(loan, StudentLoan)

    def test_loan_class_has_principle_var(self):
        """StudentLoan class has a principal variable."""
        loan = StudentLoan(dummyPrinciple, dummyApr, dummyCompPeriods)
        self.assertTrue(hasattr(loan, 'principle'))

    def test_loan_class_initializes_principle(self):
        """StudentLoan class initializes principal variable."""
        princ = 10000  # starting principle
        loan = StudentLoan(princ, dummyApr, dummyCompPeriods)
        self.assertEqual(loan.principle, princ)

    def test_loan_class_has_apr_var(self):
        """StudentLoan class has a apr variable."""
        loan = StudentLoan(dummyPrinciple, dummyApr, dummyCompPeriods)
        self.assertTrue(hasattr(loan, 'apr'))

    def test_loan_class_initializes_apr(self):
        """StudentLoan class initializes apr variable."""
        apr = 12  # Annual interest rate (percentage)
        loan = StudentLoan(dummyPrinciple, apr, dummyCompPeriods)
        self.assertEqual(loan.apr, apr)

    def test_loan_class_has_compoundPeriods_var(self):
        """StudentLoan class has a compoundPeriods variable."""
        loan = StudentLoan(dummyPrinciple, dummyApr, dummyCompPeriods)
        self.assertTrue(hasattr(loan, 'compoundPeriods'))

    def test_loan_class_initializes_compoundPeriods(self):
        """StudentLoan class initializes compoundPeriods variable."""
        periods = 12  # compound periods per year
        loan = StudentLoan(dummyPrinciple, dummyApr, periods)
        self.assertEqual(loan.compoundPeriods, periods)

    def test_StudentLoan_classes_do_not_share_initial_resources(self):
        loan1 = StudentLoan(2000, 6, 6)
        loan2 = StudentLoan(4000, 12, 12)
        self.assertNotEqual(loan1.principle, loan2.principle)
        self.assertNotEqual(loan1.apr, loan2.apr)
        self.assertNotEqual(loan1.compoundPeriods, loan2.compoundPeriods)

    def test_loan_class_calculate_single_period_interest(self):
        """StudentLoan class can calculate the loan's accrued interest from a single period."""
        princ = 10000         # starting principle
        apr_perc = 12         # annual interest rate (percentage)
        periods = 12          # compound periods per year
        loan = StudentLoan(princ, apr_perc, periods)
        interest = loan.calculate_single_period_interest()
        self.assertEqual(interest, 100)

    def test_loan_class_has_apply_payment_method(self):
        """StudentLoan class has apply_payment method."""
        loan = StudentLoan(dummyPrinciple, dummyApr, dummyCompPeriods)
        self.assertTrue(hasattr(loan, 'apply_payment'))

    def test_loan_apply_loan_payment(self):
        """apply_payment lowers principle correctly."""
        princ = 10000         # starting principle
        apr_perc = 12         # annual interest rate (percentage)
        periods = 12          # compound periods per year
        loan = StudentLoan(princ, apr_perc, periods)
        loan.apply_payment(200)
        # The payment should pay 100 to interest and the rest to principle
        self.assertEqual(loan.principle, 9900)

    def test_loan_class_has_last_payment_details_method(self):
        """StudentLoan class has last_payment_details method."""
        loan = StudentLoan(dummyPrinciple, dummyApr, dummyCompPeriods)
        self.assertTrue(hasattr(loan, 'last_payment_details'))

    def test_verify_last_payment_details(self):
        """Verify last_payment_details accuracy."""
        princ = 10000         # starting principle
        apr_perc = 12         # annual interest rate (percentage)
        periods = 12          # compound periods per year
        loan = StudentLoan(princ, apr_perc, periods)
        loan.apply_payment(250)
        self.assertEqual(loan.last_payment_details["payment"], 250)
        self.assertEqual(loan.last_payment_details["principleReduction"], 150)
        self.assertEqual(loan.last_payment_details["InterestPaid"], 100)

if __name__ == '__main__':
    unittest.main()
