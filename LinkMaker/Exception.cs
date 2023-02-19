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

public class DriveLetterNotEqualException : Exception
{
}

public class TargetNotFoundException : Exception
{
    public TargetNotFoundException()
    {
    }

    public TargetNotFoundException(string message) : base(message)
    {
    }
}

public enum FileSystemType
{
    File,
    Directory
}

public class TargetFileSystemTypeException : Exception
{
    public FileSystemType Requirement { get; init; }
}