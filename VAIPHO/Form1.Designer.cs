namespace VAIPHO
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.Play = new System.Windows.Forms.PictureBox();
            this.labelSpeed = new System.Windows.Forms.Label();
            this.labelIP = new System.Windows.Forms.Label();
            this.labelPseu = new System.Windows.Forms.Label();
            this.publicidad = new System.Windows.Forms.PictureBox();
            this.sygic = new System.Windows.Forms.PictureBox();
            this.autenticados = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.Pause = new System.Windows.Forms.PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.Atasco = new System.Windows.Forms.Button();
            this.Park = new System.Windows.Forms.Button();
            this.Borrar = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            resources.ApplyResources(this.menuItem1, "menuItem1");
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            resources.ApplyResources(this.menuItem2, "menuItem2");
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // txtLog
            // 
            resources.ApplyResources(this.txtLog, "txtLog");
            this.txtLog.Name = "txtLog";
            // 
            // Play
            // 
            resources.ApplyResources(this.Play, "Play");
            this.Play.Name = "Play";
            this.Play.Click += new System.EventHandler(this.Play_Click);
            // 
            // labelSpeed
            // 
            this.labelSpeed.BackColor = System.Drawing.Color.Transparent;
            this.labelSpeed.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            resources.ApplyResources(this.labelSpeed, "labelSpeed");
            this.labelSpeed.Name = "labelSpeed";
            // 
            // labelIP
            // 
            this.labelIP.BackColor = System.Drawing.Color.Transparent;
            this.labelIP.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            resources.ApplyResources(this.labelIP, "labelIP");
            this.labelIP.Name = "labelIP";
            // 
            // labelPseu
            // 
            this.labelPseu.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelPseu, "labelPseu");
            this.labelPseu.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.labelPseu.Name = "labelPseu";
            // 
            // publicidad
            // 
            resources.ApplyResources(this.publicidad, "publicidad");
            this.publicidad.Name = "publicidad";
            this.publicidad.Click += new System.EventHandler(this.publicidad_Click);
            // 
            // sygic
            // 
            this.sygic.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.sygic, "sygic");
            this.sygic.Name = "sygic";
            this.sygic.Click += new System.EventHandler(this.sygic_Click_1);
            // 
            // autenticados
            // 
            this.autenticados.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.autenticados, "autenticados");
            this.autenticados.Name = "autenticados";
            this.autenticados.Click += new System.EventHandler(this.autenticados_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.Play);
            this.panel1.Controls.Add(this.labelPseu);
            this.panel1.Controls.Add(this.labelIP);
            this.panel1.Controls.Add(this.labelSpeed);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Name = "panel1";
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            // 
            // Pause
            // 
            resources.ApplyResources(this.Pause, "Pause");
            this.Pause.Name = "Pause";
            this.Pause.Click += new System.EventHandler(this.Pause_Click_1);
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // pictureBox3
            // 
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.Name = "pictureBox3";
            // 
            // Atasco
            // 
            resources.ApplyResources(this.Atasco, "Atasco");
            this.Atasco.Name = "Atasco";
            this.Atasco.Click += new System.EventHandler(this.Atasco_Click);
            // 
            // Park
            // 
            resources.ApplyResources(this.Park, "Park");
            this.Park.Name = "Park";
            this.Park.Click += new System.EventHandler(this.Park_Click);
            // 
            // Borrar
            // 
            resources.ApplyResources(this.Borrar, "Borrar");
            this.Borrar.Name = "Borrar";
            this.Borrar.Click += new System.EventHandler(this.Borrar_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.White;
            this.ControlBox = false;
            this.Controls.Add(this.Borrar);
            this.Controls.Add(this.Park);
            this.Controls.Add(this.Atasco);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Pause);
            this.Controls.Add(this.autenticados);
            this.Controls.Add(this.sygic);
            this.Controls.Add(this.publicidad);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox3);
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.PictureBox Play;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.Label labelSpeed;
        private System.Windows.Forms.Label labelIP;
        private System.Windows.Forms.Label labelPseu;
        private System.Windows.Forms.PictureBox publicidad;
        private System.Windows.Forms.PictureBox sygic;
        private System.Windows.Forms.PictureBox autenticados;
        private System.Windows.Forms.PictureBox Pause;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Atasco;
        private System.Windows.Forms.Button Park;
        private System.Windows.Forms.Button Borrar;
    }
}

