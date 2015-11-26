namespace System.Windows.Forms
{
	using System.ComponentModel;
	
	/// <summary>
	/// Description of FormsExtensions.
	/// </summary>
	public static class FormsExtensions
	{
		public static void InvokeIfNecessary(this Control control, MethodInvoker action)
		{
		    //When the form, thus the control, isn't visible yet, InvokeRequired returns false, resulting still in a cross-thread exception.
		    while (!control.Visible)
		    {
		        System.Threading.Thread.Sleep(50);
		    }
		    if (control.InvokeRequired) {
		        control.Invoke(action);
		    } else {
		        action();
		    }
		}
		
		public static void InvokeIfNecessary(this ISynchronizeInvoke obj, MethodInvoker action)
		{
		    if (obj.InvokeRequired) {
		        var args = new object[0];
		        obj.Invoke(action, args);
		    } else {
		        action();
		    }
		}
	}
}
