using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace PXLed.Controls
{
    public partial class IntegerUpDown : UserControl
    {
        public IntegerUpDown()
        {
            InitializeComponent();

            // Display number in text box
            textBox.Text = value.ToString();

            // Subscribe to all relevant events
            textBox.TextChanged += TextBox_TextChanged;
            textBox.LostKeyboardFocus += TextBox_LostKeyboardFocus;
            upButton.Click += (s, e) => Value++;
            downButton.Click += (s, e) => Value--;
        }

        public int Min { get; set; } = int.MinValue;
        public int Max { get; set; } = int.MaxValue;

        public event TextChangedEventHandler TextChanged;

        public int Value
        {
            get
            {
                return value;
            }
            set
            {
                // Prevent caret position being reset if we remove text
                int currentCaretIndex = textBox.CaretIndex;

                // Keep the entered number between Min and Max
                int numberInRange = Math.Min(Max, Math.Max(Min, value));

                // Save number and display number
                this.value = numberInRange;
                textBox.Text = numberInRange.ToString();

                textBox.CaretIndex = currentCaretIndex;
            }
        }

        int value;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check validity of the text that was just inputted
            string text = textBox.Text;

            if (int.TryParse(text, out int number)) {
                // If text is valid integer, save it
                Value = number;
            } else
            {
                // If text is not a valid integer, reset the text box to the last valid value

                // Exception:   If the text box is empty or only a negative sign ('-'), do not reset the text to the last value 
                //              so that a zero doesn't appear out of nowhere if 
                //              the user wants to set a completely new value
                
                if (text != "" && text != "-")
                {
                    // Prevent caret position being reset if we remove text
                    int currentCaretIndex = textBox.CaretIndex;

                    textBox.Text = Value.ToString();

                    textBox.CaretIndex = currentCaretIndex - 1;
                }
            }

            if (TextChanged != null)
                TextChanged(this, e);
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // If there is no number in text box, set it to zero
            if (textBox.Text == "" && textBox.Text != "-")
                Value = 0;
        }
    }
}