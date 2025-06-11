# EventEase - Event Management System

## Overview
EventEase is a comprehensive event management system built with ASP.NET Core that helps organizations manage events, venues, and bookings efficiently. The system provides full-text search capabilities, venue management, event scheduling, and booking functionality.

## Features

### Event Management
- Create, edit, and delete events
- Schedule events with date and time
- Assign venues to events
- Categorize events by type
- Full-text search across event details

### Venue Management
- Track venue availability
- Manage venue capacity
- Location-based venue search
- Venue booking system
- Real-time availability updates

### Search Functionality
- Combined search for events and venues
- Full-text search capabilities
- Filter by:
  - Event type
  - Date range
  - Venue availability
  - Venue capacity
  - Location

### Booking System
- Create and manage event bookings
- Check venue availability
- Track booking status
- Manage capacity constraints

## Technologies Used

- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **Search**: SQL Server Full-Text Search
- **Frontend**: Bootstrap 5, Razor Views
- **Authentication**: ASP.NET Core Identity
- **Caching**: In-memory caching for improved performance

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository:
```powershell
git clone [repository-url]
```

2. Navigate to the project directory:
```powershell
cd EventEase
```

3. Update the database connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EventEase;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

4. Apply database migrations:
```powershell
dotnet ef database update
```

5. Run the application:
```powershell
dotnet run
```

## Database Setup

The system uses SQL Server with Full-Text Search capabilities. The necessary migrations are included in the project:
- Initial database creation
- Event and Venue relationships
- Full-text search indexes for Events and Venues

## Usage

### Event Management
1. Navigate to the Events section
2. Use the search functionality to find existing events
3. Create new events with the "Create New Event" button
4. Edit or delete events using the action buttons

### Venue Management
1. Access the Venues section
2. View available venues
3. Check venue capacity and availability
4. Manage venue details and bookings

### Search
1. Use the combined search bar for events and venues
2. Apply filters for:
   - Date range
   - Venue capacity
   - Location
   - Availability
3. View results in separate sections for events and venues

## Project Structure

```
EventEase/
├── Controllers/          # MVC Controllers
├── Models/              # Data models and view models
├── Views/               # Razor views
├── Services/            # Business logic services
├── Data/               # Database context and configurations
├── Migrations/         # Database migrations
└── SQL/                # SQL scripts for setup
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support, please create an issue in the repository or contact the development team.

## Acknowledgments

- Built by Fortune Tiriboyi (ST10126329)
- Part of CLDV7111 POE
- Developed with ASP.NET Core and modern web technologies
