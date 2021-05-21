SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/*
SELECT [dbo].[CronWord]('1/12', '/', 2)
*/
CREATE FUNCTION [dbo].[CronWord] (@Expression varchar(100), @Delimiter char(1) = ',', @Index int)
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
GO
