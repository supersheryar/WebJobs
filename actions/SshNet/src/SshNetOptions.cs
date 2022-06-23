// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Renci.SshNet;

namespace UkrGuru.WebJobs.Actions.SshNet;

public class SshNetOptions
{
    public string Host { get; set; }
    public int Port { get; set; } = 22;
    public string UserName { get; set; }
    public string Password { get; set; }
    public PrivateKeyFile[] KeyFiles { get; set; }
    public string FingerPrint { get; set; }
}