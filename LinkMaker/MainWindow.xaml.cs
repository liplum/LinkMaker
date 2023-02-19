using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MakeLinkLib;

namespace LinkMaker;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        CurrentMode = LinkMode.HardLink;
    }

    private LinkMode _currentMode;

    public LinkMode CurrentMode
    {
        get => _currentMode;
        set
        {
            _currentMode = value;
            switch (value)
            {
                case LinkMode.DirectorySymbolicLink:
                    OnlyCheck(DirectorySymbolicLinkButton);
                    break;
                case LinkMode.FileSymbolicLink:
                    OnlyCheck(FileSymbolicLinkButton);
                    break;
                case LinkMode.HardLink:
                    OnlyCheck(HardLinkButton);
                    break;
                case LinkMode.JunctionLink:
                    OnlyCheck(JunctionLinkButton);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

    private void OnlyCheck(ToggleButton checkedOne)
    {
        var buttons = new[] { HardLinkButton, DirectorySymbolicLinkButton, FileSymbolicLinkButton, JunctionLinkButton };
        foreach (var button in buttons)
        {
            button.IsChecked = button == checkedOne;
        }
    }

    private void SelectLinkDirectory_Click(object sender, RoutedEventArgs e)
    {
        SelectLinkDirectoryFuc();
    }

    private void SelectTargetPath_Click_File(object sender, RoutedEventArgs e)
    {
        SelectTargetFileFuc();
    }

    private void SelectTargetPath_Click_Folder(object sender, RoutedEventArgs e)
    {
        SelectTargetFolderFuc();
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            CreateLinkOf(
                mode: CurrentMode,
                linkName: LinkName.Text,
                linkDirPath: LinkDirectoryName.Text,
                targetPath: TargetPath.Text
            );
        }
        catch (LinkExistedException)
        {
            MessageBox.Show(Properties.Resources.LinkExistedException, Properties.Resources.Error);
        }
        catch (DriveLetterNotEqualException)
        {
            MessageBox.Show(Properties.Resources.DriveLetterNotEqualException, Properties.Resources.Error);
        }
        catch (TargetNotFoundException)
        {
            MessageBox.Show(Properties.Resources.TargetNotFoundException, Properties.Resources.Error);
        }
        catch (InvalidLinkNameException)
        {
            MessageBox.Show(Properties.Resources.InvalidLinkNameException, Properties.Resources.Error);
        }
        catch (InvalidLinkDirectoryNameException)
        {
            MessageBox.Show(Properties.Resources.InvalidLinkDirectoryNameException, Properties.Resources.Error);
        }
        catch (TargetFileSystemTypeException ex)
        {
            MessageBox.Show(
                ex.Requirement == FileSystemType.File
                    ? Properties.Resources.TargetRequireFileException
                    : Properties.Resources.TargetRequireDirectoryException, Properties.Resources.Error);
        }
    }

    private void TargetPath_PreviewDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is Array { Length: > 0 } array)
        {
            var first = array.GetValue(0);
            if (first != null)
            {
                SetTarget(first.ToString()!);
            }
        }
        else
        {
            MessageBox.Show(Properties.Resources.InvalidDraggedItem, Properties.Resources.Error);
        }

        e.Handled = true;
    }

    private void TargetPath_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;
        e.Handled = true;
    }

    private void LinkDirectoryName_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;
    }

    private void LinkDirectoryName_PreviewDrop(object sender, DragEventArgs e)
    {
        var textBox = (TextBox)sender;
        if (e.Data.GetData(DataFormats.FileDrop) is Array array)
        {
            textBox.Text = array.GetValue(0)?.ToString() ?? "";
        }
        else
        {
            MessageBox.Show(Properties.Resources.InvalidDraggedItem, Properties.Resources.Error);
        }

        e.Handled = true;
    }

    private void DirectorySymbolicLinkButton_Checked(object sender, RoutedEventArgs e)
    {
        CurrentMode = LinkMode.DirectorySymbolicLink;
        CanSelectDirectory();
    }

    private void FileSymbolicLinkButton_Checked(object sender, RoutedEventArgs e)
    {
        CurrentMode = LinkMode.FileSymbolicLink;
        CanSelectFile();
    }

    private void JunctionLinkButton_Checked(object sender, RoutedEventArgs e)
    {
        CurrentMode = LinkMode.JunctionLink;
        CanSelectDirectory();
    }

    private void HardLinkButton_Checked(object sender, RoutedEventArgs e)
    {
        CurrentMode = LinkMode.HardLink;
        CanSelectFile();
    }

    private void ClearTheLinkName_Click(object sender, RoutedEventArgs e)
    {
        ClearLinkName();
    }
}