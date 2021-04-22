SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE [dbo].[WJbLogs_Ins]
    @Data nvarchar(max)
AS
INSERT INTO WJbLogs (LogLevel, Title, MoreJson)
VALUES (JSON_VALUE(@Data, '$.logLevel'), JSON_VALUE(@Data, '$.title'), 
    ISNULL(JSON_QUERY(@Data, '$.more'), JSON_VALUE(@Data, '$.more')))

