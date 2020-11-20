namespace MahApps.Metro.SimpleChildWindow
{
    /// <summary>
    /// ChildWindow close reasons.
    /// </summary>
    public enum CloseReason
    {
        None,
        /// <summary>
        /// The dialog was closed automatically.
        /// </summary>
        AutoClose,
        /// <summary>
        /// The Dialog was closed by hitting the overlay.
        /// </summary>
        Overlay,
        /// <summary>
        /// The Dialog was closed by e.g. an Ok button (optional, can be used when it's necessary).
        /// </summary>
        Ok,
        /// <summary>
        /// The Dialog was closed by e.g. an Apply button (optional, can be used when it's necessary).
        /// </summary>
        Apply,
        /// <summary>
        /// The Dialog was closed by e.g. a Cancel button (optional, can be used when it's necessary).
        /// </summary>
        Cancel,
        /// <summary>
        /// The Dialog was closed by the Close method or the Close button on the title bar.
        /// </summary>
        Close,
        /// <summary>
        /// The Dialog was closed by the Escape key.
        /// </summary>
        Escape
    }
}