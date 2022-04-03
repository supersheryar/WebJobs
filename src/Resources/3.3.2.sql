UPDATE [dbo].[WJbActions] 
SET [ActionType] = N'RunApiProcAction, UkrGuru.WebJobs',
	[ActionMore] = N'{
  "api_settings_name": "",
  "proc": "",
  "data": null,
  "body": null,
  "timeout": null,
  "result_name": null
}'
WHERE [ActionId] = 5
AND [ActionType] = N'RunApiProcAction, UkrGuru.WebJobs.Actions'