using System;
using UnityEngine;

public interface IDataMediator<dataType> where dataType : DataType
{
  void SetDataOnCVS(int id, dataType data);   // Manda info de SO -> CSV
  void SetDataOnSO(int id, dataType data);  // Lee info de CSV -> Data
}
