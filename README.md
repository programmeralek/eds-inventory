The Inventory Service is a C# .NET microservice responsible for maintaining and updating product stock levels in response to domain events within an event-driven architecture.

It consumes Kafka events emitted by upstream services, applies ACID compliant local transactions against a MariaDB datastore, and enforces inventory specific invariants independently of order creation or payment processing. Database schema evolution is managed via migrations, and the service is fully dockerized for reproducible deployments.

The service registers with Eureka for service discovery and operates as an autonomous consumer and emitter of domain facts, preserving strict decoupling and service ownership boundaries.
