#!/usr/bin/env python
"""This script simlates month by month repayment of student loans."""

class StudentLoan:
    """Defines a student loan."""
    last_payment_details = {"payment": 0, "principleReduction": 0, "InterestPaid": 0}

    def __init__(self, principle, apr, minimum_payment):
        self.principle = principle
        self.apr = apr
        self.minimum_payment = minimum_payment

    def calculate_single_month_interest(self):
        """Calculate and return the interest accrued over a single period."""
        apr_dec = self.apr / 100  # annual interest rate (decimal)
        monthes_per_year = 12
        interest = self.principle * (1 + (apr_dec / monthes_per_year)) - self.principle
        return interest

    def apply_payment(self, payment):
        """Apply a payment to the loan."""
        interest = self.calculate_single_month_interest()
        payoff = self.principle + interest
        if payment > payoff:
            raise OverpaymentException("Over Payment")
        if (payment < self.minimum_payment) & (self.minimum_payment < payoff):
            raise UnderpaymentException("Must pay minimum")
        self.last_payment_details["payment"] = payment
        self.last_payment_details["InterestPaid"] = interest
        self.last_payment_details["principleReduction"] = payment - interest
        self.principle -= self.last_payment_details["principleReduction"]

class OverpaymentException(Exception):
    """The payment is greater than the payoff value of the loan."""
    pass

class UnderpaymentException(Exception):
    """The payment is lower than the minimum required payment of the loan."""
    pass
