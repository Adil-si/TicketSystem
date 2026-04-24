﻿# 🎫 TicketSystem

 Ticket-System entwickelt mit **ASP.NET Core MVC**, **Entity Framework Core** und **SQL Server**.

---

## 🚀 Features

### 📌 Ticket-Management
- ✅ Tickets erstellen, bearbeiten, löschen, schließen
- ✅ Datei-Upload (max. 10 MB, erlaubte Formate: jpg, png, pdf, doc, txt, zip)
- ✅ Kommentare zu Tickets (neueste zuerst)
- ✅ Ticket-Blockierung (Abhängigkeiten zwischen Tickets)
- ✅ Team-Zusammenarbeit (mehrere Mitarbeiter pro Ticket)
  
### 📌 Ticket-Prioritäten
- ✅ TicketPriority Enum (Low, Medium, High, Urgent)
- ✅ Migration AddTicketPriority
- ✅ Priorität in Create, Edit, Index, Details
- ✅ Prioritäts-Statistik im Dashboard
  
### 👥 Admin Rollen
- ✅ Admin kann User sperren/entsperren (mit Grund)
- ✅ Admin kann alles und auch User zum Admin machen
  
###  Teamleiter-Rolle
- ✅ Neue Rolle Teamleiter in Program.cs
- ✅ Teamleiter-User erstellt (teamleiter@ticket.de / Team123!)
- ✅ AdminController.cs für Teamleiter angepasst
- ✅ MakeTeamleiter und RemoveTeamleiter Methoden
- ✅ ViewBag.TeamleiterIds in Users() Methode

### 📊 Dashboard
- ✅ Statistik-Karten (Tickets gesamt, offen, in Bearbeitung, geschlossen)
- ✅ Beliebte Themen (Access, Facilities, QIAverse, etc.)
- ✅ Quick Access (Request something, Knowledge base, Report an Issue)
- ✅ Work Items mit Prozentanzeige
- ✅ User Statistic
- ✅ Ticket Filter (Suche, Kategorie, Abteilung, Status, Sortierung)
- ✅ Chat Box (simulierter Live Agent)
- ✅ Meine Tickets (kompakte Tabelle)

### 💬 Nachrichten-System
- ✅ Private Nachrichten zwischen Benutzern
- ✅ Konversationen gruppiert nach Benutzer
- ✅ Ungelesene Badges in Navbar
  
###   Benutzerverwaltung (Users.cshtml)
- ✅ Teamleiter Badge in der Tabelle
- ✅ Teamleiter hinzufügen/entfernen Buttons (nur für Admin)
- ✅ lockUser JavaScript Funktion (mit Prompt)
- ✅ Korrektur der </td und </tr Tags (Fehler behoben)
  
### 🏢 Verwaltung (Admin)
- ✅ Kategorien verwalten (CRUD)
- ✅ Abteilungen verwalten (CRUD)
- ✅ Tickets löschen
- ✅ Mitarbeiter zu Tickets zuweisen
  
###  Navigation (_Layout.cshtml)
- ✅ Begrüßung mit Benutzername
- ✅ Admin sieht nur ADMIN Link (orange)
- ✅ Teamleiter sieht: Team Dashboard, Kategorien, Teammitglieder
- ✅ Nachrichten-Link mit Badge
- ✅ Dark Mode Toggle
  
### 🎨 Design
- ✅ Dark Mode (umschaltbar, bleibt gespeichert)
- ✅ Responsive Design (mobilfreundlich)
- ✅ Glassmorphismus & 3D-Karten
- ✅ Glanz-Effekt beim Laden
  
##  TeamUsers.cshtml (für Teamleiter)
- ✅ Teamleiter sieht Teammitglieder (nur lesen, keine Aktionen)

---

## 🛠️ Technologien

| Technologie | Verwendung |
|-------------|------------|
| ASP.NET Core MVC | Web-Framework |
| Entity Framework Core | ORM |
| SQL Server | Datenbank |
| Identity Framework | Authentifizierung & Rollen |
| Bootstrap 5 | Frontend |
| Bootstrap Icons | Icons |
| Custom CSS | Eigene Styles |

---
Test-Zugänge
Rolle	Email	Passwort

Admin:	admin@ticket.de	 =>   Admin123!

Teamleiter : teamleiter@ticket.de  =>  Team123!

