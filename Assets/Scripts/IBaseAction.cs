using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IBaseAction
{ 
     void CancelAction();
     bool InterruptableWithDamageTaken();
     bool InterruptableWithEnemySeen();
}
