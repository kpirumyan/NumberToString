using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace NumberToString
{
  public class NumberForm : INotifyPropertyChanged, IDataErrorInfo
  {
    #region Fields

    private string _input;
    private string _output;
    private bool _hasError;
    private Dictionary<int, string> _dictionary;
    private List<string> _list;

    #endregion

    #region Constructor

    public NumberForm()
    {
      ConvertCommand = new BaseCommand(ExecuteConvert, CanExecuteConvert);
      _dictionary = new Dictionary<int, string>();
      _dictionary.Add(0, "zero");
      _dictionary.Add(1, "one");
      _dictionary.Add(2, "two");
      _dictionary.Add(3, "three");
      _dictionary.Add(4, "four");
      _dictionary.Add(5, "five");
      _dictionary.Add(6, "six");
      _dictionary.Add(7, "seven");
      _dictionary.Add(8, "eight");
      _dictionary.Add(9, "nine");
      _dictionary.Add(10, "ten");
      _dictionary.Add(11, "eleven");
      _dictionary.Add(12, "twelve");
      _dictionary.Add(13, "thirteen");
      _dictionary.Add(14, "fourteen");
      _dictionary.Add(15, "fifteen");
      _dictionary.Add(16, "sixteen");
      _dictionary.Add(17, "seventeen");
      _dictionary.Add(18, "eighteen");
      _dictionary.Add(19, "nineteen");
      _dictionary.Add(20, "twenty");
      _dictionary.Add(30, "thirty");
      _dictionary.Add(40, "fourty");
      _dictionary.Add(50, "fifty");
      _dictionary.Add(60, "sixty");
      _dictionary.Add(70, "seventy");
      _dictionary.Add(80, "eighty");
      _dictionary.Add(90, "ninety");
      _dictionary.Add(100, "hundred");
      _dictionary.Add(1000, "thousand");
      _dictionary.Add(1000000, "million");
      _dictionary.Add(1000000000, "billion");
    }

    #endregion

    #region DependencyProperties

    public string Input
    {
      get { return _input; }
      set
      {
        // Clear previous format (remove all whitespaces)
        string s = Regex.Replace(value, @"\s", "");

        // Format input string to decimal
        _input = long.TryParse(s, out long v) ? $"{v:N0}" : s;

        OnPropertyChanged("Input");
      }
    }

    public string Output
    {
      get { return _output; }
      set
      {
        _output = value;
        OnPropertyChanged("Output");
      }
    }

    public bool HasError
    {
      get { return _hasError; }
      set
      {
        _hasError = value;
        OnPropertyChanged("HasError");
      }
    }

    #endregion

    #region Commands

    public static BaseCommand ConvertCommand { get; set; }

    private bool CanExecuteConvert(object parameter)
    {
      if (parameter is bool) return !(bool)parameter;
      return false;
    }

    private void ExecuteConvert(object parameter)
    {
      _list = new List<string>();

      // Clear output field
      Output = string.Empty;

      // Remove all whitespaces from the input string
      string s = Regex.Replace(Input, @"\s", "");

      // Get an array of integers from string
      int[] array = Array.ConvertAll(s.ToCharArray(), n => Convert.ToInt32(n.ToString()));

      // Condition if input is equal to zero
      if (array.Length == 1 && array[0] == 0)
      {
        Output = "zero";
        return;
      }

      Array.Reverse(array);
      int l = array.Length;

      // Devide the input number to triplets (3-digit blocks) 
      for (int i = 0, j = 0; i < l; i += 3, j++)
      {
        // Condition when all 3 numbers of triplet are equal to zero. If so do not type words like thousand, million or billion.
        if (i + 2 < l && array[i] == 0 && array[i + 1] == 0 && array[i + 2] == 0) continue;

        // Variable 'j' is a triplet count 
        if (j == 1) _list.Add("thousand");
        else if (j == 2) _list.Add("million");
        else if (j == 3) _list.Add("billion");

        // Conditions how to print triplet 
        if (i + 2 < l)
          ThreeDigits(array[i], array[i + 1], array[i + 2]);
        else if (i + 1 < l)
          TwoDigits(array[i], array[i + 1]);
        else
          OneDigit(array[i]);
      }

      _list.Reverse();

      // Build final output value from list of strings
      foreach (var item in _list)
      {
        Output += item + " ";
      }

      // Capitalize output first letter
      Output = Output[0].ToString().ToUpper() + Output.Substring(1);
    }

    #endregion

    #region Methods

    // Add 1-digit converted number to output string list
    private void OneDigit(int n)
    {
      _list.Add(FromOneToNine(n));
    }

    // Add 2-digit converted number to output string list
    private void TwoDigits(int n1, int n2)
    {
      if (n2 == 0)
        _list.Add(FromOneToNine(n1));
      else if (n2 == 1)
        _list.Add(FromTenToNineteen(n2, n1));
      else _list.Add(FromTwentyToHundred(n2, n1));
    }

    // Add 3-digit converted number to output string list
    private void ThreeDigits(int n1, int n2, int n3)
    {
      TwoDigits(n1, n2);
      if (n3 > 0)
      {
        _list.Add("hundred");
        OneDigit(n3);
      }
    }

    // Convert number from 1 to 9 to word
    private string FromOneToNine(int num)
    {
      return num != 0 ? _dictionary[num] : "";
    }

    // Convert number from 10 to 19 to word
    private string FromTenToNineteen(int num1, int num2)
    {
      string numString = $"{num1}{num2}";
      int index = Convert.ToInt32(numString);
      return _dictionary[index];
    }

    // Convert number from 20 to 99 to word
    private string FromTwentyToHundred(int num1, int num2)
    {
      int index = Convert.ToInt32(num1.ToString());
      return num2 == 0 ? $"{_dictionary[num1 * 10]}" : $"{_dictionary[num1 * 10]} {_dictionary[num2]}";
    }

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName]string prop = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    #endregion

    // Validation
    #region IDataErrorInfo

    public string Error
    {
      get
      {
        return null;
      }
    }

    public string this[string columnName]
    {
      get
      {
        string msg = null;
        switch (columnName)
        {
          case "Input":
            HasError = string.IsNullOrEmpty(Input) ? true : false;
            string s = Input == null ? Input : Regex.Replace(Input, @"\s", "");
            if (!long.TryParse(s, out long num) && !string.IsNullOrEmpty(Input) || (num < 0 || num > 9999999999))
            {
              msg = "Must be a number from 0 to 9,999,999,999.";
              HasError = true;
            }
            break;

          default:
            throw new ArgumentException(
            "Unrecognized property: " + columnName);
        }

        return msg;
      }
    }

    #endregion
  }
}
