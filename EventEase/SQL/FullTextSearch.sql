IF NOT EXISTS (
    SELECT * 
    FROM sys.fulltext_catalogs 
    WHERE name = 'SearchCatalog'
)
BEGIN
    CREATE FULLTEXT CATALOG SearchCatalog;
END

IF NOT EXISTS (
    SELECT * 
    FROM sys.fulltext_indexes 
    WHERE object_id = OBJECT_ID('dbo.Events')
)
BEGIN
    CREATE FULLTEXT INDEX ON dbo.Events(
        EventName LANGUAGE 1033,           
        Description LANGUAGE 1033
    )
    KEY INDEX PK_Events
    ON SearchCatalog
    WITH CHANGE_TRACKING AUTO;
END

IF NOT EXISTS (
    SELECT * 
    FROM sys.fulltext_indexes 
    WHERE object_id = OBJECT_ID('dbo.Venues')
)
BEGIN
    CREATE FULLTEXT INDEX ON dbo.Venues(
        VenueName LANGUAGE 1033,           
        Location LANGUAGE 1033
    )
    KEY INDEX PK_Venues
    ON SearchCatalog
    WITH CHANGE_TRACKING AUTO;
END
