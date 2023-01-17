
namespace Calculate {

    class CalcController {

        EventHandler updateDisplay;
        
        public void createNewCalculatorForm(){

            Application.SetCompatibleTextRenderingDefault(false);

            CalculatorForm calcForm = new CalculatorForm();

            updateDisplay = delegate (object sender, EventArgs e){
                CalculatorModel.updateDisplay(calcForm.CurrentWorkingNumber, 
                calcForm.CurrentCalculation, calcForm.History, calcForm.Memory);
            };

            string[] memoryButtonsText = new string[] { "MC", "MR", "M+", "M-" };
            string[] operatorButtonsText = new string[] { "x", "÷", "+", "-" };
            string[] numberButtonsText = new string[] { "7", "8", "9", "4", "5", "6", "1", "2", "3", "0", "." };

            int[] firstFourColumns = new int[] { 0, 1, 2, 3 };
            int[] numberColumns = new int[] { 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1 };
            int[] numberRows = new int[] { 4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7 };

            Font signFont = new Font(calcForm.globalFontFamily, 33, FontStyle.Bold, GraphicsUnit.Pixel);
            Font memoryFont = new Font(calcForm.globalFontFamily, 31, FontStyle.Bold, GraphicsUnit.Pixel);
            Font ceFont = new Font(calcForm.globalFontFamily, 33, FontStyle.Bold, GraphicsUnit.Pixel);
            Font backspaceFont = new Font(calcForm.globalFontFamily, 40, FontStyle.Bold, GraphicsUnit.Pixel);

            createMemoryButtons(calcForm, memoryFont, memoryButtonsText, firstFourColumns, 2);
            createOperatorButtons(calcForm, operatorButtonsText, firstFourColumns, 3);
            createNumberButtons(calcForm, numberButtonsText, numberColumns, numberRows);
            setSignButton(calcForm, signFont, "+/-", 2, 7);
            setBackspaceButton(calcForm, backspaceFont, "⌫", 3, 4);
            setCEButton(calcForm, ceFont, "CE", 3, 5);
            setCButton(calcForm, "C", 3, 6);
            setEqualsButton(calcForm, "=", 3, 7);
            setBinButton(calcForm, "🗑️", 7, 7);

            Application.EnableVisualStyles();
            Application.Run(calcForm);
        }

        public void createMemoryButtons(CalculatorForm calcForm, Font memoryFont, string[] text, int[] columns, int row){

            List<EventHandler> memoryMethods = new List<EventHandler>();

            memoryMethods.Add(CalculatorModel.updateMemoryWinform);

            memoryMethods.Add(CalculatorModel.updateOperandInProgress);

            memoryMethods.Add(updateDisplay);


            for (int i = 0; i < text.Length; i++){
                calcForm.addNewButtonToList(calcForm.MemoryButtons, memoryMethods, memoryFont, text[i], columns[i], row);
            }
        }

        public void createOperatorButtons(CalculatorForm calcForm, string[] ops, int[] columns, int row){

            List<EventHandler> operatorMethods = new List<EventHandler>();

            /* Clicking an operator by definition means you are about to provide a new value for operand2. Therefore
            this is always safe to reset here. However the specific reason for resetting here is so that the user
            can keep clicking equals to reuse operand2 and the current operator until another operator is clicked. */
            operatorMethods.Add(delegate(object sender, EventArgs e){
                CalculatorModel.Operand2 = double.MaxValue;
            });

            operatorMethods.Add(CalculatorModel.setOperand);

            operatorMethods.Add(CalculatorModel.setOperator);

            operatorMethods.Add(CalculatorModel.callPerformCalculation);

            operatorMethods.Add(updateDisplay);

            operatorMethods.Add(delegate(object sender, EventArgs e){

                // Setting calculationComplete to false means that number and CE buttons will not call resetFields
                // We would only want them to do that if we are starting a new calculation and if we have clicked
                // an operator button then we are clearly continuing an existing calculation. 
                CalculatorModel.CalculationComplete = false;

                CalculatorModel.OperatorClick = true;

                if (CalculatorModel.NonNumericResult == true) CalculatorModel.resetFields(sender, e);
                CalculatorModel.NonNumericResult = false;
            });


            for (int i = 0; i < ops.Length; i++){
                calcForm.addNewButtonToList(calcForm.OperatorButtons, operatorMethods, calcForm.globalFont, ops[i], 
                columns[i], row);
            }
        }

