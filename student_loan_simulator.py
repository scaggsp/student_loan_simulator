#!/usr/bin/env python
"""This script simlates month by month repayment of student loans."""

class StudentLoan:
    """Defines a student loan."""
    last_payment_details = {"payment": 0,"principleReduction": 0,"InterestPaid": 0}

    def __init__(self, principle, apr, compoundPeriods):
        self.principle = principle
        self.apr = apr
        self.compoundPeriods = compoundPeriods

    def calculate_single_period_interest(self):
        """Calculate the compound interest accrued over a single period."""
        apr_dec = self.apr / 100  # annual interest rate (decimal)
        interest = self.principle * (1 + (apr_dec / self.compoundPeriods)) - self.principle
        return interest

    def apply_payment(self, payment):
        self.last_payment_details["payment"] = payment
        interest = self.calculate_single_period_interest()
        self.last_payment_details["InterestPaid"] = interest
        self.last_payment_details["principleReduction"] = payment - interest
        self.principle -= payment - interest
