using System.Reflection;
using System.Runtime.InteropServices;

namespace Jatetxea.Desktop.UI;

/// <summary>
/// Parpadeorik gabeko (flicker-free) errendimendu-laguntzaile profesionalak WinForms-erako:
/// - DoubleBuffering errekurtsiboa edukiontzi guztietan (FlowLayoutPanel, Panel, TableLayoutPanel, etab.)
/// - Win32 WM_SETREDRAW (BeginUpdate / EndUpdate) aldaketa masiboetarako
/// - AutoScroll posizioaren gordetze automatikoa freskatzeetan
/// </summary>
public static class UiPerf
{
    private const int WM_SETREDRAW = 0x000B;

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    private static readonly PropertyInfo? DoubleBufferedProp =
        typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// Kontrol bati eta bere azpikontrol guztiei DoubleBuffered = true aktibatzen die,
    /// eta etorkizunean dinamikoki gehitzen diren kontrolei ere automatikoki aplikatzen die.
    /// </summary>
    public static void EnableDoubleBuffering(Control root)
    {
        ApplyDoubleBufferSingle(root);

        foreach (Control child in root.Controls)
        {
            EnableDoubleBuffering(child);
        }

        // Dinamikoki sortutako txartel eta panelek ere DoubleBuffered izan dezaten
        root.ControlAdded -= OnControlAddedEnableDoubleBuffer;
        root.ControlAdded += OnControlAddedEnableDoubleBuffer;
    }

    private static void OnControlAddedEnableDoubleBuffer(object? sender, ControlEventArgs e)
    {
        if (e.Control != null)
        {
            EnableDoubleBuffering(e.Control);
        }
    }

    private static void ApplyDoubleBufferSingle(Control ctrl)
    {
        if (ctrl is TextBoxBase or ComboBox or DateTimePicker or NumericUpDown)
        {
            return;
        }

        try
        {
            DoubleBufferedProp?.SetValue(ctrl, true, null);
        }
        catch
        {
            // Kontrol batzuek ez badute onartzen, isilean jarraitu
        }
    }

    /// <summary>
    /// Edukiontzi baten margotzea (WM_SETREDRAW) aldi baterako izozten du, ekintza exekutatzen du,
    /// scroll posizioa mantentzen du, eta bukaeran behin bakarrik margotzen du parpadeorik gabe.
    /// </summary>
    public static void RunFlickerFree(Control container, Action action, bool preserveScroll = true)
    {
        if (container.IsDisposed) return;

        Point? savedScroll = null;
        if (preserveScroll && container is ScrollableControl sc && sc.AutoScroll)
        {
            // AutoScrollPosition negatiboa da; berrezartzean positibo bihurtu behar da
            savedScroll = new Point(-sc.AutoScrollPosition.X, -sc.AutoScrollPosition.Y);
        }

        bool canLockRedraw = container.IsHandleCreated && container.Visible;
        if (canLockRedraw)
        {
            SendMessage(container.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }

        container.SuspendLayout();
        try
        {
            action();
        }
        finally
        {
            container.ResumeLayout(true);

            if (savedScroll.HasValue && container is ScrollableControl scRestore && !scRestore.IsDisposed)
            {
                scRestore.AutoScrollPosition = savedScroll.Value;
            }

            if (canLockRedraw && !container.IsDisposed)
            {
                SendMessage(container.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                container.Invalidate(true);
                container.Update();
            }
        }
    }
}
