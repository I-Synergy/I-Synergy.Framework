# Clean Code Principles voor C# Projecten

## 1. Bestands- en Mapstructuur
- Elk publiekelijk toegankelijk type in een eigen C#-bestand.
- Logische mapstructuur: /Models, /Services, /Controllers, enzovoort.
- Tests in aparte map/project.
- Bestandsnaam = typenaam, geen nietszeggende folders.

## 2. Naamgeving
- Klassen, enums, structs: PascalCase.
- Interfaces: 'I' + PascalCase.
- Methodes & properties: PascalCase.
- Velden: _camelCase (privé), s_pascalCase (static).
- Lokale variabelen/parameters: camelCase.
- Geen afkortingen of Hongaarse notatie.

## 3. Klassen & Methodes
- Eén verantwoordelijkheid per klasse/methode.
- Gebruik interfaces voor abstrahering.
- Heldere, beschrijvende namen.

## 4. Toegangsmodificatoren
- Per default zo beperkt mogelijk.
- Publieke leden gegroepeerd bovenin.
- Volgorde: public, protected, private.

## 5. Exception Handling
- Exceptions alleen voor uitzonderingen.
- Nooit catch zonder logging/her-throw.
- Eigen exception types eindigen op Exception.
- Gebruik InnerException waar passend.
- Vermijd ‘swallowen’ van exceptions.

## 6. Commentaar & Documentatie
- XML-doc op publieke classes/members.
- Beschrijf ‘waarom’, geen triviale comments.
- TODO/FIXME duidelijk, met motivatie.

## 7. Code Indeling & Formatting
- Vier spaties, geen tabs.
- Accolades op nieuwe regel (Allman style).
- Eén regel wit tussen members/types.
- Usings bovenaan, ongebruikte weg.
- Groepeer fields, properties, constructors, methods.

## 8. Unit Testing
- Publieke logica moet testbaar zijn én getest worden.
- Testnaam: [Methode]_[Scenario]_[Resultaat].
- AAA-structuur: Arrange, Act, Assert.
- Mocks/doubles voor afhankelijke services.

## 9. Afhankelijkheden
- Alleen noodzakelijke packages na overleg.
- Gebruik interfaces/injectie.
- Vermijd reflectie/dynamiek.
- Lock/package-management bijhouden.

## 10. SOLID & Clean Code

- SOLID (Single Responsibility, Open/Closed, Liskov substitution, Interface Segregation, Dependency Inversion) wordt per default toegepast.
- Vermijd duplicatie (DRY – Don’t Repeat Yourself).
- Vermijd lange argument-lijsten: hanteer voorkeur voor DTO’s of parameter-objecten.
- Vermijd magische waarden: gebruik constante definities of enums.
