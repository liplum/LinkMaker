using System;
using System.Windows;
using System.Windows.Controls;

namespace LinkMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            catch (LinkHasBeenExistedException)
            {
                MessageBox.Show(Properties.Resources.LinkHasBeenExistedException, Properties.Resources.Error);
            }
            catch (DifferentFromDriveLetterException)
            {
                MessageBox.Show(Properties.Resources.DifferentFromDriveLetterException, Properties.Resources.Error);
            }
            catch (HardLinkIsInapplicableException)
            {
                MessageBox.Show(Properties.Resources.HardLinkIsInapplicableException, Properties.Resources.Error);
            }
            catch (TargetNeitherFileNorDirectoryException)
            {
                MessageBox.Show(Properties.Resources.TargetNeitherFileNorDirectoryException, Properties.Resources.Error);
            }
            catch (LinkModeNotSelectedException)
            {
                MessageBox.Show(Properties.Resources.NotSelectLinkModeException, Properties.Resources.Error);
            }
            catch (LinkDirectoryIsNotExistedException)
            {
                MessageBox.Show(Properties.Resources.LinkDirectroyIsNotExistedException, Properties.Resources.Error);
            }
            catch (LinkNameIsInvalidException)
            {
                MessageBox.Show(Properties.Resources.LinkNameIsInvalidException, Properties.Resources.Error);
            }
            catch (CancelOperationException)
            {
                ;
            }
            catch (FileSymbolicLinkIsInapplicableException)
            {
                MessageBox.Show(Properties.Resources.FileSymbolicLinkIsInapplicableException, Properties.Resources.Error);
            }
            catch (DirectorySymbolicLinkIsInapplicableException)
            {
                MessageBox.Show(Properties.Resources.DirectorySymbolicLinkIsInapplicableException, Properties.Resources.Error);
            }
            catch (LinkDirectoryNameIsInvalidException)
            {
                MessageBox.Show(Properties.Resources.LinkDirectoryNameIsInvalidException, Properties.Resources.Error);

            }
        }

        private void TargetPath_PreviewDrop(object sender, DragEventArgs e)
        {

            var textbox = (TextBox)sender;
            var array = (Array)e.Data.GetData(DataFormats.FileDrop);
            if (array == null)
            {
                MessageBox.Show(Properties.Resources.DraggedThingIsNotVaild, Properties.Resources.Error);
                e.Handled = true;
            }
            else
            {
                SetTarget(array.GetValue(0).ToString());
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
            var textbox = (TextBox)sender;
            var array = (Array)e.Data.GetData(DataFormats.FileDrop);
            if (array == null)
            {
                MessageBox.Show(Properties.Resources.DraggedThingIsNotVaild, Properties.Resources.Error);
                e.Handled = true;
            }
            else
            {
                textbox.Text = array.GetValue(0).ToString();
            }
        }

        private void DirectorySymbolicLinkButton_Checked(object sender, RoutedEventArgs e)
        {
            CanSelectDirectory();
        }

        private void FileSymbolicLinkButton_Checked(object sender, RoutedEventArgs e)
        {
            CanSelectFile();
        }

        private void JunctionLinkButton_Checked(object sender, RoutedEventArgs e)
        {
            CanSelectDirectory();
        }

        private void HardLinkButton_Checked(object sender, RoutedEventArgs e)
        {
            CanSelectFile();
        }

        private void ClearTheLinkName_Click(object sender, RoutedEventArgs e)
        {
            ClearLinkName();
        }
    }
}
