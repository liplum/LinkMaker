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

        private void SelectTargetPath_Click(object sender, RoutedEventArgs e)
        {
            SelectTargetFuc();
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
            catch (HardLinkIsInapplicableExcption)
            {
                MessageBox.Show(Properties.Resources.HardLinkIsInapplicableExcption, Properties.Resources.Error);
            }
            catch (JunctionLinkIsInapplicableExcption)
            {
                MessageBox.Show(Properties.Resources.JunctionLinkIsInapplicableExcption, Properties.Resources.Error);
            }
            catch (TargetOrLinkIsErrorExcption)
            {
                MessageBox.Show(Properties.Resources.TargetOrLinkIsErrorExcption, Properties.Resources.Error);
            }
            catch (NotSelectLinkModeException)
            {
                MessageBox.Show(Properties.Resources.NotSelectLinkModeException, Properties.Resources.Error);
            }
            catch (LinkDirectroyIsNotExistedException)
            {
                MessageBox.Show(Properties.Resources.LinkDirectroyIsNotExistedException, Properties.Resources.Error);
            }
            catch (LinkNameIsInvalidException)
            {
                MessageBox.Show(Properties.Resources.LinkNameIsInvalidException, Properties.Resources.Error);
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
                textbox.Text = array.GetValue(0).ToString();
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
    }
}
