[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/ZmnpLZnF)
# C# Mobile - Individuele opdracht

**Student:** Mehmet Cekic  
**Email:** Mehmet.cekic@student.pxl.be

# CampMate Mobil App
## Beschrijving
CampMate is een mobiele applicatie ontwikkeld met .NET MAUI, bedoeld voor kampeerliefhebbers. Gebruikers kunnen campings zoeken, details bekijken, favorieten beheren en hun ervaringen delen via reacties en beoordelingen. De app maakt verbinding met een ASP.NET Core Web API backend en gebruikt een SQL-database voor het opslaan van gegevens.

## Features

* **Authenticatie:** Registreren en inloggen op basis van UserID-verificatie (eenvoudige implementatie voor demonstratiedoeleinden).
* **Home Dashboard:** Een overzicht van alle campings met zoek-, filter- en nieuwe kamptoevoegingsfuncties.
* **Filters:** Geavanceerd filteren op locatie (bijv. Antwerpen, Limburg) en faciliteiten (Zwembad, Wifi, etc.).
* **Favorieten:** Gebruikers kunnen campings liken. Deze worden opgeslagen in de database en zijn gekoppeld aan het unieke ID van de gebruiker.
* **Details & Reviews:** Gedetailleerde informatie per camping, inclusief de mogelijkheid om reacties te plaatsen en een sterrenbeoordeling (1-5) te geven.
* **Profielbeheer:** Gebruikers kunnen hun profielgegevens (Naam, Email, Telefoon) bewerken en een profielfoto uploaden via de media picker.
* **Admin Rechten:** Admins hebben de bevoegdheid om ongepaste reacties of campings te verwijderen.
* **Foutafhandeling:** Gebruiksvriendelijke foutmeldingen en validatie bij het invoeren van gegevens.

## Technische Architectuur

<details>
<summary><b>Klik hier om het Entity Relationship Diagram (ERD) te bekijken</b></summary>

Hieronder ziet u het databasemodel inclusief relaties (Users, Campsites, Comments, Favorites):

<img width="582" height="732" alt="screenshot21" src="https://github.com/user-attachments/assets/e6d5c09b-476a-450b-8674-4444da5630ca" />


</details>

## Screenshots
<img width="515" height="1142" alt="campmate1" src="https://github.com/user-attachments/assets/5cdfea69-7fc6-4c02-87e7-9803b5452537" />
<img width="515" height="1139" alt="campmate2" src="https://github.com/user-attachments/assets/7ca3ad30-b381-404b-807d-fa2b136d9e6e" />
<img width="515" height="1144" alt="campmate3" src="https://github.com/user-attachments/assets/c732bd0c-4d33-48b7-83e2-36a78c115ab9" />
<img width="515" height="1144" alt="campmate4" src="https://github.com/user-attachments/assets/57af1b18-3c7e-4a19-9aba-294dfd7671e8" />
<img width="515" height="1142" alt="campmate5" src="https://github.com/user-attachments/assets/a7a62825-f88c-45ad-809b-51fda8e741dc" />
<img width="515" height="1141" alt="campmate6" src="https://github.com/user-attachments/assets/f1af31e3-e3e2-48f4-86f5-e525577c548d" />
<img width="515" height="1178" alt="campmate18" src="https://github.com/user-attachments/assets/c57af4ad-3ea0-477d-8e4d-fb434e2a3d4e" />

