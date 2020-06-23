﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
        /// <param name="window">The owning window with a container of the child window.</param>
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
        public static Task ShowChildWindowAsync(this Window window, ChildWindow dialog, OverlayFillBehavior overlayFillBehavior = OverlayFillBehavior.WindowContent)
        {
            return window.ShowChildWindowAsync<object>(dialog, overlayFillBehavior);
        }

        /// <summary>
        /// Shows the given child window on the MetroWindow dialog container in an asynchronous way.
        /// When the dialog was closed it returns a result.
        /// </summary>
        /// <param name="window">The owning window with a container of the child window.</param>
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
        public static async Task<TResult> ShowChildWindowAsync<TResult>(this Window window, ChildWindow dialog, OverlayFillBehavior overlayFillBehavior = OverlayFillBehavior.WindowContent)
        {
            window.Dispatcher.VerifyAccess();

            var metroDialogContainer = window.Template.FindName("PART_MetroActiveDialogContainer", window) as Grid;
            metroDialogContainer = metroDialogContainer ?? window.Template.FindName("PART_MetroInactiveDialogsContainer", window) as Grid;
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

            return await ShowChildWindowInternalAsync<TResult>(dialog, metroDialogContainer);
        }

        /// <summary>
        /// Shows the given child window on the given container in an asynchronous way.
        /// When the dialog was closed it returns a result.
        /// </summary>
        /// <param name="window">The owning window with a container of the child window.</param>
        /// <param name="dialog">A child window instance.</param>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// The provided child window can not add, there is no container defined.
        /// or
        /// The provided child window is already visible in the specified window.
        /// </exception>
        public static Task ShowChildWindowAsync(this Window window, ChildWindow dialog, Panel container)
        {
            return window.ShowChildWindowAsync<object>(dialog, container);
        }

        /// <summary>
        /// Shows the given child window on the given container in an asynchronous way.
        /// </summary>
        /// <param name="window">The owning window with a container of the child window.</param>
        /// <param name="dialog">A child window instance.</param>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// The provided child window can not add, there is no container defined.
        /// or
        /// The provided child window is already visible in the specified window.
        /// </exception>
        public static async Task<TResult> ShowChildWindowAsync<TResult>(this Window window, ChildWindow dialog, Panel container)
        {
            window.Dispatcher.VerifyAccess();

            if (container == null)
            {
                throw new InvalidOperationException("The provided child window can not add, there is no container defined.");
            }

            if (container.Children.Contains(dialog))
            {
                throw new InvalidOperationException("The provided child window is already visible in the specified window.");
            }

            return await ShowChildWindowInternalAsync<TResult>(dialog, container);
        }

        private static async Task<TResult> ShowChildWindowInternalAsync<TResult>(ChildWindow dialog, Panel container)
        {
            container.Children.Add(dialog);

            return await OpenDialogAsync<TResult>(dialog, container);
        }

        private static async Task<TResult> OpenDialogAsync<TResult>(ChildWindow dialog, Panel container)
        {
            var tcs = new TaskCompletionSource<TResult>();

            void OnDialogClosingFinished(object sender, RoutedEventArgs args)
            {
                dialog.ClosingFinished -= OnDialogClosingFinished;
                container.Children.Remove(dialog);
                tcs.TrySetResult(dialog.ChildWindowResult is TResult result ? result : (dialog.ClosedBy is TResult closedBy ? closedBy : default));
            }

            dialog.ClosingFinished += OnDialogClosingFinished;

            dialog.SetCurrentValue(ChildWindow.IsOpenProperty, true);

            return await tcs.Task;
        }
    }
}