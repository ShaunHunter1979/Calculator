using static System.Console;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Calculate {

    public class CalculatorForm : Form {

        TableLayoutPanel panel;
        public Font globalFont;
        public FontFamily globalFontFamily = new FontFamily("Arial");

        RoundCornerLabel currentWorkingNumber;
        public RoundCornerLabel CurrentWorkingNumber 
        { get { return currentWorkingNumber; } set { currentWorkingNumber = value; } }
        
        Label currentCalculation;
        public Label CurrentCalculation { get { return currentCalculation; } set { currentCalculation = value; } }

        Label history;
        public Label History { get { return history; } set { history = value; } }

        RoundCornerLabel memory;
        public RoundCornerLabel Memory { get { return memory; } set { memory = value; } }

        Rectangle originalFormRect;
        float currentWorkingNumberFontSize;
        float currentCalculationFontSize;
        float historyFontSize;
        float memoryFontSize;
        float fontscale = 0.75f;

        RoundCornerButton cButton = new RoundCornerButton();
        public RoundCornerButton CButton { get { return cButton; } set { cButton = value; } }
        RoundCornerButton equalsButton = new RoundCornerButton();
        public RoundCornerButton EqualsButton { get { return equalsButton; } set { equalsButton = value; } }
        RoundCornerButton binButton = new RoundCornerButton();
        public RoundCornerButton BinButton { get { return binButton; } set { binButton = value; } }
        RoundCornerButton backspaceButton = new RoundCornerButton();
        public RoundCornerButton BackspaceButton { get { return backspaceButton; } set { backspaceButton = value; } }
        RoundCornerButton ceButton = new RoundCornerButton();
        public RoundCornerButton CEButton{ get { return ceButton; } set { ceButton = value; } }
        RoundCornerButton signButton = new RoundCornerButton();
        public RoundCornerButton SignButton { get { return signButton; } set { signButton = value; } }
        List<RoundCornerButton> numberButtons = new List<RoundCornerButton>();
        public List<RoundCornerButton> NumberButtons { get { return numberButtons; } set { numberButtons = value; } }
        List<RoundCornerButton> operatorButtons = new List<RoundCornerButton>();
        public List<RoundCornerButton> OperatorButtons { get { return operatorButtons; } set { operatorButtons = value; } }
        List<RoundCornerButton> memoryButtons = new List<RoundCornerButton>();
        public List<RoundCornerButton> MemoryButtons { get { return memoryButtons; } set { memoryButtons = value; } }

        int tabIndex;

        public CalculatorForm(){

            globalFont = new Font(globalFontFamily, 50, FontStyle.Bold, GraphicsUnit.Pixel);

            this.ClientSize = new Size(620, 620);
            this.MinimumSize = new Size(637, 659);
            this.Font = globalFont;
            this.Text = "Calculator (created by Shaun Hunter)";
            this.BackColor = ColorTranslator.FromHtml("#C2D4B1");
            // this.TopMost = true;

            this.KeyPreview = true;
            this.KeyDown += CalculatorForm_KeyDown;
            this.Resize += CalculatorForm_Resize;

            panel = new TableLayoutPanel();
            panel.Size = new Size(620, 620);
            panel.Location = new Point(0, 0);
            panel.Dock = DockStyle.Fill;
    
            panel.ColumnCount = 8;
            float columnWidth = 100 / panel.ColumnCount;
            for (int i = 0; i < panel.ColumnCount; i++)
                this.panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, columnWidth));

            panel.RowCount = 8;
            float rowWidth = 100 / panel.RowCount;
            for (int i = 0; i < panel.RowCount; i++)
                this.panel.RowStyles.Add(new RowStyle(SizeType.Percent, rowWidth));

            this.Controls.Add(panel);


            Font labelFont = new Font(globalFontFamily, 30, FontStyle.Regular, GraphicsUnit.Pixel);

            currentWorkingNumber = new RoundCornerLabel();
            currentWorkingNumber.Font = labelFont;
            currentWorkingNumber.Text = "0";
            currentWorkingNumber.TextAlign = ContentAlignment.MiddleRight;
            currentWorkingNumber.BackColor = Color.RoyalBlue;
            currentWorkingNumber.Margin = new Padding(3, 0, 3, 2);
            currentWorkingNumber.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);
            panel.SetColumnSpan(currentWorkingNumber, 8);
            panel.Controls.Add(currentWorkingNumber, 0, 1);

            currentCalculation = new Label();
            currentCalculation.Font = labelFont;
            currentCalculation.TextAlign = ContentAlignment.MiddleRight;
            currentCalculation.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);
            panel.SetColumnSpan(currentCalculation, 8);
            panel.Controls.Add(currentCalculation, 0, 0);

            memory = new RoundCornerLabel();
            memory.Text = "MEMORY:\n0";
            memory.Font = new Font(globalFontFamily, 17, FontStyle.Regular, GraphicsUnit.Pixel);
            memory.BackColor = Color.RoyalBlue;
            memory.Margin = new Padding(3);
            memory.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);
            panel.SetColumnSpan(memory, 3);
            panel.Controls.Add(memory, 4, 7);

            history = new Label();
            history.Font = new Font(globalFontFamily, 20, FontStyle.Regular, GraphicsUnit.Pixel);
            history.AutoSize = true;
            history.MaximumSize = new Size(310, 10000);
            history.TextAlign = ContentAlignment.MiddleRight;

            FlowLayoutPanel historyPanel = new FlowLayoutPanel();
            historyPanel.AutoScroll = true;
            historyPanel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);
            historyPanel.FlowDirection = FlowDirection.RightToLeft;
       
            historyPanel.Controls.Add(history);

            panel.SetColumnSpan(historyPanel, 4);
            panel.SetRowSpan(historyPanel, 5);
            panel.Controls.Add(historyPanel, 4, 2);


            originalFormRect = new Rectangle(this.Location, this.Size);
            currentWorkingNumberFontSize = currentWorkingNumber.Font.Size;
            currentCalculationFontSize = currentCalculation.Font.Size;
            historyFontSize = history.Font.Size;
            memoryFontSize = memory.Font.Size;
        }

        void CalculatorForm_Resize(object sender, EventArgs e){

            ResizeControlFont(currentWorkingNumber, currentWorkingNumberFontSize, fontscale);

            ResizeControlFont(currentCalculation, currentCalculationFontSize, fontscale);

            ResizeControlFont(history, historyFontSize, fontscale);

            ResizeControlFont(memory, memoryFontSize, 0.8f);

            ResizeLabelWidth(history, 310);
        }

        void ResizeLabelWidth(Label l, float originalLabelWidth){

            float xRatio = (float)this.ClientRectangle.Width / (float)originalFormRect.Width;

            float newWidth = originalLabelWidth * xRatio;

            l.MaximumSize = new Size((int)newWidth, 10000);
        }

        void ResizeControlFont(Control control, float originalFontSize, float fontscale){

            float xRatio = (float)this.ClientRectangle.Width / (float)originalFormRect.Width;
            float yRatio = (float)this.ClientRectangle.Height / (float)originalFormRect.Height;

            float ratio = xRatio;
            if (xRatio >= yRatio) ratio = yRatio;

            float newFontSize = originalFontSize * ratio * fontscale;

            try{
                control.Font = new Font(control.Font.FontFamily, newFontSize);
            }
            catch (System.ArgumentException){ 
                /* exception raised when minimising form as ratio is 0 therefore newFontSize is 0 */ 
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            
            if (keyData == (Keys.Enter | Keys.Return)){
                equalsButton.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        void CalculatorForm_KeyDown(object sender, KeyEventArgs e){

            if (e.KeyCode == Keys.D7 | e.KeyCode == Keys.NumPad7) numberButtons[0].PerformClick();
            else if (e.KeyCode == Keys.D8 | e.KeyCode == Keys.NumPad8) numberButtons[1].PerformClick();
            else if (e.KeyCode == Keys.D9 | e.KeyCode == Keys.NumPad9) numberButtons[2].PerformClick();
            else if (e.KeyCode == Keys.D4 | e.KeyCode == Keys.NumPad4) numberButtons[3].PerformClick();
            else if (e.KeyCode == Keys.D5 | e.KeyCode == Keys.NumPad5) numberButtons[4].PerformClick();
            else if (e.KeyCode == Keys.D6 | e.KeyCode == Keys.NumPad6) numberButtons[5].PerformClick();
            else if (e.KeyCode == Keys.D1 | e.KeyCode == Keys.NumPad1) numberButtons[6].PerformClick();
            else if (e.KeyCode == Keys.D2 | e.KeyCode == Keys.NumPad2) numberButtons[7].PerformClick();
            else if (e.KeyCode == Keys.D3 | e.KeyCode == Keys.NumPad3) numberButtons[8].PerformClick();
            else if (e.KeyCode == Keys.D0 | e.KeyCode == Keys.NumPad0) numberButtons[9].PerformClick();
            else if (e.KeyCode == Keys.Decimal | e.KeyCode == Keys.OemPeriod) numberButtons[10].PerformClick();

            else if (e.KeyCode == Keys.L && e.Modifiers == Keys.Control) memoryButtons[0].PerformClick(); // MC
            else if (e.KeyCode == Keys.R && e.Modifiers == Keys.Control) memoryButtons[1].PerformClick(); // MR
            else if (e.KeyCode == Keys.P && e.Modifiers == Keys.Control) memoryButtons[2].PerformClick(); // M+
            else if (e.KeyCode == Keys.Q && e.Modifiers == Keys.Control) memoryButtons[3].PerformClick(); // M-

            else if (e.KeyCode == Keys.Delete) ceButton.PerformClick();
            else if (e.KeyCode == Keys.Escape) cButton.PerformClick();
            else if (e.KeyCode == Keys.F9) signButton.PerformClick();
            else if (e.KeyCode == Keys.Back) backspaceButton.PerformClick();
            else if (e.KeyCode == Keys.H && e.Modifiers == Keys.Control) binButton.PerformClick();

            else if ((e.KeyCode == Keys.D8 && e.Modifiers == Keys.Shift) | e.KeyCode == Keys.Multiply) 
                operatorButtons[0].PerformClick();
            else if (e.KeyCode == Keys.OemQuestion | e.KeyCode == Keys.Divide) 
                operatorButtons[1].PerformClick();
            else if ((e.KeyCode == Keys.Oemplus && e.Modifiers == Keys.Shift) | e.KeyCode == Keys.Add) 
                operatorButtons[2].PerformClick();
            else if (e.KeyCode == Keys.OemMinus | e.KeyCode == Keys.Subtract) 
                operatorButtons[3].PerformClick();
        }

        public void addNewButtonToList(List<RoundCornerButton> list, IEnumerable<EventHandler> methods, Font font, 
        string text, int column, int row){

            RoundCornerButton button = new RoundCornerButton();
            list.Add(button);

            setButtonProperties(button, methods, font, text, column, row);
        }

        public void setButtonProperties(RoundCornerButton button, IEnumerable<EventHandler> methods, Font font, 
        string text, int column, int row){

            foreach (EventHandler m in methods) button.Click += new EventHandler(m);

            button.BackColor = Color.CadetBlue;
            button.Dock = DockStyle.Fill;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Text = text;
            button.Font = font;

            button.TabIndex = tabIndex;
            tabIndex++;

            this.panel.Controls.Add(button, column, row);
        }
    }

    public partial class RoundCornerButton : Button{
        int radius = 20;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        void RecreateRegion(){

            var bounds = ClientRectangle;
            this.Region = Region.FromHrgn(CreateRoundRectRgn(bounds.Left, bounds.Top,
                bounds.Right, bounds.Bottom, radius, radius));
        }
        protected override void OnSizeChanged(EventArgs e){

            base.OnSizeChanged(e);
            this.RecreateRegion();
        }
    }

    public partial class RoundCornerLabel : Label {
        int radius = 20;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        void RecreateRegion(){

            var bounds = ClientRectangle;
            this.Region = Region.FromHrgn(CreateRoundRectRgn(bounds.Left, bounds.Top,
                bounds.Right, bounds.Bottom, radius, radius));
        }
        protected override void OnSizeChanged(EventArgs e){

            base.OnSizeChanged(e);
            this.RecreateRegion();
        }
    }
}