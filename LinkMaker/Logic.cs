using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using MakeLinkLib;
using Microsoft.WindowsAPICodePack.Dialogs;
using static MakeLinkLib.MkLinkEnum;

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
        SelectTargetPath.Content = Properties.Resources.SelectTargetFileButton;
    }

    private void CanSelectDirectory()
    {
        SelectTargetPath.IsEnabled = true;
        SelectTargetPath.Click -= SelectTargetPath_Click_Folder;
        SelectTargetPath.Click -= SelectTargetPath_Click_File;
        SelectTargetPath.Click += SelectTargetPath_Click_Folder;
        SelectTargetPath.Content = Properties.Resources.SelectTargetFolderButton;
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
    /// <exception cref="LinkHasBeenExistedException"></exception>
    /// <exception cref="DifferentFromDriveLetterException"></exception>
    /// <exception cref="LinkModeNotSelectedException"></exception>
    /// <exception cref="LinkDirectoryIsNotExistedException"></exception>
    /// <exception cref="LinkNameIsInvalidException"></exception>
    /// <exception cref="CancelOperationException"></exception>
    /// <exception cref="LinkDirectoryNameIsInvalidException"></exception>
    private void CreateFunc()
    {
        if (!IsValid(LinkName.Text))
        {
            throw new LinkNameIsInvalidException();
        }

        if (!IsValid(LinkDirectoryName.Text))
        {
            throw new LinkDirectoryNameIsInvalidException();
        }

        var linkFullName = $@"{LinkDirectoryName.Text}\{LinkName.Text}";
        var linkDir = new DirectoryInfo(LinkDirectoryName.Text);

        var ifLinkIsDir = new DirectoryInfo(linkFullName);
        var ifLinkFile = new FileInfo(linkFullName);

        var ifTargetFile = new FileInfo(TargetPath.Text);
        var ifTargetDir = new DirectoryInfo(TargetPath.Text);

        if (ifLinkIsDir.Exists || ifLinkFile.Exists) //当所要创建的链接已经存在的时候
        {
            throw new LinkHasBeenExistedException();
        }

        if (!linkDir.Exists) //所要创建的链接，它所在的文件夹不存在时
        {
            throw new LinkDirectoryIsNotExistedException(LinkDirectoryName.Text);
        }

        LinkMode link;

        //当选择Hard Link时，此时需要两者都是文件
        if (HardLinkButton.IsChecked == true)
        {
            //硬链接要求目标文件存在
            if (ifTargetFile.Exists)
            {
                if (GetDriveLetter(ifTargetFile.FullName) == GetDriveLetter(ifLinkFile.FullName))
                {
                    link = LinkMode.HardLink;
                    var targetFileExtension = ifTargetFile.Extension;
                    if (targetFileExtension != ifLinkFile.Extension)
                    {
                        var res = MessageBox.Show(
                            string.Format(Properties.Resources.ExtensionIsNotSame, targetFileExtension),
                            Properties.Resources.Tip, MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            LinkName.Text += $"{targetFileExtension}";
                            linkFullName = $@"{LinkDirectoryName.Text}\{LinkName.Text}";
                        }
                    }
                }
                else
                {
                    throw new DifferentFromDriveLetterException();
                }
            }
            else
            {
                throw new HardLinkIsInapplicableException(TargetPath.Text);
            }
        }
        //当选择Junction Link时
        else if (JunctionLinkButton.IsChecked == true)
        {
            //判断目标地址是否是文件夹，此时需要两者都是目录
            if (!ifTargetDir.Exists)
            {
                var res = MessageBox.Show(
                    $"{Properties.Resources.TargetHasNotExistedYet}\n{Properties.Resources.WhetherMkDirTarget}",
                    Properties.Resources.Tip, MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    ifTargetDir.Create();
                }
            }

            link = LinkMode.JunctionLink;
        }
        //当选择Directory Symbolic Link时
        else if (DirectorySymbolicLinkButton.IsChecked == true)
        {
            if (ifTargetFile.Exists)
            {
                throw new DirectorySymbolicLinkIsInapplicableException();
            }

            //当目标地址是文件夹时
            if (!ifTargetDir.Exists)
            {
                var res = MessageBox.Show(
                    $"{Properties.Resources.TargetHasNotExistedYet}\n{Properties.Resources.WhetherMkDirTarget}",
                    Properties.Resources.Tip, MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    ifTargetDir.Create();
                }
            }

            link = LinkMode.DirectorySymbolLink;
        }
        //当选择File Symbolic Link时
        else if (FileSymbolicLinkButton.IsChecked == true)
        {
            if (ifTargetDir.Exists)
            {
                throw new FileSymbolicLinkIsInapplicableException();
            }

            if (!ifTargetFile.Exists)
            {
                var res = MessageBox.Show(
                    $"{Properties.Resources.TargetHasNotExistedYet}\n{Properties.Resources.WhetherContinue}",
                    Properties.Resources.Tip, MessageBoxButton.OKCancel);
                if (res != MessageBoxResult.OK)
                {
                    throw new CancelOperationException();
                }
            }

            link = LinkMode.FileSymbolLink;
        }
        else
        {
            throw new LinkModeNotSelectedException();
        }

        // Run the command
        var cmd = new MkLink
        {
            Mode = link,
            Link = linkFullName,
            Target = TargetPath.Text
        };
        cmd.Run();
    }

    [GeneratedRegex("^[\\/:*?\"<>|]$")]
    private static partial Regex LinkNameValidator();

    private static bool IsValid(string linkName)
    {
        if (string.IsNullOrWhiteSpace(linkName)) return false;

        return LinkNameValidator().Matches(linkName).Count == 0;
    }

    [GeneratedRegex("^[c-zC-Z]")]
    private static partial Regex DriveLetterRegex();

    private static string GetDriveLetter(string fileFullName)
    {
        return DriveLetterRegex().Match(fileFullName).Value;
    }

    private static DirectoryInfo SelectFolder(string caption)
    {
        using var dialog = new CommonOpenFileDialog(caption)
        {
            IsFolderPicker = true
        };

        return dialog.ShowDialog() == CommonFileDialogResult.Ok ? new DirectoryInfo(dialog.FileName) : null;
    }

    private static FileInfo SelectFile(string Caption)
    {
        using var dialog = new CommonOpenFileDialog(Caption);

        return dialog.ShowDialog() == CommonFileDialogResult.Ok ? new FileInfo(dialog.FileName) : null;
    }

}