namespace Calculate{
    public static class CalculatorModel{

        static string operandInProgress = "";
        static string currentCalculation = "";
        static string history = "";
        public static string History { set { history = value; } }
        static double operand1 = double.MaxValue;
        static double operand2 = double.MaxValue;
        public static double Operand2 { set { operand2 = value; } }
        static Operator currentOperator;
        static Operator standbyOperator;
        static string currentOperatorSymbol;
        static string standbyOperatorSymbol;
        static bool nonNumericResult;
        public static bool NonNumericResult { get { return nonNumericResult; } set { nonNumericResult = value; } }
        static int maxLength = 16;
        static int operandMaxLength = maxLength;
        static bool noOperatorSet = true;
        static double memoryNumber;
        static string potentialMemoryNumber;
        static bool memoryRecall;
        public static bool MemoryRecall { set { memoryRecall = value; } }
        static bool operatorClick;
        public static bool OperatorClick { set { operatorClick = value; } }
        static bool calculationComplete;
        public static bool CalculationComplete 
        { get { return calculationComplete; } set { calculationComplete = value; } }
        static bool clearEntry;
        public static bool ClearEntry { set { clearEntry = value; } }

        public enum Operator {
            None = 0,
            Add = 1,
            Divide = 2,
            Multiply = 3,            
            Subtract = 4
        }

        public static void updateMemoryWinform(object sender, EventArgs e){
            updateMemory(((Button)sender).Text);
        }

        public static void updateMemory(string buttonText){

            if (buttonText == "MC") memoryNumber = 0;
            if (buttonText == "M+") memoryNumber += Convert.ToDouble(potentialMemoryNumber);
            if (buttonText == "M-") memoryNumber -= Convert.ToDouble(potentialMemoryNumber);
            if (buttonText == "MR") memoryRecall = true;
        }

        static void buildNumber(object sender, EventArgs e){

            Button b = (Button)sender;

            if (b.Text == "." & !operandInProgress.Contains('.')){

                if (operandInProgress == "") operandInProgress = "0.";
                else operandInProgress += ".";

                operandMaxLength++;
            }

            else if ("0123456789".Contains(b.Text)){

                if (operandInProgress == "0") operandInProgress = $"{b.Text}";
                else operandInProgress += $"{b.Text}";
            }
        }

        // Called by numbers, backspace, CE, sign and memory buttons
        public static void updateOperandInProgress(object sender, EventArgs e){

            Button b = (Button)sender;

            if (b.Text == "+/-"){

                if (operandInProgress.Contains("-")){

                    operandInProgress = operandInProgress.Substring(1);
                    operandMaxLength--;
                }
                else if (operandInProgress != "" & operandInProgress != "0"){
                    operandInProgress = operandInProgress.Insert(0, "-");
                    operandMaxLength++;
                }
            }

            else if (b.Text == "⌫" & operandInProgress != ""){

                int minus1 = operandInProgress.Length - 1;

                if (operandInProgress[minus1] == '.') operandMaxLength--;

                operandInProgress = operandInProgress.Substring(0, minus1);

                if (operandInProgress == "-" | operandInProgress == "-0"){
                    operandInProgress = "";
                    operandMaxLength--;
                }
            }

            else if (b.Text == "CE" | b.Text == "MR"){
                operandInProgress = "";
                operandMaxLength = maxLength;
            }

            else if (operandInProgress.Length < operandMaxLength) buildNumber(sender, e);
        }

        static string addCommas(string number){

            int minCharactersForCommas = 4;
            if (number.Contains("-")) minCharactersForCommas++;

            if (!number.Contains("e")){

                if (number.Contains(".")) insertCommas(number.IndexOf("."));
                else insertCommas(number.Length);
            }

            void insertCommas(int length){

                while (length >= minCharactersForCommas){

                    length -= 3;
                    number = number.Insert(length, ",");
                }
            }

            return number;
        }

        static string setCurrentWorkingNumber(){

            string number = "";

            // nonNumericResult, clearEntry and memoryRecall cannot be true simultaneously
            // nonNumericResult and clearEntry are reset on the same button click
            // memoryRecall is reset on the following button click
            if (nonNumericResult){

                if (operand1.ToString().Contains("∞") & operand2 == 0) number = "Cannot divide by zero";
                if (operand1.ToString().Contains("∞") & operand2 != 0) number = "Maximum possible number exceeded"; 
                if (operand1.ToString() == "NaN") number = "Result is undefined";
            }
            else if (clearEntry) { 
                number = "0"; 
            }
            else if (memoryRecall) {
                number = memoryNumber.ToString();
            }
            else if (operandInProgress != "") {
                number = operandInProgress;
            }
            else if (operand1 == double.MaxValue) {
                number = "0";
            }
            else {
                number = operand1.ToString().Replace("E", "e");
            }

            return number;
        }

        // Called by all buttons
        public static void updateDisplay(Label currentWorkingNo, Label currentCalc, Label hist, Label memory){

            currentWorkingNo.Text = addCommas(setCurrentWorkingNumber());

            potentialMemoryNumber = currentWorkingNo.Text;

            currentCalc.Text = currentCalculation.Replace("E", "e");

            hist.Text = history.Replace("E", "e");

            memory.Text = "MEMORY:\n" + addCommas(memoryNumber.ToString().Replace("E", "e"));
        }

