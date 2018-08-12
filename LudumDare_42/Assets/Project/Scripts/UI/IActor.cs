using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public enum ObjectAction
{
    [Description("Abrir")] OPEN,
    [Description("Fechar")] CLOSE,
    [Description("Limpar")] CLEAR,
}

public interface IActor
{
    List<ObjectAction> ActionsList { get; }
}
