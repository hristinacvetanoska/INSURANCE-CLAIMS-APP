# Insurance Claims API

A multi-tier backend application for managing insurance claims. This system allows users to create, read, and delete claims while enforcing validations and auditing.

# Overview

This application is structured with layered architecture separating:

Controllers – handle HTTP requests and responses.

Services – implement business logic.

Repositories – handle data access.

Auditing – asynchronously logs create/delete actions for claims and covers.

The system is designed for extensibility, testability, and maintainability.

# Features

Create, read, and delete Claims.

Create, read, and delete Covers.

Validation of claims and covers to ensure data integrity.

Asynchronous auditing with resilience to failures.

Premium calculation for covers based on type and insurance period.

# Architecture

Controllers interact only with DTOs and Services, not directly with Entities.

Services encapsulate business rules and orchestrate data operations.

Repositories abstract database access.

AuditBackgroundService consumes in-memory audit messages without blocking the main request pipeline.

# Validation Rules
Claim

DamageCost cannot exceed 100,000.

Created date must fall within the related cover's period.

Cover

StartDate cannot be in the past.

Total insurance period cannot exceed 1 year.

# Auditing

Audit messages are queued when claims or covers are created/deleted.

AuditBackgroundService reads messages from the in-memory queue.

Each message is saved asynchronously to the database.

Errors during auditing are logged but do not interrupt processing.

# Premium Computation

Premium depends on:

Type of cover (Yacht, Passenger Ship, Tanker, Other).

Length of the insurance period:

Base rate per day: 1250

Yacht: +10%, Passenger Ship: +20%, Tanker: +50%, Other: +30%

Progressive discounts after 30 and 180 days.

The computation is encapsulated in a PremiumCalculator service, fully unit-tested.

# Getting Started

1. Clone the repository: git clone https://github.com/hristinacvetanoska/INSURANCE-CLAIMS-APP.git
2. Open Claims.sln in Visual Studio / Rider.
3. Ensure Docker is installed and running.
4. Start the API via Visual Studio
5. Open Swagger UI to test endpoints: https://localhost:7052/swagger/index.html\
   
# Testing

Unit and integration tests cover:

ClaimService and CoverService business logic.

Controller endpoints returning proper HTTP status codes.

Premium calculations and validations. 
