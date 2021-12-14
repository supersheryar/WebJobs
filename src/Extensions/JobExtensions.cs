// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace UkrGuru.WebJobs.Data
{
    public static class JobExtensions
    {
        public static dynamic CreateAction(this Job job)
        {
            job.ThrowIfNull(nameof(job));

            var type = Type.GetType(job.ActionType) ?? Type.GetType($"UkrGuru.WebJobs.Actions.{job.ActionType}");

            dynamic action = Activator.CreateInstance(type);

            action.Init(job);

            return action;
        }
    }
}