using Microsoft.EntityFrameworkCore.Migrations;

namespace EventEase.Migrations
{
    public partial class AddFullTextSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create a full-text catalog
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'EventSearchCatalog')
                BEGIN
                    CREATE FULLTEXT CATALOG EventSearchCatalog;
                END");

            // Create full-text index on Events table
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT * 
                    FROM sys.fulltext_indexes 
                    WHERE object_id = OBJECT_ID('Events')
                )
                BEGIN
                    CREATE FULLTEXT INDEX ON Events(
                        EventName Language 1033,
                        Description Language 1033
                    )
                    KEY INDEX PK_Events
                    ON EventSearchCatalog
                    WITH CHANGE_TRACKING AUTO;
                END");

            // Create indexes for frequently searched columns
            migrationBuilder.Sql("CREATE INDEX IX_Events_EventDate ON Events(EventDate);");
            migrationBuilder.Sql("CREATE INDEX IX_Events_EventTypeId_EventDate ON Events(EventTypeId, EventDate);");
            migrationBuilder.Sql("CREATE INDEX IX_Venues_Location ON Venues(Location);");
            migrationBuilder.Sql("CREATE INDEX IX_Venues_Capacity ON Venues(Capacity);");
            migrationBuilder.Sql("CREATE INDEX IX_Venues_Availability ON Venues(Availability);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop full-text index
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT * 
                    FROM sys.fulltext_indexes 
                    WHERE object_id = OBJECT_ID('Events')
                )
                BEGIN
                    DROP FULLTEXT INDEX ON Events;
                END");

            // Drop full-text catalog
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'EventSearchCatalog')
                BEGIN
                    DROP FULLTEXT CATALOG EventSearchCatalog;
                END");

            // Drop indexes
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Events_EventDate ON Events;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Events_EventTypeId_EventDate ON Events;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Venues_Location ON Venues;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Venues_Capacity ON Venues;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Venues_Availability ON Venues;");
        }
    }
}
