Experiment surrounding CQRS and Event Sourcing

# Overview

This is a very simple (and probably naive) implementation of [CQRS](https://en.m.wikipedia.org/wiki/Command–query_separation) and [Event Sourcing](http://martinfowler.com/eaaDev/EventSourcing.html) used to create a very simple invoicing system. 

## Commands

Commands are used to perform an action within the invoicing system, such as creating an invoice, adding new line items, applying a payment, and pretty much anything else that isn't pure data retrieval. Commands themselves don't actually do anything, instead they generate a stream of events that models what should happen when the command occurs. For example, the CreateInvoice command will generate a _InvoiceCreated_ event that contains all the necessary information about the invoice that was created, while the AddLineItem command will generate a _LineItemAdded_ and _InvoiceBalanceUpdated_ event. These events got stored in an _event store_ where they serve as a record of everything that has occurred in the system, providing auditing and the ability to see the state of the data at any point in time.

Other code in the system, called event handlers, listen for these events to be published by the commands and then perform actions in response. Currently the only event handlers that have been implemented are those that sync the _read-only model_ with the events that have been generated, but it's possible for additional event handlers to subscribe to the same events.

## Events

As mentioned earlier, events are generated in response to commands that are sent. These events are first saved to the _event store_, which then publishes them to whatever event handlers are subscribed to them. These events can then be applied in order to rebuild the state of any domain object in the system. These events can also be used to generate a log of every change made to the system and, since an inverse event should exist for every event generated, should be easily undoable.

Each event has a time stamp that is used to determine when the event was generated, as well as a GUID to make each event uniquely identifiable. And since events can be generated in response to other events, it's also possible to track which events generated other events. 
