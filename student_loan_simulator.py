#!/usr/bin/env python
"""This script simlates month by month repayment of student loans."""

class StudentLoan:
    """Defines a student loan."""
    def __init__(self, principle, apr, compoundPeriods):
        self.principle = principle
        self.apr = apr
        self.compoundPeriods = compoundPeriods

    def calculate_single_period_interest(self):
        """Calculate the compound interest accrued over a single period."""
        apr_dec = self.apr / 100  # annual interest rate (decimal)
        interest = self.principle * (1 + (apr_dec / self.compoundPeriods)) - self.principle
        return interest
