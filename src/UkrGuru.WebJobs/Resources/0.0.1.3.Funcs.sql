SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CronExpandAll]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'
CREATE FUNCTION [dbo].[CronExpandAll] (@Expression varchar(100), @Min int, @Max int) 
RETURNS @Values TABLE (Value int)
AS BEGIN
	INSERT @Values (Value) 
	SELECT DISTINCT Value = CONVERT(int, Value) FROM (
		SELECT Value FROM dbo.CronExpandInt(@Expression, @Min, @Max)
		--UNION
		--SELECT DISTINCT Value FROM dbo.CronExpandStar(@Expression, @Min, @Max)
		UNION
		SELECT DISTINCT Value FROM STRING_SPLIT(@Expression, '','')
		WHERE ISNUMERIC(Value) = 1 AND Value BETWEEN @Min AND @Max
		UNION
		SELECT DISTINCT E.Value FROM STRING_SPLIT(@Expression, '','') S
		CROSS APPLY dbo.CronExpandStep(S.value, @Min, @Max) E
		UNION
		SELECT DISTINCT E.Value FROM STRING_SPLIT(@Expression, '','') S
		CROSS APPLY dbo.CronExpandRange(S.Value, @Min, @Max) E
	) AS V
	ORDER BY Value
	
	RETURN
END
' 
END

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CronExpandInt]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'
/*
SELECT * FROM dbo.CronExpandInt(''0'', 0, 59)
*/
CREATE FUNCTION [dbo].[CronExpandInt] (@Expression varchar(100), @Min int, @Max int) 
RETURNS @Values TABLE (Value int)
AS BEGIN
	IF @Expression = ''*'' OR @Min IS NULL OR @Max IS NULL OR @Min > @Max RETURN;
	IF CHARINDEX('','', @Expression, 1) + CHARINDEX(''/'', @Expression, 1) + CHARINDEX(''-'', @Expression, 1) > 0 RETURN;
	IF CONVERT(int, @Expression) < @Min OR CONVERT(int, @Expression) > @Max RETURN;

	INSERT @Values (Value) SELECT Value = CONVERT(int, @Expression)

	RETURN	
END
' 
END

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CronExpandRange]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'

/*
SELECT * FROM [dbo].[CronExpandRange] (''0-15'', 10, 59)
*/
CREATE FUNCTION [dbo].[CronExpandRange] (@Expression varchar(100), @Min int, @Max int) 
RETURNS @Values TABLE (Value int)
AS BEGIN
	IF CHARINDEX(''-'', @Expression, 1) = 0 RETURN;

	DECLARE @From int= dbo.CronWord(@Expression, ''-'', 1)
        ,@To int = dbo.CronWord(@Expression, ''-'', 2)

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
' 
END

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CronExpandStar]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'

CREATE FUNCTION [dbo].[CronExpandStar] (@Expression varchar(100), @Min int, @Max int) 
RETURNS @Values TABLE (Value int)
AS BEGIN
	IF @Expression <> ''*'' OR @Min IS NULL OR @Max IS NULL or @Min > @Max RETURN;

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
' 
END

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CronExpandStep]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'
/*
SELECT * FROM [dbo].[CronExpandStep] (''0/15'', 0, 59)
*/
CREATE FUNCTION [dbo].[CronExpandStep] (@Expression varchar(100), @Min int, @Max int)
RETURNS @Values TABLE (Value int)
AS BEGIN
	IF CHARINDEX(''/'', @Expression, 1) = 0 RETURN;

	DECLARE @Start int = dbo.CronWord(@Expression, ''/'', 1)
        ,@Step int = dbo.CronWord(@Expression, ''/'', 2)

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
' 
END

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CronPart]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'
/*
-- minute, mi, n
-- hour, hh
-- day, dd, d
-- month, mm, m
-- weekday, dw, w

SELECT [dbo].[CronPart](''1 2 3 4 5'', ''yy'')
*/
CREATE FUNCTION [dbo].[CronPart] (@Expression varchar(100), @CronPart varchar(10))
RETURNS varchar(100)
AS
BEGIN
    DECLARE @Result varchar(100);

    DECLARE @PartPos int = CASE
        WHEN @CronPart in (''minute'', ''mi'', ''n'') THEN 1
        WHEN @CronPart in (''hour'', ''hh'') THEN 2
        WHEN @CronPart in (''day'', ''dd'', ''d'') THEN 3
        WHEN @CronPart in (''month'', ''mm'', ''m'') THEN 4
        WHEN @CronPart in (''weekday'', ''dw'', ''w'') THEN 5
        ELSE 0 END

    IF @PartPos = 0 GOTO EXIT_PROC

    DECLARE @CurrPos int = 1
    WHILE CHARINDEX('' '', @Expression) > 0 BEGIN
        IF @CurrPos = @PartPos BEGIN
            SET @Result = SUBSTRING(@Expression, 1, CHARINDEX('' '', @Expression) - 1)
            GOTO EXIT_PROC
        END
        SET @Expression = SUBSTRING(@Expression, CHARINDEX('' '', @Expression) + 1, 100)
        SET @CurrPos = @CurrPos + 1
    END

    SET @Result = SUBSTRING(@Expression, 1, 100)