<img width="515" height="1178" alt="campmate19" src="https://github.com/user-attachments/assets/1546d494-0852-453c-8e4d-44d755ab7e65" />
<img width="515" height="1144" alt="campmate7" src="https://github.com/user-attachments/assets/40a392d9-ae1e-4923-a270-4d7e603678a3" />
<img width="515" height="1147" alt="campmate8" src="https://github.com/user-attachments/assets/aa90b0b8-d43c-407d-ac3a-6e9bbd3f90f1" />
<img width="515" height="1151" alt="campmate9" src="https://github.com/user-attachments/assets/ed59eda0-115f-439b-9e68-91d0cb36933d" />
<img width="515" height="1142" alt="campmate10" src="https://github.com/user-attachments/assets/dabb6575-2b9f-41d3-af08-6eb11bd521a6" />
<img width="515" height="1142" alt="campmate11" src="https://github.com/user-attachments/assets/500e15c1-a3a7-4785-b9c2-4d4be3c66c9c" />
<img width="515" height="1141" alt="campmate12" src="https://github.com/user-attachments/assets/df5e431c-27f0-4d53-b3ae-d1c638ab42bd" />
<img width="515" height="1139" alt="campmate13" src="https://github.com/user-attachments/assets/0bdb9e58-9845-46fa-b347-bbc881873a68" />
<img width="515" height="1145" alt="campmate14" src="https://github.com/user-attachments/assets/ad63bd1a-b6ed-4524-9f1a-11f36e9238a9" />
<img width="515" height="1145" alt="campmate15" src="https://github.com/user-attachments/assets/4ac55db8-c7ba-4be7-b7be-c6d8eebe8c1e" />
<img width="515" height="1142" alt="campmate16" src="https://github.com/user-attachments/assets/d95936f8-088b-4f55-9053-7b5c531caad3" />
<img width="515" height="1144" alt="campmate17" src="https://github.com/user-attachments/assets/f620eaea-0697-4ec4-8b1f-45c21c066fc0" />


<img width="515" height="1144" alt="splashscreen" src="https://github.com/user-attachments/assets/8fd08ebe-f1e3-449d-a78b-6856eaf6dc2e" />





## Screencast
https://youtu.be/k9cL0mK6iKE

## Bronnen

### 1. Documentatie & Cursusmateriaal
* **PXL Cursusmateriaal:** Slide decks, video's en oefeningen van het vak Mobile Development (C#).
* **Microsoft Learn (.NET MAUI):** Officiële documentatie geraadpleegd voor XAML controls (zoals `CollectionView`, `Border`), pagina-navigatie (`Shell`) en data binding.
* **Microsoft Learn (ASP.NET Core):** Gebruikt voor het opzetten van de RESTful Web API, Controllers en Dependency Injection.

### 2. Gebruikte Bibliotheken (NuGet Packages)
* **CommunityToolkit.Mvvm:** Essentieel voor het MVVM-patroon. Gebruikt voor:
    * `[ObservableProperty]` en `[RelayCommand]` om boilerplate code te verminderen.
    * `WeakReferenceMessenger` voor communicatie tussen ViewModels (bijv. bij het updaten van de favorietenlijst).
* **System.Net.Http.Json:** Voor het vereenvoudigen van asynchrone HTTP-aanroepen (GET, POST, PUT, DELETE) en JSON-serialisatie tussen de app en de API.
* **Microsoft.EntityFrameworkCore:** Voor de interactie met de SQL-database (Code-First approach en migraties).

### 3. UI & Design
* **Material Design Icons (MDI):** Gebruikt voor de visuele elementen zoals de hartjes (favorieten), sterren (rating) en prullenbak-iconen via een custom font bestand.
* **Android MediaPicker:** Implementatie voor het selecteren van foto's uit de galerij van het toestel.

### 4. Specifieke Technische Oplossingen & Onderzoek
* **Android Emulator Networking:** Onderzoek naar de `http://10.0.2.2` alias om verbinding te maken met de lokale `localhost` API vanuit de Android-omgeving.
* **Data Persistentie:** Gebruik van `SecureStorage` en `Preferences` om gebruikerssessies (UserID) lokaal te bewaren zodat gebruikers niet telkens opnieuw hoeven in te loggen.
* **Bestandsuploads:** Implementatie van `MultipartFormDataContent` om profielfoto's correct naar de backend te sturen.
* **Validatie:** Oplossen van DTO-validatieproblemen (zoals nullable velden voor profielafbeeldingen) om `400 Bad Request` fouten te voorkomen.
