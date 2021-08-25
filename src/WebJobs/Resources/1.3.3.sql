UPDATE WJbRules
SET RuleName = N'Send Template Rule', RulePriority = 2, ActionId = 3, RuleMore = N'{
  "template_subject": "Hello DEAR_NAME",
  "tvalue_DEAR_NAME": "John",
  "next": "5",
  "next_to": "test@test.test"
}', Disabled = 0
WHERE RuleId = 6