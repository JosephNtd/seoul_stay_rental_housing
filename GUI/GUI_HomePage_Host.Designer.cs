namespace DangNhap_Form
{
    partial class GUI_HomePage_Host
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.main = new DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormContainer();
            this.accordionControl1 = new DevExpress.XtraBars.Navigation.AccordionControl();
            this.acc_MyProfile = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.acc_MyList = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.accordionControlSeparator1 = new DevExpress.XtraBars.Navigation.AccordionControlSeparator();
            this.fluentDesignFormControl1 = new DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl();
            this.accordionControlSeparator2 = new DevExpress.XtraBars.Navigation.AccordionControlSeparator();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.accordionControlElement2 = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.acc_MyBookings = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fluentDesignFormControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // main
            // 
            this.main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.main.Location = new System.Drawing.Point(260, 31);
            this.main.Name = "main";
            this.main.Size = new System.Drawing.Size(540, 419);
            this.main.TabIndex = 0;
            // 
            // accordionControl1
            // 
            this.accordionControl1.Appearance.AccordionControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.accordionControl1.Appearance.AccordionControl.BorderColor = System.Drawing.Color.White;
            this.accordionControl1.Appearance.AccordionControl.Options.UseBackColor = true;
            this.accordionControl1.Appearance.AccordionControl.Options.UseBorderColor = true;
            this.accordionControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.acc_MyProfile,
            this.accordionControlSeparator1});
            this.accordionControl1.Location = new System.Drawing.Point(0, 31);
            this.accordionControl1.LookAndFeel.SkinMaskColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.accordionControl1.LookAndFeel.SkinMaskColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.accordionControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.accordionControl1.Name = "accordionControl1";
            this.accordionControl1.RootDisplayMode = DevExpress.XtraBars.Navigation.AccordionControlRootDisplayMode.Footer;
            this.accordionControl1.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.Touch;
            this.accordionControl1.ShowFilterControl = DevExpress.XtraBars.Navigation.ShowFilterControl.Always;
            this.accordionControl1.Size = new System.Drawing.Size(260, 419);
            this.accordionControl1.TabIndex = 1;
            this.accordionControl1.ViewType = DevExpress.XtraBars.Navigation.AccordionControlViewType.HamburgerMenu;
            // 
            // acc_MyProfile
            // 
            this.acc_MyProfile.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.acc_MyList,
            this.acc_MyBookings});
            this.acc_MyProfile.Expanded = true;
            this.acc_MyProfile.Name = "acc_MyProfile";
            this.acc_MyProfile.Text = "My Listing";
            // 
            // acc_MyList
            // 
            this.acc_MyList.Appearance.Hovered.Font = new System.Drawing.Font("Open Sans", 12F, System.Drawing.FontStyle.Bold);
            this.acc_MyList.Appearance.Hovered.Options.UseFont = true;
            this.acc_MyList.Appearance.Normal.Font = new System.Drawing.Font("Plus Jakarta Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acc_MyList.Appearance.Normal.ForeColor = System.Drawing.Color.White;
            this.acc_MyList.Appearance.Normal.Options.UseFont = true;
            this.acc_MyList.Appearance.Normal.Options.UseForeColor = true;
            this.acc_MyList.Appearance.Pressed.Font = new System.Drawing.Font("Plus Jakarta Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acc_MyList.Appearance.Pressed.Options.UseFont = true;
            this.acc_MyList.Name = "acc_MyList";
            this.acc_MyList.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.acc_MyList.Text = "My Listings";
            this.acc_MyList.Click += new System.EventHandler(this.acc_MyList_Click);
            // 
            // accordionControlSeparator1
            // 
            this.accordionControlSeparator1.Name = "accordionControlSeparator1";
            // 
            // fluentDesignFormControl1
            // 
            this.fluentDesignFormControl1.FluentDesignForm = this;
            this.fluentDesignFormControl1.Location = new System.Drawing.Point(0, 0);
            this.fluentDesignFormControl1.Name = "fluentDesignFormControl1";
            this.fluentDesignFormControl1.Size = new System.Drawing.Size(800, 31);
            this.fluentDesignFormControl1.TabIndex = 2;
            this.fluentDesignFormControl1.TabStop = false;
            // 
            // accordionControlSeparator2
            // 
            this.accordionControlSeparator2.Name = "accordionControlSeparator2";
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // accordionControlElement2
            // 
            this.accordionControlElement2.Name = "accordionControlElement2";
            this.accordionControlElement2.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.accordionControlElement2.Text = "Element2";
            // 
            // acc_MyBookings
            // 
            this.acc_MyBookings.Appearance.Normal.Font = new System.Drawing.Font("Plus Jakarta Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acc_MyBookings.Appearance.Normal.Options.UseFont = true;
            this.acc_MyBookings.Appearance.Pressed.Font = new System.Drawing.Font("Plus Jakarta Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acc_MyBookings.Appearance.Pressed.Options.UseFont = true;
            this.acc_MyBookings.Name = "acc_MyBookings";
            this.acc_MyBookings.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.acc_MyBookings.Text = "My Bookings";
            this.acc_MyBookings.Click += new System.EventHandler(this.acc_MyBookings_Click);
            // 
            // GUI_HomePage_Host
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlContainer = this.main;
            this.Controls.Add(this.main);
            this.Controls.Add(this.accordionControl1);
            this.Controls.Add(this.fluentDesignFormControl1);
            this.FluentDesignFormControl = this.fluentDesignFormControl1;
            this.Name = "GUI_HomePage_Host";
            this.NavigationControl = this.accordionControl1;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fluentDesignFormControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormContainer main;
        private DevExpress.XtraBars.Navigation.AccordionControl accordionControl1;
        private DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl fluentDesignFormControl1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement acc_MyProfile;
        private DevExpress.XtraBars.Navigation.AccordionControlSeparator accordionControlSeparator1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement acc_MyList;
        private DevExpress.XtraBars.Navigation.AccordionControlSeparator accordionControlSeparator2;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private DevExpress.XtraBars.Navigation.AccordionControlElement accordionControlElement2;
        private DevExpress.XtraBars.Navigation.AccordionControlElement acc_MyBookings;
    }
}