using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using MakeLinkLib;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace LinkMaker;

public partial class MainWindow
{
    private void SelectLinkDirectoryFuc()
    {
        var linkDir = SelectFolder(Properties.Resources.SelectLinkDirectoryCaption);
        if (linkDir is { Exists: true })
        {
            LinkDirectoryName.Text = linkDir.FullName;
        }
    }

    private void SelectTargetFileFuc()
    {
        var target = SelectFile(Properties.Resources.SelectTargetFileCaption);
        SetTarget(target.FullName);
    }

    private void SetTarget(string targetName)
    {
        if (string.IsNullOrEmpty(targetName)) return;
        TargetPath.Text = targetName;
        AutoGenerateName();
    }

    private void SelectTargetFolderFuc()
    {
        var targetDir = SelectFolder(Properties.Resources.SelectTargetFolderCaption);
        SetTarget(targetDir.FullName);
    }

    private void ClearLinkName()
    {
        LinkName.Text = "";
    }

    private void CanSelectFile()
    {
        SelectTargetPath.IsEnabled = true;
        SelectTargetPath.Click -= SelectTargetPath_Click_File;
        SelectTargetPath.Click -= SelectTargetPath_Click_Folder;
        SelectTargetPath.Click += SelectTargetPath_Click_File;
        SelectTargetPath.Content = Properties.Resources.SelectFileButton;
    }

    private void CanSelectDirectory()
    {
        SelectTargetPath.IsEnabled = true;
        SelectTargetPath.Click -= SelectTargetPath_Click_Folder;
        SelectTargetPath.Click -= SelectTargetPath_Click_File;
        SelectTargetPath.Click += SelectTargetPath_Click_Folder;
        SelectTargetPath.Content = Properties.Resources.SelectFolderButton;
    }

    private void AutoGenerateName()
    {
        var parts = TargetPath.Text.Split('\\');
        var lastName = parts[^1];
        if (!lastName.Equals(LinkName.Text))
        {
            LinkName.Text = lastName;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="LinkExistedException"></exception>
    /// <exception cref="DriveLetterNotEqualException"></exception>
    /// <exception cref="InvalidLinkNameException"></exception>
    /// <exception cref="InvalidLinkDirectoryNameException"></exception>
    private void CreateFunc()
    {
        var linkName = LinkName.Text;
        var linkDirPath = LinkDirectoryName.Text;
        var targetPath = TargetPath.Text;

        if (!IsValid(linkName))
        {
            throw new InvalidLinkNameException();
        }

        if (!IsValid(linkDirPath))
        {
            throw new InvalidLinkDirectoryNameException();
        }

        var linkFullName = $@"{linkDirPath}\{linkName}";

        if (File.Exists(linkFullName) || Directory.Exists(linkFullName))
        {
            throw new LinkExistedException();
        }

        if (!File.Exists(linkFullName) && !Directory.Exists(linkFullName))
        {
            throw new TargetNotFoundException();
        }

        if (!Directory.Exists(linkDirPath))
        {
            Directory.CreateDirectory(linkDirPath);
        }

        switch (CurrentMode)
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
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CreateHardLink(string linkFullName, string targetPath)
    {
        if (!File.Exists(targetPath))
            throw new TargetNotFoundException(targetPath);
        
        if (Path.GetPathRoot(targetPath) != Path.GetPathRoot(linkFullName))
            throw new DriveLetterNotEqualException();
        
        var targetFileExtension = Path.GetExtension(targetPath);
        if (targetFileExtension == Path.GetExtension(linkFullName)) return;
        
        var res = MessageBox.Show(
            string.Format(Properties.Resources.ExtensionNotEqual, targetFileExtension),
            Properties.Resources.Tip, MessageBoxButton.OKCancel);
        if (res == MessageBoxResult.OK)
        {
            linkFullName = @$"{linkFullName}\{targetFileExtension}";
            LinkName.Text = linkFullName;
        }


        new MkLink
        {
            Mode = LinkMode.HardLink,
            Link = linkFullName,
            Target = targetPath
        }.Run();
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

        new MkLink
        {
            Mode = LinkMode.DirectorySymbolicLink,
            Link = linkFullPath,
            Target = targetPath
        }.Run();
    }

    private void CreateDirectorySymbolicLink(string linkFullName, string targetPath)
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

        new MkLink
        {
            Mode = LinkMode.FileSymbolicLink,
            Link = linkFullName,
            Target = targetPath
        }.Run();
    }

    private void CreateJunctionLink(string linkFullName, string targetPath)
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

        new MkLink
        {
            Mode = LinkMode.JunctionLink,
            Link = linkFullName,
            Target = targetPath
        }.Run();
    }

    [GeneratedRegex("^[\\/:*?\"<>|]$")]
    private static partial Regex LinkNameValidator();

    private static bool IsValid(string linkName)
    {
        if (string.IsNullOrWhiteSpace(linkName)) return false;

        return LinkNameValidator().Matches(linkName).Count == 0;
    }

    private static DirectoryInfo SelectFolder(string caption)
    {
        using var dialog = new CommonOpenFileDialog(caption)
        {
            IsFolderPicker = true
        };

        return dialog.ShowDialog() == CommonFileDialogResult.Ok ? new DirectoryInfo(dialog.FileName) : null;
    }

    private static FileInfo SelectFile(string caption)
    {
        using var dialog = new CommonOpenFileDialog(caption);

        return dialog.ShowDialog() == CommonFileDialogResult.Ok ? new FileInfo(dialog.FileName) : null;
    }
}