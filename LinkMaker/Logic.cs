using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using MakeLinkLib;
using Microsoft.WindowsAPICodePack.Dialogs;
using static MakeLinkLib.LinkHelper;

namespace LinkMaker;

public partial class MainWindow
{
    private void SelectLinkDirectory()
    {
        var linkDir = SelectFolder(Properties.Resources.SelectLinkDirectoryCaption);
        if (linkDir is { Exists: true })
        {
            LinkDirectoryTextBox.Text = linkDir.FullName;
        }
    }

    private void SelectTargetFile()
    {
        var target = SelectFile(Properties.Resources.SelectTargetFileCaption);
        if (target != null)
        {
            SetTarget(target.FullName);
        }
    }

    private void SetTarget(string targetName)
    {
        if (string.IsNullOrEmpty(targetName)) return;
        TargetPathTextBox.Text = targetName;
        LinkNameTextBox.Text = Path.GetFileName(targetName);
    }

    private void SelectTargetFolder()
    {
        var targetDir = SelectFolder(Properties.Resources.SelectTargetFolderCaption);
        if (targetDir != null)
        {
            SetTarget(targetDir.FullName);
        }
    }

    private void ClearLinkName()
    {
        LinkNameTextBox.Text = "";
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="LinkExistedException"></exception>
    /// <exception cref="DriveLetterNotEqualException"></exception>
    /// <exception cref="InvalidLinkNameException"></exception>
    /// <exception cref="InvalidLinkDirectoryNameException"></exception>
    private void CreateLinkOf(LinkMode mode, string linkName, string linkDirPath, string targetPath)
    {
        if (!IsValid(linkName))
        {
            throw new InvalidLinkNameException();
        }

        if (!IsValid(linkDirPath))
        {
            throw new InvalidLinkDirectoryNameException();
        }

        var linkFullName = Path.Join(linkDirPath,linkName);

        if (File.Exists(linkFullName) || Directory.Exists(linkFullName))
        {
            throw new LinkExistedException();
        }

        if (!File.Exists(targetPath) && !Directory.Exists(targetPath))
        {
            throw new TargetNotFoundException();
        }

        if (!Directory.Exists(linkDirPath))
        {
            Directory.CreateDirectory(linkDirPath);
        }

        switch (mode)
        {
            case LinkMode.DirectorySymbolicLink:
                CreateDirectorySymbolicLink(linkFullName, targetPath);
                break;
            case LinkMode.FileSymbolicLink:
                CreateFileSymbolicLink(linkFullName, targetPath);
                break;
            case LinkMode.HardLink:
                CreateHardLink(linkFullName, targetPath);
                break;
            case LinkMode.JunctionLink:
                CreateJunctionLink(linkFullName, targetPath);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    /// <param name="linkFullPath">Its parent directory exists, and itself doesn't exist.</param>
    /// <param name="targetPath">Already exists.</param>
    private void CreateHardLink(string linkFullPath, string targetPath)
    {
        if (!File.Exists(targetPath))
            throw new TargetNotFoundException(targetPath);

        if (Path.GetPathRoot(targetPath) != Path.GetPathRoot(linkFullPath))
            throw new DriveLetterNotEqualException();

        var targetFileExtension = Path.GetExtension(targetPath);
        if (targetFileExtension != Path.GetExtension(linkFullPath))
        {
            var res = MessageBox.Show(
                string.Format(Properties.Resources.ExtensionNotEqual, targetFileExtension),
                Properties.Resources.Tip, MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                targetPath = Path.GetFileNameWithoutExtension(targetPath) + targetFileExtension;
                LinkNameTextBox.Text = linkFullPath;
            }
        }

        MakeLink(
            mode: LinkMode.HardLink,
            linkPath: linkFullPath,
            targetPath: targetPath
        );
        OnLinkCreated();
    }

    /// <param name="linkFullPath">Its parent directory exists, and itself doesn't exist.</param>
    /// <param name="targetPath">Already exists.</param>
    private void CreateFileSymbolicLink(string linkFullPath, string targetPath)
    {
        if (!File.Exists(targetPath))
        {
            throw new TargetFileSystemTypeException
            {
                Requirement = FileSystemType.File
            };
        }

        MakeLink(
            mode: LinkMode.FileSymbolicLink,
            linkPath: linkFullPath,
            targetPath: targetPath
        );
        OnLinkCreated();
    }

    /// <param name="linkFullPath">Its parent directory exists, and itself doesn't exist.</param>
    /// <param name="targetPath">Already exists.</param>
    private void CreateDirectorySymbolicLink(string linkFullPath, string targetPath)
    {
        if (File.Exists(targetPath))
        {
            throw new TargetFileSystemTypeException
            {
                Requirement = FileSystemType.Directory
            };
        }

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        MakeLink(
            mode: LinkMode.DirectorySymbolicLink,
            linkPath: linkFullPath,
            targetPath: targetPath
        );
        OnLinkCreated();
    }

    /// <param name="linkFullPath">Its parent directory exists, and itself doesn't exist.</param>
    /// <param name="targetPath">Already exists.</param>
    private void CreateJunctionLink(string linkFullPath, string targetPath)
    {
        if (File.Exists(targetPath))
        {
            throw new TargetFileSystemTypeException
            {
                Requirement = FileSystemType.Directory
            };
        }

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        MakeLink(
            mode: LinkMode.JunctionLink,
            linkPath: linkFullPath,
            targetPath: targetPath
        );

        OnLinkCreated();
    }

    private void OnLinkCreated()
    {
        TargetPathTextBox.Text = "";
        LinkDirectoryTextBox.Text = "";
        LinkNameTextBox.Text = "";
        MessageBox.Show(Properties.Resources.LinkCreatedTip);
    }

    [GeneratedRegex("^[\\/:*?\"<>|]$")]
    private static partial Regex LinkNameValidator();

    private static bool IsValid(string linkName)
    {
        if (string.IsNullOrWhiteSpace(linkName)) return false;

        return LinkNameValidator().Matches(linkName).Count == 0;
    }

    private static DirectoryInfo? SelectFolder(string caption)
    {
        using var dialog = new CommonOpenFileDialog(caption)
        {
            IsFolderPicker = true
        };

        return dialog.ShowDialog() == CommonFileDialogResult.Ok ? new DirectoryInfo(dialog.FileName) : null;
    }

    private static FileInfo? SelectFile(string caption)
    {
        using var dialog = new CommonOpenFileDialog(caption);

        return dialog.ShowDialog() == CommonFileDialogResult.Ok ? new FileInfo(dialog.FileName) : null;
    }
}