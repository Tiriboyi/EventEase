-- Update existing venues to have EventTypeId = 1 (Wedding)
UPDATE [dbo].[Venues] SET [EventTypeId] = 1;
GO

-- Add the foreign key constraint
ALTER TABLE [dbo].[Venues]
ADD CONSTRAINT [FK_Venues_EventTypes_EventTypeId]
FOREIGN KEY ([EventTypeId]) REFERENCES [dbo].[EventTypes] ([EventTypeId])
ON DELETE NO ACTION;