EXIT_PROC:
    RETURN @Result
END
' 
END

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CronPrepare]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'

CREATE FUNCTION [dbo].[CronPrepare] (@Expression varchar(100)) 
RETURNS varchar(100)
AS BEGIN
	RETURN 
		REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
		REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
		REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
		REPLACE(UPPER(@Expression), ''JAN'', ''1''),
		''FEB'', ''2''),''MAR'', ''3''),''APR'', ''4''),''MAY'', ''5''),''JUN'', ''6''),''JUL'', ''7''),
		''AUG'', ''8''),''SEP'', ''9''),''OCT'', ''10''),''NOV'', ''11''),''DEC'', ''12''),''SUN'', ''1''),
		''MON'', ''2''),''TUE'', ''3''),''WED'', ''4''),''THU'', ''5''),''FRI'', ''6''),''SAT'', ''7'')
END
' 
END

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CronValidate]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'
/*
SELECT [dbo].[CronValidate](''0 17 L * *'', ''2020-11-29 17:00'')
SELECT DATEPART(day, EOMONTH ( GETDATE() )) AS Result;  
*/
CREATE FUNCTION [dbo].[CronValidate] (@Expression varchar(100), @Now datetime)
RETURNS bit
AS
BEGIN
    SET @Now = ISNULL(@Now, GETDATE());
    SET @Expression = dbo.CronPrepare(@Expression);

    DECLARE @CronMinute varchar(100) = dbo.CronPart(@Expression, ''minute'')
        ,@CronHour varchar(100) = dbo.CronPart(@Expression, ''hour'')
        ,@CronDay varchar(100) = dbo.CronPart(@Expression, ''day'')
        ,@CronMonth varchar(100) = dbo.CronPart(@Expression, ''month'')
        ,@CronWeekday varchar(100) = dbo.CronPart(@Expression, ''weekday'')

    IF NOT (@CronMinute = ''*'' OR DATEPART(MINUTE, @Now) IN (SELECT Value FROM dbo.CronExpandAll(@CronMinute, 0, 59))) 
        RETURN 0;

    IF  NOT (@CronHour = ''*'' OR DATEPART(HOUR, @Now) IN (SELECT Value FROM dbo.CronExpandAll(@CronHour, 0, 24)))
        RETURN 0;

    IF (@CronDay = ''L'') SET @CronDay = DATEPART(DAY, EOMONTH (@Now)) 

    IF  NOT (@CronDay = ''*'' OR DATEPART(DAY, @Now) IN (SELECT Value FROM dbo.CronExpandAll(@CronDay, 1, 31))) 
        RETURN 0;

    IF  NOT (@CronMonth = ''*'' OR DATEPART(MONTH, @Now) IN (SELECT Value FROM dbo.CronExpandAll(@CronMonth, 1, 12)))
        RETURN 0;

    IF  NOT (@CronWeekday = ''*'' OR DATEPART(WEEKDAY, @Now) IN (SELECT Value FROM dbo.CronExpandAll(@CronWeekday, 1, 7)))
        RETURN 0;

EXIT_PROC:
    RETURN 1
END
' 
END

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CronWord]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'

/*
SELECT [dbo].[CronWord](''1/12'', ''/'', 2)
*/
CREATE FUNCTION [dbo].[CronWord] (@Expression varchar(100), @Delimiter char(1) = '','', @Index int)
RETURNS varchar(100)
AS
BEGIN
    DECLARE @Word varchar(100);

    DECLARE @CurrIndex int = 1
    WHILE CHARINDEX(@Delimiter, @Expression) > 0 BEGIN
        IF @CurrIndex = @Index BEGIN
            SET @Word = SUBSTRING(@Expression, 1, CHARINDEX(@Delimiter, @Expression) - 1)
            GOTO EXIT_PROC
        END
        SET @Expression = SUBSTRING(@Expression, CHARINDEX(@Delimiter, @Expression) + 1, 100)
        SET @CurrIndex = @CurrIndex + 1
    END

    IF @CurrIndex = @Index
        SET @Word = SUBSTRING(@Expression, 1, 100)

EXIT_PROC:
    RETURN @Word
END
' 
END
