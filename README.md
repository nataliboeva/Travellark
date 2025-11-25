# **Travellark – Personal Travel Companion**
*A minimalist travel journal with interactive maps, statistics, favorites, and a modern ASP.NET Core UI.*

---

## Overview

**Travellark** is a personal travel companion built with **ASP.NET Core 8**, combining destination management, an interactive map, analytics dashboard, and secure user profiles.
The goal of the project is to offer a clean and intuitive way for travelers to track visited locations, plan wishlist trips, analyze travel habits, and keep memories in one place.

The application focuses on simplicity and personalization, avoiding social features in favor of a private, customizable travel journal.

---

## Key Features

### 🗺️ Destination Management (CRUD)
- Create, edit, delete and view travel destinations
- Each destination includes:
  - Name, country, region
  - Location type (City, Nature, Historical, etc.)
  - Status: *Visited* or *Wishlist*
  - Star rating (1–5)
  - Personal notes & inspirations
  - Coordinates selected via **map picker**
  - Image upload (local or URL)

### Smart Filtering & Real-Time Search
- Filter by status (Visited/Wishlist)
- Filter by type
- Combine filters dynamically
- Real-time search by name with instant results

### Interactive Map (Leaflet)
- Custom markers for visited vs wishlist
- Popups with quick links to details
- Map picker for selecting coordinates when creating destinations

### Dashboard & Analytics
- Top visited countries
- Favorite destinations
- Distribution by type
- Recent visits
- Rating visualization

### Favorites
- Mark and unmark favorite destinations
- Quick access to personal highlights

### Profile & Account Management
- Register, login, logout
- Login using **email or username**
- Change email, username, or password
- Forgot password flow
- Two-factor authentication (2FA) support
- Download or delete personal data

---

## Tech Stack

- **Backend:** ASP.NET Core 8 (MVC + Identity)
- **Frontend:** Bootstrap 5.3, Leaflet.js, Razor Views
- **Database:** SQL Server / LocalDB
- **Auth:** ASP.NET Core Identity
- **Other:** Font Awesome / Bootstrap Icons
- **GenAI Tools:** ChatGPT, GPT-4, Cursor (used for ideation and assistance)

---

## 📷 Gallery & Screenshots

| **Home & Navigation** | **Dashboard & Analytics** |
| :--- | :--- |
| <img src="https://github.com/user-attachments/assets/23019e9d-b692-4991-bfd9-b1d21305c24d" width="100%"> | <img src="https://github.com/user-attachments/assets/f78ad132-aa1b-475d-b245-c6b04fbd4e1f" width="100%"> |
| **Destinations List** | **Interactive Map** |
| <img src="https://github.com/user-attachments/assets/babb02a2-3ee9-460b-88f2-50ffae582b61" width="100%"> | <img src="https://github.com/user-attachments/assets/0ec597ef-09ba-48f1-8c2b-de10c00227c4" width="100%"> |
| **Create/Edit Destination** | **Destination Details** |
| <img src="https://github.com/user-attachments/assets/8c0314f8-db21-4abe-a45a-b0eca621b83b" width="100%"> | <img src="https://github.com/user-attachments/assets/01730e6d-6843-44c8-88f4-e03e759314e2" width="100%"> |

<p align="center">
  <strong>Profile & Login Management</strong><br>
  <img src="https://github.com/user-attachments/assets/caad6d78-57a7-4b4d-a6b3-de0495a5983f" width="80%">
</p>

---

## Future Improvements

- Trips / itineraries (connecting destinations into routes)
- AI-powered destination suggestions
- Push notifications or calendar integration
- Offline mode / PWA support
- Social sharing or collaborative journals

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server / LocalDB

### Run locally

```bash
git clone [https://github.com/yourusername/Travellark.git](https://github.com/yourusername/Travellark.git)
cd Travellark
dotnet restore
dotnet ef database update
dotnet run
