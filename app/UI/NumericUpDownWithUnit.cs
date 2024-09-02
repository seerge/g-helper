using System.Globalization;
using System.Text.RegularExpressions;

public class NumericUpDownWithUnit : NumericUpDown
{
    #region| Fields |

    private string unit = null;
    private bool unitFirst = false;

    #endregion

    #region| Properties |

    public string Unit
    {
        get => unit;
        set
        {
            unit = value;

            UpdateEditText();
        }
    }

    public bool UnitFirst
    {
        get => unitFirst;
        set
        {
            unitFirst = value;

            UpdateEditText();
        }
    }

    #endregion

    #region| Methods |

    /// <summary>
    /// Method called when updating the numeric updown text.
    /// </summary>
    protected override void UpdateEditText()
    {
        // If there is a unit we handle it ourselfs, if there is not we leave it to the base class.
        if (Unit != null && Unit != string.Empty)
        {
            if (UnitFirst)
            {
                Text = $"({Unit}) {Value}";
            }
            else
            {
                Text = $"{Value} ({Unit})";
            }
        }
        else
        {
            base.UpdateEditText();
        }
    }

    /// <summary>
    /// Validate method called before actually updating the text.
    /// This is exactly the same as the base class but it will use the new ParseEditText from this class instead.
    /// </summary>
    protected override void ValidateEditText()
    {
        // See if the edit text parses to a valid decimal considering the label unit
        ParseEditText();
        UpdateEditText();
    }

    /// <summary>
    /// Converts the text displayed in the up-down control to a numeric value and evaluates it.
    /// </summary>
    protected new void ParseEditText()
    {
        try
        {
            // The only difference of this methods to the base one is that text is replaced directly
            // with the property Text instead of using the regex.
            // We now that the only characters that may be on the textbox are from the unit we provide.
            // because the NumericUpDown handles invalid input from user for us.
            // This is where the magic happens. This regex will match all characters from the unit
            // (so your unit cannot have numbers). You can change this regex to fill your needs
            var regex = new Regex($@"[^(?!{Unit} )]+");
            var match = regex.Match(Text);

            if (match.Success)
            {
                var text = match.Value;

                // VSWhidbey 173332: Verify that the user is not starting the string with a "-"
                // before attempting to set the Value property since a "-" is a valid character with
                // which to start a string representing a negative number.
                if (!string.IsNullOrEmpty(text) && !(text.Length == 1 && text == "-"))
                {
                    if (Hexadecimal)
                    {
                        Value = Constrain(Convert.ToDecimal(Convert.ToInt32(Text, 16)));
                    }
                    else
                    {
                        Value = Constrain(Decimal.Parse(text, CultureInfo.CurrentCulture));
                    }
                }
            }
        }
        catch
        {
            // Leave value as it is
        }
        finally
        {
            UserEdit = false;
        }
    }

    /// </summary>
    /// Returns the provided value constrained to be within the min and max.
    /// This is exactly the same as the one in base class (which is private so we can't directly use it).
    /// </summary>
    private decimal Constrain(decimal value)
    {
        if (value < Minimum)
        {
            value = Minimum;
        }

        if (value > Maximum)
        {
            value = Maximum;
        }

        return value;
    }

    #endregion
}