using System;

namespace LinkMaker;

public class InvalidLinkNameException : Exception
{
    public InvalidLinkNameException()
    {
    }

    public InvalidLinkNameException(string message) : base(message)
    {
    }
}

public class InvalidLinkDirectoryNameException : Exception
{
    public InvalidLinkDirectoryNameException()
    {
    }

    public InvalidLinkDirectoryNameException(string message) : base(message)
    {
    }
}

public class LinkExistedException : Exception
{
    public LinkExistedException()
    {
    }

    public LinkExistedException(string message) : base(message)
    {
    }
}

public class DifferentFromDriveLetterException : Exception
{
    public DifferentFromDriveLetterException()
    {
    }

    public DifferentFromDriveLetterException(string message) : base(message)
    {
    }
}

public class HardLinkIsInapplicableException : Exception
{
    public HardLinkIsInapplicableException()
    {
    }

    public HardLinkIsInapplicableException(string message) : base(message)
    {
    }
}

public class LinkModeNotSelectedException : Exception
{
    public LinkModeNotSelectedException()
    {
    }

    public LinkModeNotSelectedException(string message) : base(message)
    {
    }
}