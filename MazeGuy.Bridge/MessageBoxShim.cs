namespace System.Windows.Forms
{
    public enum MessageBoxButtons { OK, YesNo }
    public enum DialogResult { Yes, No, OK }

    public static class MessageBox
    {
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            if (buttons == MessageBoxButtons.YesNo)
            {
                return Bridge.Html5.Global.Confirm(text) ? DialogResult.Yes : DialogResult.No;
            }
            else
            {
                Bridge.Html5.Global.Alert(text);
                return DialogResult.OK;
            }
        }
    }
}