Benutzer: lisa@schmidt.de -	test@test.de - john@doe.de =>	test123

generierten Benutzer: haben das Passwort                   Demo123!

---
🐛 Bekannte Probleme & Lösungen
Problem	Lösung
Migration funktioniert nicht	dotnet ef database drop -f → Migration neu erstellen
Dark Mode speichert nicht	Cache leeren (Strg + F5)
Chat-Nachrichten nicht sichtbar	CSS-Cache leeren

---

## 📥 **So speicherst du die README.md:**

### **Windows (Notepad):**
1. `Strg + A` (alles markieren)
2. `Strg + C` (kopieren)
3. Notepad öffnen
4. `Strg + V` (einfügen)
5. `Strg + S` (speichern)
6. Dateiname: `README.md`
7. **"Alle Dateien (*.*)"** auswählen
8. Speichern im Hauptverzeichnis: `C:\Users\deins\source\repos\TicketSystem\`

### **Visual Studio Code:**
1. Neue Datei
2. Einfügen
3. `Strg + S`
4. Name: `README.md`
5. Speichern im Hauptverzeichnis

---
Screenshot
<img width="1891" height="935" alt="Screenshot 2026-04-24 082626" src="https://github.com/user-attachments/assets/c3916b31-1c92-4562-872b-38ab9e89f9d2" />
---

## 🚀 **Nach dem Speichern:**

```powershell
cd C:\Users\deins\source\repos\TicketSystem
git add README.md
git commit -m "README.md hinzugefügt"
git push

##🚀 Schnell-Check für neuen PC

git clone https://github.com/Adil-si/TicketSystem.git
cd TicketSystem

dotnet restore

dotnet build

cd TicketSystem.Infrastructure
dotnet ef database update --startup-project ../TicketSystem

cd ../TicketSystem
dotnet run

---
📋 Weitere nützliche Befehle:
Befehl	Beschreibung
git branch	Alle Branches anzeigen
git checkout main	Wechsel zu main Branch
git checkout develop	Wechsel zu develop Branch
git merge develop	Merged develop in den aktuellen Branch
git branch -d develop	Löscht develop lokal

---
Wer darf Was: 
| Aktion                   | Admin     | Ticket-Ersteller     | Andere User |                   |  Teamleiter  |
|--------                  |-------    |------------------    |-------------|                     -------------
| Ticket  erstellen          | ✅          | ✅                   ✅ |                               ✅

| Eigene Tickets ansehen     | ✅          | ✅                   ✅ |                               ✅

| Fremde Tickets ansehen     | ✅        ❌ (nur eigene Ag)|    ❌ (nur eigene Abteilung)            ❌

| Ticket bearbeiten          | ✅          | ✅                 | ❌ |                               ✅ 
| Ticket schließen           | ✅          | ✅                 | ❌ |                               ✅ (nur eigene Abteilung)
| Ticket löschen             | ✅          | ❌                 | ❌ |                               ❌
| Kommentar hinzufügen       | ✅          | ✅                 | ✅ |                               ✅ 
| Kommentar löschen          | ✅          | ❌                 | ❌ |                               ❌
| Attachment hochladen       | ✅          | ✅                 | ✅ |                               ✅
| Attachment löschen         | ✅          | ❌                 | ❌ |                               ❌
| Ticket blockieren          | ✅          | ❌                 | ❌ |                               ✅
| Mitarbeiter zuweisen       | ✅          | ❌                 | ❌ |                               ✅
| User sperren/entsperren    | ✅          | ❌                 | ❌ |                               ❌
| User zum Admin machen      | ✅          | ❌                 | ❌ |                               ❌
| Kategorien verwalten       | ✅          | ❌                 | ❌ |                               ✅
| Abteilungen verwalten      | ✅          | ❌                 | ❌ |                               ❌
| Nachrichten senden         | ✅          | ✅                 | ✅ |                               ✅
| Dashboard sehen            | ✅          | ✅                 | ✅ |                               ✅
| Teammitglieder sehen       | ✅          | ❌                 | ❌ |                               ✅    (nur eigene Abt.)
| Mitarbeiter zu TK zuweisen | ✅          | ❌                 | ❌ |                               ✅    (nur eigene Abt.)       
| Alle Tickets sehen         | ✅          | ❌                 | ❌ |                               ✅
 




---

