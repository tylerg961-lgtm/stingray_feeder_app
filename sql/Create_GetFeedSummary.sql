-- Creates a stored procedure returning per-stingray feed totals for a date range.
-- Parameters:
--   @StartDate datetime
--   @EndDate   datetime
--   @MinQuantity int (optional, default 0)
CREATE OR ALTER PROCEDURE dbo.GetFeedSummary
    @StartDate DATETIME,
    @EndDate DATETIME,
    @MinQuantity INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.Id            AS StingrayId,
        s.Name          AS StingrayName,
        SUM(fe.Quantity) AS TotalQuantity,
        MAX(fe.EventTime) AS LastFeedTime
    FROM FeedEvents fe
    INNER JOIN Stingrays s ON s.Id = fe.StingrayId
    WHERE fe.EventTime >= @StartDate
      AND fe.EventTime <= @EndDate
      AND fe.Quantity >= @MinQuantity
    GROUP BY s.Id, s.Name
    ORDER BY TotalQuantity DESC;
END;