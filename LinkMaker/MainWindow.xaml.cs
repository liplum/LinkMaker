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
            CreateFunc();
        }
        catch (LinkExistedException)
        {
            MessageBox.Show(Properties.Resources.LinkHasBeenExistedException, Properties.Resources.Error);
        }
        catch (DriveLetterNotEqualException)
        {
            MessageBox.Show(Properties.Resources.DifferentFromDriveLetterException, Properties.Resources.Error);
        }
        catch (HardLinkIsInapplicableException)
        {
            MessageBox.Show(Properties.Resources.HardLinkIsInapplicableException, Properties.Resources.Error);
        }
        catch (LinkModeNotSelectedException)
        {
            MessageBox.Show(Properties.Resources.NotSelectLinkModeException, Properties.Resources.Error);
        }
        catch (InvalidLinkNameException)
        {
            MessageBox.Show(Properties.Resources.LinkNameIsInvalidException, Properties.Resources.Error);
        }
        catch (InvalidLinkDirectoryNameException)
        {
            MessageBox.Show(Properties.Resources.LinkDirectoryNameIsInvalidException, Properties.Resources.Error);
        }
    }

    private void TargetPath_PreviewDrop(object sender, DragEventArgs e)
    {
        var array = (Array)e.Data.GetData(DataFormats.FileDrop);
        if (array == null || array.Length > 0)
        {
            MessageBox.Show(Properties.Resources.DraggedThingIsNotVaild, Properties.Resources.Error);
            e.Handled = true;
        }
        else
        {
            SetTarget(array.GetValue(0)?.ToString());
        }
    }

    private void TargetPath_PreviewDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void LinkDirectoryName_PreviewDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void LinkDirectoryName_PreviewDrop(object sender, DragEventArgs e)
    {
        var textBox = (TextBox)sender;
        var array = (Array)e.Data.GetData(DataFormats.FileDrop);
        if (array == null)
        {
            MessageBox.Show(Properties.Resources.DraggedThingIsNotVaild, Properties.Resources.Error);
            e.Handled = true;
        }
        else
        {
            textBox.Text = array.GetValue(0)?.ToString() ?? "";
        }
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