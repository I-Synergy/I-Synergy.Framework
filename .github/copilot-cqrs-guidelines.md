# CQRS Style Guide (C#)

## 1. Bestands- en Mapstructuur
- Splits commands en queries in afzonderlijke mappen.
- Gebruik submappen per feature/domein.
- Eén class per bestand.

## 2. Naamgeving
- Eindig command classes op `Command`, queries op `Query`.
- Handlers eindigen altijd op `Handler`.
- DTO's op `Dto`.
- Gebruik PascalCase voor types, camelCase voor parameters/velden.
- Interface-benaming: start altijd met 'I'.

## 3. Command- en Query Objecten
- Bevatten alleen input-properties, geen logica.
- Maak immutable indien mogelijk.
- Handler verwerkt ieder object.

## 4. Handler Implementaties
- Gebruik `ICommandHandler<TCommand, TResult>`, `IQueryHandler<TQuery, TResult>` interfaces.
- Handlers bevinden zich in relevante (sub)map, bevatten alle business-logica.
- Injecteer afhankelijkheden via constructor.
- Gebruik async/await.

## 5. Dependency Injection & Mediation
- Gebruik dispatcher/mediator voor handler-activatie (zie het ShardedKernel project).
- Controllers communiceren alleen via mediator.

## 6. Exception Handling
- Handlers gooien alleen business/domain exceptions.
- Centrale afhandeling op endpoint- of middleware-niveau.

## 7. Validatie
- Scheid validatie in losse validator-classes.
- Valideer vóór handler-executie.

## 8. Documentatie & Commentaar
- XML-comments op publieke interfaces en methoden.
- Beschrijf gedrag, parameters en return types.
- Voeg korte business-doelomschrijving toe bij handlers.

## 9. Unit Testing
- Iedere handler krijgt (minimaal) unit tests op alle kritieke paden met mocks/stubs.

## 10. Externe Afhankelijkheden
- Injecteer infrastructuur via interfaces.
- Plaats databaselogica buiten handlers.
