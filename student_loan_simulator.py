#!/usr/bin/env python
"""This script simlates month by month repayment of student loans."""

def calculate_single_period_interest(principal, apr, periods):
    """Calculate the compound interest accrued over a single period.

    Keyword arguments:
    principal -- principal at the start of the period
    apr       -- Annual percentage Rate
    periods   -- Number of times per year interest is calculated
    """
    apr_dec = apr / 100  # annual interest rate (decimal)
    interest = principal * (1 + (apr_dec / periods)) - principal
    return interest

class StudentLoan:
    """Defines a student loan."""
    def __init__(self, principle, apr):
        self.principle = principle
        self.apr = apr
