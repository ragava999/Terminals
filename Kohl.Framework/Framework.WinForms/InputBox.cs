using System;

namespace System.Windows.Forms
{
	/// <summary>
	/// Description of InputBox.
	/// </summary>
	public static class InputBox
	{
		public static DialogResult Show(ref string input, string caption = "Dialog", char? passwordChar = null)
	    {
	        System.Drawing.Size size = new System.Drawing.Size(300, 70);
	        Form inputBox = new Form();
	
	        inputBox.FormBorderStyle = FormBorderStyle.FixedDialog;
	        inputBox.ClientSize = size;
	        inputBox.Text = caption;
	
	        TextBox textBox = new TextBox();
	        textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
	        textBox.Location = new System.Drawing.Point(5, 5);
	        textBox.Text = input;
	        if (passwordChar.HasValue)
	        	textBox.PasswordChar = passwordChar.Value;
	        inputBox.Controls.Add(textBox);
	
	        Button okButton = new Button();
	        okButton.DialogResult = DialogResult.OK;
	        okButton.Name = "okButton";
	        okButton.Size = new System.Drawing.Size(75, 23);
	        okButton.Text = "&OK";
	        okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
	        inputBox.Controls.Add(okButton);
	
	        Button cancelButton = new Button();
	        cancelButton.DialogResult = DialogResult.Cancel;
	        cancelButton.Name = "cancelButton";
	        cancelButton.Size = new System.Drawing.Size(75, 23);
	        cancelButton.Text = "&Cancel";
	        cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
	        inputBox.Controls.Add(cancelButton);
	
	        inputBox.AcceptButton = okButton;
	        inputBox.CancelButton = cancelButton; 
	        inputBox.StartPosition = FormStartPosition.CenterParent;
	
	        DialogResult result = inputBox.ShowDialog();
	        input = textBox.Text;
	        return result;
	    }
	}
}
