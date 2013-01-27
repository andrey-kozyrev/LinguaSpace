using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using LinguaSpace.Common;
using LinguaSpace.Common.Resources;

namespace LinguaSpace.StringMask
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtArray;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtVariable;
		private System.Windows.Forms.TextBox txtLiteral;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtArray = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtVariable = new System.Windows.Forms.TextBox();
			this.txtLiteral = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(240, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Literal:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "&Definition:";
			// 
			// txtArray
			// 
			this.txtArray.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtArray.Location = new System.Drawing.Point(80, 46);
			this.txtArray.Multiline = true;
			this.txtArray.Name = "txtArray";
			this.txtArray.Size = new System.Drawing.Size(584, 90);
			this.txtArray.TabIndex = 3;
			this.txtArray.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "&Variable:";
			// 
			// txtVariable
			// 
			this.txtVariable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtVariable.Location = new System.Drawing.Point(80, 14);
			this.txtVariable.Name = "txtVariable";
			this.txtVariable.Size = new System.Drawing.Size(136, 20);
			this.txtVariable.TabIndex = 5;
			this.txtVariable.Text = "";
			this.txtVariable.TextChanged += new System.EventHandler(this.txtSource_TextChanged);
			// 
			// txtLiteral
			// 
			this.txtLiteral.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtLiteral.Location = new System.Drawing.Point(288, 14);
			this.txtLiteral.Name = "txtLiteral";
			this.txtLiteral.Size = new System.Drawing.Size(376, 20);
			this.txtLiteral.TabIndex = 6;
			this.txtLiteral.Text = "";
			this.txtLiteral.TextChanged += new System.EventHandler(this.txtSource_TextChanged);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(680, 151);
			this.Controls.Add(this.txtVariable);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtArray);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtLiteral);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.Text = "String Masker";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

		private void txtSource_TextChanged(object sender, System.EventArgs e)
		{
			txtVariable.Text = txtLiteral.Text.ToUpper();
			
			String[] lines = new String[4];
			lines[0] = "/// <summary>";
			lines[1] = "///" + txtLiteral.Text;
			lines[2] = "/// </summary>";
			lines[3] = "public static StringBox " + txtVariable.Text + " = MakeStringBox(new ushort[] " + StringRes.EncodeArray(txtLiteral.Text) + ");";
			txtArray.Lines = lines;
		}

	}
}
