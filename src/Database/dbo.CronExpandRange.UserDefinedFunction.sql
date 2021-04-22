SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON


/*
SELECT * FROM [dbo].[CronExpandRange] ('0-15', 10, 59)
*/
CREATE FUNCTION [dbo].[CronExpandRange] (@Expression varchar(100), @Min int, @Max int) 
RETURNS @Values TABLE (Value int)
AS BEGIN
	IF CHARINDEX('-', @Expression, 1) = 0 RETURN;

	DECLARE @From int= dbo.CronWord(@Expression, '-', 1)
        ,@To int = dbo.CronWord(@Expression, '-', 2)

	IF @From IS NULL OR @To IS NULL RETURN;

    IF @From < @Min SET @From = @Min
    IF @To > @Max SET @To = @Max

    IF @From > @To RETURN;

	WITH Ranger(Value) AS (
		SELECT @From AS Value
		UNION ALL
		SELECT Value + 1 FROM Ranger
		WHERE Value + 1 <= @To
	)
	INSERT @Values (Value) 
    SELECT Value FROM Ranger

	RETURN	
END

