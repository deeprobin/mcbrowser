﻿namespace MinecraftServerlist.Data.Entities.Financial.Stripe;

// Reason for the refund, either user-provided (duplicate, fraudulent, or requested_by_customer) or generated by Stripe internally (expired_uncaptured_charge).
// https://stripe.com/docs/api/refunds
public enum StripeRefundReason
{
    Duplicate,
    Fraudulent,
    RequestedByCustomer,
    ExpiredUncapturedCharge
}