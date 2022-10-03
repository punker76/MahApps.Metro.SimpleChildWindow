using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.SimpleChildWindow
{
    /// <summary>
    /// A static class to show ChildWindow's
    /// </summary>
    public static class ChildWindowManager
    {
        /// <summary>
        /// An enumeration to control the fill behavior of the behavior
        /// </summary>
        public enum OverlayFillBehavior
        {
            /// <summary>
            /// The overlay covers the full window
            /// </summary>
            FullWindow,

            /// <summary>
            /// The overlay covers only then window content, so the window title bar is useable
            /// </summary>
            WindowContent
        }

        /// <summary>
        /// Shows the given child window on the MetroWindow dialog container in an asynchronous way.
        /// </summary>
        /// <param name="control">The owning control with a container for the child window.</param>
        /// <param name="dialog">A child window instance.</param>
        /// <param name="overlayFillBehavior">The overlay fill behavior.</param>
        /// <returns>
        /// A task representing the operation.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The provided child window can not add, the container can not be found.
        /// or
        /// The provided child window is already visible in the specified window.
        /// </exception>
        public static Task ShowChildWindowAsync(this Control control, ChildWindow dialog, OverlayFillBehavior overlayFillBehavior = OverlayFillBehavior.WindowContent)
        {
            return control.ShowChildWindowAsync<object>(dialog, overlayFillBehavior);
        }

        /// <summary>
        /// Shows the given child window on the MetroWindow dialog container in an asynchronous way.
        /// When the dialog was closed it returns a result.
        /// </summary>
        /// <param name="control">The owning control with a container for the child window.</param>
        /// <param name="dialog">A child window instance.</param>
        /// <param name="overlayFillBehavior">The overlay fill behavior.</param>
        /// <returns>
        /// A task representing the operation.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The provided child window can not add, the container can not be found.
        /// or
        /// The provided child window is already visible in the specified window.
        /// </exception>
        public static async Task<TResult> ShowChildWindowAsync<TResult>(this Control control, ChildWindow dialog, OverlayFillBehavior overlayFillBehavior = OverlayFillBehavior.WindowContent)
        {
            var tcs = new TaskCompletionSource<TResult>();

            control.Dispatcher.VerifyAccess();

            var metroDialogContainer = control.Template.FindName("PART_MetroActiveDialogContainer", control) as Grid;
            metroDialogContainer = metroDialogContainer ?? control.Template.FindName("PART_MetroInactiveDialogsContainer", control) as Grid;
            if (metroDialogContainer == null)
            {
                throw new InvalidOperationException("The provided child window can not add, there is no container defined.");
            }

            if (metroDialogContainer.Children.Contains(dialog))
            {
                throw new InvalidOperationException("The provided child window is already visible in the specified window.");
            }

            if (overlayFillBehavior == OverlayFillBehavior.WindowContent)
            {
                metroDialogContainer.SetCurrentValue(Grid.RowProperty, (int)metroDialogContainer.GetValue(Grid.RowProperty) + 1);
                metroDialogContainer.SetCurrentValue(Grid.RowSpanProperty, 1);
            }

            return await OpenDialogAsync(dialog, metroDialogContainer, tcs);
        }

        /// <summary>
        /// Shows the given child window on the given container in an asynchronous way.
        /// When the dialog was closed it returns a result.
        /// </summary>
        /// <param name="control">The owning control with a container for the child window.</param>
        /// <param name="dialog">A child window instance.</param>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// The provided child window can not add, there is no container defined.
        /// or
        /// The provided child window is already visible in the specified window.
        /// </exception>
        public static Task ShowChildWindowAsync(this Control control, ChildWindow dialog, Panel container)
        {
            return control.ShowChildWindowAsync<object>(dialog, container);
        }

        /// <summary>
        /// Shows the given child window on the given container in an asynchronous way.
        /// </summary>
        /// <param name="control">The owning control with a container for the child window.</param>
        /// <param name="dialog">A child window instance.</param>
        /// <param name="container">The container.</param>
        /// <returns />
        /// <exception cref="System.InvalidOperationException">
        /// The provided child window can not add, there is no container defined.
        /// or
        /// The provided child window is already visible in the specified window.
        /// </exception>
        public static async Task<TResult> ShowChildWindowAsync<TResult>(this Control control, ChildWindow dialog, Panel container)
        {
            var tcs = new TaskCompletionSource<TResult>();

            control.Dispatcher.VerifyAccess();

            if (container == null)
            {
                throw new InvalidOperationException("The provided child window can not add, there is no container defined.");
            }

            if (container.Children.Contains(dialog))
            {
                throw new InvalidOperationException("The provided child window is already visible in the specified window.");
            }

            return await OpenDialogAsync(dialog, container, tcs);
        }

        private static async Task<TResult> OpenDialogAsync<TResult>(ChildWindow dialog, Panel container, TaskCompletionSource<TResult> tcs)
        {
            if (!dialog.IsOpen)
            {
                if (dialog.TryFindParent<Panel>() is null)
                {
                    container.Children.Add(dialog);
                }

                void OnDialogClosingFinished(object sender, RoutedEventArgs args)
                {
                    dialog.ClosingFinished -= OnDialogClosingFinished;
                    container.Children.Remove(dialog);
                    tcs.TrySetResult(dialog.ChildWindowResult is TResult result ? result : (dialog.ClosedBy is TResult closedBy ? closedBy : default));
                }

                dialog.ClosingFinished += OnDialogClosingFinished;

                dialog.SetCurrentValue(ChildWindow.IsOpenProperty, true);
            }

            return await tcs.Task;
        }
    }
}