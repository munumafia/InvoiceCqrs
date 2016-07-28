﻿using System;
using System.Collections.Generic;
using System.Linq;
using InvoiceCqrs.Domain;
using InvoiceCqrs.Messages.Events;

namespace InvoiceCqrs.Visitors

{
    public class InvoiceHistoryVisitor : IInvoiceEventVisitor
    {
        public IList<string> Messages { get; set; } = new List<string>();

        private readonly Invoice _Invoice = new Invoice();
        private readonly IList<Payment> _Payments = new List<Payment>();

        public void Visit(InvoiceCreated evt)
        {
            evt.Apply(_Invoice);
            Messages.Add($"{evt.EventDateTime}: Invoice created");
        }

        public void Visit(LineItemAdded evt)
        {
            var lineItem = new LineItem();
            evt.Apply(lineItem);

            _Invoice.AddLineItem(lineItem);

            Messages.Add($"{evt.EventDateTime}: Line item {evt.Id} added to invoice {evt.InvoiceId}");
            Messages.Add($"New balance ${_Invoice.Balance}");
        }

        public void Visit(PaymentApplied evt)
        {
            evt.Apply(_Invoice);

            Messages.Add($"{evt.EventDateTime}: Payment {evt.PaymentId} applied to line item {evt.LineItemId}"); 
            Messages.Add($"New balance ${_Invoice.Balance}");
        }

        public void Visit(PaymentUnapplied evt)
        {
            throw new NotImplementedException();
        }
    }
}
