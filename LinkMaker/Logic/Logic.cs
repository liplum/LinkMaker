using MakeLinkLib;
using Microsoft.WindowsAPICodePack.Dialogs;
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
        void SelectTargetFuc()
        {
            var target = SelectFile(Properties.Resources.SelectTargetPathCaption);
            if (target != null && target.Exists)
            {
                TargetPath.Text = target.FullName;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="LinkHasBeenExistedException"></exception>
        /// <exception cref="DifferentFromDriveLetterException"></exception>
        /// <exception cref="HardLinkIsInapplicableExcption"></exception>
        /// <exception cref="JunctionLinkIsInapplicableExcption"></exception>
        /// <exception cref="TargetOrLinkIsErrorExcption"></exception>
        /// <exception cref="NotSelectLinkModeException"></exception>
        /// <exception cref="LinkDirectroyIsNotExistedException"></exception>
        /// <exception cref="LinkNameIsInvalidException"></exception>
        /// 
        void CreateFunc()
        {
            if (IsValid(LinkName.Text))
            {
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

                if (linkDir.Exists)//所要创建的链接，它所在的文件夹存在时
                {
                    //主体开始


                    LinkMode link;
                    //当选择Hard Link时，此时需要两者都是文件
                    if (HardLinkButton.IsChecked == true)
                    {
                        if (ifTargetFile.Exists)
                        {
                            if (GetDriveLetter(ifTargetFile.FullName) == GetDriveLetter(ifLinkFile.FullName))
                            {
                                link = LinkMode.H;
                                string TargetFileExtension = ifTargetFile.Extension;
                                if (TargetFileExtension != ifLinkFile.Extension)
                                {
                                    var res = MessageBox.Show($"{Properties.Resources.ExtensionIsNotSame_1}\"{TargetFileExtension}\"{Properties.Resources.ExtensionIsNotSame_2}", Properties.Resources.Tip, MessageBoxButton.OKCancel);
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
                            throw new HardLinkIsInapplicableExcption(TargetPath.Text);
                        }

                    }
                    //当选择Junction Link时
                    else if (JunctionLinkButton.IsChecked == true)
                    {
                        //判断目标地址是否是文件夹，此时需要两者都是目录
                        if (ifTargetDir.Exists)
                        {
                            link = LinkMode.J;
                        }
                        else
                        {
                            throw new JunctionLinkIsInapplicableExcption(TargetPath.Text);
                        }
                    }
                    //当选择Symbolic Link时，此时需要分为：文件符号链接 与 目录符号链接
                    else if (SymbolicLinkButton.IsChecked == true)
                    {
                        //当目标地址是文件时
                        if (ifTargetFile.Exists)
                        {
                            link = LinkMode.Dfile;
                        }
                        //当目标地址是文件夹时
                        else if (ifTargetDir.Exists)
                        {
                            link = LinkMode.Ddir;
                        }
                        else
                        {
                            throw new TargetOrLinkIsErrorExcption(TargetPath.Text);
                        }
                    }
                    //此时没有选择Mode
                    else
                    {
                        throw new NotSelectLinkModeException();
                    }

                    //运行CMD
                    var cmd = new MkLink(link, linkFullName, TargetPath.Text);
                    cmd.Run();


                    //主体结束
                }
                else
                {
                    throw new LinkDirectroyIsNotExistedException(LinkDirectoryName.Text);
                }
            }
            else
            {
                throw new LinkNameIsInvalidException();
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

        public class HardLinkIsInapplicableExcption : Exception
        {
            public HardLinkIsInapplicableExcption()
            {

            }
            public HardLinkIsInapplicableExcption(string message) : base(message)
            {

            }
        }

        public class JunctionLinkIsInapplicableExcption : Exception
        {
            public JunctionLinkIsInapplicableExcption()
            {

            }
            public JunctionLinkIsInapplicableExcption(string message) : base(message)
            {

            }
        }

        public class TargetOrLinkIsErrorExcption : Exception
        {
            public TargetOrLinkIsErrorExcption()
            {

            }
            public TargetOrLinkIsErrorExcption(string message) : base(message)
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