        // Called by operator buttons and equals button
        public static void setOperand(object sender, EventArgs e){

            if (memoryRecall){
                updateOperand(memoryNumber);
                memoryRecall = false;
            }

            else if (operandInProgress != ""){

                updateOperand(Convert.ToDouble(operandInProgress));

                operandInProgress = "";
                operandMaxLength = maxLength;
            }

            else if (operand1 == double.MaxValue) operand1 = 0;
  
            else if (operatorClick & ((Button)sender).Text == "=") operand2 = operand1;


            void updateOperand(double newOperand){

                if (operand1 == double.MaxValue) operand1 = newOperand;
                else operand2 = newOperand;
            }
        }

        // Called by operator buttons and equals button
        public static void setOperator(object sender, EventArgs e){

            Button b = (Button)sender;
                      
            // If operand2 is at MaxValue then we know that a calculation won't be performed on this click. 
            // There must be two non-default operands for a calculation to be performed. However we need
            // to display the next operator that will be used so we set currentOperatorSymbol at this stage. 
            if (operand2 == double.MaxValue & b.Text != "=") currentOperatorSymbol = b.Text;

            else{
                currentOperator = standbyOperator;
                currentOperatorSymbol = standbyOperatorSymbol;
            }

            if (b.Text == "+")         standbyOperator = Operator.Add;
            else if (b.Text == "÷")    standbyOperator = Operator.Divide;
            else if (b.Text == "x")    standbyOperator = Operator.Multiply;
            else if (b.Text == "-")    standbyOperator = Operator.Subtract;

            if (b.Text != "=")         standbyOperatorSymbol = b.Text;

            if (standbyOperator != Operator.None) noOperatorSet = false;
        }


        // Called by operators buttons and equals button
        public static void callPerformCalculation(object sender, EventArgs e){

            // currentCalculation is updated twice for each calculation. It could be placed in a block with operand2
            // not having a value as a condition, as in setOperator. The reason for this duplication is so that the user
            // can click equals repeatedly to reuse the same value of operand2. currentOperatorSymbol will not change
            // in this case but operand1 will and so will need to be updated even though operand2 is not changing.
            currentCalculation = operand1 + " " + currentOperatorSymbol;

            // This is a "simulation calculation" which simply tells you that operand1 = operand1
            // If noOperatorSet is true there cannot be a non-default value of operand2
            if (noOperatorSet){
                currentCalculation = operand1 + " = ";
                history = history.Insert(0, "\n" + operand1 + " = " + addCommas(operand1.ToString()) + "\n");
            }

            // operand2 having a non-default value is the condition for a calculation to run
            else if (operand2 != double.MaxValue){

                performCalculation(ref operand1, ref operand2, ref currentOperator);

                string op1 = addCommas(operand1.ToString());

                if (!"-0123456789".Contains(op1[0])) nonNumericResult = true;

                else{

                    history = history.Insert(0, "\n" + currentCalculation + " " + operand2 + " = " + op1 + "\n");

                    if (((Button)sender).Text == "=") currentCalculation += " " + operand2 + " =";
                    else currentCalculation = operand1 + " " + standbyOperatorSymbol;
                }
            }
        }

        static void performCalculation(ref double operand1, ref double operand2, ref Operator currentOperator){

            if (currentOperator == Operator.Subtract)    operand1 -= operand2;
            if (currentOperator == Operator.Add)         operand1 += operand2;
            if (currentOperator == Operator.Multiply)    operand1 *= operand2;
            if (currentOperator == Operator.Divide)      operand1 /= operand2;

            string op1 = operand1.ToString();

            // To round the result to the desired number of digits where there is an exponent
            if (op1.Contains("E")){

                string exponent = op1.Substring(op1.IndexOf('E'));

                double mantissa = Convert.ToDouble(op1.Substring(0, op1.IndexOf('E')));

                double roundedMantissa = Math.Round(mantissa, 15);

                operand1 = Convert.ToDouble(roundedMantissa.ToString() + exponent);
            }

            // To round the result to the desired number of digits where there is not an exponent but is a decimal point. 
            else if (op1.Contains(".")){

                if (op1.Contains("-")) operandMaxLength++;

                // I have not incremented operandMaxLength to account for the decimal point. However, this works out as
                // it allows me to compare a zero-based index which will be one less due to the zero, to operandMaxLength
                // which will be one less due to not accounting for the decimal point. 

                if (op1.IndexOf(".") >= operandMaxLength) operand1 = Math.Round(operand1);

                else operand1 = Math.Round(operand1, operandMaxLength - op1.IndexOf("."));

                operandMaxLength = maxLength;
            }
        }
 
        // Called by C button only
        public static void resetFields(object sender, EventArgs e){
            operandInProgress = "";
            currentCalculation = "";
            operand1 = double.MaxValue;
            operand2 = double.MaxValue;
            currentOperator = 0;
            standbyOperator = 0;
            currentOperatorSymbol = null;
            standbyOperatorSymbol = null;
            calculationComplete = false;
            operandMaxLength = maxLength;
            noOperatorSet = true;
            operatorClick = false;
            memoryRecall = false;
            potentialMemoryNumber = "0";
        }
    }
}