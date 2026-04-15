# ⚽ RedZone — Liverpool FC Match Predictor

RedZone is an ASP.NET Core MVC web application for tracking Liverpool FC matches,
browsing football competitions, and making score predictions.

## ✨ Features

- Browse upcoming Liverpool FC matches sorted by date
- Manage football competitions (Premier League, Champions League, FA Cup, etc.)
- Authenticated users can submit score predictions for any match
- Duplicate prediction prevention — one prediction per match per user
- Users can remove their own predictions
- View full personal prediction history
- Role-based access control (Admin / User / Guest)
- Toast notifications for key actions (saved, removed, duplicate warning)
- Live search/filter on all tables
- Responsive dark-themed UI built with Bootstrap 5

---

## 🔐 Roles & Permissions

| Action | Guest | User | Admin |
|--------|-------|------|-------|
| View matches list | ✅ | ✅ | ✅ |
| View match details | ❌ | ✅ | ✅ |
| Make a prediction | ❌ | ✅ | ✅ |
| View own predictions | ❌ | ✅ | ✅ |
| Delete own prediction | ❌ | ✅ | ✅ |
| Create/Edit/Delete matches | ❌ | ❌ | ✅ |
| Create/Edit/Delete competitions | ❌ | ❌ | ✅ |

---

## 🛠️ Technologies Used

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core (.NET 10) |
| Architecture | MVC (Models, Views, Controllers) |
| ORM | Entity Framework Core |
| Database | SQL Server / LocalDB |
| Auth | ASP.NET Core Identity + Roles |
| UI | Bootstrap 5 + custom dark CSS |
| Fonts | Bebas Neue, Barlow (Google Fonts) |
| Validation | Data Annotations (server-side) + jQuery Validation (client-side) |

---

## 📁 Project Structure
```
RedZone/
├── RedZone.Common/              # Shared validation constants
├── RedZone.Data/                # DbContext and migrations
├── RedZone.Data.Models/         # Entity models (Match, Competition, Prediction)
├── RedZone.Services.Core/       # Service interfaces and implementations
├── RedZone.ViewModels/          # ViewModels for each feature
└── RedZone.Web/                 # Controllers, Views, wwwroot (CSS, JS)
```

---

## ⚙️ Setup Instructions

### Prerequisites

- [.NET 10 SDK]
- SQL Server or SQL Server Express / LocalDB

### 1. Clone the repository
```bash
git clone https://github.com/Ivanovv34/RedZone.git
cd RedZone
```

### 2. Configure the database connection

Open `RedZone.Web/appsettings.json` and update the connection string if needed:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RedZoneDb;Trusted_Connection=True;Encrypt=False"
}
```

> For SQL Server Express use: `Server=.\\SQLEXPRESS;Database=RedZoneDb;Trusted_Connection=True;Encrypt=False`

### 3. Apply migrations
```bash
cd RedZone.Web
dotnet ef database update
```

### 4. Run the application
```bash
dotnet run
```

The app will be available at `https://localhost:7152` or the port shown in the terminal.

---

## 🔑 Environment Variables & Credentials

No external API keys required. All configuration is in `appsettings.json`.


## 👤 Default Admin Account

A default admin account is created automatically on first run:

| Field | Value |
|-------|-------|
| Email | adminnew@redzone.com |
| Password | Admin123! |

---

## 🗄️ Sample Data

The following data is seeded automatically on first run so the app is ready to use immediately:

**Competitions:** Premier League, Champions League, FA Cup, EFL Cup

**Matches:** 8 upcoming Liverpool fixtures across all competitions
