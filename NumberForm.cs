using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace NumberToString
{
  public class NumberForm : INotifyPropertyChanged, IDataErrorInfo
  {
    #region Fields

    private string _input;
    private string _output;

    #endregion

    #region Constructor

    public NumberForm()
    {
      ConvertCommand = new ConvertCommand();
    }

    #endregion

    #region Properties



    #endregion

    #region Commands

    public static ConvertCommand ConvertCommand { get; set; }

    #endregion

    #region Methods



    #endregion

    #region DependencyProperties

    public string Input
    {
      get { return _input; }
      set
      {
        _input = value;
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
            long num;
            if (!long.TryParse(Input, out num) && !string.IsNullOrEmpty(Input) || (num < 0 || num > 9999999999))
              msg = "Must be a number from 0 to 9,999,999,999.";
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
