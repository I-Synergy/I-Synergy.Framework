# SOLID Richtlijnen voor C# Codegeneratie

## 1. Single Responsibility Principle (SRP)
- Iedere class, struct of method heeft één duidelijke verantwoordelijkheid.
- Vermijd ‘klassen met meerdere taken’ of ‘utility classes’ met te brede scope.
- Splits code op zodra je meerdere redenen voor verandering identificeert.

## 2. Open/Closed Principle (OCP)
- Classes moeten uitbreidbaar zijn zonder bestaande code te wijzigen.
- Realiseer uitbreidbaarheid via abstracties (interfaces, abstracte classes), niet via wijziging originele klassen.
- Voeg functionaliteit altijd toe via extensie, niet aanpassing.

## 3. Liskov Substitution Principle (LSP)
- Subklassen moeten te gebruiken zijn als vervanging voor hun superklassen zonder onverwachte bijwerkingen.
- Overriden gedrag mag de verwachtingen van de gebruiker van het type niet schaden.
- Introduceer geen breaking changes in subklassen.

## 4. Interface Segregation Principle (ISP)
- Houd interfaces klein, specifiek en taakgericht.
- Vermijd het verplichten tot het implementeren van niet-benodigde methoden.
- Gebruik liever meerdere kleine interfaces dan één grote.

## 5. Dependency Inversion Principle (DIP)
- Afhankelijkheden lopen altijd via abstracties, nooit via concrete implementaties.
- Pas constructor injection toe voor afhankelijkheden, waar mogelijk.
- Maak geen concrete instanties binnen een klasse; injecteer deze.
- Gebruik DI-frameworks om afhankelijkheden te beheren op projectniveau.
