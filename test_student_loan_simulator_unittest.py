#!/usr/bin/env python
"""This script performs unit testing on the student_loan_simulator script.""" 

import unittest
from student_loan_simulator import calculate_single_period_interest
from student_loan_simulator import StudentLoan

dummyPrinciple = 0
dummyApr = 0
dummyCompPeriods = 0

class TestStudentLoanSimulator(unittest.TestCase):

    def setUp(self):
        pass
 
    def test_interest_calculator(self):
        """Verify interst calculator function is accurate."""
        princ = 10000         # starting principle
        apr_perc = 10         # annual interest rate (percentage)
        periods = 12          # compound periods per year
        interest = calculate_single_period_interest(princ, apr_perc, periods)
        self.assertAlmostEqual(interest, 83.33333333)
 
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
        self.assertEquals(loan.principle, princ)

    def test_loan_class_has_apr_var(self):
        """StudentLoan class has a apr variable."""
        loan = StudentLoan(dummyPrinciple, dummyApr, dummyCompPeriods)
        self.assertTrue(hasattr(loan, 'apr'))

    def test_loan_class_initializes_apr(self):
        """StudentLoan class initializes apr variable."""
        apr = 10  # Annual interest rate (percentage)
        loan = StudentLoan(dummyPrinciple, apr, dummyCompPeriods)
        self.assertEquals(loan.apr, apr)

    def test_loan_class_has_compoundPeriods_var(self):
        """StudentLoan class has a compoundPeriods variable."""
        loan = StudentLoan(dummyPrinciple, dummyApr, dummyCompPeriods)
        self.assertTrue(hasattr(loan, 'compoundPeriods'))

    def test_loan_class_initializes_compoundPeriods(self):
        """StudentLoan class initializes compoundPeriods variable."""
        periods = 12  # compound periods per year
        loan = StudentLoan(dummyPrinciple, dummyApr, periods)
        self.assertEquals(loan.compoundPeriods, periods)

if __name__ == '__main__':
    unittest.main()
