
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using NCalc;
using System.Text.RegularExpressions;
using ExtendedNumerics;
using NCalc.Handlers;
using NCalc.Domain;
using NCalc.Visitors;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private string calc = "";
        private decimal currentResult = 0;
        private string resultString;
        private string lastAnswerString = "0";
        private string lastExpression = "";

        private bool wasDelPressed = false;

        string[] operators = ["(", "+", "-", "*", "/", "**", "e"];

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Button_Click(object _sender, RoutedEventArgs e)
        {
            Button sender = (Button)_sender;

            switch ((string)sender.Content)
            {
                case "Ans":
                    calc = $"({lastAnswerString})";
                    break;

                case "(":
                    if (calc.Length > 0)
                        if (Char.IsNumber(calc[^1]))
                            calc += "*";
                    calc += "(";
                    break;

                case "Del":
                    if (calc.Length > 0)
                    {
                        wasDelPressed = true;
                        calc = calc[..(calc.Length - 1)];
                    }
                    break;

                case "Sqrt":
                    if (calc.Length > 0)
                        if (Char.IsNumber(calc[^1]))
                            calc += "*";
                    calc += "Sqrt(";
                    break;

                default:
                    if (operators.Contains((string)sender.Content) && calc.Length == 0 && !wasDelPressed)
                        calc += Convert.ToDecimal(lastAnswerString) > 0 ? lastAnswerString : "";

                    calc += (string)sender.Content;
                    break;
            }

            ManageText(true, false);
        }

        public void Calculate(object _send, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(calc))
                {
                    int _start = 0;
                    for (int i = 0; i < lastExpression.Length; i++)
                    {
                        if (lastExpression[i] == '(')
                            _start++;

                        if (operators.Contains(Convert.ToString(lastExpression[i])) && !operators.Contains(Convert.ToString(lastExpression[Math.Clamp(i-1, 0, int.MaxValue)])))
                        {
                            calc = lastExpression.Substring(0, _start) + lastAnswerString + lastExpression.Substring(i);
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(calc))
                        calc = lastAnswerString;
                }

                currentResult = Convert.ToDecimal(new NCalc.Expression(calc).Evaluate());
                resultString = currentResult.ToString();

                if (resultString.Length > 8)
                {
                    if (0 > currentResult && currentResult < 10)
                        resultString = currentResult.ToString()[..8];

                    else
                        resultString = currentResult.ToString("E8");
                }
            }

            catch (Exception ex)
            {
                resultString = "Err.";
                Console.Write(ex.StackTrace);
            }

            ManageText(true, true);
            lastExpression = calc;
            wasDelPressed = false;
            calc = "";
            lastAnswerString = resultString != "Err." ? resultString.Replace(',', '.') : "";

        }

        public void Reset(object sender, RoutedEventArgs e)
        {
            calc = "";
            currentResult = 0;
            lastAnswerString = "0";
            resultString = "";
            wasDelPressed = false;
            ManageText(false, false);
        }

        private void ManageText(bool showCalcText, bool showResultText)
        {
            if (showCalcText)
                calcText.Content = calc;
            else
                calcText.Content = "";

            if (showResultText)
                resultText.Content = resultString;
            else
                resultText.Content = "";
        }
    }
}