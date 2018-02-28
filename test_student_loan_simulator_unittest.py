#!/usr/bin/env python
"""This script performs unit testing on the student_loan_simulator script.""" 

import unittest
from student_loan_simulator import calculate_single_period_interest

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
 
if __name__ == '__main__':
    unittest.main()
