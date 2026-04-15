# 🎫 TicketSystem

 Ticket-System entwickelt mit **ASP.NET Core MVC**, **Entity Framework Core** und **SQL Server**.

---

## 🚀 Features

### 📌 Ticket-Management
- ✅ Tickets erstellen, bearbeiten, löschen, schließen
- ✅ Datei-Upload (max. 10 MB, erlaubte Formate: jpg, png, pdf, doc, txt, zip)
- ✅ Kommentare zu Tickets (neueste zuerst)
- ✅ Ticket-Blockierung (Abhängigkeiten zwischen Tickets)
- ✅ Team-Zusammenarbeit (mehrere Mitarbeiter pro Ticket)

### 👥 Benutzer & Rollen
- ✅ Registrierung, Login, Logout
- ✅ Admin & Benutzer Rollen
- ✅ Admin kann User sperren/entsperren (mit Grund)
- ✅ Admin kann User zum Admin machen

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

### 🏢 Verwaltung (Admin)
- ✅ Kategorien verwalten (CRUD)
- ✅ Abteilungen verwalten (CRUD)
- ✅ Tickets löschen
- ✅ Mitarbeiter zu Tickets zuweisen

### 🎨 Design
- ✅ Dark Mode (umschaltbar, bleibt gespeichert)
- ✅ Responsive Design (mobilfreundlich)
- ✅ Glassmorphismus & 3D-Karten
- ✅ Glanz-Effekt beim Laden

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

Benutzer: lisa@schmidt.de -	test@test.de - john@doe.de =>	test123

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

## 🚀 **Nach dem Speichern:**

```powershell
cd C:\Users\deins\source\repos\TicketSystem
git add README.md
git commit -m "README.md hinzugefügt"
git push

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
| Aktion                   | Admin     | Ticket-Ersteller     | Andere User |
|--------                  |-------    |------------------    |-------------|
| Ticket  erstellen          | ✅          | ✅                   ✅ |

| Eigene Tickets ansehen     | ✅          | ✅                   ✅ |

| Fremde Tickets ansehen     | ✅        ❌ (nur eigene Ag)|    ❌ (nur eigene Abteilung) 

| Ticket bearbeiten          | ✅          | ✅                 | ❌ |
| Ticket schließen           | ✅          | ✅                 | ❌ |
| Ticket löschen             | ✅          | ❌                 | ❌ |
| Kommentar hinzufügen       | ✅          | ✅                 | ✅ |
| Kommentar löschen          | ✅          | ❌                 | ❌ |
| Attachment hochladen       | ✅          | ✅                 | ✅ |
| Attachment löschen         | ✅          | ❌                 | ❌ |
| Ticket blockieren          | ✅          | ❌                 | ❌ |
| Mitarbeiter zuweisen       | ✅          | ❌                 | ❌ |
| User sperren/entsperren    | ✅          | ❌                 | ❌ |
| User zum Admin machen      | ✅          | ❌                 | ❌ |
| Kategorien verwalten       | ✅          | ❌                 | ❌ |
| Abteilungen verwalten      | ✅          | ❌                 | ❌ |
| Nachrichten senden         | ✅          | ✅                 | ✅ |
| Dashboard sehen            | ✅          | ✅                 | ✅ |

---

📋 KI  Prompt zum Kopieren –
# TicketSystem – Weiterentwicklung 

## 🔧 Aktueller Stand
- TicketSystem läuft lokal mit SQL Server
- Dark Mode funktioniert
- Chat Box bleibt weiß (auch im Dark Mode)
- User sperren/entsperren funktioniert (mit Prompt)
- Team-Zusammenarbeit (Assignees) funktioniert
- Prioritäten (Low, Medium, High, Urgent) implementiert
- Dashboard mit Statistiken und Filtern

## 🐛 Bekannte Probleme (noch offen)
1. **User sperren** – Der Button hatte noch `data-bs-toggle="modal"`, wurde aber behoben
2. **Bootstrap JS** – Ist auskommentiert, weil es Konflikte gab
3. **Chat-Nachrichten** – Klick führt nicht automatisch zum Ticket

## 🚀 Nächste Schritte (optional)
- [ ] E-Mail-Benachrichtigungen
- [ ] Ticket-Export als PDF/Excel
- [ ] Dashboard mit Diagrammen (Chart.js)
- [ ] Passwort vergessen / Zurücksetzen
- [ ] Zwei-Faktor-Authentifizierung

## 📁 Wichtige Dateien 
| Datei | Pfad |
|-------|------|
| `StyleSheet.css` | `wwwroot/css/StyleSheet.css` |
| `_Layout.cshtml` | `Views/Shared/_Layout.cshtml` |
| `Index.cshtml` (Dashboard) | `Views/Home/Index.cshtml` |
| `Users.cshtml` | `Views/Admin/Users.cshtml` |
| `TicketController.cs` | `Controllers/TicketController.cs` |
| `AdminController.cs` | `Controllers/AdminController.cs` |
| `ApplicationUser.cs` | `Domain/Models/ApplicationUser.cs` |
| `Ticket.cs` | `Domain/Models/Ticket.cs` |
| `Program.cs` | `Program.cs` |

## 🛠️ Befehle 
```bash
# 1. Repository klonen
git clone https://github.com/Adil-si/TicketSystem.git
cd TicketSystem

# 2. Datenbank erstellen
cd TicketSystem.Infrastructure
dotnet ef database update --startup-project ../TicketSystem

# 3. App starten
cd ../TicketSystem
dotnet run

# 4. Browser öffnen
https://localhost:7107

