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
    /// <exception cref="DifferentFromDriveLetterException"></exception>
    /// <exception cref="LinkModeNotSelectedException"></exception>
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

        if (File.Exists(linkFullName))
        {
            throw new LinkExistedException();
        }

        if (!Directory.Exists(linkDirPath)) //所要创建的链接，它所在的文件夹不存在时
        {
            Directory.CreateDirectory(linkDirPath);
        }

        LinkMode link;

        //当选择Hard Link时，此时需要两者都是文件
        if (HardLinkButton.IsChecked == true)
        {
            //硬链接要求目标文件存在
            if (File.Exists(targetPath))
            {
                if (Path.GetPathRoot(targetPath) == Path.GetPathRoot(linkFullName))
                {
                    link = LinkMode.HardLink;
                    var targetFileExtension = Path.GetExtension(targetPath);
                    if (targetFileExtension != Path.GetExtension(linkFullName))
                    {
                        var res = MessageBox.Show(
                            string.Format(Properties.Resources.ExtensionIsNotSame, targetFileExtension),
                            Properties.Resources.Tip, MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            LinkName.Text += $"{targetFileExtension}";
                            linkFullName = $@"{linkDirPath}\{linkName}";
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
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            link = LinkMode.JunctionLink;
        }
        //当选择Directory Symbolic Link时
        else if (DirectorySymbolicLinkButton.IsChecked == true)
        {
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            link = LinkMode.DirectorySymbolLink;
        }
        //当选择File Symbolic Link时
        else if (FileSymbolicLinkButton.IsChecked == true)
        {
            if (!File.Exists(targetPath))
            {
                var res = MessageBox.Show(
                    $"{Properties.Resources.TargetHasNotExistedYet}\n{Properties.Resources.WhetherContinue}",
                    Properties.Resources.Tip,
                    MessageBoxButton.OKCancel
                );
                if (res != MessageBoxResult.OK)
                {
                    return;
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