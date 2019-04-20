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
      ConvertCommand = new RelayCommand(ExecuteConvert, CanExecuteConvert);
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

    #region Properties



    #endregion

    #region Commands

    public static RelayCommand ConvertCommand { get; set; }

    private bool CanExecuteConvert(object parameter)
    {
      if (parameter is bool) return !(bool)parameter;
      return false;
    }

    private void ExecuteConvert(object parameter)
    {
      _list = new List<string>();
      Output = string.Empty;
      string s = Regex.Replace(Input, @"\s", "");
      int[] array = Array.ConvertAll(s.ToCharArray(), n => Convert.ToInt32(n.ToString()));

      if (array.Length == 1 && array[0] == 0)
      {
        Output = "zero";
        return;
      }

      Array.Reverse(array);

      int l = array.Length;
      for (int i = 0, j = 0; i < l; i += 3, j++)
      {
        if (i + 2 < l && array[i] == 0 && array[i + 1] == 0 && array[i + 2] == 0) continue;

        if (j == 1) _list.Add("thousand");
        else if (j == 2) _list.Add("million");
        else if (j == 3) _list.Add("billion");

        if (i + 2 < l)
          ThreeDigits(array[i], array[i + 1], array[i + 2]);
        else if (i + 1 < l)
          TwoDigits(array[i], array[i + 1]);
        else
          OneDigit(array[i]);
      }

      _list.Reverse();
      foreach (var item in _list)
      {
        Output += item + " ";
      }
    }

    #endregion

    #region Methods

    private void OneDigit(int n)
    {
      _list.Add(FromOneToNine(n));
    }

    private void TwoDigits(int n1, int n2)
    {
      if (n2 == 0)
        _list.Add(FromOneToNine(n1));
      else if (n2 == 1)
        _list.Add(FromTenToNineteen(n2, n1));
      else _list.Add(FromTwentyToHundred(n2, n1));
    }

    private void ThreeDigits(int n1, int n2, int n3)
    {
      TwoDigits(n1, n2);
      if (n3 > 0)
      {
        _list.Add("hundred");
        OneDigit(n3);
      }
    }

    private string FromOneToNine(int num)
    {
      return num != 0 ? _dictionary[num] : "";
    }

    private string FromTenToNineteen(int num1, int num2)
    {
      string numString = $"{num1}{num2}";
      int index = Convert.ToInt32(numString);
      return _dictionary[index];
    }

    private string FromTwentyToHundred(int num1, int num2)
    {
      int index = Convert.ToInt32(num1.ToString());
      return num2 == 0 ? $"{_dictionary[num1 * 10]}" : $"{_dictionary[num1 * 10]} {_dictionary[num2]}";
    }

    #endregion

    #region DependencyProperties

    public string Input
    {
      get { return _input; }
      set
      {
        string s = Regex.Replace(value, @"\s", "");
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

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName]string prop = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    #endregion

    #region IDataErrorInof

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
