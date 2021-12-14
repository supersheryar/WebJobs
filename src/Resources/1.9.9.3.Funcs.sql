EXEC dbo.sp_executesql @statement = N'
-- ==============================================================
-- Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
-- ==============================================================
CREATE OR ALTER FUNCTION [dbo].[CronValidate] (@Expression varchar(100), @Now datetime)
RETURNS bit
AS
BEGIN
    SET @Expression =   REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
		                REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
		                REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(
                        UPPER(@Expression), 
		                ''JAN'', ''1''),''FEB'', ''2''),''MAR'', ''3''),''APR'', ''4''),''MAY'', ''5''),''JUN'', ''6''),
		                ''JUL'', ''7''),''AUG'', ''8''),''SEP'', ''9''),''OCT'', ''10''),''NOV'', ''11''),''DEC'', ''12''),
		                ''SUN'', ''0''),''MON'', ''1''),''TUE'', ''2''),''WED'', ''3''),''THU'', ''4''),''FRI'', ''5''),''SAT'', ''6'')

    IF @Expression LIKE ''%[^0-9*,-/ ]%'' RETURN 0

    IF dbo.CronValidatePart(dbo.CronWord(@Expression, '' '', 1), DATEPART(MINUTE, @Now), 0, 59) = 0 RETURN 0;

    IF dbo.CronValidatePart(dbo.CronWord(@Expression, '' '', 2), DATEPART(HOUR, @Now), 0, 23) = 0 RETURN 0;

    IF dbo.CronValidatePart(dbo.CronWord(@Expression, '' '', 3), DATEPART(DAY, @Now), 1, 31) = 0 RETURN 0;

    IF dbo.CronValidatePart(dbo.CronWord(@Expression, '' '', 4), DATEPART(MONTH, @Now), 1, 12) = 0 RETURN 0;

    IF dbo.CronValidatePart(dbo.CronWord(@Expression, '' '', 5), dbo.CronWeekDay(@Now), 0, 6) = 0 RETURN 0;

    RETURN 1
END
';
EXEC dbo.sp_executesql @statement = N'
-- ==============================================================
-- Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
-- ==============================================================
CREATE OR ALTER FUNCTION [dbo].[CronValidatePart](@Expression varchar(100), @Value int, @Min int, @Max int)
RETURNS tinyint
AS
BEGIN
    IF @Expression LIKE ''%[^0-9*,-/]%'' RETURN 0
    IF @Value IS NULL OR @Min IS NULL OR @Max IS NULL OR NOT @Value BETWEEN @Min AND @Max RETURN 0  

    IF @Expression = ''*'' RETURN 1

    IF CHARINDEX('','', @Expression, 0) > 0 BEGIN
        RETURN (SELECT MAX(dbo.CronValidatePart(value, @Value, @Min, @Max)) 
            FROM STRING_SPLIT(@Expression, '','') 
            WHERE LEN(value) > 0);
    END

    IF CHARINDEX(''-'', @Expression, 0) > 0 
        RETURN dbo.CronValidateRange(@Expression, @Value, @Min, @Max)

    ELSE IF CHARINDEX(''/'', @Expression, 0) > 0 
        RETURN dbo.CronValidateStep(@Expression, @Value, @Min, @Max)

    ELSE IF TRY_CAST(@Expression as int) = @Value 
        RETURN 1

    RETURN 0
END
';
EXEC dbo.sp_executesql @statement = N'
-- ==============================================================
-- Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
-- ==============================================================
CREATE OR ALTER FUNCTION [dbo].[CronValidateRange](@Expression varchar(100), @Value int, @Min int, @Max int)
RETURNS tinyint
AS
BEGIN
    IF @Expression LIKE ''%[^0-9*-/]%'' RETURN 0
    IF @Value IS NULL OR @Min IS NULL OR @Max IS NULL OR NOT @Value BETWEEN @Min AND @Max RETURN 0  

    DECLARE @Begin int, @End int, @Step int

    IF CHARINDEX(''/'', @Expression, 0) > 0 BEGIN
        SET @Step = TRY_CAST(dbo.CronWord(@Expression, ''/'', 2) as int);
        IF ISNULL(@Step, 0) <= 0 RETURN 0;

        SET @Expression = dbo.CronWord(@Expression, ''/'', 1);
    END

    SET @Begin = TRY_CAST(dbo.CronWord(@Expression, ''-'', 1) as int);
    SET @End = TRY_CAST(dbo.CronWord(@Expression, ''-'', 2) as int);
    SET @Step = ISNULL(@Step, 1);

    IF @Begin IS NULL OR @End IS NULL RETURN 0;

    DECLARE @i int = @Begin, @OneMore int = CASE WHEN @End < @Begin THEN 1 ELSE 0 END 
    
    IF @OneMore = 0 AND @End < @Max SET @Max = @End 

    WHILE @i <= @Max BEGIN

        IF @i = @Value RETURN 1

        SET @i += @Step;

        IF @i > @Max AND @OneMore = 1 BEGIN
            SET @i -= (@Max + 1);
            SET @Max = @End
            SET @OneMore = 0; 
        END
    END
   
    RETURN 0
END
';
EXEC dbo.sp_executesql @statement = N'
-- ==============================================================
-- Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
-- ==============================================================
CREATE OR ALTER FUNCTION [dbo].[CronValidateStep](@Expression varchar(100), @Value int, @Min int, @Max int)
RETURNS tinyint
AS
BEGIN
    IF @Expression LIKE ''%[^0-9*/]%'' RETURN 0
    IF @Value IS NULL OR @Min IS NULL OR @Max IS NULL OR NOT @Value BETWEEN @Min AND @Max RETURN 0  

    DECLARE @Start int = ISNULL(TRY_CAST(dbo.CronWord(@Expression, ''/'', 1) as int), 0),
        @Step int = TRY_CAST(dbo.CronWord(@Expression, ''/'', 2) as int);

    IF @Start IS NULL OR @Step IS NULL OR ISNULL(@Step, 0) <= 0 RETURN 0;

    DECLARE @i int = @Start;
    WHILE @i <= @Max 
    BEGIN
        IF @i = @Value RETURN 1;
            
        SET @i += @Step;
    END

    RETURN 0
END
';
EXEC dbo.sp_executesql @statement = N'
-- ==============================================================
-- Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
-- ==============================================================
CREATE OR ALTER FUNCTION [dbo].[CronWeekDay](@Now datetime)
RETURNS int
AS
BEGIN
    RETURN (DATEPART(weekday, @Now) + @@DATEFIRST + 6) % 7
END
';
EXEC dbo.sp_executesql @statement = N'
-- ==============================================================
-- Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
-- ==============================================================
CREATE OR ALTER FUNCTION [dbo].[CronWord](@Expression varchar(100), @Separator char(1) = '' '', @Index int)
RETURNS varchar(100)
AS
BEGIN
    DECLARE @i int = 0, @v varchar(100);

    SELECT @i = @i + 1, @v = CASE WHEN @i <= @Index THEN value ELSE @v END   
    FROM STRING_SPLIT(@Expression, @Separator)
    WHERE LEN(value) > 0

    RETURN CASE WHEN @i < @Index THEN NULL ELSE @v END
END
';