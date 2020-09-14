using MakeLinkLib;
using Microsoft.WindowsAPICodePack.Dialogs;
using PhotosCategorier.Algorithm;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using static MakeLinkLib.MkLinkEumn;

namespace LinkMaker
{
    public partial class MainWindow
    {
        void SelectLinkDirectoryFuc()
        {
            var linkDir = SelectFolder(Properties.Resources.SelectLinkDirectoryCaption);
            if (linkDir != null && linkDir.Exists)
            {
                LinkDirectoryName.Text = linkDir.FullName;
            }
        }

        void SelectTargetFileFuc()
        {
            var target = SelectFile(Properties.Resources.SelectTargetFileCaption);
            SetTarget(target.FullName);
        }

        void SetTarget(string targetName)
        {
            if (!string.IsNullOrEmpty(targetName))
            {
                TargetPath.Text = targetName;
                AutoGenerateName();
            }
        }

        void SelectTargetFolderFuc()
        {
            var targetDir = SelectFolder(Properties.Resources.SelectTargetFolderCaption);
            SetTarget(targetDir.FullName);
        }

        void ClearLinkName()
        {
            LinkName.Text = "";
        }

        void CanSelectFile()
        {
            SelectTargetPath.IsEnabled = true;
            SelectTargetPath.Click -= SelectTargetPath_Click_File;
            SelectTargetPath.Click -= SelectTargetPath_Click_Folder;
            SelectTargetPath.Click += SelectTargetPath_Click_File;
            SelectTargetPath.Content = Properties.Resources.SelectTargetFileButton;
        }

        void CanSelectDircetory()
        {
            SelectTargetPath.IsEnabled = true;
            SelectTargetPath.Click -= SelectTargetPath_Click_Folder;
            SelectTargetPath.Click -= SelectTargetPath_Click_File;
            SelectTargetPath.Click += SelectTargetPath_Click_Folder;
            SelectTargetPath.Content = Properties.Resources.SelectTargetFolderButton;
        }

        void AutoGenerateName()
        {
            string lastName = TargetPath.Text.GetLastName();
            if (lastName == null)
            {
                return;
            }
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
        /// <exception cref="HardLinkIsInapplicableExcption"></exception>
        /// <exception cref="TargetNeitherFileNorDirectoryExcption"></exception>
        /// <exception cref="NotSelectLinkModeException"></exception>
        /// <exception cref="LinkDirectroyIsNotExistedException"></exception>
        /// <exception cref="LinkNameIsInvalidException"></exception>
        /// <exception cref="CancelOperationException"></exception>
        /// <exception cref="DirectorySymbolicLinkIsInapplicableExcption"></exception>
        /// <exception cref="FileSymbolicLinkIsInapplicableExcption"></exception>
        /// <exception cref="LinkDirectoryNameIsInvalidException"></exception>
        void CreateFunc()
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

            if (ifLinkIsDir.Exists || ifLinkFile.Exists)//当所要创建的链接已经存在的时候
            {
                throw new LinkHasBeenExistedException();
            }

            if (!linkDir.Exists)//所要创建的链接，它所在的文件夹不存在时
            {
                throw new LinkDirectroyIsNotExistedException(LinkDirectoryName.Text);
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
                        link = LinkMode.H;
                        string TargetFileExtension = ifTargetFile.Extension;
                        if (TargetFileExtension != ifLinkFile.Extension)
                        {
                            var res = MessageBox.Show(string.Format(Properties.Resources.ExtensionIsNotSame, TargetFileExtension), Properties.Resources.Tip, MessageBoxButton.OKCancel);
                            if (res == MessageBoxResult.OK)
                            {
                                LinkName.Text += $"{TargetFileExtension}";
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
                    var res = MessageBox.Show($"{Properties.Resources.TargetHasNotExistedYet}\n{Properties.Resources.WhetherMkDirTarget}", Properties.Resources.Tip, MessageBoxButton.OKCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        ifTargetDir.Create();
                    }
                }
                link = LinkMode.J;
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
                    var res = MessageBox.Show($"{Properties.Resources.TargetHasNotExistedYet}\n{Properties.Resources.WhetherMkDirTarget}", Properties.Resources.Tip, MessageBoxButton.OKCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        ifTargetDir.Create();
                    }
                }
                link = LinkMode.Ddir;
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
                    var res = MessageBox.Show($"{Properties.Resources.TargetHasNotExistedYet}\n{Properties.Resources.WhetherContinue}", Properties.Resources.Tip, MessageBoxButton.OKCancel);
                    if (res != MessageBoxResult.OK)
                    {
                        throw new CancelOperationException();
                    }
                }
                link = LinkMode.Dfile;
            }
            //此时没有选择Mode
            else
            {
                throw new NotSelectLinkModeException();
            }

            //运行CMD
            var cmd = new MkLink(link, linkFullName, TargetPath.Text);
            cmd.Run();
        }
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

        public class LinkDirectroyIsNotExistedException : Exception
        {
            public LinkDirectroyIsNotExistedException()
            {

            }
            public LinkDirectroyIsNotExistedException(string message) : base(message)
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

        public class TargetNeitherFileNorDirectoryException : Exception
        {
            public TargetNeitherFileNorDirectoryException()
            {

            }
            public TargetNeitherFileNorDirectoryException(string message) : base(message)
            {

            }
        }

        public class NotSelectLinkModeException : Exception
        {
            public NotSelectLinkModeException()
            {

            }

            public NotSelectLinkModeException(string message) : base(message)
            {

            }
        }

        bool IsValid(string linkName)
        {
            if (string.IsNullOrWhiteSpace(linkName))
                return false;

            if (Regex.Matches(linkName, "^[\\/:*?\"<>|]$").Count != 0)
            {
                return false;
            }

            return true;
        }

        string GetDriveLetter(string fileFullName)
        {
            return Regex.Match(fileFullName, @"^[c-zC-Z]").Value;
        }

        private static DirectoryInfo SelectFolder(string Caption)
        {
            using var dialog = new CommonOpenFileDialog(Caption)
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return new DirectoryInfo(dialog.FileName);
            }
            return null;
        }

        private static FileInfo SelectFile(string Caption)
        {
            using var dialog = new CommonOpenFileDialog(Caption);

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return new FileInfo(dialog.FileName);
            }
            return null;
        }
    }
}
