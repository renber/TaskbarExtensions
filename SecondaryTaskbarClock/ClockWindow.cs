using TaskBarExt.Native;
using TaskBarExt.Renderers;
using TaskBarExt.Utils;
using SecondaryTaskbarClock.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using SecondaryTaskbarClock.Components;
using TaskBarExt;

namespace SecondaryTaskbarClock
{
    public class ClockWindow : TaskbarWindow
    {
        ClockViewModel ViewModel { get; set; }
        ToolTip toolTip;
        private ContextMenuStrip popupMenu;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem secondaryTaskbarClockToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mi_quit;
        bool tooltipShown = false;

        public ClockWindow(TaskbarRef targetTaskbar, ClockViewModel viewModel)
            // currently we always use the Windows 10 renderer
            : base(targetTaskbar, TaskbarWindowPlacement.LeftOfTray, new ClockComponent(viewModel))
        {
            InitializeComponent();

            ViewModel = viewModel;                                

            // the tool tip which contains a long version of the day including Weekday
            toolTip = new ToolTip();
            toolTip.Active = true;            
            toolTip.UseFading = false;

            // we have to show the tooltip manually for correct positioning
            MouseMove += ClockWindow_MouseMove;
            MouseLeave += ClockWindow_MouseLeave;            

            // when clicked, open the calendar flyout
            MouseClick += ClockWindow_MouseClick;
        }

        private void ClockWindow_MouseClick(object sender, MouseEventArgs e)
        {
            switch(e.Button)
            {
                case MouseButtons.Left:
                    // show the calendar flyout next to this clock
                    Task.Run(() =>
                    {                        
                        TaskbarUtils.ShowCalendarFlyOut(this.Bounds, Taskbar.DockPosition.GetCorrespondingFlyoutPosition());
                    });
                    break;
                case MouseButtons.Right:
                    {
                        // force the popup menu to calculate its height
                        popupMenu.SuspendLayout();
                        popupMenu.ResumeLayout();                        

                        Point popupLocation;
                        // open the context menu, correctly positioned
                        // depending on the taskbar location
                        switch (Taskbar.DockPosition)
                        {                            
                            case TaskbarDockPosition.Top:
                                popupLocation = new Point(this.Bounds.Right - popupMenu.Width, this.Bounds.Bottom);
                                break;
                            case TaskbarDockPosition.Left:
                                popupLocation = new Point(this.Bounds.Right, this.Bounds.Bottom - popupMenu.Height);
                                break;
                            case TaskbarDockPosition.Right:
                                popupLocation = new Point(this.Bounds.Left - popupMenu.Width, this.Bounds.Bottom - popupMenu.Height);
                                break;                            
                            case TaskbarDockPosition.Bottom:
                            default:
                                popupLocation = new Point(this.Bounds.Right - popupMenu.Width, this.Bounds.Top - popupMenu.Height);
                                break;
                        }

                        popupMenu.Show(popupLocation);
                        break;
                    }
            }
        }

        private void ClockWindow_MouseMove(object sender, MouseEventArgs e)
        {            
            // only invoke the tooltip once
            if (!tooltipShown)
            {
                tooltipShown = true;
                // we need the focus to show a tooltip
                this.Focus();
                          
                // we have to use the Show() method of the tooltip, since otherwise
                // it will always be positioned at the exact mouse position and not
                // next to the window
                toolTip.Show(ViewModel.CurrentDateTime.ToLongDateString(), this, 5000);
            }
        }
        
        private void ClockWindow_MouseLeave(object sender, EventArgs e)
        {            
            tooltipShown = false;
        }

        private void mi_quit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.popupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.secondaryTaskbarClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mi_quit = new System.Windows.Forms.ToolStripMenuItem();
            this.popupMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // popupMenu
            // 
            this.popupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.secondaryTaskbarClockToolStripMenuItem,
            this.toolStripMenuItem1,
            this.mi_quit});
            this.popupMenu.Name = "popupMenu";
            this.popupMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.popupMenu.Size = new System.Drawing.Size(200, 54);
            // 
            // secondaryTaskbarClockToolStripMenuItem
            // 
            this.secondaryTaskbarClockToolStripMenuItem.Name = "secondaryTaskbarClockToolStripMenuItem";
            this.secondaryTaskbarClockToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.secondaryTaskbarClockToolStripMenuItem.Text = "SecondaryTaskbarClock";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(196, 6);
            // 
            // mi_quit
            // 
            this.mi_quit.Name = "mi_quit";
            this.mi_quit.Size = new System.Drawing.Size(199, 22);
            this.mi_quit.Text = "Quit";
            this.mi_quit.Click += new System.EventHandler(this.mi_quit_Click);
            // 
            // ClockWindow
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "ClockWindow";
            this.popupMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }        
    }
}
