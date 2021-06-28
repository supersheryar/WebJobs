EXEC dbo.sp_executesql @statement = N'
UPDATE WJbActions
SET ActionType = ''CustomActions.YourSqlProcAction, CustomActions''
WHERE ActionId = 1000
';