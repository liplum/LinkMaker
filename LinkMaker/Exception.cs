using System;

namespace LinkMaker;

public class DirectorySymbolicLinkIsInapplicableException : Exception
{
    public DirectorySymbolicLinkIsInapplicableException()
    {
    }

    public DirectorySymbolicLinkIsInapplicableException(string message) : base(message)
    {
    }
}

public class FileSymbolicLinkIsInapplicableException : Exception
{
    public FileSymbolicLinkIsInapplicableException()
    {
    }

    public FileSymbolicLinkIsInapplicableException(string message) : base(message)
    {
    }
}

public class CancelOperationException : Exception
{
    public CancelOperationException()
    {
    }

    public CancelOperationException(string message) : base(message)
    {
    }
}

public class LinkNameIsInvalidException : Exception
{
    public LinkNameIsInvalidException()
    {
    }

    public LinkNameIsInvalidException(string message) : base(message)
    {
    }
}

public class LinkDirectoryNameIsInvalidException : Exception
{
    public LinkDirectoryNameIsInvalidException()
    {
    }

    public LinkDirectoryNameIsInvalidException(string message) : base(message)
    {
    }
}

public class LinkDirectoryIsNotExistedException : Exception
{
    public LinkDirectoryIsNotExistedException()
    {
    }

    public LinkDirectoryIsNotExistedException(string message) : base(message)
    {
    }
}

public class LinkHasBeenExistedException : Exception
{
    public LinkHasBeenExistedException()
    {
    }

    public LinkHasBeenExistedException(string message) : base(message)
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