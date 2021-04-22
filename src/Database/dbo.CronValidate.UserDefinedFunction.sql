SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

/*
SELECT [dbo].[CronValidate]('0 17 L * *', '2020-11-29 17:00')
SELECT DATEPART(day, EOMONTH ( GETDATE() )) AS Result;  
*/
CREATE FUNCTION [dbo].[CronValidate] (@Expression varchar(100), @Now datetime)
RETURNS bit
AS
BEGIN
    SET @Now = ISNULL(@Now, GETDATE());
    SET @Expression = dbo.CronPrepare(@Expression);

    DECLARE @CronMinute varchar(100) = dbo.CronPart(@Expression, 'minute')
        ,@CronHour varchar(100) = dbo.CronPart(@Expression, 'hour')
        ,@CronDay varchar(100) = dbo.CronPart(@Expression, 'day')
        ,@CronMonth varchar(100) = dbo.CronPart(@Expression, 'month')
        ,@CronWeekday varchar(100) = dbo.CronPart(@Expression, 'weekday')

    IF NOT (@CronMinute = '*' OR DATEPART(MINUTE, @Now) IN (SELECT Value FROM dbo.CronExpandAll(@CronMinute, 0, 59))) 
        RETURN 0;

    IF  NOT (@CronHour = '*' OR DATEPART(HOUR, @Now) IN (SELECT Value FROM dbo.CronExpandAll(@CronHour, 0, 24)))
        RETURN 0;

    IF (@CronDay = 'L') SET @CronDay = DATEPART(DAY, EOMONTH (@Now)) 

    IF  NOT (@CronDay = '*' OR DATEPART(DAY, @Now) IN (SELECT Value FROM dbo.CronExpandAll(@CronDay, 1, 31))) 
        RETURN 0;

    IF  NOT (@CronMonth = '*' OR DATEPART(MONTH, @Now) IN (SELECT Value FROM dbo.CronExpandAll(@CronMonth, 1, 12)))
        RETURN 0;

    IF  NOT (@CronWeekday = '*' OR DATEPART(WEEKDAY, @Now) IN (SELECT Value FROM dbo.CronExpandAll(@CronWeekday, 1, 7)))
        RETURN 0;

EXIT_PROC:
    RETURN 1
END

