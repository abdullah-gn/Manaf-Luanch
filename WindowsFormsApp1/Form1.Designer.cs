using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	partial class Form1
	{
		private System.ComponentModel.IContainer components = null;
		private Label lblTitle;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.lblTitle = new System.Windows.Forms.Label();
			// Initialize all controls
			this.pnlContainer = new DoubleBufferedPanel();
			this.pnlDays = new DoubleBufferedPanel();
			this.pnlHours = new DoubleBufferedPanel();
			this.pnlMinutes = new DoubleBufferedPanel();
			this.pnlSeconds = new DoubleBufferedPanel();

			this.lblDays = new System.Windows.Forms.Label();
			this.lblHours = new System.Windows.Forms.Label();
			this.lblMinutes = new System.Windows.Forms.Label();
			this.lblSeconds = new System.Windows.Forms.Label();

			this.lblDaysText = new System.Windows.Forms.Label();
			this.lblHoursText = new System.Windows.Forms.Label();
			this.lblMinutesText = new System.Windows.Forms.Label();
			this.lblSecondsText = new System.Windows.Forms.Label();

			// Container Panel - adjusted for rectangular shape
			this.pnlContainer.Size = new System.Drawing.Size(800, 200);
			this.pnlContainer.BackColor = System.Drawing.Color.Transparent;
			

			// Setup all panels
			SetupPanel(this.pnlDays);
			SetupPanel(this.pnlHours);
			SetupPanel(this.pnlMinutes);
			SetupPanel(this.pnlSeconds);

			// Setup number labels
			SetupNumberLabel(this.lblDays);
			SetupNumberLabel(this.lblHours);
			SetupNumberLabel(this.lblMinutes);
			SetupNumberLabel(this.lblSeconds);

			// Setup text labels with Arabic text
			SetupTextLabel(this.lblDaysText, "يوم");
			SetupTextLabel(this.lblHoursText, "ساعة");
			SetupTextLabel(this.lblMinutesText, "دقيقة");
			SetupTextLabel(this.lblSecondsText, "ثانية");

			this.lblTitle.AutoSize = false;
			this.lblTitle.Size = new System.Drawing.Size(600, 200);
			this.lblTitle.Font = new System.Drawing.Font("Georgia ", 80F, System.Drawing.FontStyle.Bold);
			this.lblTitle.Text = "إنطلاقة منـاف";
			this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
			this.lblTitle.ForeColor = Color.FromArgb(129, 172, 149);
			this.lblTitle.BackColor = System.Drawing.Color.Transparent;
			this.lblTitle.RightToLeft = RightToLeft.Yes;
			this.Controls.Add(this.lblTitle);

			// Form settings
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1920,1080);
			this.Text = "Countdown Timer";
			this.Controls.Add(this.pnlContainer);
			this.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			
		}

		private void SetupPanel(Panel panel)
		{
			panel.Size = new System.Drawing.Size(160, 160);  // Larger circles
			panel.BackColor = System.Drawing.Color.Transparent;
		}

		private void SetupNumberLabel(Label label)
		{
			label.Size = new System.Drawing.Size(140, 60);  // Increased height
			label.Font = new System.Drawing.Font("Arial", 42F, System.Drawing.FontStyle.Bold);  // Bigger font
			label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			label.BackColor = System.Drawing.Color.Transparent;
			label.ForeColor = System.Drawing.Color.Black;
		}

		private void SetupTextLabel(Label label, string text)
		{
			label.Size = new System.Drawing.Size(140, 40);  // Increased height
			label.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);  // Bigger font
			label.Text = text;
			label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			label.BackColor = System.Drawing.Color.Transparent;
			label.ForeColor = System.Drawing.Color.Black;
		}

		private void CenterTitle()
		{
			lblTitle.Location = new Point(
				(this.ClientSize.Width - lblTitle.Width) / 2,
				50  // 50 pixels from top
			);
		}


		private DoubleBufferedPanel pnlContainer;
		private DoubleBufferedPanel pnlDays;
		private DoubleBufferedPanel pnlHours;
		private DoubleBufferedPanel pnlMinutes;
		private DoubleBufferedPanel pnlSeconds;
		private System.Windows.Forms.Label lblDays;
		private System.Windows.Forms.Label lblHours;
		private System.Windows.Forms.Label lblMinutes;
		private System.Windows.Forms.Label lblSeconds;
		private System.Windows.Forms.Label lblDaysText;
		private System.Windows.Forms.Label lblHoursText;
		private System.Windows.Forms.Label lblMinutesText;
		private System.Windows.Forms.Label lblSecondsText;
	}
}
