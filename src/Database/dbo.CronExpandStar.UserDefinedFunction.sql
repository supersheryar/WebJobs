SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON


CREATE FUNCTION [dbo].[CronExpandStar] (@Expression varchar(100), @Min int, @Max int) 
RETURNS @Values TABLE (Value int)
AS BEGIN
	IF @Expression <> '*' OR @Min IS NULL OR @Max IS NULL or @Min > @Max RETURN;

	WITH Starter(Value) AS (
		SELECT @Min AS Value
		UNION ALL
		SELECT Value + 1 FROM Starter
		WHERE Value + 1 <= @Max
	)
	INSERT @Values (Value) 
    SELECT Value FROM Starter

	RETURN	
END