        public void createNumberButtons(CalculatorForm calcForm, string[] numbers, int[] columns, int[] rows){

            List<EventHandler> numberMethods = new List<EventHandler>();

            numberMethods.Add(delegate(object sender, EventArgs e){

                // If a number button is clicked following equals, this will be the start of a new calculation. 
                // The equals button sets calculationComplete to true which will cause a number click to call resetFields. 
                // resetFields resets calculationComplete so it is only called on the 1st number click after a calculation. 
                if (CalculatorModel.CalculationComplete == true) CalculatorModel.resetFields(sender, e);

                // If the user has clicked the MR button and then decided they want to type their own number instead
                // setting memoryRecall to false will ensure that setCurrentWorkingNumber displays operandInProgress instead.
                // And also that setOperand will not use operandInProgress instead of memoryNumber. 
                CalculatorModel.MemoryRecall = false;

                CalculatorModel.OperatorClick = false;
            });

            numberMethods.Add(CalculatorModel.updateOperandInProgress);

            numberMethods.Add(updateDisplay);


            for (int i = 0; i < numbers.Length; i++){

                calcForm.addNewButtonToList(calcForm.NumberButtons, numberMethods, calcForm.globalFont, numbers[i], 
                columns[i], rows[i]);
            }
        }

        public void setSignButton(CalculatorForm calcForm, Font signFont, string text, int column, int row){

            List<EventHandler> signMethods = new List<EventHandler>();

            signMethods.Add(CalculatorModel.updateOperandInProgress);

            signMethods.Add(updateDisplay);


            calcForm.setButtonProperties(calcForm.SignButton, signMethods, signFont, text, column, row);
        }

        public void setBackspaceButton(CalculatorForm calcForm, Font backspaceFont, string text, int column, int row){

            List<EventHandler> backspaceMethods = new List<EventHandler>();

            backspaceMethods.Add(CalculatorModel.updateOperandInProgress);

            backspaceMethods.Add(updateDisplay);


            calcForm.setButtonProperties(calcForm.BackspaceButton, backspaceMethods, backspaceFont, text, column, row);
        }

        public void setCEButton(CalculatorForm calcForm, Font ceFont, string text, int column, int row){

            List<EventHandler> ceMethods = new List<EventHandler>();

            ceMethods.Add(delegate(object sender, EventArgs e){

                if (CalculatorModel.CalculationComplete == true) CalculatorModel.resetFields(sender, e);
                else {CalculatorModel.ClearEntry = true;
                CalculatorModel.Operand2 = 0;
                CalculatorModel.OperatorClick = false;
                CalculatorModel.MemoryRecall = false;}
            });

            ceMethods.Add(CalculatorModel.updateOperandInProgress);

            ceMethods.Add(updateDisplay);

            ceMethods.Add(delegate(object sender, EventArgs e){
                CalculatorModel.ClearEntry = false;
            });


            calcForm.setButtonProperties(calcForm.CEButton, ceMethods, ceFont, text, column, row);
        }

        public void setCButton(CalculatorForm calcForm, string text, int column, int row){

            List<EventHandler> clearMethods = new List<EventHandler>();

            clearMethods.Add(CalculatorModel.resetFields);

            clearMethods.Add(updateDisplay);


            calcForm.setButtonProperties(calcForm.CButton, clearMethods, calcForm.globalFont, text, column, row);
        }

        public void setEqualsButton(CalculatorForm calcForm, string text, int column, int row){

            List<EventHandler> equalsMethods = new List<EventHandler>();

            equalsMethods.Add(CalculatorModel.setOperand);

            equalsMethods.Add(CalculatorModel.setOperator);

            equalsMethods.Add(CalculatorModel.callPerformCalculation);

            equalsMethods.Add(updateDisplay);

            equalsMethods.Add(delegate(object sender, EventArgs e){

                // Setting calculationComplete to true means that if number or CE button is clicked following this
                // resetFields will be called and a new calculation will be started. 
                CalculatorModel.CalculationComplete = true;

                CalculatorModel.OperatorClick = false;

                if (CalculatorModel.NonNumericResult == true) CalculatorModel.resetFields(sender, e);
                CalculatorModel.NonNumericResult = false;
            });


            calcForm.setButtonProperties(calcForm.EqualsButton, equalsMethods, calcForm.globalFont, text, column, row);
        }

        public void setBinButton(CalculatorForm calcForm, string text, int column, int row){

            List<EventHandler> binMethods = new List<EventHandler>();

            binMethods.Add(delegate (object sender, EventArgs e){
                CalculatorModel.History = "";
            });

            binMethods.Add(updateDisplay);


            calcForm.setButtonProperties(calcForm.BinButton, binMethods, calcForm.globalFont, text, column, row);
        }
    }
}