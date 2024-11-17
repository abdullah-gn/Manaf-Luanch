using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{
		private bool isUpdating = false;
		private readonly object updateLock = new object();
		private Timer timer;
		private DateTime targetDate;
		private Timer hoverTimer;
		private bool isHoveringEdge;
		private Button closeButton;
		private readonly Color[] progressColors = new Color[]
		{
			Color.FromArgb(255, 46, 204, 113),  // Brighter green
			Color.FromArgb(255, 46, 204, 113),
			Color.FromArgb(255, 46, 204, 113),
			Color.FromArgb(255, 46, 204, 113)
		};

		//private readonly Dictionary<Panel, int> panelColorIndices;
		private readonly Dictionary<DoubleBufferedPanel, int> panelColorIndices;
		private const int CIRCLE_THICKNESS = 8;  // Reduced from 15
		private const float CIRCLE_COMPLETION_THICKNESS = 10;  // Reduced from 20
		private const int ANIMATION_STEPS = 100;
		private float currentRotation = 0;
		private BufferedGraphics bufferedGraphics;
		private BufferedGraphicsContext context;

		public Form1()
		{
			// Set double buffering before InitializeComponent
			SetStyle(ControlStyles.OptimizedDoubleBuffer |
					ControlStyles.AllPaintingInWmPaint |
					ControlStyles.UserPaint, true);
			this.DoubleBuffered = true;


			InitializeComponent();

			// Initialize other variables
			targetDate = new DateTime(2024, 12, 31, 23, 59, 59);
			panelColorIndices = new Dictionary<DoubleBufferedPanel, int>();
			context = BufferedGraphicsManager.Current;

			// Setup controls and background
			SetupControls();
			SetupTimer();
			SetupBackgroundImage();
			CenterTitle();

			// Handle the Load event to ensure the form is ready
			this.Load += (s, e) => UpdateBufferedGraphics();

			// Set the FormBorderStyle to None
			this.FormBorderStyle = FormBorderStyle.None;


			// Initialize the hover timer
			hoverTimer = new Timer();
			hoverTimer.Interval = 100; // Check every 100 ms
			hoverTimer.Tick += HoverTimer_Tick;
			hoverTimer.Start();

			// Initialize the close button but keep it hidden initially
			closeButton = new Button();
			closeButton.Size = new Size(30, 30);
			closeButton.Text = "X";
			closeButton.Font = new Font("Arial", 10, FontStyle.Bold);
			closeButton.ForeColor = Color.White;
			closeButton.BackColor = Color.Red;
			closeButton.FlatStyle = FlatStyle.Flat;
			closeButton.FlatAppearance.BorderSize = 0;
			closeButton.Visible = false; // Initially hidden
			closeButton.Click += (s, e) => this.Close(); // Close form on click

			// Add the close button to the form's controls
			this.Controls.Add(closeButton);

		}

		private void HoverTimer_Tick(object sender, EventArgs e)
		{
			// Check if the mouse is near the form’s edges
			var mousePos = this.PointToClient(Cursor.Position);
			int hoverMargin = 5; // Edge proximity threshold

			isHoveringEdge = mousePos.X <= hoverMargin || mousePos.X >= this.Width - hoverMargin ||
							 mousePos.Y <= hoverMargin || mousePos.Y >= this.Height - hoverMargin;

			// Toggle visibility of the close button and redraw based on hover state
			closeButton.Visible = isHoveringEdge;
			if (isHoveringEdge)
			{
				closeButton.Location = new Point(this.Width - closeButton.Width - 10, 0); // Position at top right
			}
			this.Invalidate();
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (isHoveringEdge)
			{
				// Draw border when mouse is near the edge
				int borderWidth = 3;
				Color borderColor = Color.Gray;
				using (Pen borderPen = new Pen(borderColor, borderWidth))
				{
					e.Graphics.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				this.Capture = false; // Release the mouse capture
				var msg = Message.Create(this.Handle, 0xA1, new IntPtr(2), IntPtr.Zero);
				this.WndProc(ref msg); // Enable window drag
			}
		}


		private void UpdateBufferedGraphics()
		{
			// Dispose existing buffered graphics if it exists
			if (bufferedGraphics != null)
			{
				bufferedGraphics.Dispose();
				bufferedGraphics = null;
			}

			// Check if the form handle is created and dimensions are valid
			if (!this.IsHandleCreated || this.Width <= 0 || this.Height <= 0)
			{
				return;
			}

			try
			{
				// Update the maximum buffer size
				context = BufferedGraphicsManager.Current;
				context.MaximumBuffer = new Size(Math.Max(1, this.Width + 1),
											   Math.Max(1, this.Height + 1));

				// Create new buffered graphics
				bufferedGraphics = context.Allocate(this.CreateGraphics(),
					new Rectangle(0, 0, this.Width, this.Height));
			}
			catch (Exception ex)
			{
				// Handle any potential exceptions during graphics initialization
				System.Diagnostics.Debug.WriteLine($"Error in UpdateBufferedGraphics: {ex.Message}");
			}
		}

		private void SetupControls()
		{
			CenterContainer();
			ConfigurePanels();
			AddHoverEffects();

			// Configure container panel with black border
			pnlContainer.BorderStyle = BorderStyle.None;
			pnlContainer.Paint += ContainerPanel_Paint;
		}

		private void ContainerPanel_Paint(object sender, PaintEventArgs e)
		{
			// Draw semi-transparent background
			using (SolidBrush brush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
			{
				e.Graphics.FillRectangle(brush, pnlContainer.ClientRectangle);
			}

			// Draw border
			using (Pen pen = new Pen(Color.FromArgb(50, 255, 255, 255), 2))
			{
				e.Graphics.DrawRectangle(pen,
					1, 1,
					pnlContainer.Width - 2,
					pnlContainer.Height - 2);
			}
		}

		private void SetupBackgroundImage()
			{
				try
				{
				// Define the path to the image in the output directory
				string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "ManafBG.jpeg");

				var path = @"C:\Users\Abdullah-Gn\Desktop\Free\CountDown\WindowsFormsApp1\assets\ManafBG.jpeg";



				using (Image bgImage = Image.FromFile(imagePath))
					{
						Bitmap resized = new Bitmap(this.Width, this.Height);   
						using (Graphics g = Graphics.FromImage(resized))
						{
							g.InterpolationMode = InterpolationMode.HighQualityBicubic;
							g.CompositingQuality = CompositingQuality.HighQuality;
							g.SmoothingMode = SmoothingMode.HighQuality;
							g.PixelOffsetMode = PixelOffsetMode.HighQuality;

							// Calculate dimensions to fit while maintaining aspect ratio
							double ratioX = (double)this.Width / bgImage.Width;
							double ratioY = (double)this.Height / bgImage.Height;
							double ratio = Math.Min(ratioX, ratioY);

							int newWidth = (int)(bgImage.Width * ratio);
							int newHeight = (int)(bgImage.Height * ratio);

							// Center the image
							int x = (this.Width - newWidth) / 2;
							int y = (this.Height - newHeight) / 2;

							// Fill background first
							g.Clear(Color.Black);
                
							// Draw the image
							Rectangle destRect = new Rectangle(0, 0, this.Width, this.Height);
							Rectangle srcRect = new Rectangle(0, 0, bgImage.Width, bgImage.Height);
							g.DrawImage(bgImage, destRect, srcRect, GraphicsUnit.Pixel);
						}
						if (this.BackgroundImage != null)
						{
							this.BackgroundImage.Dispose();
						}
						this.BackgroundImage = resized;
						this.BackgroundImageLayout = ImageLayout.None;
					}
				}
				catch (Exception)
				{
					this.BackColor = Color.FromArgb(44, 62, 80);
				}
			}
		private Rectangle CalculateAspectRatioFit(int srcWidth, int srcHeight, int maxWidth, int maxHeight)
		{
			var ratio = Math.Min((float)maxWidth / srcWidth, (float)maxHeight / srcHeight);
			var newWidth = (int)(srcWidth * ratio);
			var newHeight = (int)(srcHeight * ratio);

			// Center the image
			int x = (maxWidth - newWidth) / 2;
			int y = (maxHeight - newHeight) / 2;

			return new Rectangle(x, y, newWidth, newHeight);
		}

		private void DrawProgressCircle(DoubleBufferedPanel panel, Graphics g, float progressPercentage)
		{
			int padding = 10;  // Reduced padding
			Rectangle rect = new Rectangle(
				padding,
				padding,
				panel.Width - (padding * 2),
				panel.Height - (padding * 2)
			);

			g.SmoothingMode = SmoothingMode.AntiAlias;

			// Draw background circle with thinner line
			using (Pen backgroundPen = new Pen(Color.FromArgb(30, 255, 255, 255), 8))  // Reduced thickness
			{
				g.DrawEllipse(backgroundPen, rect);
			}

			// Draw progress arc with thinner line
			int colorIndex = panelColorIndices[panel];
			using (LinearGradientBrush gradientBrush = new LinearGradientBrush(
				rect,
				progressColors[colorIndex],
				Color.FromArgb(200, progressColors[colorIndex]),
				LinearGradientMode.ForwardDiagonal))
			{
				using (Pen progressPen = new Pen(gradientBrush, 10))  // Reduced thickness
				{
					progressPen.StartCap = LineCap.Round;
					progressPen.EndCap = LineCap.Round;

					float startAngle = -90;
					float sweepAngle = 360 * progressPercentage;
					sweepAngle = Math.Min(sweepAngle, 360);

					g.DrawArc(progressPen, rect, startAngle, sweepAngle);
				}
			}

			// Add subtle glow effect
			using (GraphicsPath path = new GraphicsPath())
			{
				float sweepAngle = 360 * progressPercentage;
				sweepAngle = Math.Min(sweepAngle, 360);
				path.AddArc(rect, -90, sweepAngle);

				using (Pen glowPen = new Pen(Color.FromArgb(30, progressColors[colorIndex]), 12))  // Adjusted glow
				{
					glowPen.StartCap = LineCap.Round;
					glowPen.EndCap = LineCap.Round;
					g.DrawPath(glowPen, path);
				}
			}
		}

		private void DrawPanelBackground(DoubleBufferedPanel panel, PaintEventArgs e, int colorIndex)
		{
			TimeSpan remaining = targetDate - DateTime.Now;
			float progressPercentage = 0f;

			if (panel == pnlDays)
			{
				progressPercentage = (float)(remaining.TotalDays % 365) / 365;
			}
			else if (panel == pnlHours)
			{
				progressPercentage = (float)remaining.Hours / 24;
			}
			else if (panel == pnlMinutes)
			{
				progressPercentage = (float)remaining.Minutes / 60;
			}
			else if (panel == pnlSeconds)
			{
				progressPercentage = (float)remaining.Seconds / 60;
			}

			DrawProgressCircle(panel, e.Graphics, progressPercentage);
		}

		private void PanelPaintHandler(object sender, PaintEventArgs e)
		{
			if (!isUpdating && sender is DoubleBufferedPanel panel && panelColorIndices.ContainsKey(panel))
			{
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
				e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

				DrawPanelBackground(panel, e, panelColorIndices[panel]);
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (isUpdating) return;

			lock (updateLock)
			{
				isUpdating = true;
				try
				{
					TimeSpan remaining = targetDate - DateTime.Now;

					if (remaining.TotalSeconds > 0)
					{
						// Convert to Arabic numerals
						UpdateLabel(lblDays, ConvertToArabicNumbers(Math.Floor(remaining.TotalDays).ToString("00")));
						UpdateLabel(lblHours, ConvertToArabicNumbers(remaining.Hours.ToString("00")));
						UpdateLabel(lblMinutes, ConvertToArabicNumbers(remaining.Minutes.ToString("00")));
						UpdateLabel(lblSeconds, ConvertToArabicNumbers(remaining.Seconds.ToString("00")));

						foreach (Control control in pnlContainer.Controls)
						{
							if (control is DoubleBufferedPanel panel)
							{
								panel.Invalidate();
							}
						}
					}
					else
					{
						UpdateLabel(lblDays, "٠٠");
						UpdateLabel(lblHours, "٠٠");
						UpdateLabel(lblMinutes, "٠٠");
						UpdateLabel(lblSeconds, "٠٠");
						timer.Stop();
					}
				}
				finally
				{
					isUpdating = false;
				}
			}
		}

		private string ConvertToArabicNumbers(string input)
		{
			string[] english = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
			string[] arabic = { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };

			for (int i = 0; i < english.Length; i++)
			{
				input = input.Replace(english[i], arabic[i]);
			}
			return input;
		}

		private void UpdateLabel(Label label, string newText)
		{
			if (label.Text != newText)
			{
				label.Text = newText;
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			if (bufferedGraphics != null)
			{
				bufferedGraphics.Dispose();
			}
			if (context != null)
			{
				context.Dispose();
			}
		}


		private void CenterContainer()
		{
			// Position container at bottom center
			pnlContainer.Location = new Point(
				(this.ClientSize.Width - pnlContainer.Width) / 2,
				this.ClientSize.Height - pnlContainer.Height - 50 // 50 pixels from bottom
			);
		}



		private void ConfigurePanels()
		{
			DoubleBufferedPanel[] panels = { pnlSeconds, pnlMinutes, pnlHours, pnlDays };
			Label[] numberLabels = { lblSeconds, lblMinutes, lblHours, lblDays };
			Label[] textLabels = { lblSecondsText, lblMinutesText, lblHoursText , lblDaysText};

			int padding = 50;
			int startX = padding;

			for (int i = 0; i < panels.Length; i++)
			{
				var panel = panels[i];
				var numberLabel = numberLabels[i];
				var textLabel = textLabels[i];

				panelColorIndices[panel] = i;

				panel.Location = new Point(startX, padding);
				panel.Paint += PanelPaintHandler;

				// Center the labels within the panel
				numberLabel.Size = new Size(panel.Width, 60);  // Increased height
				textLabel.Size = new Size(panel.Width, 40);    // Increased height

				// Adjust vertical positioning
				int totalHeight = numberLabel.Height + textLabel.Height;
				int startY = (panel.Height - totalHeight) / 2;

				numberLabel.Location = new Point(0, startY);
				textLabel.Location = new Point(0, startY + numberLabel.Height - 10); // Reduced gap

				panel.Controls.Add(numberLabel);
				panel.Controls.Add(textLabel);
				pnlContainer.Controls.Add(panel);

				startX += panel.Width + padding;
			}

			pnlContainer.Size = new Size(
				(panels[0].Width + padding) * panels.Length + padding,
				panels[0].Height + (padding * 2)
			);
		}


		private void AddHoverEffects()
		{
			foreach (Control control in pnlContainer.Controls)
			{
				if (control is DoubleBufferedPanel panel)
				{
					panel.MouseEnter += (s, e) =>
					{
						panel.Cursor = Cursors.Hand;
						panel.Invalidate();
					};
					panel.MouseLeave += (s, e) =>
					{
						panel.Cursor = Cursors.Default;
						panel.Invalidate();
					};
				}
			}
		}

		// Add these overrides to reduce flickering
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;  // WS_EX_COMPOSITED
				return cp;
			}
		}

		private void SetupTimer()
		{
			timer = new Timer();
			timer.Interval = 1000; // Update every second
			timer.Tick += Timer_Tick;

			// Initial update
			UpdateTimeDisplay();
			timer.Start();
		}


		private void UpdateTimeDisplay()
		{
			TimeSpan remaining = targetDate - DateTime.Now;

			if (remaining.TotalSeconds > 0)
			{
				lblDays.Text = ((int)remaining.TotalDays).ToString("00");
				lblHours.Text = remaining.Hours.ToString("00");
				lblMinutes.Text = remaining.Minutes.ToString("00");
				lblSeconds.Text = remaining.Seconds.ToString("00");
			}
			else
			{
				// Countdown finished
				lblDays.Text = "00";
				lblHours.Text = "00";
				lblMinutes.Text = "00";
				lblSeconds.Text = "00";
				timer.Stop();
			}

			pnlSeconds.Invalidate();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			SetupBackgroundImage();
			UpdateBufferedGraphics();
			CenterContainer();
			CenterTitle();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			CenterContainer();
			CenterTitle();
		}
	}
}
