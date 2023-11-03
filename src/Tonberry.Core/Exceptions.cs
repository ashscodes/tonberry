using System;
using System.Collections.Generic;

namespace Tonberry.Core;

public abstract class TonberryException : Exception
{
    public Dictionary<string, object> Result { get; set; }

    public TonberryException(string message) : base(message) { }
}

public class TonberryApplicationException : TonberryException
{
    public TonberryApplicationException(string message) : base(message) { }

    public TonberryApplicationException(string message, params object[] args) : base(string.Format(message, args)) { }
}

public class TonberryCommitException : TonberryException
{
    public string Branch { get; }

    public string Remote { get; }

    public TonberryCommitException(string remoteName, string branchName)
        : base(string.Format(Resources.NoCommitsFound, remoteName, branchName))
    {
        Branch = branchName;
        Remote = remoteName;
    }
}