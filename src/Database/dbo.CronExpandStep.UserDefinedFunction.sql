SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
SELECT * FROM [dbo].[CronExpandStep] ('0/15', 0, 59)
*/
CREATE FUNCTION [dbo].[CronExpandStep] (@Expression varchar(100), @Min int, @Max int)
RETURNS @Values TABLE (Value int)
AS BEGIN
	IF CHARINDEX('/', @Expression, 1) = 0 RETURN;

	DECLARE @Start int = dbo.CronWord(@Expression, '/', 1)
        ,@Step int = dbo.CronWord(@Expression, '/', 2)

	IF @Start IS NULL OR @Step IS NULL OR @Start < @Min RETURN;

	WITH Stepper(Value) AS (
		SELECT @Start AS Value
		UNION ALL
		SELECT Value + @Step FROM Stepper
		WHERE Value + @Step <= @Max
	)
	INSERT @Values (Value) 
    SELECT Value FROM Stepper

	RETURN	
END
GO
